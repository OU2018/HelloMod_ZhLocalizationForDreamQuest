using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class ShopDialogueTalentViewerOverride
    {
        private static string currentTalentName = string.Empty;
        public static void TalentNamePostfix(ref string __result,ShopDialogueTalentViewer __instance)
        {
            if (!string.IsNullOrEmpty(__result))
            {
                currentTalentName = __result;
                __result = TR.GetStr("TalentName", __result);
            }
        }

        public static void TalentEffectPostfix(ref string __result, ShopDialogueTalentViewer __instance)
        {
            if (!string.IsNullOrEmpty(__result))
            {
                __instance.TalentName();
                __result = HelloMod.Csv.GetTranslationByID("TalentEffect", "_" + currentTalentName);
            }
        }
        public static void TalentRepeatablePostfix(ref string __result, ShopDialogueTalentViewer __instance)
        {
            if (!string.IsNullOrEmpty(__result))
            {
                __result = TR.GetStr(DungeonPhysicalOverride.TableKey, __result); ;
            }
        }
    }
}
