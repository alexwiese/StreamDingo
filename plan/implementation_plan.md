# StreamDingo - Implementation Plan

> **Core Concept**: .NET library to enable event sourcing with hash-based integrity verification

## Core Architecture

### Event Sourcing Flow
Each event handler takes the previous snapshot as input, applies the logic which generates the mutated snapshot. The last snapshot is the current value of that entity.

### Hash-Based Integrity
- Each event handler code has a hash (achieved using alexwiese/hashstamp library)
- Each snapshot has a hash
- Event integrity verification through cryptographic hashing

### Smart Replay Logic
- **Event Order Changes**: Go back to previous key snapshot, replay events until last reordered event, then replay until resulting snapshot hash does not change
- **Handler Code Changes**: Go back to previous key snapshot and replay events until the resulting snapshot hash does not change, repeat for each instance of this type of event

### Advanced Features
- Forward and backward diffs for snapshots on each event
- Hash verification for diffs
- Comprehensive integrity verification (code, diffs, snapshots)
- Resulting snapshot hash is saved but not the snapshot itself (space optimization)

## 📋 Detailed Implementation

**For comprehensive implementation details, stages, timelines, and progress tracking, see:**

➡️ **[Detailed Implementation Plan](detailed_implementation_plan.md)**

The detailed plan includes:
- 8 implementation stages with clear objectives
- Timeline and milestone tracking
- Success metrics and quality targets
- Development guidelines and best practices
- Progress tracking with checkboxes
- Community and FOSS library requirements
