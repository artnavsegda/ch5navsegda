# Shell Template - Export Assets

'Export Assets' exports assets from the './app/project/assets/' folder (removing the excluded files) using command-line statements.
 
## Installation

### Install Dependencies

*npm i rimraf --save-dev*

*npm i fs-extra --save-dev*

*npm i zip-lib --save-dev*

*npm i config --save-dev*

*npm i find-remove --save-dev*

## How to Use

```
Usage: 
    yarn export:assets [options]
    npm run export:assets [options]

You could use shortcuts as the following:
    yarn exp:a
    npm run exp:a

Options:
    -h, --help,         Help for Exporting assets from the './app/project/assets/' folder
    -l, --list,         Prefix for list of file names
    --all               Select this option to export all the files

```

You could use Yarn / NPM to export the project assets. There are two options available to export assets:

1. Export the complete assets directory from './app/project/assets/' folder. To achieve this, use the below commands:

    yarn export:assets --all 

    npm run export:assets -- --all 

    yarn exp:a --all 

    npm run exp:a -- --all 

2. Export selected assets from './app/project/assets/' folder. In this case, the file names are mandatory in the command-prompt. The filename must follow the complete path starting from './app/project/assets/....'. Only file names can be provided here (no folder paths). Multiple file names can be provided in the command-prompt. To achieve this, use the below commands:

    yarn export:assets -l ./app/project/assets/data/translation/en.json ./app/project/assets/scss/_variables.scss

    npm run export:assets -- -l ./app/project/assets/data/translation/en.json ./app/project/assets/scss/_variables.scss

    yarn export:assets --list ./app/project/assets/data/translation/en.json ./app/project/assets/scss/_variables.scss

    npm run export:assets -- --list ./app/project/assets/data/translation/en.json ./app/project/assets/scss/_variables.scss

    yarn exp:a -l ./app/project/assets/data/translation/en.json ./app/project/assets/scss/_variables.scss

    npm run exp:a -- -l ./app/project/assets/data/translation/en.json ./app/project/assets/scss/_variables.scss

To access help, you need to execute `yarn export:assets --help` or `npm run export:assets -- --help`.

By default, the zip file is created inside the project 'dist' folder. This file has the naming convention of `exported-assets.zip`.

### Use Tab Completion Feature

#### Tab Completion on Windows:
The Windows Command Prompt does not allow you to use tab completion for commands and their options. However, it does support tab completion for folder and file names.

For example, we can open a Command Prompt, type cd D, and press Tab. As we are in our user folder by default, tab completion will automatically file in cd Desktop, so we can press Enter to change directories to our desktop directory. This also helps when trying to run a command on a specific file name.

Tab completion also works in PowerShell. It can be used to automatically fill in the name of a file path.

Unlike in the Bash Shell, Windows requires you press Tab multiple times to cycle through available options — it won’t just show you them all in a list. This applies to both the Command Prompt and PowerShell’s tab completion features.

#### Tab Completion on Linux:
Tab completion is especially useful when typing file names, directories, and paths. Rather than trying to type a long file name that may involve spaces and special characters you will need to properly escape, you can just start typing the beginning of the name and press Tab.

For example, if we have a long, complex file name beginning with the letter L, we just have to type L and press Tab to automatically complete it. If we had multiple file names beginning with L, we just need to type a bit more of the file’s name before pressing Tab again.

#### Tab Completion on Mac OS:
Mac OS X also includes the Bash shell, so tab completion works just like it does on Linux. Tap the tab key while typing a command, file path, or option — the shell will automatically fill in the rest or show you the available options you can type.

Any other operating system that uses the Bash shell will work the same. Tab completion features should also work similarly on many other shells on Unix-like systems.

### Change Configuration Parameters

All configuration parameters are available in the default.json file located at './shell-utilities/config/'

Parameters for "export:assets" are
- "requiredFolderPath": "./app/project/assets/" - This indicates the folder path of the project asset files.
- "templatesPath": "./shell-utilities/export-assets/templates/" - This indicates the path where  the shell-utilities templates can be found.
- "outputFileName": "exported-assets.zip" - This indicates the output zip file name.
- "outputTempFolderName": "Exported-Assets-Code-Folder-Temp" - This indicates the temporary path created for copying output files.
- "zipFolderName": "exported-assets", - This is the name of the folder inside outputTempFolderName where the zip file will be created.
- "zipFileDestinationPath": "./dist/", - This is the path where the zip file will be created. Ensure that it starts with './' and ends with '/'.
- "ignoreFilesFolders": Indicates all the files / folders that must not be included in the zip file.


### Copyright
Copyright (C) 2020 to the present, Crestron Electronics, Inc.

All rights reserved.

No part of this software may be reproduced in any form, machine
or natural, without the express written consent of Crestron Electronics.

Use of this source code is subject to the terms of the Crestron Software License Agreement 
under which you licensed this source code. 