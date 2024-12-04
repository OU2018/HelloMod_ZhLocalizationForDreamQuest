using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class LordlyDecreesOverride
    {
        public static bool AddDecree(Decree d, LordlyDecrees __instance)
        {
            __instance.decrees.Add(d);
            string text = string.Empty;
            if (__instance.decrees.Count > 1)
            {
                text = " \n (And " + (__instance.decrees.Count - 1) + " More)";
            }
            __instance.game.SetTopMessage(d.DecreeString() + text);
            return false;
        }

        public static string[] DecreeStrings(LordlyDecrees __instance)
        {
            string[] array = new string[__instance.decrees.Count];
            for (int i = 0; i < __instance.decrees.Count; i++)
            {
                array[__instance.decrees.Count - i - 1] = __instance.decrees[i].DecreeString();
            }
            return array;
        }

        public static bool BuildButton(LordlyDecrees __instance)
        {
            __instance.game.me.infoblock.AddSpecialButton("View Decrees",()=> { ViewDecrees(__instance); });
            return false;
        }

        public static void ViewDecrees(LordlyDecrees __instance)
        {
            __instance.game.physical.AddToVisualStackNoYield(__instance.game.physical, new object[]
            {
            "Current Decrees",
            DecreeStrings(__instance)
            }, "BuildBulletTexts");
        }
    }
}
