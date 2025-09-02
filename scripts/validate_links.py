#!/usr/bin/env python3
"""
Script to validate links in markdown documentation files.
"""

import os
import re
import sys
import requests
import argparse
from pathlib import Path
from urllib.parse import urljoin, urlparse
import time


def find_markdown_files(directory):
    """Find all markdown files in directory."""
    markdown_files = []
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith('.md'):
                markdown_files.append(os.path.join(root, file))
    return markdown_files


def extract_links(file_path):
    """Extract markdown links from file."""
    links = []
    
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Find markdown links [text](url)
    markdown_links = re.findall(r'\[([^\]]*)\]\(([^)]+)\)', content)
    
    for text, url in markdown_links:
        links.append({
            'text': text,
            'url': url,
            'line': None,  # Could be improved to track line numbers
            'type': 'markdown'
        })
    
    # Find reference links [text][ref] and [ref]: url
    ref_links = re.findall(r'\[([^\]]+)\]:\s*(.+)', content)
    for ref, url in ref_links:
        links.append({
            'text': ref,
            'url': url.strip(),
            'line': None,
            'type': 'reference'
        })
    
    return links


def validate_local_link(link_url, base_path, docs_root):
    """Validate a local link."""
    # Remove fragment (#section)
    clean_url = link_url.split('#')[0]
    
    if not clean_url:  # Just a fragment
        return True, "Fragment only"
    
    # Resolve relative path
    if clean_url.startswith('/'):
        # Absolute path from docs root
        full_path = os.path.join(docs_root, clean_url.lstrip('/'))
    else:
        # Relative path from current file
        full_path = os.path.join(os.path.dirname(base_path), clean_url)
    
    full_path = os.path.abspath(full_path)
    
    # Check if file exists
    if os.path.exists(full_path):
        return True, "File exists"
    
    # Check if it's a directory with index.md
    if os.path.isdir(full_path):
        index_path = os.path.join(full_path, 'index.md')
        if os.path.exists(index_path):
            return True, "Directory with index.md"
    
    return False, f"File not found: {full_path}"


def validate_external_link(url, session):
    """Validate an external HTTP/HTTPS link."""
    try:
        response = session.head(url, timeout=10, allow_redirects=True)
        if response.status_code < 400:
            return True, f"HTTP {response.status_code}"
        else:
            # Try GET if HEAD fails
            response = session.get(url, timeout=10, allow_redirects=True)
            if response.status_code < 400:
                return True, f"HTTP {response.status_code} (GET)"
            else:
                return False, f"HTTP {response.status_code}"
    except requests.exceptions.Timeout:
        return False, "Timeout"
    except requests.exceptions.ConnectionError:
        return False, "Connection error"
    except requests.exceptions.RequestException as e:
        return False, f"Request error: {str(e)}"


def validate_links(docs_directory, check_external=True):
    """Validate all links in documentation."""
    
    markdown_files = find_markdown_files(docs_directory)
    print(f"Found {len(markdown_files)} markdown files to check")
    
    total_links = 0
    broken_links = []
    
    # Create session for external links
    session = requests.Session()
    session.headers.update({
        'User-Agent': 'StreamDingo-Doc-Validator/1.0 (+https://github.com/alexwiese/StreamDingo)'
    })
    
    for file_path in markdown_files:
        print(f"Checking {file_path}...")
        
        links = extract_links(file_path)
        total_links += len(links)
        
        for link in links:
            url = link['url'].strip()
            
            # Skip empty URLs
            if not url:
                continue
            
            # Skip email links
            if url.startswith('mailto:'):
                continue
            
            # Skip javascript links
            if url.startswith('javascript:'):
                continue
            
            # Check external links
            if url.startswith(('http://', 'https://')):
                if check_external:
                    is_valid, reason = validate_external_link(url, session)
                    if not is_valid:
                        broken_links.append({
                            'file': file_path,
                            'url': url,
                            'text': link['text'],
                            'reason': reason,
                            'type': 'external'
                        })
                    time.sleep(0.1)  # Be nice to external servers
                continue
            
            # Check local links
            is_valid, reason = validate_local_link(url, file_path, docs_directory)
            if not is_valid:
                broken_links.append({
                    'file': file_path,
                    'url': url,
                    'text': link['text'],
                    'reason': reason,
                    'type': 'local'
                })
    
    # Report results
    print(f"\n=== Link Validation Results ===")
    print(f"Total links checked: {total_links}")
    print(f"Broken links found: {len(broken_links)}")
    
    if broken_links:
        print("\n=== Broken Links ===")
        for broken in broken_links:
            print(f"❌ {broken['file']}")
            print(f"   Link: {broken['url']}")
            print(f"   Text: '{broken['text']}'")
            print(f"   Error: {broken['reason']}")
            print(f"   Type: {broken['type']}")
            print()
    
    return len(broken_links) == 0


def main():
    parser = argparse.ArgumentParser(description='Validate links in markdown documentation')
    parser.add_argument('docs_dir', help='Documentation directory to check')
    parser.add_argument('--skip-external', action='store_true', 
                       help='Skip validation of external HTTP/HTTPS links')
    
    args = parser.parse_args()
    
    if not os.path.exists(args.docs_dir):
        print(f"Error: Directory '{args.docs_dir}' does not exist")
        return 1
    
    check_external = not args.skip_external
    success = validate_links(args.docs_dir, check_external)
    
    return 0 if success else 1


if __name__ == '__main__':
    sys.exit(main())