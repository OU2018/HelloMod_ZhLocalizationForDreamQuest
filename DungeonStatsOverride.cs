using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class DungeonStatsOverride
    {
        public static ShopDialogueObject UnlockProgress(DungeonUser u)
        {
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(TR.GetStr(DungeonPhysicalOverride.TableKey, "Classes Unlocked: ") + u.AchievementCountString(AchievementFlavor.CLASS), 32, Color.black);
            ShopDialogueDynamicText shopDialogueDynamicText2 = SDB.DynamicText(TR.GetStr(DungeonPhysicalOverride.TableKey, "Passives Unlocked: ") + u.AchievementCountString(AchievementFlavor.PASSIVE), 32, Color.black);
            ShopDialogueDynamicText shopDialogueDynamicText3 = SDB.DynamicText(TR.GetStr(DungeonPhysicalOverride.TableKey, "Talents Unlocked: ") + u.AchievementCountString(AchievementFlavor.TALENT), 32, Color.black);
            ShopDialogueDynamicText shopDialogueDynamicText4 = SDB.DynamicText(TR.GetStr(DungeonPhysicalOverride.TableKey, "Cards Unlocked: ") + u.AchievementCountString(AchievementFlavor.CARD), 32, Color.black);
            ShopDialogueObject shopDialogueObject = SDB.Align(new ShopDialogueDynamicText[] { shopDialogueDynamicText, shopDialogueDynamicText3 }, "VPAL", 0.2f);
            ShopDialogueObject shopDialogueObject2 = SDB.Align(new ShopDialogueDynamicText[] { shopDialogueDynamicText2, shopDialogueDynamicText4 }, "VPAR", 0.2f);
            return SDB.Align(new ShopDialogueObject[] { shopDialogueObject, shopDialogueObject2 }, "HP", (float)2);
        }

        
        public static bool CreateScoreDialogueOverride(ref ShopDialogueObject __result ,int originalPoints, string highScoreText,DungeonStats __instance)
        {
            float num = 5f;
            ShopDialogueObject shopDialogueObject = __instance.CreateScoreLine(TR.GetStr(DungeonPhysicalOverride.TableKey, "Previous Total:"), originalPoints + string.Empty, num);
            shopDialogueObject = SDB.Align(new ShopDialogueObject[]
            {
            shopDialogueObject,
            __instance.CreateScoreLine(TR.GetStr(DungeonPhysicalOverride.TableKey, "Floors Cleared (") + __instance.GetAttribute(Stats.BOSSES) + "):", "+" + (float)__instance.FloorPoints() * __instance.DifficultyMult() + string.Empty, num)
            }, "VP", 0.2f);
            shopDialogueObject = SDB.Align(new ShopDialogueObject[]
            {
            shopDialogueObject,
            __instance.CreateScoreLine(TR.GetStr(DungeonPhysicalOverride.TableKey, "Monsters killed (") + __instance.GetAttribute(Stats.KILLS) + "):", "+" + (float)__instance.GetAttribute(Stats.KILL_EXP) * __instance.DifficultyMult(), num)
            }, "VP", 0.2f);
            if (__instance.GetAttribute(Stats.BOSS_DAMAGE) > 0)
            {
                shopDialogueObject = SDB.Align(new ShopDialogueObject[]
                {
                shopDialogueObject,
                __instance.CreateScoreLine(TR.GetStr(DungeonPhysicalOverride.TableKey, "Lord of the Dream Damage (") + (float)__instance.GetAttribute(Stats.BOSS_DAMAGE) * __instance.DifficultyMult() + "):", "+" + __instance.GetAttribute(Stats.BOSS_DAMAGE), num)
                }, "VP", 0.2f);
            }
            shopDialogueObject = SDB.Align(new ShopDialogueObject[]
            {
            shopDialogueObject,
            SDB.ShopTexture(1f, 0.03f, (Texture)Resources.Load("Textures/BlackLine", typeof(Texture)))
            }, "VPAR", 0.04f);
            shopDialogueObject = SDB.Align(new ShopDialogueObject[]
            {
            shopDialogueObject,
            __instance.CreateScoreLine(TR.GetStr(DungeonPhysicalOverride.TableKey, "Total Points:"), originalPoints + __instance.TotalScore() + string.Empty, num)
            }, "VP", 0.2f);
            shopDialogueObject = SDB.Align(new ShopDialogueObject[]
            {
            shopDialogueObject,
            SDB.DynamicText(highScoreText, 32, Color.black)
            }, "VP", 0.4f);
            __result = SDB.Align(new ShopDialogueObject[]
            {
            shopDialogueObject,
            UnlockProgress(GameManager.LoadCurrentUser())
            }, "VP", 0.5f);
            return false;
        }

        private static string _achievementName = "AchievementName";
        private static string _achievementReq = "AchievementReq";
        private static string _achievementRew = "AchievementRew";
        private static string _achievementProcess = "AchievementProcess";

        public static bool BuildEndAchievementViewer(ref ShopDialogueObject __result,List<Achievement> achievements, float width,DungeonStats __instance)
        {
            ShopDialogueObject shopDialogueObject;

            var CreateEndDialogueDict = new Dictionary<string, string> {
                { "Congratulations!", TR.GetStr(DungeonPhysicalOverride.TableKey, "Congratulations!") },
                { "The ", TR.GetStr(DungeonPhysicalOverride.TableKey, "The") },
                { " only earns class specific achievements!", TR.GetStr(DungeonPhysicalOverride.TableKey, " only earns class specific achievements!") },
                { "Runs on Kitten difficulty do not earn achievements!", TR.GetStr(DungeonPhysicalOverride.TableKey, "Runs on Kitten difficulty do not earn achievements!") },
                { "Replays do not earn achievments!", TR.GetStr(DungeonPhysicalOverride.TableKey, "Replays do not earn achievments!") },
                { "New Achievements!", TR.GetStr(DungeonPhysicalOverride.TableKey, "New Achievements!") },
                { "Main Menu", TR.GetStr(DungeonPhysicalOverride.TableKey, "Main Menu") },
            };
            // 判断条件
            if (!__instance.dungeon.player.profession.EarnsAchievements() && achievements.Count == 0)
            {
                shopDialogueObject = SDB.DynamicText(CreateEndDialogueDict["The "] + __instance.dungeon.player.profession.ClassName() + CreateEndDialogueDict[" only earns class specific achievements!"], 48, Color.black);
            }
            else if (__instance.dungeon.difficulty == Difficulty.EASY)
            {
                shopDialogueObject = SDB.DynamicText(CreateEndDialogueDict["Runs on Kitten difficulty do not earn achievements!"], 48, Color.black);
            }
            else if (__instance.dungeon.builtFromSeed)
            {
                shopDialogueObject = SDB.DynamicText(CreateEndDialogueDict["Replays do not earn achievements!"], 48, Color.black);
            }
            else
            {
                List<ShopDialogueObject> list = new List<ShopDialogueObject>();

                // 用 foreach 遍历 achievements 列表
                foreach (var achievement in achievements)
                {
                    // 假设 achievement 是一个 Achievement 类型的对象
                    ShopDialogueObject shopDialogueObject2 = SDB.ShopTexture(width * 0.15f, width * 0.15f, achievement.GetTexture());

                    // 创建 ShopDialogueText 对象来展示成就的不同信息
                    ShopDialogueText shopDialogueText = SDB.CenteredText(width * 0.65f, TR.GetStr(_achievementName, achievement.name) , 36, Color.blue);
                    ShopDialogueText shopDialogueText2 = SDB.CenteredText(width * 0.65f, TR.GetStr(_achievementRew, achievement.reward), 28, Color.black);
                    ShopDialogueText shopDialogueText3 = SDB.CenteredText(width * 0.65f, TR.GetStr(_achievementReq, achievement.requirements), 28, Color.black);

                    // 对齐文本
                    ShopDialogueObject shopDialogueObject3 = SDB.Align(new ShopDialogueText[] { shopDialogueText3, shopDialogueText2 }, "VP", 0.2f);
                    shopDialogueObject3 = SDB.Align(new ShopDialogueObject[] { shopDialogueText, shopDialogueObject3 }, "VP", 0.2f);
                    shopDialogueObject3 = SDB.Padded(shopDialogueObject3, new Vector2(width * 0.65f, width * 0.2f), ShopDialogueCardinal.uppercenter);
                    shopDialogueObject3 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject2, shopDialogueObject3 }, "HP", 0.2f);

                    // 将成就对象添加到列表中
                    list.Add(shopDialogueObject3);
                }

                // 如果有多个成就，则使用分页
                shopDialogueObject = SDB.Paged(list.ToArray());
            }
            __result = shopDialogueObject;
            return false;
        }

    }
}
