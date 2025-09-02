#!/usr/bin/env python3
"""
Script to generate API documentation from .NET XML documentation files.
"""

import os
import xml.etree.ElementTree as ET
import argparse
from pathlib import Path


def extract_summary(member_elem):
    """Extract summary from XML documentation member."""
    summary_elem = member_elem.find('summary')
    if summary_elem is not None and summary_elem.text:
        return summary_elem.text.strip()
    return ""


def extract_returns(member_elem):
    """Extract returns documentation from XML documentation member."""
    returns_elem = member_elem.find('returns')
    if returns_elem is not None and returns_elem.text:
        return returns_elem.text.strip()
    return ""


def extract_params(member_elem):
    """Extract parameter documentation from XML documentation member."""
    params = []
    for param_elem in member_elem.findall('param'):
        name = param_elem.get('name', '')
        desc = param_elem.text.strip() if param_elem.text else ''
        if name:
            params.append((name, desc))
    return params


def parse_member_name(name):
    """Parse .NET XML documentation member name."""
    if not name:
        return None, None, None, None
    
    member_type = name[0]  # T, M, P, F, etc.
    full_name = name[2:]  # Skip "T:", "M:", etc.
    
    if '.' not in full_name:
        return member_type, full_name, '', ''
    
    parts = full_name.split('.')
    if len(parts) >= 2:
        namespace_class = '.'.join(parts[:-1])
        member_name = parts[-1]
        
        # Split namespace and class
        if '.' in namespace_class:
            namespace_parts = namespace_class.split('.')
            namespace = '.'.join(namespace_parts[:-1])
            class_name = namespace_parts[-1]
        else:
            namespace = ''
            class_name = namespace_class
            
        return member_type, namespace, class_name, member_name
    
    return member_type, full_name, '', ''


def generate_api_docs(xml_file, output_dir):
    """Generate API documentation markdown files from XML documentation."""
    
    if not os.path.exists(xml_file):
        print(f"XML documentation file not found: {xml_file}")
        return False
    
    # Parse XML documentation
    try:
        tree = ET.parse(xml_file)
        root = tree.getroot()
    except ET.ParseError as e:
        print(f"Error parsing XML file: {e}")
        return False
    
    assembly = root.find('assembly')
    assembly_name = ""
    if assembly is not None:
        name_elem = assembly.find('name')
        if name_elem is not None:
            assembly_name = name_elem.text or ""
    
    members = root.find('members')
    if members is None:
        print("No members found in XML documentation")
        return False
    
    # Group members by class
    classes = {}
    
    for member in members.findall('member'):
        name = member.get('name', '')
        member_type, namespace, class_name, member_name = parse_member_name(name)
        
        if not member_type:
            continue
            
        full_class_name = f"{namespace}.{class_name}" if namespace else class_name
        
        if full_class_name not in classes:
            classes[full_class_name] = {
                'namespace': namespace,
                'class_name': class_name,
                'summary': '',
                'methods': [],
                'properties': [],
                'fields': [],
                'types': []
            }
        
        summary = extract_summary(member)
        returns = extract_returns(member)
        params = extract_params(member)
        
        if member_type == 'T':  # Type (class, interface, etc.)
            classes[full_class_name]['summary'] = summary
        elif member_type == 'M':  # Method
            classes[full_class_name]['methods'].append({
                'name': member_name,
                'summary': summary,
                'returns': returns,
                'parameters': params
            })
        elif member_type == 'P':  # Property
            classes[full_class_name]['properties'].append({
                'name': member_name,
                'summary': summary,
                'returns': returns
            })
        elif member_type == 'F':  # Field
            classes[full_class_name]['fields'].append({
                'name': member_name,
                'summary': summary
            })
    
    # Generate markdown files
    os.makedirs(output_dir, exist_ok=True)
    
    # Generate index file
    index_content = f"""# {assembly_name} API Reference

This documentation is automatically generated from XML documentation comments in the source code.

## Classes

"""
    
    for full_class_name, class_info in classes.items():
        if class_info['class_name']:  # Skip empty class names
            index_content += f"- [{class_info['class_name']}]({class_info['class_name'].lower()}.md)\n"
    
    with open(os.path.join(output_dir, 'index.md'), 'w') as f:
        f.write(index_content)
    
    # Generate individual class documentation files
    for full_class_name, class_info in classes.items():
        if not class_info['class_name']:
            continue
            
        filename = f"{class_info['class_name'].lower()}.md"
        filepath = os.path.join(output_dir, filename)
        
        content = f"# {class_info['class_name']}\n\n"
        
        if class_info['namespace']:
            content += f"**Namespace:** `{class_info['namespace']}`\n\n"
        
        if class_info['summary']:
            content += f"{class_info['summary']}\n\n"
        
        # Methods
        if class_info['methods']:
            content += "## Methods\n\n"
            for method in class_info['methods']:
                content += f"### {method['name']}\n\n"
                if method['summary']:
                    content += f"{method['summary']}\n\n"
                
                if method['parameters']:
                    content += "**Parameters:**\n\n"
                    for param_name, param_desc in method['parameters']:
                        content += f"- `{param_name}`: {param_desc}\n"
                    content += "\n"
                
                if method['returns']:
                    content += f"**Returns:** {method['returns']}\n\n"
        
        # Properties
        if class_info['properties']:
            content += "## Properties\n\n"
            for prop in class_info['properties']:
                content += f"### {prop['name']}\n\n"
                if prop['summary']:
                    content += f"{prop['summary']}\n\n"
                if prop['returns']:
                    content += f"**Type:** {prop['returns']}\n\n"
        
        # Fields
        if class_info['fields']:
            content += "## Fields\n\n"
            for field in class_info['fields']:
                content += f"### {field['name']}\n\n"
                if field['summary']:
                    content += f"{field['summary']}\n\n"
        
        with open(filepath, 'w') as f:
            f.write(content)
    
    print(f"Generated API documentation for {len(classes)} classes in {output_dir}")
    return True


def main():
    parser = argparse.ArgumentParser(description='Generate API documentation from .NET XML docs')
    parser.add_argument('xml_file', help='Path to XML documentation file')
    parser.add_argument('output_dir', help='Output directory for generated markdown files')
    
    args = parser.parse_args()
    
    success = generate_api_docs(args.xml_file, args.output_dir)
    return 0 if success else 1


if __name__ == '__main__':
    exit(main())