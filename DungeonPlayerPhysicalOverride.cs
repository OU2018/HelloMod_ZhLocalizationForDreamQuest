using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class DungeonPlayerPhysicalOverride
    {
        //TODO:等级字符串
        public static void LevelString(ref string __result, DungeonPlayerPhysical __instance)
        {
            __result = TR.GetStr("DungeonFeatureName", "Level ") + __instance.player.level;
        }

        public static void DungeonPlayer_GetName(ref string __result,DungeonPlayer __instance)
        {
            
        }
    }
}
