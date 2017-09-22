# CapuchinSync
A Windows command-line utility for synchronizing directories using precomputed hashes.

As awesome as Robocopy is at synchonizing directories across a network, when you have tons of small files, the traffic that it requires to examine each file can end up being quite burdonsome.  CapuchinSync takes a two step (admittedly not novel) approach to speeding things up:
1. Generate a dictionary of hashes of each file in the folder.
2. Transfer the hashes to the target computer as a single file, and use those hashes to verify each corresponding file.  Then transfer the files that don't match their hash.
 
The hash dictionary only needs to be recalculated when the source directory is updated.  What's more, directories having duplicate files can be synched even faster if we transfer the data across the network once, and update all matching files at the same time.

## Usage
CapuchinSync consists of two separate executables: CapuchinSync.Hash.exe, which creates a dictionary of hashes for a directory, and CapuchinSync.exe, which uses a hash dictionary to efficiently update a directory.  
**Command Line Syntax**  

```
CapuchinSync.Hash dir:<directory> [...] [xf:<extensionToExclude> [...]] [verbosity:<loggingLevel>]

	directory:          An absolute path to a directory for which a hash dictionary file should be created.  The dictionary will be written as a .capuchinSync file in this directory.  Note that if you wish to generate hash dictionaries for multiple nested directories, specifying all directories in one run of CapuchinSync.Hash will be more efficient than running CapuchinSync.Hash once for each directory, as the program will then be able to ensure that it only hashes each file once.
	extensionToExclude: A file extension that should be excluded from the hash dictionaries that are created.
	loggingLevel:       Trace, Debug, Info, Warning, Error, or Fatal.  All log statements less severe than this level will be filtered from the console.
   
CapuchinSync source:<sourceDir>;destination:<destinationDir> [...] [maxParallelCopies:<maxThreads>] [verbosity:<loggingLevel>] [openLogInEditor]
	
    sourceDir:          An absolute path to a directory for which CapuchinSync.Hash has been run.
    destinationDir:     An absolute path to a directory that will be synchronized with sourceDir.  Once the synchronization is complete, the contents in this directory will be updated to match the contents of sourceDir.
    maxThreads:         The maximum number of synchronous copies that will be allowed at any time.  This defaults to 8, but can  be any number in the range [1,128].
	loggingLevel:       Trace, Debug, Info, Warning, Error, or Fatal.  All log statements less severe than this level will be filtered from the console.
    openLogInEditor:    After synchronization is complete, write all log entries to a temporary text file and open that file in a text editor.  If NotePad++ is installed at C:\Program Files (x86)\Notepad++\notepad++.exe, it will be used.  Otherwise, Windows Notepad will be used.
```  
**Examples**  

```
> CapuchinSync.Hash dir:\\remoteMachine\share
> CapuchinSync.Hash dir:"C:\Source Folder 1" dir:"C:\Source Folder 1\Subdir" dir:"C:\Source Folder 2" 
> CapuchinSync.Hash dir:C:\temp xf:pdb xf:obj
> CapuchinSync.Hash dir:C:\temp verbosity:debug

> CapuchinSync source:\\remoteMachine\share;destination:C:\binaries
> CapuchinSync source:\\remoteMachine\share;destination:C:\binaries source:\\remoteMachine\share;destination:D:\Backup
> CapuchinSync source:C:\Dir1;destination:D:\Dir2 maxParallelCopies:4
> CapuchinSync source:C:\Dir1;destination:D:\Dir2 verbosity:error
> CapuchinSync source:"C:\Program Files";destination:\\backupServer\backup openLogInEditor
```

**Notes**  
CapuchinSync is built with the awesome [AppVeyor](https://www.appveyor.com) CI provider.  It's free for open source projects, and is pretty darn easy to use.  You can see the builds [here](https://ci.appveyor.com/project/schallot/capuchinsync).
