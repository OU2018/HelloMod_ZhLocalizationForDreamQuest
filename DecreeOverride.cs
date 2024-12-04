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
                    text = "Each turn, DO play a card whose name starts with the letter '" + __instance.strength + "'";
                }
                else
                {
                    text = "DO NOT play cards whose names start with the letter '" + __instance.strength + "'";
                }
            }
            else if (decreeType == DecreeType.NAME_CONTAINS)
            {
                if (__instance.isLike)
                {
                    text = "Each turn, DO play a card whose name contains the letter '" + __instance.strength + "'";
                }
                else
                {
                    text = "DO NOT play cards whose names contain the letter '" + __instance.strength + "'";
                }
            }
            else if (decreeType == DecreeType.CARD_TYPE)
            {
                if (__instance.isLike)
                {
                    text = "Each turn, DO play " + __instance.ModArticle() + " " + __instance.strength + " card";
                }
                else
                {
                    text = "DO NOT play " + __instance.strength + " cards";
                }
            }
            else if (decreeType == DecreeType.SPECIFIC_CARD)
            {
                if (__instance.isLike)
                {
                    text = "Each turn, DO play a card named '" + __instance.strength + "'";
                }
                else
                {
                    text = "DO NOT play cards named '" + __instance.strength + "'";
                }
            }
            __result = text;
        }
    }
}
