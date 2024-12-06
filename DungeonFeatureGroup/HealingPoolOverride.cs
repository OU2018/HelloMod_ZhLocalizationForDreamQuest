using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class HealingPoolOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(HealingPool).GetMethod("Name"),
                typeof(HealingPoolOverride).GetMethod("Name"));
            modCenter.PatchTargetPostfix(
                typeof(HealingPool).GetMethod("FirstText"),
                typeof(HealingPoolOverride).GetMethod("FirstText"));
            modCenter.PatchTargetPostfix(
                typeof(HealingPool).GetMethod("GenerateText"),
                typeof(HealingPoolOverride).GetMethod("GenerateText"));
            modCenter.PatchTargetPrefix(
                typeof(HealingPool).GetMethod("DisplayInMini"),
                typeof(HealingPoolOverride).GetMethod("DisplayInMini")
                );
            modCenter.PatchTargetPrefix(
                typeof(HealingPool).GetMethod("Investigate"),
                typeof(HealingPoolOverride).GetMethod("Investigate")
                );
        }
        public static void Name(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Healing Pool");
        }

        public static void GenerateText(HealingPool __instance)
        {
            __instance.text = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_healingpool_content");
        }

        public static bool DisplayInMini(HealingPool __instance)
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

        public static void FirstText(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_healingpool_firstText");
        }

        public static bool Investigate(HealingPool __instance)
        {
            __instance.EnteringShop();
            if (__instance.ShouldSmash())
            {
                __instance.SmashPage();
            }
            else
            {
                int num = 0;
                float x = __instance.dungeon.physical.WindowSize().x;
                ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), (int)((float)48 + (float)num * 1.5f), Color.black);
                __instance.GenerateText();
                string text = __instance.text;
                if (__instance.first)
                {
                    text += "\n" + __instance.FirstText();
                }
                ShopDialogueText shopDialogueText = SDB.Text(x, __instance.text, 32 + num, Color.black);
                SDB.Background(shopDialogueText);
                ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize() * 1.2f, TR.GetStr(DungeonPhysicalOverride.TableKey, "Heal (") + __instance.HealCost() + TR.GetStr(DungeonPhysicalOverride.TableKey, " Gold)"), __instance.Heal, __instance.CanAffordHeal);
                ShopDialogueButton shopDialogueButton2 = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize() * 1.2f, GenerateFullHealName(__instance), __instance.FullHeal,__instance.CanAffordFullHeal);
                ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueButton[] { shopDialogueButton, shopDialogueButton2 }, "HP", (float)2);
                ShopDialogueAligned shopDialogueAligned2 = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueText }, "VP", 0.1f);
                shopDialogueAligned2 = SDB.Align(new ShopDialogueAligned[] { shopDialogueAligned2, shopDialogueAligned }, "VP", 0.4f);
                SDB.Background(shopDialogueAligned2, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
                SDB.CancelButton(shopDialogueAligned2, __instance.dungeon.WindowBack);
                __instance.AddGoldTracker(shopDialogueAligned2);
                shopDialogueAligned2.UpperCenterTo(__instance.dungeon.ShopLocation());
                __instance.dungeon.activeShop = shopDialogueAligned2;
                shopDialogueAligned2.DoneBuilding();
            }
            return false;
        }

        public static string FullHealName(HealingPool __instance)
        {
            return (!__instance.first) ? (__instance.FullHealCost() + TR.GetStr(DungeonPhysicalOverride.TableKey, " Gold")) : TR.GetStr(DungeonPhysicalOverride.TableKey, "Free!");
        }
        public static string GenerateFullHealName(HealingPool __instance)
        {
		    return TR.GetStr(DungeonPhysicalOverride.TableKey, "Heal Max (") + FullHealName(__instance) + ")";
	    }
    }
}
