using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using U3DXT.iOS.Native.Foundation;
using UnityEngine;

namespace HelloMod
{
    internal class InfoblockOverride
    {
        public static bool RestartBlockPrefix(Infoblock __instance)
        {
            string _noteKey = "RestartBlockPrefix";
            float num = 4.5f;
            int num2 = 0;
            if (GameManager.IsIPhone())
            {
                num *= (float)2;
                num2 += 8;
            }
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(TR.GetStr(DungeonPhysicalOverride.TableKey, "Restart?", _noteKey), 32 + 2 * num2, Color.black);
            int num3 = __instance.player.game.currentDungeon.player.RestartCost();
            int num4 = __instance.player.game.currentDungeon.player.TotalPoints();
            string text = TR.GetStr(DungeonPhysicalOverride.TableKey, "your last level up?", _noteKey);
            if (__instance.player.game.currentDungeon.levelUpSaveIsFloorStart)
            {
                text = TR.GetStr(DungeonPhysicalOverride.TableKey, "the start of this floor?", _noteKey);
            }
            string content = "Would you like to spend your achievement points to restart from " + text + "  You won't receive credit for your accomplishments since then.";
            if (!DreamQuestConfig.IsEn)
            {
                content = TR.GetStr(DungeonPhysicalOverride.TableKey, "RestartBlockMainContent", _noteKey);
                content = content.Replace("{value}", text);
            }
            ShopDialogueText shopDialogueText = SDB.Text(num, content, 28 + num2, Color.black);
            float num5 = 1f;
            if (GameManager.IsIPhone())
            {
                num5 = 1.5f;
            }
            content = "Current:\nCost:";
            if (!DreamQuestConfig.IsEn)
            {
                content = TR.GetStr(DungeonPhysicalOverride.TableKey, "Current:Cost:", _noteKey);
            }
            ShopDialogueText shopDialogueText2 = SDB.Text(num5, content, 28 + num2, Color.black);
            ShopDialogueText shopDialogueText3 = SDB.Text(num5, num4 + "\n" + num3, 28 + num2, Color.black);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueText[] { shopDialogueText2, shopDialogueText3 }, "HP", 0.1f);
            ShopDialogueAligned shopDialogueAligned2 = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueText }, "VP", 0.15f);
            shopDialogueAligned2 = SDB.Align(new ShopDialogueAligned[] { shopDialogueAligned2, shopDialogueAligned }, "VP", 0.15f);
            float num6 = 1f;
            if (GameManager.IsIPhone())
            {
                num6 *= 1.5f;
            }
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(1f, 0.3f) * num6, TR.GetStr(DungeonPhysicalOverride.TableKey, "Yes"), __instance.YesRestart);
            ShopDialogueButton shopDialogueButton2 = SDB.BasicButton(new Vector2(1f, 0.3f) * num6, TR.GetStr(DungeonPhysicalOverride.TableKey, "No"), __instance.NoRestart);
            shopDialogueButton.UIButton = true;
            shopDialogueButton2.UIButton = true;
            ShopDialogueAligned shopDialogueAligned3 = SDB.Align(new ShopDialogueButton[] { shopDialogueButton, shopDialogueButton2 }, "HC", num * 0.8f);
            shopDialogueAligned3 = SDB.Align(new ShopDialogueAligned[] { shopDialogueAligned2, shopDialogueAligned3 }, "VP", 0.3f);
            SDB.Background(shopDialogueAligned3, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            shopDialogueAligned3.UpperCenterTo(new Vector3(-1.524903f, (float)1, (float)5));
            __instance.player.game.activeShop = shopDialogueAligned3;
            shopDialogueAligned3.DoneBuilding();
            return false;
        }

        public static bool AskReallyConcedePrefix(Infoblock __instance)
        {
            string _noteKey = "AskReallyConcedePrefix";
            int num = 0;
            if (GameManager.IsIPhone())
            {
                num += 12;
            }
            string content = "Are you sure you want\nto concede?";
            if (!DreamQuestConfig.IsEn)
            {
                content = TR.GetStr(DungeonPhysicalOverride.TableKey, "Are you sure you want to concede?", _noteKey);
            }
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(content, 28 + num, Color.black);
            float width = shopDialogueDynamicText.r.width;
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(width * 0.35f, 0.3f), TR.GetStr(DungeonPhysicalOverride.TableKey, "Yes"), __instance.ReallyConcede);
            ShopDialogueButton shopDialogueButton2 = SDB.BasicButton(new Vector2(width * 0.35f, 0.3f), TR.GetStr(DungeonPhysicalOverride.TableKey, "No"), __instance.player.game.DestroyActiveShopNow);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueButton[] { shopDialogueButton, shopDialogueButton2 }, "HC", width * 0.8f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueAligned }, "VP", 0.1f);
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            shopDialogueAligned.UpperLeftTo(new Vector3(4.92804f, 3.7f, (float)5) + new Vector3(-0.2f, (float)0, (float)0));
            __instance.player.game.activeShop = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }
        public static bool AreYouSurePrefix(Infoblock __instance)
        {
            string _noteKey = "AreYouSurePrefix";

            //考虑添加一个设置，可以直接结束回合
            if (DreamQuestConfig.SkipEndTurnConfirm)
            {
                __instance.ReallyEndTurn();
                return false;
            }

            int num = 0;
            if (GameManager.IsIPhone())
            {
                num += 12;
            }

            string content = "Are you sure you want\nto end your turn?";
            if (!DreamQuestConfig.IsEn)
            {
                content = TR.GetStr(DungeonPhysicalOverride.TableKey, "Are you sure you want to end your turn?", _noteKey);
            }
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(content, 28 + num, Color.black);
            float width = shopDialogueDynamicText.r.width;
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(width * 0.35f, 0.3f), TR.GetStr(DungeonPhysicalOverride.TableKey, "Yes"), __instance.ReallyEndTurn);
            ShopDialogueButton shopDialogueButton2 = SDB.BasicButton(new Vector2(width * 0.35f, 0.3f), TR.GetStr(DungeonPhysicalOverride.TableKey, "No"), __instance.player.game.DestroyActiveShopNow);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueButton[] { shopDialogueButton, shopDialogueButton2 }, "HC", width * 0.8f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueAligned }, "VP", 0.1f);
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            //获得私有变量
            // 获取字段的反射信息，'ExampleClass' 是类名，'hourglassBase' 是字段名
            FieldInfo fieldInfo = AccessTools.Field(typeof(Infoblock), "hourglassBase");
            // 使用反射获取字段值
            var hourglassBase = (Vector3)fieldInfo.GetValue(__instance);
            shopDialogueAligned.LowerCenterTo(hourglassBase + new Vector3((float)0, 0.1f, (float)4));
            __instance.player.game.activeShop = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }
        public static void GetAbilityTexturesPostfix(ref List<Infoblock.PlayerEffectWrapper> __result,Infoblock __instance)
        {
            // 匹配所有 <XX> 中的 XX 部分
            if (!DreamQuestConfig.IsEn)
            {
                foreach (var buff in __result)
                {
                    PlayerAttributes attr = buff.att;
                    if (attr == PlayerAttributes.MONSTER_COUNTER)//怪物特殊词条
                    {
                        int value = buff.n;
                        buff.s = __instance.player.MonsterCounterString(value);
                    }else if (attr == PlayerAttributes.ARCHMAGE)//咒术免费词条 TODO:Buff词条优化
                    {
                        if(__instance.freeSpellTurn > 0)
                        {
                            buff.s = HelloMod.Csv.GetTranslationByID("PlayerEffectDescription", "_" + attr + "_TURN");
                        }
                        else
                        {
                            string beforeReplace = HelloMod.Csv.GetTranslationByID("PlayerEffectDescription", "_" + attr);
                            buff.s = ReplaceNumberInsideStr(buff.s, beforeReplace, buff.n);
                        }
                    }
                    else if (attr == PlayerAttributes.TEMP_SPELL_REDUCE)//回复生命和减少咒术，不知道为啥用同一个枚举
                    {
                        string beforeReplace = string.Empty;
                        if (buff.s.Contains("mana"))
                        {
                            beforeReplace = HelloMod.Csv.GetTranslationByID("PlayerEffectDescription", "_" + attr + "_MANA");
                        }
                        else
                        {
                            beforeReplace = HelloMod.Csv.GetTranslationByID("PlayerEffectDescription", "_" + attr + "_HEALTH");
                        }
                        buff.s = ReplaceNumberInsideStr(buff.s, beforeReplace, buff.n);
                    }
                    else if (attr == PlayerAttributes.ELEMENTAL_FORM)//属性攻击
                    {
                        buff.s = HelloMod.Csv.GetTranslationByID("PlayerEffectDescription", "_" + attr + "_" + __instance.elementalForm);
                    }
                    else
                    {
                        int value = buff.n;
                        string content = buff.s;
                        string beforeReplace = string.Empty;
                        //原始字符串
                        beforeReplace = HelloMod.Csv.GetTranslationByID("PlayerEffectDescription", "_" + attr);
                        buff.s = ReplaceNumberInsideStr(content, beforeReplace, value);
                    }
                }
            }
        }

        public static string ReplaceNumberInsideStr(string origin,string target,int value)
        {
            string result = string.Empty;
            string pattern = @"<(\d+)>";//匹配 <>之间的数字
                                        // 创建正则表达式对象
            Regex regex = new Regex(pattern);

            // 获取所有匹配的内容
            MatchCollection matches = regex.Matches(origin);
            if (matches.Count > 0)
            {
                string replaceValue = matches[0].Groups[1].Value;
                result = target.Replace("{value}", replaceValue);
            }
            else if (value == 0)
            {
                string replaceValue = "1";
                result = target.Replace("{value}", replaceValue);
            }
            return result;
        }

        public static bool EscapePhysical(int x, Infoblock __instance)
        {
            string text = HelloMod.Csv.GetTranslationByID("UIextra", "_run_away_1");
            if (x == 2)
            {
                text = HelloMod.Csv.GetTranslationByID("UIextra", "_run_away_2");
            }
            else if (x == 3)
            {
                text = HelloMod.Csv.GetTranslationByID("UIextra", "_run_away_3");
            }
            text = text.Replace("\\\\n", "\n");
            __instance.StartCoroutine_Auto(__instance.ScrollTextDurationColor(text, 5, Color.red));
            return false;
        }
    }
}
