using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class DungeonActionOverride
    {
        public static void ButtonNamePostfix(ref string __result,DungeonAction __instance)//地牢技能名称
        {
            if (__instance.GetType() == typeof(DungeonActionHoard)) {
                if (!DreamQuestConfig.IsEn)
                {
                    __result = TR.GetStr("DungeonAction", "Hoard").Replace(TR.PlaceHolder, (__instance as DungeonActionHoard).Cost().ToString());
                }
            }
            else
            {
                __result = TR.GetStr("DungeonAction", __result);
            }
        }
    }
}
