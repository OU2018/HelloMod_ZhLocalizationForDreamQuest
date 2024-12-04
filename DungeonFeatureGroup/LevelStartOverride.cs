using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod.DungeonFeatureGroup
{
    internal class LevelStartOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(LevelStart).GetMethod("Title"),
                typeof(LevelStartOverride).GetMethod("Title_Postfix"));
            modCenter.PatchTargetPostfix(
                typeof(LevelStart).GetMethod("ButtonText"),
                typeof(LevelStartOverride).GetMethod("ButtonText"));
            modCenter.PatchTargetPostfix(
                typeof(LevelStart).GetMethod("BuildIntroText"),
                typeof(LevelStartOverride).GetMethod("BuildIntroText"));
            modCenter.PatchTargetPostfix(
                typeof(LevelStart).GetMethod("WaterText"),
                typeof(LevelStartOverride).GetMethod("WaterText"));
            modCenter.PatchTargetPostfix(
                typeof(LevelStart).GetMethod("VolcanoText"),
                typeof(LevelStartOverride).GetMethod("VolcanoText"));
            modCenter.PatchTargetPostfix(
                typeof(LevelStart).GetMethod("ForestText"),
                typeof(LevelStartOverride).GetMethod("ForestText"));
            modCenter.PatchTargetPostfix(
                typeof(LevelStart).GetMethod("DungeonText"),
                typeof(LevelStartOverride).GetMethod("DungeonText"));
            modCenter.PatchTargetPostfix(
                typeof(LevelStart).GetMethod("SkyText"),
                typeof(LevelStartOverride).GetMethod("SkyText"));
            modCenter.PatchTargetPostfix(
                typeof(LevelStart).GetMethod("CryptText"),
                typeof(LevelStartOverride).GetMethod("CryptText"));
            modCenter.PatchTargetPostfix(
                typeof(LevelStart).GetMethod("BuildFinalBossText"),
                typeof(LevelStartOverride).GetMethod("BuildFinalBossText"));

            modCenter.PatchTargetPrefix(
                typeof(LevelStart).GetMethod("Text"),
                typeof(LevelStartOverride).GetMethod("Text")
                );
        }

        public static void Title_Postfix(ref string __result,LevelStart __instance)
        {
            string text = string.Empty;
            int depth = __instance.dungeon.board.depth;
            if (depth == 1)
            {
                text = "A Strange Place";
            }
            else if (depth == 2)
            {
                text = "A Stranger Place";
            }
            else if (depth == 3)
            {
                text = "The Strangest Place";
            }
            else if (depth == 4)
            {
                text = "The End?";
            }
            __result = TR.GetStr(_dungeonFeatureTableName, text);
        }

        public static void BuildIntroText(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_levelStart_buildIntroText");
        }

        public static void WaterText(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_levelStart_waterText");
        }

        public static void VolcanoText(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_levelStart_volcanoText");
        }

        public static void ForestText(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_levelStart_forestText");
        }

        public static void DungeonText(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_levelStart_dungeonText");
        }

        public static void SkyText(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_levelStart_skyText");
        }

        public static void CryptText(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_levelStart_CryptText");
        }

        public static void BuildFinalBossText(ref string __result, LevelStart __instance)
        {
            string originText = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_levelStart_buildFinalBossText");

            string text = "\"Beware the Lord of the Dream.  He has " + __instance.BossAdj() + " " + __instance.BossNoun() + " and " + __instance.BossPhrase() + ".";
            originText = originText.Replace("{BossAdj}", BossAdj()).Replace("{BossNoun}", BossNoun()).Replace("{BossPhrase}", BossPhrase());
            BossAttr[] array = __instance.dungeon.BossAttributes();
            for (int i = 0; i < __instance.dungeon.board.depth; i++)
            {
                if (i < array.Length)
                {
                    BossAttr bossAttr = array[i];
                    if (bossAttr == BossAttr.DECREE)
                    {
                        text += "  When he speaks, those that disobey are punished cruelly.";

                    }
                    else if (bossAttr == BossAttr.CHOICES)
                    {
                        text += "  When he sings, those who listen go slowly mad.";
                    }
                    else if (bossAttr == BossAttr.GIFTS)
                    {
                        text += "  When he beckons, all must give him gifts.";
                    }
                    else if (bossAttr == BossAttr.CANCELS)
                    {
                        text += "  When he dances, thoughts wither and die.";
                    }
                    else if (bossAttr == BossAttr.PHYS_IMMUNE)
                    {
                        text += "  His skin shatters swords.";
                    }
                    else if (bossAttr == BossAttr.ELEMENT_IMMUNE)
                    {
                        text += "  His body breaks storms.";
                    }
                    else if (bossAttr == BossAttr.FAST)
                    {
                        text += "  His gaze halts arrows in flight.";
                    }
                    else if (bossAttr == BossAttr.DOUBLE_TURN)
                    {
                        text += "  His will bends time.";
                    }
                    else if (bossAttr == BossAttr.VAMPIRIC)
                    {
                        text += "  They say his mother was a vampire.";
                    }
                    else if (bossAttr == BossAttr.FLAVORED)
                    {
                        text += "  They say his mother was a harpy.";
                    }
                    else if (bossAttr == BossAttr.POISONOUS)
                    {
                        text += "  They say his father was a serpent.";
                    }
                    else if (bossAttr == BossAttr.SUPER_EQUIPPED)
                    {
                        text += "  They say his father was a titan.";
                    }
                    //text += HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_" + bossAttr.ToString());
                    originText += HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_" + bossAttr.ToString());
                }
            }
            //__result = text + (BossWarning() + "\"");
            __result = originText + (BossWarning() + "”");
        }

        public static string BossAdj()
        {
            int num = Game.RandomRange(0, 5);
            string text = string.Empty;
            int num2 = num;
            if (num2 == 0)
            {
                text = "green";
            }
            else if (num2 == 1)
            {
                text = "a hundred";
            }
            else if (num2 == 2)
            {
                text = "enormous";
            }
            else if (num2 == 3)
            {
                text = "flaming";
            }
            else if (num2 == 4)
            {
                text = "serrated";
            }
            else if (num2 == 5)
            {
                text = "yellow";
            }
            text = TR.GetStr(_dungeonFeatureTableName, text, "LEVELSTART");
            return text;
        }

        public static string BossNoun()
        {
            int num = Game.RandomRange(0, 5);
            string text = string.Empty;
            int num2 = num;
            if (num2 == 0)
            {
                text = "toes";
            }
            else if (num2 == 1)
            {
                text = "arms";
            }
            else if (num2 == 2)
            {
                text = "ears";
            }
            else if (num2 == 3)
            {
                text = "fingernails";
            }
            else if (num2 == 4)
            {
                text = "elbows";
            }
            else if (num2 == 5)
            {
                text = "knees";
            }
            text = TR.GetStr(_dungeonFeatureTableName, text, "LEVELSTART");

            return text;
        }

        public static string BossPhrase()
        {
            int num = Game.RandomRange(0, 5);
            string text = string.Empty;
            if (num == 0)
            {
                text = "breath that can curdle " + Liquids();
            }
            else if (num == 1)
            {
                text = "eyes on the back of his neck";
            }
            else if (num == 2)
            {
                text = "a devastating sneeze";
            }
            else if (num == 3)
            {
                text = "hair that dances on its own";
            }
            else if (num == 4)
            {
                text = "tiny feet";
            }
            else if (num == 5)
            {
                text = "an unending appetite for " + Appetite();
            }

            if(num == 0 || num == 5)
            {
                if (num == 0) {
                    text = TR.GetStr(_dungeonFeatureTableName, "breath that can curdle ", "LEVELSTART") + Liquids();
                }
                if (num == 5)
                {
                    text = TR.GetStr(_dungeonFeatureTableName, "an unending appetite for ", "LEVELSTART") + Appetite();

                }
            }
            else
            {
                text = TR.GetStr(_dungeonFeatureTableName, text, "LEVELSTART");
            }
            return text;
        }

        public static string Liquids()
        {
            int num = Game.RandomRange(0, 5);
            string text = string.Empty;
            int num2 = num;
            if (num2 == 0)
            {
                text = "grape juice";
            }
            else if (num2 == 1)
            {
                text = "orange juice";
            }
            else if (num2 == 2)
            {
                text = "lemonade";
            }
            else if (num2 == 3)
            {
                text = "soda";
            }
            else if (num2 == 4)
            {
                text = "apple juice";
            }
            else if (num2 == 5)
            {
                text = "ginger ale";
            }
            text = TR.GetStr(_dungeonFeatureTableName, text, "LEVELSTART");
            return text;
        }

        public static string Appetite()
        {
            int num = Game.RandomRange(0, 5);
            string text = string.Empty;
            int num2 = num;
            if (num2 == 0)
            {
                text = "cheese";
            }
            else if (num2 == 1)
            {
                text = "yogurt";
            }
            else if (num2 == 2)
            {
                text = "noodles";
            }
            else if (num2 == 3)
            {
                text = "donuts";
            }
            else if (num2 == 4)
            {
                text = "romance novels";
            }
            else if (num2 == 5)
            {
                text = "crackers";
            }
            text = TR.GetStr(_dungeonFeatureTableName, text, "LEVELSTART");
            return text;
        }

        public static string BossWarning()
        {
            int num = Game.RandomRange(0, 5);
            string text = string.Empty;
            if (num == 0)
            {
                text = "  Stay far away!";
            }
            else if (num == 1)
            {
                text = "  Beware his wrath!";
            }
            else if (num == 2)
            {
                text = "  Flee!  Flee!";
            }
            else if (num == 3)
            {
                text = "  You can still escape!";
            }
            else if (num == 4)
            {
                text = "  Hide while you can!";
            }
            else if (num == 5)
            {
                text = "  Nothing can defeat him.";
            }
            text = TR.GetStr(_dungeonFeatureTableName, text, "LEVELSTART");
            return text;
        }

        public static bool Text(ref string __result,LevelStart __instance)
        {
            string text = string.Empty;
            if (__instance.dungeon.board.depth < 4)
            {
                string text2 = string.Empty;
                if (__instance.dungeon.board.depth == 1)
                {
                    text2 = __instance.BuildIntroText();
                }
                string text3 = __instance.BuildMainText();
                string text4 = __instance.BuildFinalBossText();
                text = text2 + text3 + text4;
            }
            else
            {
                text = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_levelStart_endText");
            }
            __result = text;
            return false;
        }

        public static void ButtonText(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Cool!");
        }
    }
}
