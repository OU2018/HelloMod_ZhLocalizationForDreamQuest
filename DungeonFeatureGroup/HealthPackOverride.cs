using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class HealthPackOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            //modCenter.PatchTargetPostfix();
            //modCenter.PatchTargetPrefix();
            modCenter.PatchTargetPostfix(
                typeof(HealthPack).GetMethod("Name"),
                typeof(HealthPackOverride).GetMethod("Name"));
            modCenter.PatchTargetPrefix(
                typeof(HealthPack).GetMethod("Open"),
                typeof(HealthPackOverride).GetMethod("Open")
                );
        }

        public static void Name(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Bandage");//Bandage
        }
        //TriggerMoveHere ==> Open ==> Finished ==> Destory
        public static bool Open(HealthPack __instance)
        {
            Vector3 vector = __instance.physical.transform.position + new Vector3(0.2f, 0.35f, (float)0);
            //
            __instance.dungeon.player.LocationText(TR.GetStr(_dungeonFeatureTableName, "Heal ") + __instance.health, 3, Utility.darkGreen, vector);
            __instance.dungeon.player.Heal(__instance.health);
            __instance.Finished();
            return false;
        }
    }
}
