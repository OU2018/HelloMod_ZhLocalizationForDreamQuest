using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class MushroomOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {

            modCenter.PatchTargetPostfix(
                typeof(Mushroom).GetMethod("Name"),
                typeof(MushroomOverride).GetMethod("Name")
                );
            modCenter.PatchTargetPostfix(
                typeof(Mushroom).GetMethod("SpecificLongTextEffect"),
                typeof(MushroomOverride).GetMethod("SpecificLongTextEffect")
                );
            modCenter.PatchTargetPostfix(
                typeof(Mushroom).GetMethod("ShortTextEffects"),
                typeof(MushroomOverride).GetMethod("ShortTextEffects")
                );
            modCenter.PatchTargetPostfix(
                typeof(Mushroom).GetMethod("LongText"),
                typeof(MushroomOverride).GetMethod("LongText")
                );
            modCenter.PatchTargetPrefix(
                typeof(Mushroom).GetMethod("Open"),
                typeof(MushroomOverride).GetMethod("Open")
                );
        }

        public static void Name(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Mushroom");
        }

        public static void SpecificLongTextEffect(ref string __result, Mushroom __instance)
        {
            string text = string.Empty;
            MushroomEffect mushroomEffect = __instance.effect;
            text = HelloMod.Csv.GetTranslationByID("MushroomSpecificLongTextEffect", "_" + mushroomEffect.ToString()).Replace(TR.PlaceHolder, __instance.strength.ToString());
            __result = text;
        }

        public static void ShortTextEffects(ref string __result, Mushroom __instance)
        {
            string text = string.Empty;
            MushroomEffect mushroomEffect = __instance.effect;
            text = HelloMod.Csv.GetTranslationByID("MushroomShortTextEffects", "_" + mushroomEffect.ToString()).Replace(TR.PlaceHolder, __instance.strength.ToString());
            __result = text;
        }

        public static void LongText(ref string __result, Mushroom __instance)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_mushroom_longtext") + __instance.SpecificLongTextEffect();
        }

        public static bool Open(Mushroom __instance)
        {
            if (!__instance.mushroomPatch.mushroomText)
            {
                BasicFeatureText basicFeatureText = new BasicFeatureText(__instance.dungeon, __instance.Name(), __instance.LongText(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Yuck!"), __instance.MushroomTextFinish, false);
                basicFeatureText.Build();
            }
            else
            {
                Vector3 vector = __instance.physical.transform.position + new Vector3(0.2f, 0.35f, (float)0);
                __instance.dungeon.player.LocationText(__instance.ShortTextEffects(), 3, __instance.ShortTextColor(), vector);
                int num = __instance.mushroomPatch.MaxMushrooms() - __instance.mushroomPatch.mushroomsLeft + 1;
                __instance.dungeon.player.LocationText(num + "/" + __instance.mushroomPatch.MaxMushrooms(), 3, Color.black, vector - new Vector3((float)0, 0.35f, (float)0));
                __instance.DoAndFinish();
            }
            return false;
        }
    }
}
