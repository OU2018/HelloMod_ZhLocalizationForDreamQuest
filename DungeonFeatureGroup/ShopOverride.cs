using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityScript.Lang;

namespace HelloMod.DungeonFeatureGroup
{
    internal class ShopOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(Shop).GetMethod("Name"),
                typeof(ShopOverride).GetMethod("Name"));
            modCenter.PatchTargetPostfix(
                typeof(Shop).GetMethod("GenerateText"),
                typeof(ShopOverride).GetMethod("GenerateText"));
            modCenter.PatchTargetPrefix(
                typeof(Shop).GetMethod("DisplayInMini"),
                typeof(ShopOverride).GetMethod("DisplayInMini"));
            modCenter.PatchTargetPrefix(
                typeof(Shop).GetMethod("Enter"),
                typeof(ShopOverride).GetMethod("Enter"));
        }

        public static void GenerateText(Shop __instance)
        {
            __instance.text = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_shop_content");
        }

        public static void Name(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Gouda's Gummy Goodness");
        }

        public static bool DisplayInMini(Shop __instance)
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

        public static bool Enter(Shop __instance)
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

        public static ShopDialogueObject BuildLootObjects(float width,Shop __instance)
        {
            int count = __instance.items.Count;
            float num = __instance.dungeon.physical.PreferredCardWidth();
            ShopDialogueObject shopDialogueObject;
            if (count == 0)
            {
                shopDialogueObject = null;
            }
            else
            {
                float num2 = width;
                if (count == 2)
                {
                    num2 *= 0.75f;
                }
                ShopDialogueObject[] array = new ShopDialogueObject[count];
                ShopDialogueObject[] array2 = new ShopDialogueObject[count];
                int num3 = 0;
                foreach (string text in __instance.items)
                {
                    ShopDialogueCard shopDialogueCard = SDB.Card(num, text);
                    array[num3] = shopDialogueCard;

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
                            if(matches.Count > 0)
                            {
                                value = matches[0].Value;
                                sddtText = sddtText.Replace(value, "").Replace(" ", "");
                                sddtText = TR.GetStr("CardName", sddtText);
                                sddtText = value + " " + sddtText;
                            }
                        }
                        var shopDialogueDynamicText = SDB.DynamicText(sddtText + string.Empty, 22, Color.black);
                        HelloMod.mLogger.LogMessage("origin:" + shopDialogueCard.card.cardName + "||" + shopDialogueDynamicText.text.text);
                        array[num3] = SDB.Align(new ShopDialogueObject[] { shopDialogueCard, shopDialogueDynamicText }, "VP", 0.1f);
                    }

                    var shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), NameFunctionFactory(shopDialogueCard.card), null, __instance.CanAffordFunctionFactory(shopDialogueCard.card));
                    if (GameManager.IsIPhone())
                    {
                        shopDialogueButton.FontSize(36);
                    }

                    shopDialogueButton.SetCallback(__instance.AcquireFunction(shopDialogueCard.card, new ShopDialogueObject[] { array[num3], shopDialogueButton }));
                    array2[num3] = shopDialogueButton;
                    num3++;
                }

                ShopDialogueAligned shopDialogueAligned = SDB.Align(array, "HC", num2);
                float num4 = shopDialogueAligned.CenteringDistance();
                ShopDialogueAligned shopDialogueAligned2 = SDB.Align(array2, "HF", num4);
                shopDialogueAligned = SDB.Align(new ShopDialogueAligned[] { shopDialogueAligned, shopDialogueAligned2 }, "VP", 0.1f);
                __instance.boughtItems = 0;
                __instance.maxItems = count;
                shopDialogueObject = shopDialogueAligned;
            }
            return shopDialogueObject;
        }

        public static string NameFunctionFactory(Card card)
        {

            return TR.GetStr(DungeonPhysicalOverride.TableKey, "Buy (")
                 + card.IBCost() + TR.GetStr(DungeonPhysicalOverride.TableKey, " Gold)");
        }
    }
}
