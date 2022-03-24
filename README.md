# Yapped - Rune Bear
An editor for Elden Ring param files, which determine the properties of items, attacks, effects, enemies, objects, and a whole lot more. 

Requires .NET 4.7.2 - Windows 10 users should already have this.

Credit to JKAnderson for the [original version](https://github.com/JKAnderson/Yapped). 

# Usage

To edit params and have them used by the game, you need to first unpack and patch the game with UXM.

Once unpacked, open the regulation.bin file found in the ELDEN RING\Game with Yapped.

Tools:
- Import Names: this is used to import row names to populate the row name fields. Do this if you are starting from a fresh regulation.bin file.
- Export Names: this is used to export rows names to the source textfiles.
- Import Data: this is used to import row and cell data from a .csv file. Note this is a total import, so it replaces all existing data and can take a while for long param files.
- Export Data: this is used to export row and cell data into a .csv file.
- Mass Import Data: this will import row and cell data for all params if they have an existing .csv file present.
- Mass Export Data: this will export row and cell data into a .csv file for all params.
- Field Exporter: this is used to export field value data for a specific field. Useful for generating lists.
- Field Adjuster: this is used to quickly change field values for a specific field. Useful for simple mass edits.
- Value Reference Finder: this is used to find rows that have fields that contain the specified value.
- Row Reference Finder: this is used to find rows whose ID matches the specified value.

# Updating a Mod
If you want to update a mod to the latest patch without re-building it from scratch, you can do the following:
- Open the vanilla regulation.bin from Elden Ring/Game/ in Yapped.
- Change the Project Name to "Vanilla".
- Click "Mass Export Data". Wait for it to finish.
- Open your mod's regulation.bin.
- Change the Project Name to the name of your mod, for example "Great-Rune".
- Click "Mass Export Data". Wait for it to finish.
- You can now use a file diff tool such as WinMerge or K-Diff to quickly check which rows are different between vanilla's and your mod's CSV files. Merge in those that are not changed on your side, and choose whether to merge in those that are.
- Once you have merged in the differences from vanilla into your mod's CSV files, open Yapped and load your mod's regulation.bin
- Make sure the Project Name is the name of your mod, and click "Mass Import Data".
- This will import data from the merged CSV files you just changed.

# Issues
Note that the layouts are incomplete, so using altered params in-game may result in unintended behavior.

# Warning

As far as we know, in Elden Ring any edits to the regulation file (where params are stored) will trigger anticheat, including simply opening it and resaving it.

Only use modified params in offline mode. Back up your save file and restore it before going online again if you're doing anything that could affect it.
