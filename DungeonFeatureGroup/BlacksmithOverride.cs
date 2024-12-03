using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class BlacksmithOverride:DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(Blacksmith).GetMethod("Title"),
                typeof(BlacksmithOverride).GetMethod("Title"));
            modCenter.PatchTargetPostfix(
                typeof(Blacksmith).GetMethod("Text"),
                typeof(BlacksmithOverride).GetMethod("Text"));
            modCenter.PatchTargetPrefix(
                typeof(Blacksmith).GetMethod("DisplayInMini"),
                typeof(BlacksmithOverride).GetMethod("DisplayInMini")
                );
            modCenter.PatchTargetPrefix(
                typeof(Blacksmith).GetMethod("ConstructDeckViewer"),
                typeof(BlacksmithOverride).GetMethod("ConstructDeckViewer")
                );
            modCenter.PatchTargetPrefix(
                typeof(Blacksmith).GetMethod("Enter"),
                typeof(BlacksmithOverride).GetMethod("Enter")
                );
        }
        public static void Title(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Blacksmith");
        }

        // Token: 0x060013DB RID: 5083 RVA: 0x0005A640 File Offset: 0x00058840
        public static void Text(ref string __result)
        {

            __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_blacksmith_content");
        }

        // Token: 0x060013DC RID: 5084 RVA: 0x0005A648 File Offset: 0x00058848
        public static bool DisplayInMini(Blacksmith __instance)
        {
            DungeonPlayerPhysical physical = __instance.dungeon.player.physical;
            float x = physical.renderer.bounds.size.x;
            float y = physical.renderer.bounds.size.y;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Title(), 32, Color.white);
            ShopDialogueClickableIcon shopDialogueClickableIcon = SDB.ClickableIcon(new Vector2(x * 0.5f, x * 0.5f), __instance.Portrait(), string.Empty);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(x * 0.6f, 0.35f), TR.GetStr(DungeonPhysicalOverride.TableKey, "Use"), __instance.Enter
                );
            shopDialogueButton.ColliderMod(1.2f, 1.5f);
            shopDialogueButton.FontSize(24);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueClickableIcon }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", 0.2f);
            shopDialogueAligned.CenterTo(new Vector3(physical.transform.position.x, physical.transform.position.y - 0.35f * y, physical.transform.position.z + 0.2f));
            physical.miniDisplay = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }

        // Token: 0x060013DD RID: 5085 RVA: 0x0005A7C4 File Offset: 0x000589C4
        public static bool Enter(Blacksmith __instance)
        {
            __instance.EnteringShop();
            if (__instance.ShouldSmash())
            {
                __instance.SmashPage();
            }
            else
            {
                float x = __instance.dungeon.physical.WindowSize().x;
                int num = 0;
                ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Title(), (int)((float)48 + (float)num * 1.5f), Color.black);
                ShopDialogueText shopDialogueText = SDB.Text(x, __instance.Text(), 32 + num, Color.black);
                SDB.Background(shopDialogueText);
                Vector2 vector = __instance.dungeon.physical.DefaultButtonSize();
                vector.x += 0.2f;
                vector *= 1.2f;
                ShopDialogueButton shopDialogueButton = SDB.BasicButton(vector, ButtonText(__instance), __instance.ResolveBlacksmith, __instance.CanAfford);
                ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueText }, "VP", 0.1f);
                shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", (float)1);
                SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
                SDB.CancelButton(shopDialogueAligned, __instance.dungeon.WindowBack);
                __instance.AddGoldTracker(shopDialogueAligned);
                shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation());
                __instance.dungeon.activeShop = shopDialogueAligned;
                shopDialogueAligned.DoneBuilding();
            }
            return false;
        }

        public static string ButtonText(Blacksmith __instance)
        {
            return (__instance.uses != 0) ? 
                (TR.GetStr(DungeonPhysicalOverride.TableKey, "Upgrade (", "BLACKSMITH")
                + __instance.GetCost() + 
                TR.GetStr(DungeonPhysicalOverride.TableKey, " Gold)")) : 
                TR.GetStr(DungeonPhysicalOverride.TableKey, "Upgrade (Free!)", "BLACKSMITH");
        }

        public static bool ConstructDeckViewer(Blacksmith __instance)
        {
            __instance.dungeon.activeDungeonFeature = __instance;
            __instance.dungeon.DeckViewer(TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose a card to upgrade"), DeckButtons(__instance), __instance.CancelDeckViewer, __instance.AllowOnlyUpgradable);
            return false;
        }

        public static List<ShopDialogueButton> DeckButtons(Blacksmith __instance)
        {
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Confirm"), __instance.ConfirmPressed, __instance.AllowDone);
            return new List<ShopDialogueButton> { shopDialogueButton };
        }

        
    }
}
