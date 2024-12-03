using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod.CombatAbilityGroup
{
    internal class CombatAbilityNormalPatch
    {
        private static string enName = string.Empty;
        public static void DescriptionPostfix(ref string __result,CombatAbility __instance)
        {
            __instance.Name();
            __result = HelloMod.Csv.GetTranslationByID("CombatAbilityDescription", "_" + enName);
        }

        public static void NamePostfix(ref string __result)
        {
            enName = __result;
            __result = TR.GetStr("CombatAbilityName", __result);
        }
    }
}
