# Hades Extender
Experimental project to extend the scripting capabilities of the game Hades.

## Debugging

To debug hades:
* Install the [vscode lua debugger](https://marketplace.visualstudio.com/items?itemName=devCAT.lua-debug)
* Create a workspace with vscode
* Add the folders `Hades/Content/Scripts` and `Hades/Content/Mods` to the workspace
* Create a launch.json config and add the following:
```json
{
    "name": "attach-hades",
    "type": "lua",
    "request": "attach",
    "workingDirectory": "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Hades",
    "sourceBasePath": "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Hades",
    "listenPublicly": false,
    "listenPort": 56789,
    "encoding": "UTF-8"
}
```
* Run the debugger named 'attach-hades' in vscode.
* Launch hades using HadesExtender
