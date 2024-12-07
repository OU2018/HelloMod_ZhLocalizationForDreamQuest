using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class TrapOverride:DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(Trap).GetMethod("Title"),
                typeof(TrapOverride).GetMethod("Title")
                );
            modCenter.PatchTargetPostfix(
                typeof(Trap).GetMethod("Text"),
                typeof(TrapOverride).GetMethod("Text")
                );
            modCenter.PatchTargetPostfix(
                typeof(Trap).GetMethod("ButtonText"),
                typeof(TrapOverride).GetMethod("ButtonText")
                );

            modCenter.PatchTargetPrefix(
                typeof(Trap).GetMethod("DisplayTrap"),
                typeof(TrapOverride).GetMethod("DisplayTrap")
                );
        }
        public static void Title(ref string __result, Trap __instance)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Spike Trap");
        }

        public static void Text(ref string __result, Trap __instance)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_trap_text");
        }

        public static bool DisplayTrap(Trap __instance)
        {
            float x = __instance.dungeon.physical.WindowSize().x;
            int num = 0;
            if (GameManager.IsIPhone())
            {
                num = 8;
            }
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Title(), (int)((float)48 + (float)num * 1.5f), Color.black);
            ShopDialogueText shopDialogueText = SDB.Text(x, __instance.Text(), 32 + num, Color.black);
            SDB.Background(shopDialogueText);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), __instance.ButtonText(), __instance.ResolveTrap);
            if (GameManager.IsIPhone())
            {
                shopDialogueButton.FontSize(36);
            }
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueText }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", 0.3f);
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
            shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }

        public static void ButtonText(ref string __result, Trap __instance)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Ouch");
        }
    }
}
