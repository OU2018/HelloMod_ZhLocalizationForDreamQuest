using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SocialPlatforms;
using UnityEngine;
using System.Collections;
using UnityScript.Lang;

namespace HelloMod
{
    internal class DungeonPlayerOverride
    {
        public static bool CoolingDownAbilities(ref string __result,int x,DungeonPlayer __instance)
        {
            string text = string.Empty;

            // 假设 GetActions() 返回一个集合，这里使用 List<T> 举例
            var actions = __instance.GetActions(); // 获取动作列表

            foreach (var obj in actions)
            {
                // 强制转换为 DungeonAction 类型
                if (obj is DungeonAction dungeonAction)
                {
                    // 检查当前冷却时间
                    if (dungeonAction.currentCooldown != 0)
                    {
                        // 检查冷却时间为 1 或符合重置条件
                        if (dungeonAction.currentCooldown == 1 || __instance.profession.WillReset(dungeonAction, x))
                        {
                            if (text != string.Empty)
                            {
                                text += "\n";  // 如果不是第一个动作，换行
                            }
                            text += dungeonAction.ButtonName() + TR.GetStr(DungeonPhysicalOverride.TableKey, " is ready!");  // 添加动作名称
                        }
                    }
                }
            }
            __result = text;
            return false;
        }

