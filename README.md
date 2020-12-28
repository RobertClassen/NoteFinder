# NoteFinder for Unity

This tool parses script files in a Unity project for certain comment Tags to filter their Notes using this syntax:
`//[TAG](:) [DESCRIPTION]`
E.g. "//TODO: implement jump button" or "//BUG player explodes instead of jumping"

## Features
* Automatically detects changes to Notes in script files and updates its view (after each assembly reload)
* Displays Notes in a tree hierarchy (matching the folder structure in the Project window, excluding "empty" folders in which no Notes are found)
* Allows collapsing individual Notes or entire folders to hide them if they are not needed (as well as collapsing/ expanding all via right-click menu)
* Allows toggling Tags (default "TODO" and "Bug") and specifying custom ones via the "Tags" menu
* Allows opening the relevant script files and jumping to the correct line by pressing the line number button on a Note (using the default IDE which has been set up in Unity)
* Allows searching for strings and substrings which appear in Note descriptions, file names, or directory names
* Allows jumping to script files in the Project window via the button on the right side for quick navigation
* Supports both the Light and Dark themes:
![light theme](https://i.imgur.com/l6z8OSQ.png)
![dark theme](https://i.imgur.com/S0QgFuO.png)

## Installation

### Installation via Git URL (recommended)
See [here](https://docs.unity3d.com/Manual/upm-ui-giturl.html) for how to install packages via Git URL by using the Unity Package Manager.  
See [here](https://docs.unity3d.com/Manual/upm-git.html) for how to do so manually by editing the "manifest.json" file in `[your project folder]/Packages/`.

### Installation from a local package (alternative)
See [here](https://docs.unity3d.com/Manual/upm-ui-local.html) for how to install packages from a local folder using the Unity Package Manager.  
See [here](https://docs.unity3d.com/Manual/upm-localpath.html) for how to do so manually by editing the "manifest.json" file in `[your project folder]/Packages/`.

## Credits
Based on Denis Sylkin's [ToDoManager](https://github.com/densylkin/ToDoManager) (although by now the code has undergone a 100% rewrite).