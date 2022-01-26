using System;
using System.Reflection;
using BepInEx;
using UnboundLib;

namespace LobbyCodes
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class LobbyCodes : BaseUnityPlugin
    {
        private const string ModId = "com.rounds.LpbbyCodes";
        private const string ModName = "Lobby Codes";
        public const string Version = "1.0.0"; // What version are we on (major.minor.patch)?

        public static LobbyCodes instance { get; private set; }

        void Start()
        {
            instance = this;
        }
    }
}
