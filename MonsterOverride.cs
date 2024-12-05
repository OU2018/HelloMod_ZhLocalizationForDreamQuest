using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class MonsterOverride
    {
        private static string _monsterName = "MonsterName";
        //疑似是巨龙的吞噬技能
        public static bool DevourStringPostfix(string s,Monster __instance)
        {
            if (s.Contains("Gained"))
            {
                string[] para = s.Split(' ');
                string cardName = para[1];
                s = TR.GetStr(TranslationManager.specialTableKey, "Gained") + TR.GetStr("CardName", cardName);
            }
            else
            {
                s = s.Replace("Fully Heal", TR.GetStr(TranslationManager.specialTableKey, "Fully Heal"));
                s = s.Replace("Max Health", TR.GetStr(TranslationManager.specialTableKey, "Max Health"));
                s = s.Replace("Equipment Slot", TR.GetStr(TranslationManager.specialTableKey, "Equipment Slot"));
                s = s.Replace("Damage Taken!", TR.GetStr(TranslationManager.specialTableKey, "Damage Taken!"));
                s = s.Replace("Card", TR.GetStr(TranslationManager.specialTableKey, "Card"));
                s = s.Replace("Action", TR.GetStr(TranslationManager.specialTableKey, "Action"));
                s = s.Replace("Mana", TR.GetStr(TranslationManager.specialTableKey, "Mana"));
            }

            if (s.StartsWith("Yuck"))
            {
                __instance.dungeon.player.PlayerSpriteTextQueued(s, 3, Utility.darkRed);
            }
            else
            {
                __instance.dungeon.player.PlayerSpriteTextQueued(s, 3, Utility.darkGreen);
            }
            return false;
        }

        private static Dictionary<string, string> monsterNameCacheDict = new Dictionary<string, string>();
        public static void NamePostfix(ref string __result,Monster __instance)
        {
            if (!TextboxFormatTextAnalize.ContainsChinese(__result))
            {
                string key = __instance.GetType().ToString();
                if (!monsterNameCacheDict.ContainsKey(key))
                {
                    HelloMod.mLogger.LogMessage("Register Origin" + __result);
                    monsterNameCacheDict.Add(key, __result);
                }
                __result = TR.GetStr(_monsterName, __result);
            }
        }

        public static void PowerStringPostfix(ref string __result,Monster __instance) {
            if (!TextboxFormatTextAnalize.ContainsChinese(__result))
            {
                if(__instance.GetType() == typeof(FinalBoss))
                {
                    //最终boss需要进行特殊处理
                    __result = __instance.dungeon.FinalBossPowers();
                }
                else
                {
                    __instance.Name();
                    __result = HelloMod.Csv.GetTranslationByID("MonsterPower", "_" + monsterNameCacheDict[__instance.GetType().ToString()]);
                }
            }
        }

        public static void MonsterCounterString(ref string __result,int x, Monster __instance)
        {
            if (!string.IsNullOrEmpty(__instance.monsterPowers))
            {
                __instance.Name();
                __result = HelloMod.Csv.GetTranslationByID("MonsterMPowers", "_" + monsterNameCacheDict[__instance.GetType().ToString()]);
            }
        }
        //AirElemental
        public static void AEMonsterCounterString(ref string __result, int x, AirElemental __instance)
        {
            __result = HelloMod.Csv.GetTranslationByID("MonsterMPowers", "_" + monsterNameCacheDict[__instance.GetType().ToString()])
                .Replace(TR.PlaceHolder, x.ToString());
        }
        //WaterElemental
        public static void WEMonsterCounterString(ref string __result, int x, WaterElemental __instance)
        {
            __result = HelloMod.Csv.GetTranslationByID("MonsterMPowers", "_" + monsterNameCacheDict[__instance.GetType().ToString()])
                .Replace(TR.PlaceHolder, x.ToString());
        }

        public static void CombatPowerStringPostfix(ref string __result, Monster __instance)
        {
            __result = "<*" + __instance.Name() + "> \n <*" + __instance.LevelString() + "> \n " + __instance.PowerString();
        }

        public static void LevelStringPostfix(ref string __result)
        {
            __result = __result.Replace("Level", TR.GetStr(TranslationManager.specialTableKey, "Level"));
            __result = __result.Replace("Boss", TR.GetStr(TranslationManager.specialTableKey, "Boss"));
            __result = __result.Replace("Elite", TR.GetStr(TranslationManager.specialTableKey, "Elite"));
            __result = __result.Replace(" - Damaged", TR.GetStr(TranslationManager.specialTableKey, " - Damaged"));
        }

        public static void DefeatedPostfix(ref string __result)
        {
            __result = __result.Replace("gold", TR.GetStr(TranslationManager.specialTableKey, "gold"));
            __result = __result.Replace("experience", TR.GetStr(TranslationManager.specialTableKey, "experience"));
            __result = __result.Replace("You have found", TR.GetStr(DungeonPhysicalOverride.TableKey, "You have found", "DEFAET"));
            __result = __result.Replace("You have gained", TR.GetStr(DungeonPhysicalOverride.TableKey, "You have gained", "DEFAET"));
            __result = __result.Replace("a treasure chest", TR.GetStr(DungeonPhysicalOverride.TableKey, "a treasure chest", "DEFAET"));
        }
    }
}
