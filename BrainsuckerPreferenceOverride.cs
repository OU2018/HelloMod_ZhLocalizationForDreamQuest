using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class BrainsuckerPreferenceOverride
    {
        public static void LikeString(ref string __result,BrainsuckerPreference __instance)
        {
            string text = string.Empty;
            BrainsuckerCardProperty brainsuckerCardProperty = __instance.property;
            if (brainsuckerCardProperty == BrainsuckerCardProperty.NAME_STARTS_WITH)
            {
                text = "Cards whose names start with the letter '{value}'";
            }
            else if (brainsuckerCardProperty == BrainsuckerCardProperty.NAME_CONTAINS)
            {
                text = "Cards whose names contain the letter '{value}'";
            }
            else if (brainsuckerCardProperty == BrainsuckerCardProperty.CARD_TYPE)
            {
                text = "{value} cards";
            }
            else if (brainsuckerCardProperty == BrainsuckerCardProperty.LEVELABLE)
            {
                text = "Cards that have multiple ranks";
            }
            text = HelloMod.Csv.GetTranslationByID("BrainsuckerPreference", "_" + brainsuckerCardProperty);
            if(brainsuckerCardProperty == BrainsuckerCardProperty.CARD_TYPE)
            {
                text = text.Replace(TR.PlaceHolder, TR.GetStr(TR.SK, __instance.strength));
            }
            else if(brainsuckerCardProperty == BrainsuckerCardProperty.NAME_STARTS_WITH
                || brainsuckerCardProperty == BrainsuckerCardProperty.NAME_CONTAINS)
            {
                text = text.Replace(TR.PlaceHolder, __instance.strength);
            }
            __result = text;
        }
    }
}
