using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class AltarOverride
    {
        private static string _altarTableName = "Altar";
        private static string _dungeonFeatureTableName = "DungeonFeatureName";
        public static void Name(ref string __result)
        {
            if (!DreamQuestConfig.IsEn)
            {
                __result = __result.Replace("Altar to ", "");//删除前面的 Altar to
                __result += TR.GetStr(_dungeonFeatureTableName, "Altar", "OUTFIX");
            }
        }

        public static void GodName(ref string __result)
        {
            __result = TR.GetStr(_altarTableName, __result);
        }

        public static void Hint(ref string __result)
        {

        }

        public static void CarvedMessage(ref string __result)
        {

        }
        public static void AcceptMessage(ref string __result)
        {

        }
        public static void SigilDescription(ref string __result)
        {

        }
    }
}
