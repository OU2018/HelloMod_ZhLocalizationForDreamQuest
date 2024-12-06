using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityScript.Lang;

namespace HelloMod.DungeonFeatureGroup
{
    internal class TreasureChestOverride:DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(TreasureChest).GetMethod("Name"),
                typeof(TreasureChestOverride).GetMethod("Name"));
            modCenter.PatchTargetPostfix(
                typeof(TreasureChest).GetMethod("GenerateText"),
                typeof(TreasureChestOverride).GetMethod("GenerateText"));
            modCenter.PatchTargetPrefix(
                typeof(TreasureChest).GetMethod("DisplayInMini"),
                typeof(TreasureChestOverride).GetMethod("DisplayInMini")
                );
        }

        public static void GenerateText(TreasureChest __instance)
        {
            __instance.text = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_treasureChest_content");
        }
        public static void Name(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Treasure Chest", "TREASURE");
        }

        public static bool DisplayInMini(TreasureChest __instance)
        {
            DungeonPlayerPhysical physical = __instance.dungeon.player.physical;
            float x = physical.renderer.bounds.size.x;
            float y = physical.renderer.bounds.size.y;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), 32, Color.white);
            ShopDialogueClickableIcon shopDialogueClickableIcon = SDB.ClickableIcon(new Vector2(x * 0.5f, x * 0.5f), __instance.Portrait(), string.Empty);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(x * 0.6f, 0.35f), TR.GetStr(_dungeonFeatureTableName, "Open", "TREASURE"), () =>
            {
                Open(__instance);
            });
            shopDialogueButton.FontSize(24);
            shopDialogueButton.ColliderMod(1.2f, 1.5f);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueClickableIcon }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", 0.2f);
            shopDialogueAligned.CenterTo(new Vector3(physical.transform.position.x, physical.transform.position.y - 0.35f * y, physical.transform.position.z + 0.2f));
            physical.miniDisplay = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }

        public static void Open( TreasureChest __instance)
        {
            __instance.EnteringShop();
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

            Vector2 btnSize = new Vector2(1.6f * 2.5f, 0.35f * 1.5f);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(btnSize, TR.GetStr(_dungeonFeatureTableName, "Discard The Rest", "TREASURE"), __instance.Finished);
            ShopDialogueButton shopDialogueButton2 = SDB.BasicButton(btnSize, TR.GetStr(_dungeonFeatureTableName, "Come Back Later", "TREASURE"), __instance.dungeon.WindowBack);
            if (GameManager.IsIPhone())
            {
                shopDialogueButton.FontSize(36);
            }
            if (GameManager.IsIPhone())
            {
                shopDialogueButton2.FontSize(36);
            }
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueText }, "VP", 0.1f);
            if (shopDialogueObject)
            {
                shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueObject }, "VP", 0.2f);
            }
            ShopDialogueAligned shopDialogueAligned2 = SDB.Align(new ShopDialogueButton[] { shopDialogueButton, shopDialogueButton2 }, "HP", (float)1);
            shopDialogueAligned = SDB.Align(new ShopDialogueAligned[] { shopDialogueAligned, shopDialogueAligned2 }, "VP", 0.5f);
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
            SDB.CancelButton(shopDialogueAligned, __instance.dungeon.WindowBack);
            shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
        }
        public static ShopDialogueObject BuildLootObjects(float width, TreasureChest __instance)
        {
            int num = __instance.loot.Count;
            float num2 = __instance.dungeon.physical.PreferredCardWidth();

            if (__instance.gold > 0)
            {
                num++;
            }

            ShopDialogueObject shopDialogueObject;

            // 如果没有物品，返回 null
            if (num == 0)
            {
                shopDialogueObject = null;
            }
            else
            {
                float num3 = width;
                if (num == 2)
                {
                    num3 *= 0.75f;
                }

                // 创建两个数组来存储对话框元素和按钮
                ShopDialogueObject[] array = new ShopDialogueObject[num];
                ShopDialogueObject[] array2 = new ShopDialogueObject[num];
                int num4 = 0;

                // 遍历 loot 列表
                foreach (var item in __instance.loot)
                {
                    string text = item as string;

                    if (text == null)
                    {
                        // 如果类型不匹配，可以考虑抛出异常或跳过
                        continue;
                    }

                    // 创建卡片
                    ShopDialogueCard shopDialogueCard = SDB.Card(num2, text);
                    array[num4] = shopDialogueCard;

                    // 如果卡片是 BonusCard 类型，处理额外信息
                    string text2 = TR.GetStr(_dungeonFeatureTableName, "Learn", "TREASURE");
                    if (shopDialogueCard.card is BonusCard)
                    {
                        text2 = TR.GetStr(_dungeonFeatureTableName, "Gain", "TREASURE");
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
                        float num6 = 0.1f;
                        if (GameManager.IsIPhone())
                        {
                            num6 = 0.05f;
                        }
                        array[num4] = SDB.Align(new ShopDialogueObject[] { shopDialogueCard, shopDialogueDynamicText }, "VP", num6);
                    }

                    // 创建按钮
                    ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), text2, null);
                    if (GameManager.IsIPhone())
                    {
                        shopDialogueButton.FontSize(36);
                    }

                    shopDialogueButton.SetCallback(__instance.AcquireFunction(shopDialogueCard.card, new ShopDialogueObject[] { array[num4], shopDialogueButton }));
                    array2[num4] = shopDialogueButton;

                    num4++;
                }

                // 如果有金币，创建金币显示部分
                if (__instance.gold > 0)
                {
                    ShopDialogueTexture shopDialogueTexture = SDB.ShopTexture(num2 * 0.5f, num2 * 0.5f, Resources.Load<Texture>("Textures/CoinIcon"));
                    ShopDialogueDynamicText shopDialogueDynamicText2 = SDB.DynamicText(__instance.gold.ToString(), 26, Color.black);
                    ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueTexture, shopDialogueDynamicText2 }, "VP", 0);
                    array[num4] = shopDialogueAligned;

                    // 创建金币按钮
                    ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(_dungeonFeatureTableName, "Take", "TREASURE"), null);
                    shopDialogueButton.SetCallback(__instance.AcquireFunction(__instance.gold, new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }));
                    array2[num4] = shopDialogueButton;
                    num4++;
                }

                // 对齐所有的元素
                ShopDialogueAligned shopDialogueAligned2 = SDB.Align(array, "HC", num3);
                float num7 = shopDialogueAligned2.CenteringDistance();
                ShopDialogueAligned shopDialogueAligned3 = SDB.Align(array2, "HF", num7);
                shopDialogueAligned2 = SDB.Align(new ShopDialogueAligned[] { shopDialogueAligned2, shopDialogueAligned3 }, "VP", 0.1f);

                __instance.takenLoot = 0;
                __instance.maxLoot = num;
                shopDialogueObject = shopDialogueAligned2;
            }

            return shopDialogueObject;
        }
    }
}
