using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class MonasteryOverride:DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(Monastery).GetMethod("Title"),
                typeof(MonasteryOverride).GetMethod("Title"));
            modCenter.PatchTargetPostfix(
                typeof(Monastery).GetMethod("Text"),
                typeof(MonasteryOverride).GetMethod("Text"));
            modCenter.PatchTargetPrefix(
                typeof(Monastery).GetMethod("DisplayInMini"),
                typeof(MonasteryOverride).GetMethod("DisplayInMini")
                );
            modCenter.PatchTargetPrefix(
                typeof(Monastery).GetMethod("Enter"),
                typeof(MonasteryOverride).GetMethod("Enter")
                );
            modCenter.PatchTargetPrefix(
                typeof(Monastery).GetMethod("ConstructDeckViewer"),
                typeof(MonasteryOverride).GetMethod("ConstructDeckViewer")
                );
        }
        public static void Title(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Monastery");
        }

        public static void Text(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_monastery_content");
        }

        public static bool DisplayInMini(Monastery __instance)
        {
            DungeonPlayerPhysical physical = __instance.dungeon.player.physical;
            float x = physical.renderer.bounds.size.x;
            float y = physical.renderer.bounds.size.y;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Title(), 32, Color.white);
            ShopDialogueClickableIcon shopDialogueClickableIcon = SDB.ClickableIcon(new Vector2(x * 0.5f, x * 0.5f), __instance.Portrait(), string.Empty);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(x * 0.6f, 0.35f), TR.GetStr(DungeonPhysicalOverride.TableKey, "Enter"), __instance.Enter);
            shopDialogueButton.FontSize(24);
            shopDialogueButton.ColliderMod(1.2f, 1.5f);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueClickableIcon }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", 0.2f);
            shopDialogueAligned.CenterTo(new Vector3(physical.transform.position.x, physical.transform.position.y - 0.35f * y, physical.transform.position.z + 0.2f));
            physical.miniDisplay = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }

        public static bool Enter(Monastery __instance)
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
                if (GameManager.IsIPhone())
                {
                    num = 8;
                }
                ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Title(), (int)((float)48 + (float)num * 1.5f), Color.black);
                ShopDialogueText shopDialogueText = SDB.Text(x, __instance.Text(), 32 + num, Color.black);
                SDB.Background(shopDialogueText);
                ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), ButtonText(__instance), __instance.ResolveMonastery, __instance.CanAfford);
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

        public static string ButtonText(Monastery __instance)
        {
            return (__instance.uses != 0) ?
                (TR.GetStr(DungeonPhysicalOverride.TableKey, "Forget (")
                + __instance.GetCost() +
                TR.GetStr(DungeonPhysicalOverride.TableKey, " Gold)")) :
                TR.GetStr(DungeonPhysicalOverride.TableKey, "Forget (Free!)");
        }

        public static bool ConstructDeckViewer(Monastery __instance)
        {
            __instance.dungeon.activeDungeonFeature = __instance;
            __instance.dungeon.DeckViewer(TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose a card to permanently delete"), DeckButtons(__instance), __instance.CancelDeckViewer);
            return false;
        }

        public static List<ShopDialogueButton> DeckButtons(Monastery __instance)
        {
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Confirm"), __instance.ConfirmPressed, __instance.AllowDone);
            return new List<ShopDialogueButton> { shopDialogueButton };
        }
    }

}
