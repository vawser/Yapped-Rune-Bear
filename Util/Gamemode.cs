using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yapped.Util
{
    internal class GameMode
    {
        public enum GameType
        {
            DemonsSouls,
            DarkSouls1,
            DarkSouls2,
            DarkSouls2Scholar,
            DarkSouls3,
            Bloodborne,
            DarkSoulsRemastered,
            Sekiro,
            EldenRing
        }

        public GameType Game { get; }
        public string Name { get; }
        public string Directory { get; }

        public GameMode(GameType game, string name, string directory)
        {
            Game = game;
            Name = name;
            Directory = directory;
        }

        public static readonly GameMode[] Modes =
        {
            new GameMode(GameType.DemonsSouls, "Demon's Souls", "DES"),
            new GameMode(GameType.DarkSouls1, "Dark Souls 1", "DS1"),
            new GameMode(GameType.DarkSouls2, "Dark Souls 2", "DS2"),
            new GameMode(GameType.DarkSouls2Scholar, "Dark Souls 2: Scholar of the First Sin", "DS2S"),
            new GameMode(GameType.DarkSouls3, "Dark Souls 3", "DS3"),
            new GameMode(GameType.Bloodborne, "Bloodborne", "BB"),
            new GameMode(GameType.DarkSoulsRemastered, "Dark Souls Remastered", "DS1R"),
            new GameMode(GameType.Sekiro, "Sekiro", "SDT"),
            new GameMode(GameType.EldenRing, "Elden Ring", "ER")
        };
    }
}

