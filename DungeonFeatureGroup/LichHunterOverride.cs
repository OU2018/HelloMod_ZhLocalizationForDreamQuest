using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class LichHunterOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(LichHunter).GetMethod("Name"),
                typeof(LichHunterOverride).GetMethod("Name"));
            modCenter.PatchTargetPostfix(
                typeof(LichHunter).GetMethod("GenerateText"),
                typeof(LichHunterOverride).GetMethod("GenerateText"));
            modCenter.PatchTargetPrefix(
                typeof(LichHunter).GetMethod("DisplayInMini"),
                typeof(LichHunterOverride).GetMethod("DisplayInMini")
                );
            modCenter.PatchTargetPrefix(
                typeof(LichHunter).GetMethod("Investigate"),
                typeof(LichHunterOverride).GetMethod("Investigate")
                );
        }

        public static void Name(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Crazy Old Man");
        }

        public static void GenerateText(HealingPool __instance)
        {
            __instance.text = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_lichhunter_content");
        }

        public static bool DisplayInMini(LichHunter __instance)
        {
            DungeonPlayerPhysical physical = __instance.dungeon.player.physical;
            float x = physical.renderer.bounds.size.x;
            float y = physical.renderer.bounds.size.y;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), 32, Color.white);
            ShopDialogueClickableIcon shopDialogueClickableIcon = SDB.ClickableIcon(new Vector2(x * 0.5f, x * 0.5f), __instance.Portrait(), string.Empty);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(x * 0.6f, 0.35f), TR.GetStr(_dungeonFeatureTableName, "Talk"), __instance.Investigate);
            shopDialogueButton.FontSize(24);
            shopDialogueButton.ColliderMod(1.2f, 1.5f);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueClickableIcon }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", 0.2f);
            shopDialogueAligned.CenterTo(new Vector3(physical.transform.position.x, physical.transform.position.y - 0.35f * y, physical.transform.position.z + 0.2f));
            physical.miniDisplay = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }
        public static bool Investigate(LichHunter __instance)
        {
            __instance.EnteringShop();
            int num = 0;
            if (GameManager.IsIPhone())
            {
                num = 8;
            }
            float x = __instance.dungeon.physical.WindowSize().x;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), (int)((float)48 + (float)num * 1.5f), Color.black);

            __instance.GenerateText();
            ShopDialogueText shopDialogueText = SDB.Text(x, __instance.text, 32 + num, Color.black);
            SDB.Background(shopDialogueText);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(_dungeonFeatureTableName, "Learn"), __instance.Learn);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueText }, "VP", 0.1f);
            ShopDialogueCard shopDialogueCard = SDB.Card(__instance.dungeon.physical.PreferredCardWidth(), "LichKick");
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueCard }, "VP", 0.7f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", (float)1);
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
            SDB.CancelButton(shopDialogueAligned, __instance.dungeon.WindowBack);
            shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }
    }
}