        public static bool LevelUpPane(int healthGain, int manaGain, int goldGain, LevelUpReward[] rewards, DungeonPlayer __instance)
        {
            LevelUpReward primaryReward = null;
            float x = __instance.dungeon.physical.WindowSize().x;
		    primaryReward = rewards[0];
            LevelUpReward levelUpReward = rewards[1];
            LevelUpReward levelUpReward2 = rewards[2];
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose"), __instance.LevelUpChooseFactory(levelUpReward, true), () =>
            {
                return primaryReward.handled;
            });
            ShopDialogueButton shopDialogueButton2 = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose"), __instance.LevelUpChooseFactory(levelUpReward2, true), () =>
            {
                return primaryReward.handled;
            });
            ShopDialogueObject shopDialogueObject = levelUpReward.GetShopDialogueObject();
            ShopDialogueObject shopDialogueObject2 = levelUpReward2.GetShopDialogueObject();
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueObject, shopDialogueObject2 }, "HC", x * 0.9f);
            shopDialogueButton.UpperCenterTo(new Vector3(shopDialogueObject.Center().x, shopDialogueAligned.LowerLeft().y - 0.1f, shopDialogueObject.Center().z));
            shopDialogueButton2.UpperCenterTo(new Vector3(shopDialogueObject2.Center().x, shopDialogueAligned.LowerLeft().y - 0.1f, shopDialogueObject2.Center().z));
            shopDialogueAligned = SDB.Group(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton, shopDialogueButton2 });
            int num = 0;
            if (GameManager.IsIPhone())
            {
                num += 8;
            }
            ShopDialogueText shopDialogueText = SDB.CenteredText(x, TR.GetStr(TranslationManager.specialTableKey, "Level") + " " + __instance.level + "!", 48, Color.black);
            ShopDialogueObject shopDialogueObject3 = SDB.CenteredText(x, TR.GetStr(DungeonPhysicalOverride.TableKey, "You have gained") + healthGain + " " + TR.GetStr(TranslationManager.specialTableKey, "health"), 32 + num, Color.black);
            if (manaGain > 0)
            {
                ShopDialogueText shopDialogueText2 = SDB.CenteredText(x, TR.GetStr(DungeonPhysicalOverride.TableKey, "You have gained") + manaGain + " " + TR.GetStr(TranslationManager.specialTableKey, "mana"), 32 + num, Color.black);
                shopDialogueObject3 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject3, shopDialogueText2 }, "VP", 0.2f);
            }
            if (goldGain > 0)
            {
                ShopDialogueText shopDialogueText3 = SDB.CenteredText(x, TR.GetStr(DungeonPhysicalOverride.TableKey, "You have gained") + goldGain + " " + TR.GetStr(TranslationManager.specialTableKey, "gold"), 32 + num, Color.black);
                shopDialogueObject3 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject3, shopDialogueText3 }, "VP", 0.2f);
            }
            ShopDialogueText shopDialogueText4 = SDB.CenteredText(x, TR.GetStr(DungeonPhysicalOverride.TableKey, "You have learned"), 32 + num, Color.black);
            ShopDialogueText shopDialogueText5 = SDB.CenteredText(x, TR.GetStr(DungeonPhysicalOverride.TableKey, "Bonus! (Choose one)"), 32 + num, Color.black);
            ShopDialogueObject shopDialogueObject4 = SDB.Align(new ShopDialogueObject[] { shopDialogueText, shopDialogueObject3 }, "VP", 0.25f);
            if (primaryReward.IsReal())
		    {
                shopDialogueObject4 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject4, shopDialogueText4 }, "VP", 0.5f);
                shopDialogueObject4 = SDB.Align(new ShopDialogueObject[]
                {
                shopDialogueObject4,
				primaryReward.GetShopDialogueObject()
                }, "VP", 0.2f);
            }
            //跳过奖励按钮
            Vector3 skipBtnSize = __instance.dungeon.physical.DefaultButtonSize();
            skipBtnSize.x *= 1.5f;
            skipBtnSize.y *= 1.1f;
            string skipBtnContent = TR.GetStr(DungeonPhysicalOverride.TableKey, "Skip Reward");
            if(DreamQuestConfig.SkipLevelUpRewardGold > 0)
            {
                skipBtnContent += ("(+" + DreamQuestConfig.SkipLevelUpRewardGold.ToString() + " " + TR.GetStr(TranslationManager.specialTableKey, "gold") + ")");
            }
            ShopDialogueButton skipBtn = SDB.BasicButton(skipBtnSize, skipBtnContent, () => { SkipRewardChoose(__instance); }
                , () =>
            {
                return primaryReward.handled;
            });
            var align = SDB.Align(new ShopDialogueObject[] { shopDialogueText5 , skipBtn}, "HP", 0.5f);
            //shopDialogueObject4 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject4, shopDialogueText5 }, "VP", 0.5f);
            shopDialogueObject4 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject4, align }, "VP", 0.5f);
            shopDialogueObject4 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject4, shopDialogueAligned }, "VP", 0.2f);
            shopDialogueObject4 = SDB.Padded(shopDialogueObject4, new Vector2(x, shopDialogueObject4.r.height), ShopDialogueCardinal.center);
            Vector2 vector = new Vector2(0.5f, 0.5f);
            if (GameManager.IsIPhone())
            {
                vector = new Vector2((float)1, (float)1);
            }
            //卡组按钮
            ShopDialogueButton shopDialogueButton3 = SDB.Button(vector, new Texture[]
            {
            (Texture)Resources.Load("Textures/DeckIcon", typeof(Texture)),
            (Texture)Resources.Load("Textures/DeckIconPressed", typeof(Texture))
            }, string.Empty, () =>
            {
                __instance.dungeon.ChildWindow(() =>
                {
                    __instance.physical.ViewDeck();
                });
            });
            shopDialogueButton3.UpperRightTo(shopDialogueObject4.UpperRight() + new Vector3(-0.15f, 0.15f, (float)0));
            shopDialogueButton3.transform.parent = shopDialogueObject4.transform;

            //设置背景
            SDB.Background(shopDialogueObject4, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.white);
            Vector3 vector2 = __instance.dungeon.ShopLocation();
            if (shopDialogueObject4.myheight > 8f)
            {
                vector2.y += shopDialogueObject4.myheight - 8f;
            }
            shopDialogueObject4.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueObject4;
            shopDialogueObject4.DoneBuilding();
            MusicManager.PlayEffect(Music.LEVELUP, 0.4f);
            return false;
        }

        public static void SkipRewardChoose(DungeonPlayer __instance)
        {
            __instance.dungeon.WindowFinished();
            __instance.dungeon.levelUpSaveIsFloorStart = false;
            __instance.dungeon.SaveDungeon();
            __instance.dungeon.SaveAtLevelUp();
            __instance.GainExp(0);
            __instance.GainGold(DreamQuestConfig.SkipLevelUpRewardGold);
        }
        //添加卡牌 重写
        public static bool AddCard(string s, bool isLevelUp,DungeonPlayer __instance)
        {
            if (!isLevelUp)
            {
                __instance.stats.GainCard(s);
            }
            if (!isLevelUp)
            {
                Card card = __instance.game.CreateMomentaryCard(s);
                for (int i = 0; i < __instance.elementalAffinity.Length; i++)
                {
                    __instance.elementalAffinity[i] = __instance.elementalAffinity[i] + card.elementalAffinity[i];
                }
            }
            string[] array = new string[__instance.deck.Length + 1];
            for (int j = 0; j < __instance.deck.Length; j++)
            {
                array[j] = __instance.deck[j];
            }
            array[__instance.deck.Length] = s;
            __instance.deck = array;
            if (__instance.dungeon.physical.IsBuilt())
            {
                __instance.game.physical.AddToVisualStackNoYield(__instance.dungeon.physical, new string[] { s }, "AddCard");
            }
            if (__instance.CanEquip(s))
            {
                __instance.Equip(s);
            }
            return false;
        }
        //从卡组删除卡牌 重写：没有重写的必要，核心的部分在 DungeonPhysical.RemoveCard 重写只是为了插入 Log
        public static bool RemoveCardFromDeck(ref bool __result, string s, DungeonPlayer __instance)
        {
            bool flag;//这个返回值似乎没有在任何地方被使用
            if (__instance.deck.Length == 0)
            {
                flag = false;
            }
            else
            {
                string[] array = new string[__instance.deck.Length - 1];
                int num = 0;
                bool searchDone = false;
                for (int i = 0; i < __instance.deck.Length; i++)
                {
                    HelloMod.mLogger.LogMessage("Try Remove From Stack==>" + s + "||Current==>" + __instance.deck[i]);
                    if (!searchDone && __instance.deck[i] == s)
                    {
                        searchDone = true;
                    }
                    else if (num < array.Length)//
                    {
                        array[num] = __instance.deck[i];
                        num++;
                    }
                }
                if (searchDone)
                {
                    __instance.deck = array;
                    if (__instance.dungeon.physical.IsBuilt())
                    {
                        HelloMod.mLogger.LogMessage("Success Add Remove Request==>" + s );
                        __instance.game.physical.AddToVisualStackNoYield(__instance.dungeon.physical, new string[] { s }, "RemoveCard");
                        //第一个参数是实例对象，第三个参数是实例对象的对应方法
                    }
                }
                __instance.VerifyEquipment(s);
                flag = searchDone;
            }
            __result = flag;
            return false;
        }
    }
}
