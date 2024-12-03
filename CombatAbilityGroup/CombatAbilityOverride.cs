using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod.CombatAbilityGroup
{
    internal class CombatAbilityOverride:CombatAbilityPatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(CombatAbility).GetMethod("FailText"),
                typeof(CombatAbilityOverride).GetMethod("FailTextPostfix"));
            /*modCenter.PatchTargetPrefix(
                typeof(CombatAbility).GetMethod("Open"),
                typeof(CombatAbilityOverride).GetMethod("Open")
                );*/
        }

        public static void FailTextPostfix(ref string __result)
        {
            __result = __result.Replace("Cooldown Remaining", TR.GetStr(DungeonPhysicalOverride.TableKey, "Cooldown Remaining"));
        }
    }
}
