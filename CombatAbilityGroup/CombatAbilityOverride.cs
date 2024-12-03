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
        public static string Description()
        {
            return "I'm an ability!";
        }

        // Token: 0x06000F82 RID: 3970 RVA: 0x000459F0 File Offset: 0x00043BF0
        public static string Name()
        {
            return "Base";
        }

        // Token: 0x06000F83 RID: 3971 RVA: 0x000459F8 File Offset: 0x00043BF8
        public static void FailTextPostfix(ref string __result)
        {
            __result = __result.Replace("Cooldown Remaining", TR.GetStr(DungeonPhysicalOverride.TableKey, "Cooldown Remaining"));
        }
    }
}
