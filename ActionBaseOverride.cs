using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class ActionBaseOverride
    {
        public static void ActionPatch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(ActionTargettedDeck).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionTargettedDeck_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionDiscard).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionDiscard_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionTeach).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionTeach_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionExile).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionExile_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionCharm).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionCharm_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionBewitch).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionBewitch_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionAnticipate).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionAnticipate_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionMeditation).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionMeditation_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionSingleTargetDiscard).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionSingleTargetDiscard_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionTargettedDiscard).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionTargettedDiscard_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionPenaltyChoice).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionPenaltyChoice_SetTargetFinderParameters")
                );
            modCenter.PatchTargetPostfix(
                typeof(ActionEndGame).GetMethod("SetTargetFinderParameters"),
                typeof(ActionBaseOverride).GetMethod("ActionEndGame_SetTargetFinderParameters")
                );
        }
        private const string _actionTextTableKey = "ActionText";
        //TargetFinderParameters有一个变量，cardButtonText，显然应该是按钮的文本。
        //但是在实际实现中，这个文本从来没有被读取过。
        public static void ActionTargettedDeck_SetTargetFinderParameters(TargetFinderParameters tf,ActionTargettedDeck __instance)
        {
            tf.cardButtonText = "Choose this";
            tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString());
        }

        public static void ActionDiscard_SetTargetFinderParameters(TargetFinderParameters tf, ActionDiscard __instance)
        {
            tf.cardButtonText = "Discard this";
            tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString()).Replace(TR.PlaceHolder, __instance.strength.ToString());
        }
        public static void ActionTeach_SetTargetFinderParameters(TargetFinderParameters tf, ActionTeach __instance)
        {
            tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString()).Replace(TR.PlaceHolder, __instance.strength.ToString());
        }

        public static void ActionExile_SetTargetFinderParameters(TargetFinderParameters tf, ActionExile __instance)
        {
            tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString()).Replace(TR.PlaceHolder, __instance.strength.ToString());
        }

        public static void ActionCharm_SetTargetFinderParameters(TargetFinderParameters tf,ActionCharm __instance)
        {
            tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString());
        }

        public static void ActionBewitch_SetTargetFinderParameters(TargetFinderParameters tf,ActionBewitch __instance)
        {
            tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString());
        }

        public static void ActionAnticipate_SetTargetFinderParameters(TargetFinderParameters tf,ActionAnticipate __instance)
        {
            tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString());
        }

        public static void ActionMeditation_SetTargetFinderParameters(TargetFinderParameters tf,ActionMeditation __instance)
        {
            tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString());
        }

        public static void ActionSingleTargetDiscard_SetTargetFinderParameters(TargetFinderParameters tf, ActionSingleTargetDiscard __instance)
        {
            tf.cardButtonText = "Discard this";
            tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString());
        }

        public static void ActionTargettedDiscard_SetTargetFinderParameters(TargetFinderParameters tf,ActionTargettedDiscard __instance)
        {
            tf.cardButtonText = "Discard this";
            tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString()).Replace(TR.PlaceHolder, __instance.strength.ToString());
        }

        public static void ActionPenaltyChoice_SetTargetFinderParameters(TargetFinderParameters tf,ActionPenaltyChoice __instance)
        {
            tf.cardButtonText = "Choose this";
            if (!string.IsNullOrEmpty(__instance.overrideText))//只有FinalBoss会设置其为 "Pick Your Poison"
            {
                tf.text = __instance.overrideText;
                tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_FinalBoss");
            }
            else
            {
                tf.text = "Make a wish";
                tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString());
            }
        }

        public static void ActionEndGame_SetTargetFinderParameters(TargetFinderParameters tf,ActionEndGame __instance)
        {
            tf.text = "The game is over.  ";
            if (__instance.card != null && __instance.card.game != null)
            {
                if (__instance.card.game.winner == __instance.card.game.me)
                {
                    tf.text += "You won!";
                    tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString() + "_win");
                }
                else
                {
                    tf.text += "You lost.";
                    tf.text = HelloMod.Csv.GetTranslationByID(_actionTextTableKey, "_" + __instance.GetType().ToString() + "_lose");
                }
            }
        }
    }
}
