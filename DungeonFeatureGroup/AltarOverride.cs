using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class AltarOverride:DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                  typeof(Altar).GetMethod("Name"),
                  typeof(AltarOverride).GetMethod("Name"));
            HelloMod.PostPatchVirtualMethodAndOverrides(harmony, typeof(Altar), "GodName",
                  typeof(AltarOverride).GetMethod("GodName"));
            HelloMod.PostPatchVirtualMethodAndOverrides(harmony, typeof(Altar), "Hint",
                  typeof(AltarOverride).GetMethod("Hint"));
            HelloMod.PostPatchVirtualMethodAndOverrides(harmony, typeof(Altar), "CarvedMessage",
                  typeof(AltarOverride).GetMethod("CarvedMessage"));
            HelloMod.PostPatchVirtualMethodAndOverrides(harmony, typeof(Altar), "AcceptMessage",
                  typeof(AltarOverride).GetMethod("AcceptMessage"));
            HelloMod.PostPatchVirtualMethodAndOverrides(harmony, typeof(Altar), "SigilDescription",
                  typeof(AltarOverride).GetMethod("SigilDescription"));

            modCenter.PatchTargetPrefix(
                  typeof(Altar).GetMethod("DisplayInMini"),
                  typeof(AltarOverride).GetMethod("DisplayInMini"));
        }
        private static string _altarTableName = "Altar";
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

        public static void Hint(ref string __result, Altar __instance)
        {

        }

        public static void CarvedMessage(ref string __result, Altar __instance)
        {

        }
        public static void AcceptMessage(ref string __result, Altar __instance)
        {

        }
        public static void SigilDescription(ref string __result, Altar __instance)
        {

        }

        public static void InitialQueryText(ref string __result, Altar __instance)
        {
            __result = "A rough grey altar leans against the wall.  Dried blood cakes the cracked and pitted stone, obscured by a thin layer of dust.  Above, etched into the wall, sits a sigil, " + __instance.SigilDescription() + "  Beneath it is a carved message: \n " + __instance.CarvedMessage();
        }

        public static void AcceptText(ref string __result,Altar __instance)
        {
            __result = "The sigil above the altar glows as you invoke its god's name. " + __instance.AcceptMessage();
        }

        public static bool DisplayInMini(Altar __instance)
        {
            DungeonPlayerPhysical physical = __instance.dungeon.player.physical;
            float x = physical.renderer.bounds.size.x;
            float y = physical.renderer.bounds.size.y;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), 32, Color.white);
            ShopDialogueClickableIcon shopDialogueClickableIcon = SDB.ClickableIcon(new Vector2(x * 0.5f, x * 0.5f), __instance.Portrait(), string.Empty);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(x * 0.6f, 0.35f), TR.GetStr(_dungeonFeatureTableName, "Investigate"), __instance.Investigate);
            shopDialogueButton.FontSize(24);
            shopDialogueButton.ColliderMod(1.2f, 1.5f);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueClickableIcon }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", 0.2f);
            shopDialogueAligned.CenterTo(new Vector3(physical.transform.position.x, physical.transform.position.y - 0.35f * y, physical.transform.position.z + 0.2f));
            physical.miniDisplay = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }

        public static bool Investigate(Altar __instance)
        {
            __instance.EnteringShop();
            if (__instance.ShouldSmash())
            {
                __instance.SmashPage();
            }
            else
            {
                string text = __instance.InitialQueryText();
                DungeonUser dungeonUser = GameManager.LoadCurrentUser();
                bool flag = false;
                if (dungeonUser != null)
                {
                    flag = dungeonUser.KnownAltar(__instance.MyBoon());
                }
                if (flag)
                {
                    text += "\n\nA faint memory tugs at your mind.  " + __instance.Hint();
                }
                BasicFeatureText basicFeatureText = new BasicFeatureText(__instance.dungeon, __instance.Name(), text, "Invoke", __instance.InvokeName, true);
                basicFeatureText.Build();
            }
            return false;
        }

        public static bool InvokeName(Altar __instance)
        {
            __instance.dungeon.player.GainBoon(__instance.MyBoon());
            DungeonUser dungeonUser = GameManager.LoadCurrentUser();
            if (dungeonUser != null)
            {
                dungeonUser.UseAltar(__instance.MyBoon());
            }
            __instance.dungeon.WindowBack();
            BasicFeatureText basicFeatureText = new BasicFeatureText(__instance.dungeon, __instance.Name(), __instance.AcceptText(), "Ok", __instance.Finished, false);
            basicFeatureText.Build();
            __instance.dungeon.player.stats.VisitAltar();
            return false;
        }
    }
}
