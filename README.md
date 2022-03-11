# Yapped - Rune Bear
An editor for Elden Ring param files, which determine the properties of items, attacks, effects, enemies, objects, and a whole lot more. 

Requires .NET 4.7.2 - Windows 10 users should already have this.

Credit to JKAnderson for the [original version](https://github.com/JKAnderson/Yapped). 

# Usage

To edit params and have them used by the game, you need to first unpack and patch the game with UXM.

Once unpacked, open the regulation.bin file found in the ELDEN RING\Game with Yapped.

# Issues
Note that the layouts are incomplete, so using altered params in-game may result in bugged behavior:

Current known bugs:
 - Saving the EquipParamGoods will result in bugged behavior with Torrent and item UI notifications.
  - You can workaround this by ensuring the "Params to Ignore" setting has "EquipParamGoods" in its setting string.

# Warning

As far as we know, in Elden Ring any edits to the regulation file (where params are stored) will trigger anticheat, including simply opening it and resaving it.

Only use modified params in offline mode. Back up your save file and restore it before going online again if you're doing anything that could affect it.
