context: .NET library to enable event sourcing. 

Each event handler takes the prev snapshot as input, applies the logic which generates the mutated snapshot. 
The last snapshot is the current value of that entity. 
Each event handler code has a hash
This is achieved using the library alexwiese/hashstamp
Each snapshot has a hash

If event order changes then go back to previous key snapshot, replay events until last reordered event, then replay until resulting snapshot has does not change 
If event handler code hash changes then go back to previous key snapshot and replay events until the resulting snapshot hash does not change, repeat for each instance of this type of event. 

Could use forward and backward diffs for snapshot on each event. Hash for diffs. 
Verify integrity of everything (code, diffs). Resulting snapshot hash is saved but not the snapshot itself. 
