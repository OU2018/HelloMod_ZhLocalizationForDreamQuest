using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class StairOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(Stair).GetMethod("Name"),
                typeof(StairOverride).GetMethod("Name"));
            modCenter.PatchTargetPostfix(
                typeof(Stair).GetMethod("GenerateText"),
                typeof(StairOverride).GetMethod("GenerateText"));
            modCenter.PatchTargetPrefix(
                typeof(Stair).GetMethod("DisplayInMini"),
                typeof(StairOverride).GetMethod("DisplayInMini"));
            modCenter.PatchTargetPrefix(
                typeof(Stair).GetMethod("Investigate"),
                typeof(StairOverride).GetMethod("Investigate"));
            modCenter.PatchTargetPrefix(
                typeof(Stair).GetMethod("DoExplorationBonus"),
                typeof(StairOverride).GetMethod("DoExplorationBonus"));
        }

        public static void Name(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Stairs Down");
        }

        public static void GenerateText(Stair __instance)
        {
            __instance.text = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_stairs_content");
            if (__instance.dungeon.MaxDepth() == __instance.dungeon.board.depth)
            {
                __instance.text = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_stairs_maxdepth_content");
            }
        }

        public static bool DisplayInMini(Stair __instance)
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

        public static bool Investigate(Stair __instance)
        {
            __instance.EnteringShop();
            float x = __instance.dungeon.physical.WindowSize().x;
            int num = 0;
            if (GameManager.IsIPhone())
            {
                num = 8;
            }
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), (int)((float)48 + (float)num * 1.5f), Color.black);
            ShopDialogueText shopDialogueText = SDB.Text(x, __instance.text, 32 + num, Color.black);
            SDB.Background(shopDialogueText);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(_dungeonFeatureTableName, "Complete Level"), __instance.TakeStairs);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueText }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", (float)1);
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
            SDB.CancelButton(shopDialogueAligned, __instance.dungeon.WindowBack);
            shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }
        //完全不知道会用在哪里的方法
        public static bool DoExplorationBonus(Stair __instance)
        {
            int num = __instance.dungeon.board.DarkSquares();
            int num2 = __instance.BonusValue();
            int num3 = __instance.BonusValue();
            string text = "Floor " + __instance.dungeon.board.depth + " Complete!";
            string text2 = "You have " + num + " unexplored tiles. \n You gain " + num2 + " experience and " + num3 + " gold.";
            if (!DreamQuestConfig.IsEn)
            {
                text = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_stairs_DoExplorationBonus1").Replace("{value}", __instance.dungeon.board.depth.ToString());
                text2 = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_stairs_DoExplorationBonus2")
                    .Replace("{num}", num.ToString())
                    .Replace("{num2}", num2.ToString())
                    .Replace("{num3}", num3.ToString());
            }
            int num4 = 5;
            ShopDialogueText shopDialogueText = SDB.CenteredText((float)num4, text, 48, Color.black);
            ShopDialogueText shopDialogueText2 = SDB.CenteredText((float)num4, text2, 36, Color.black);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(_dungeonFeatureTableName, "Great!"), __instance.ExploreComplete);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueText[] { shopDialogueText, shopDialogueText2 }, "VP", 0.3f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", (float)1);
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
            shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueAligned;
            return false;
        }

    }
}
