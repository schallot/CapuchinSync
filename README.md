# CapuchinSync
A command-line utility for synchronizing directories using precomputed hashes.

As awesome as Robocopy is at synchonizing directories across a network, when you have tons of small files, the traffic that it requires to examine each file can end up being quite burdonsome.  CapuchinSync takes a two step (admittedly not novel) approach to speeding things up:
1. Generate a dictionary of hashes of each file in the folder.
2. Transfer the hashes to the target computer as a single file, and use those hashes to verify each corresponding file.  Then transfer the files that don't match their hash.
 
The hash dictionary only needs to be recalculated when the source directory is updated.  What's more, directories having duplicate files can be synched even faster if we transfer the data across the network once, and update all matching files at the same time.

**Syntax**  
Generate hashes for a directory:  
`> CapuchinSync.Hash [PathToDirectory]`  
For example, `CapuchinSync.Hash "D:\Share\MyAwesomeBinaries"`.  Once this completes, a hash dictionary will be created at 'D:\Share\MyAwesomeBinaries\\.capuchinSync'

Synchronize a local folder with an already-hashed directory:  
`> CapuchinSync source:PATHTOSOURCEDIR;destination:PATHTODESTINATIONDIR [...]`  
For example, `CapuchinSync source:\\remoteMachine\share\MyAwesomeBinaries;destination:C:\Development\binaries`
will ensure that all files in `C:\Development\binaries` match those in `\\remoteMachine\share\MyAwesomeBinaries`.  What's more, if you have multiple directories to synchronize, you can pass in muliple source\destination pairs to synchronize them all at once, taking advantage of the fact that CapuchinSync will look across all source directories for duplicates before starting the transfer.  Thus synchronizing multiple directories at once can end up being faster than synchronizing them individually.

**Notes**
CapuchinSync is built with the awesome [AppVeyor](https://www.appveyor.com) CI provider.  It's free for open source projects, and is pretty darn easy to use.  You can see the builds [here](https://ci.appveyor.com/project/schallot/capuchinsync).
