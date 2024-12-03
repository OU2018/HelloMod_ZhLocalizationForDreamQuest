using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class DungeonUserOverride
    {
        public static void GetCompletedStringPostfix(ref string __result,Achievement ca)
        {
            string[] splitArr = __result.Split(':');
            string prefix = "Completed! \n ";
            string prefixName = splitArr[0].Replace(prefix, "");
            string outfix = splitArr.Length > 1 ? splitArr[1] : "";
            HelloMod.mLogger.LogInfo("Prefix Name:" +  prefixName);
            string result = prefixName;
            string trPrefix = TR.GetStr(MainMenuOverride.TableKey, "Completed!") + " \n";
            //对prefixName进行处理
            //优先替换 【非】 职业名称 的 长字符串
            string[] prefixDivide = prefixName.Split(' ');//用空格划分
            result = result.Replace("Cards Acquired", TR.GetStr(apKey, "Cards Acquired"));

            bool cache = TranslationManager.AutoAdd;
            TranslationManager.AutoAdd = false;
            for (int i = 0; i < prefixDivide.Length; i++) {
                if (prefixDivide[i].Equals(string.Empty))
                    continue;
                result = result.Replace(prefixDivide[i], TR.GetStr(apKey, prefixDivide[i]));
                result = result.Replace(prefixDivide[i], TR.GetStr(classKey, prefixDivide[i]));
            }
            TranslationManager.AutoAdd = cache;
            //替换其中的职业名称
            if (outfix.Equals(""))
            {
                __result = trPrefix;
            }
            else
            {
                __result = trPrefix + result + ":" + outfix;
            }
        }

        private static readonly string classKey = "ChooseClass";
        private static readonly string alterKey = "Altar";
        private static readonly string apKey = "AchievementProgress";
        public static void BaseWinStringPostfix(ref string __result)
        {
            __result = __result.Replace("Need", TR.GetStr(MainMenuOverride.TableKey, "Need"));
            __result = __result.Replace("Priest", TR.GetStr(classKey, "Priest"));
            __result = __result.Replace("Thief", TR.GetStr(classKey, "Thief"));
            __result = __result.Replace("Warrior", TR.GetStr(classKey, "Warrior"));
            __result = __result.Replace("Wizard", TR.GetStr(classKey, "Wizard"));
        }
        public static void AltarStringPostfix(ref string __result)
        {
            __result = __result.Replace("Need", TR.GetStr(MainMenuOverride.TableKey, "Need"));
            __result = __result.Replace("Alcoran", TR.GetStr(alterKey, "Alcoran"));
            __result = __result.Replace("Aston", TR.GetStr(alterKey, "Aston"));
            __result = __result.Replace("Cairn", TR.GetStr(alterKey, "Cairn"));
            __result = __result.Replace("Gauss", TR.GetStr(alterKey, "Gauss"));
            __result = __result.Replace("Jeremiad", TR.GetStr(alterKey, "Jeremiad"));
            __result = __result.Replace("Liara", TR.GetStr(alterKey, "Liara"));
        }
        public static void AdvancedWinStringPostfix(ref string __result)
        {
            __result = __result.Replace("Need", TR.GetStr(MainMenuOverride.TableKey, "Need"));
            __result = __result.Replace("Assassin", TR.GetStr(classKey, "Assassin"));
            __result = __result.Replace("Monk", TR.GetStr(classKey, "Monk"));
            __result = __result.Replace("Necromancer", TR.GetStr(classKey, "Necromancer"));
            __result = __result.Replace("Paladin", TR.GetStr(classKey, "Paladin"));
            __result = __result.Replace("Ranger", TR.GetStr(classKey, "Ranger"));
            __result = __result.Replace("Samurai", TR.GetStr(classKey, "Samurai"));
        }
        public static void FirstFloorStringPostfix(ref string __result)
        {
            __result = __result.Replace("Need", TR.GetStr(MainMenuOverride.TableKey, "Need"));
            __result = __result.Replace("Priest", TR.GetStr(classKey, "Priest"));
            __result = __result.Replace("Thief", TR.GetStr(classKey, "Thief"));
            __result = __result.Replace("Warrior", TR.GetStr(classKey, "Warrior"));
            __result = __result.Replace("Wizard", TR.GetStr(classKey, "Wizard"));
        }

        public static void AllMonstersProgressPostfix(ref string __result)
        {
            __result = __result.Replace("Need", TR.GetStr(MainMenuOverride.TableKey, "Need"));
            //TODO:
        }

        public static bool GetTutorialStringOverride(ref string __result, UserAttribute x)
        {
            string text = string.Empty;
            if (x == UserAttribute.TUTORIAL_COMBAT_BASIC)
            {
                text = "You are now in combat with a monster. \n Here, you'll play cards from your hand to deal damage and hopefully defeat it.  To do so, either drag the card from your hand to the middle of the screen or click the \"Play All\" button near your character portrait.";
                text = HelloMod.Csv.GetTranslationByID("UIextra", "_tutorial_basic");
            }
            else if (x == UserAttribute.TUTORIAL_COMBAT_END_TURN)
            {
                text = "You're out of things you can do this turn. \n To let the monster take a turn, click the hourglass button near your portrait.  You will then draw cards up to your maximum.  If you have cards in hand that you don't want, you can discard them at any time during your turn by dragging them to the discard pile in the lower left corner of your screen.";
                text = HelloMod.Csv.GetTranslationByID("UIextra", "_tutorial_endTurn");
            }
            else if (x == UserAttribute.TUTORIAL_FINAL_BOSS)
            {
                text = "Congratulations!  You've reached the Lord of the Dream and beaten the game!  The Lord of the Dream provides a way to test your deck - see how much damage you can deal to him before he overwhelms you.  Each point of damage will give you an additional point to use for buying achievements.  Good luck!";
                text = HelloMod.Csv.GetTranslationByID("UIextra", "_tutorial_finalBoss");
            }
            __result = text;
            return false;
        }

        public static bool UpdateHighScoreOverride(ref string __result,DungeonStats stats,DungeonUser __instance)
        {
            int num = stats.TotalScore();
            string internalName = stats.dungeon.player.profession.internalName;
            UserAttribute userAttribute = __instance.ClassToHS(internalName);
            string text = TR.GetStr(MainMenuOverride.TableKey, "These points may be used to purchase achievements on the title screen.");
            int num2 = __instance.GlobalHighScore();
            if (num > num2)
            {
                text = TR.GetStr(MainMenuOverride.TableKey, "Global High Score!");
            }
            else if (num > __instance.GetAttribute(userAttribute))
            {
                text = TR.GetStr(classKey, stats.dungeon.player.profession.ClassName()) + TR.GetStr(MainMenuOverride.TableKey, " High Score!");
            }
            if (num > __instance.GetAttribute(userAttribute))
            {
                __instance.SetAttributePerm(userAttribute, num);
            }
            __result = text;
            return false;
        }
    }
}
