using System.Collections.Generic;
using EnsoulSharp;
using EnsoulSharp.SDK.MenuUI;

namespace Advanced_Turn_Around
{
    internal class Variable
    {
        public enum MovementDirection
        {
            Forward = 1,
            Backward = 2
        }

        public static readonly List<Internal.ChampionInfo> ExistingChampions = new List<Internal.ChampionInfo>();
        public static Menu Config;
        public static AIHeroClient Player = ObjectManager.Player;
    }
}