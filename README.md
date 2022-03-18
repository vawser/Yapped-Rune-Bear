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
- Field Exporter: this is used to export field value data for a specific field. Useful for generating lists.
- Field Adjuster: this is used to quickly change field values for a specific field. Useful for simple mass edits.
- Value Reference Finder: this is used to find rows that have fields that contain the specified value.
- Row Reference Finder: this is used to find rows whose ID matches the specified value.

# Issues
Note that the layouts are incomplete, so using altered params in-game may result in unintended behavior.

# Warning

As far as we know, in Elden Ring any edits to the regulation file (where params are stored) will trigger anticheat, including simply opening it and resaving it.

Only use modified params in offline mode. Back up your save file and restore it before going online again if you're doing anything that could affect it.
