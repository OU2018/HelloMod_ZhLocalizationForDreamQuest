using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class DecreeOverride
    {
        public static void DecreeString_Postfix(ref string __result,Decree __instance)
        {
            if (DreamQuestConfig.IsEn)
                return;
            string text = string.Empty;
            DecreeType decreeType = __instance.property;
            if (decreeType == DecreeType.NAME_STARTS_WITH)
            {
                if (__instance.isLike)
                {
                    text = "Each turn, DO play a card whose name starts with the letter '{value}'";
                }
                else
                {
                    text = "DO NOT play cards whose names start with the letter '{value}'";
                }
            }
            else if (decreeType == DecreeType.NAME_CONTAINS)
            {
                if (__instance.isLike)
                {
                    text = "Each turn, DO play a card whose name contains the letter '{value}'";
                }
                else
                {
                    text = "DO NOT play cards whose names contain the letter '{value}'";
                }
            }
            else if (decreeType == DecreeType.CARD_TYPE)
            {
                if (__instance.isLike)
                {
                    text = "Each turn, DO play " + __instance.ModArticle() + " {value} card";
                }
                else
                {
                    text = "DO NOT play {value} cards";
                }
            }
            else if (decreeType == DecreeType.SPECIFIC_CARD)
            {
                if (__instance.isLike)
                {
                    text = "Each turn, DO play a card named '{value}'";
                }
                else
                {
                    text = "DO NOT play cards named '{value}'";
                }
            }

            if(decreeType == DecreeType.CARD_TYPE && DreamQuestConfig.IsZh)
            {
                text = HelloMod.Csv.GetTranslationByID("DecreeString", "_" + decreeType.ToString() + "_" + __instance.isLike).Replace(TR.PlaceHolder, "<" + TR.GetStr(TR.SK, __instance.strength.ToString()) + "> ");
            }
            else
            {
                text = HelloMod.Csv.GetTranslationByID("DecreeString", "_" + decreeType.ToString() + "_" + __instance.isLike).Replace(TR.PlaceHolder, __instance.strength.ToString());
            }
            __result = text;
        }
    }
}
