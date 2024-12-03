using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod.DungeonFeatureGroup
{
    internal abstract class DungeonFeaturePatch
    {
        protected static string _dungeonFeatureTableName = "DungeonFeatureName";

        public abstract void Patch(Harmony harmony,HelloMod modCenter);
    }
}
