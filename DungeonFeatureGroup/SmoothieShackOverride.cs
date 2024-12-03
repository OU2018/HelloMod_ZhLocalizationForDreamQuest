using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace HelloMod.DungeonFeatureGroup
{
    internal class SmoothieShackOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(SmoothieShack).GetMethod("Name"),
                typeof(SmoothieShackOverride).GetMethod("Name"));
            modCenter.PatchTargetPostfix(
                typeof(SmoothieShack).GetMethod("GenerateText"),
                typeof(SmoothieShackOverride).GetMethod("GenerateText"));
            modCenter.PatchTargetPrefix(
                typeof(SmoothieShack).GetMethod("DisplayInMini"),
                typeof(SmoothieShackOverride).GetMethod("DisplayInMini"));
            modCenter.PatchTargetPrefix(
                typeof(SmoothieShack).GetMethod("Enter"),
                typeof(SmoothieShackOverride).GetMethod("Enter"));
        }

        public static void Name(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Linda's Lemonade");
        }
        public static void GenerateText(SmoothieShack __instance)
        {
            __instance.text = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_smoothieShack_content");
        }

        public static bool DisplayInMini(SmoothieShack __instance)
        {
            DungeonPlayerPhysical physical = __instance.dungeon.player.physical;
            float x = physical.renderer.bounds.size.x;
            float y = physical.renderer.bounds.size.y;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), 32, Color.white);
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

        public static bool Enter(SmoothieShack __instance)
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
                ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), (int)((float)48 + (float)num * 1.5f), Color.black);
                __instance.GenerateText();
                ShopDialogueText shopDialogueText = SDB.Text(x, __instance.text, 32 + num, Color.black);
                SDB.Background(shopDialogueText);

                ShopDialogueObject shopDialogueObject = BuildLootObjects(x * 0.95f, __instance);
                ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueText }, "VP", 0.1f);
                if (shopDialogueObject)
                {
                    shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueObject }, "VP", 0.4f);
                }
                SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
                SDB.CancelButton(shopDialogueAligned, __instance.dungeon.WindowBack);
                __instance.AddGoldTracker(shopDialogueAligned);
                shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation());
                __instance.dungeon.activeShop = shopDialogueAligned;
                shopDialogueAligned.DoneBuilding();
            }
            return false;
        }

        public static ShopDialogueObject BuildLootObjects(float width, SmoothieShack __instance)
        {
            float num = __instance.dungeon.physical.PreferredCardWidth();
            ShopDialogueObject[] array = new ShopDialogueObject[3];
            ShopDialogueObject[] array2 = new ShopDialogueObject[3];
            int num2 = 0;
            int i = 0;
            string[] array3 = new string[] { "AdditionalHealth", "AdditionalMana", "AdditionalExp" };
            int length = array3.Length;
            while (i < length)
            {
                ShopDialogueCard shopDialogueCard = SDB.Card(num, array3[i]);
                array[num2] = shopDialogueCard;
                if (shopDialogueCard.card is BonusCard)
                {
                    string sddtText = shopDialogueCard.card.cardName;
                    if (!string.IsNullOrEmpty(sddtText))
                    {
                        // 使用正则表达式提取所有数字
                        Regex regex = new Regex(@"\d+");
                        string value = string.Empty;
                        // 找到所有匹配的数字
                        MatchCollection matches = regex.Matches(sddtText);
                        if (matches.Count > 0)
                        {
                            value = matches[0].Value;
                            sddtText = sddtText.Replace(value, "").Replace(" ", "");
                            sddtText = TR.GetStr("CardName", sddtText);
                            sddtText = value + " " + sddtText;
                        }
                    }
                    ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(sddtText + string.Empty, 22, Color.black);
                    array[num2] = SDB.Align(new ShopDialogueObject[] { shopDialogueCard, shopDialogueDynamicText }, "VP", 0.1f);
                    shopDialogueCard.card.useDynamicCost = true;
                }
                ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), NameFunctionFactory(shopDialogueCard.card, __instance), null, __instance.CanAffordFunctionFactory(shopDialogueCard.card));
                if (GameManager.IsIPhone())
                {
                    shopDialogueButton.FontSize(36);
                }
                int recordIndex = num2;
                shopDialogueButton.SetCallback(() =>
                {
                    HelloMod.mLogger.LogMessage("Btn Clicked||" + __instance + "||" + shopDialogueCard + "||" + shopDialogueCard.card.ToString() + "||" + num + "||" + array.Length);
                    try
                    {
                        AcquireFunction(__instance, shopDialogueCard.card, new ShopDialogueObject[]
                    {
                        array[recordIndex],
                        shopDialogueButton
                    });
                    }
                    catch (Exception ex) {
                        HelloMod.mLogger.LogError(ex);
                    }
                });
                array2[num2] = shopDialogueButton;
                num2++;
                i++;
            }
            ShopDialogueAligned shopDialogueAligned = SDB.Align(array, "HC", width);
            float num3 = shopDialogueAligned.CenteringDistance();
            ShopDialogueAligned shopDialogueAligned2 = SDB.Align(array2, "HF", num3);
            return SDB.Align(new ShopDialogueAligned[] { shopDialogueAligned, shopDialogueAligned2 }, "VP", 0.1f);
        }

        public static string NameFunctionFactory(Card card,SmoothieShack __instance)
        {
            return (!__instance.first) ? 
                (TR.GetStr(DungeonPhysicalOverride.TableKey, "Buy (")
                 + card.IBCost() + TR.GetStr(DungeonPhysicalOverride.TableKey, " Gold)")) 
                : TR.GetStr(DungeonPhysicalOverride.TableKey, "Free!");
        }

        public static void AcquireFunction(SmoothieShack __instance,Card c, ShopDialogueObject[] so)
        {
            AcquireCard(c, __instance);
            __instance.first = false;
            HelloMod.mLogger.LogMessage("NameFunctionFactory||" + NameFunctionFactory(c, __instance));
            (so[1] as ShopDialogueButton).text.text = NameFunctionFactory(c, __instance);
        }

        public static void AcquireCard(Card c, SmoothieShack __instance)
        {
            if (!__instance.first)
            {
                __instance.dungeon.player.SpendGold(c.IBCost());
            }
            __instance.dungeon.player.AddCard(c, __instance.first);
        }
    }
}
