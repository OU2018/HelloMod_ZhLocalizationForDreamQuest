using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class LordlyDecreeOverride
    {
        public static bool AddDecree(Decree d, LordlyDecrees __instance)
        {
            if (DreamQuestConfig.IsZh)
            {
                __instance.decrees.Add(d);
                string text = string.Empty;
                if (__instance.decrees.Count > 1)
                {
                    text = " \n (还差 " + (__instance.decrees.Count - 1) + " 张)";
                }
                __instance.game.SetTopMessage(d.DecreeString() + text);
                return false;

            }
            return true;
        }

        public static bool BuildButton(LordlyDecrees __instance)
        {
            if (DreamQuestConfig.IsZh)
            {
                __instance.game.me.infoblock.AddSpecialButton("查看禁令", ()=> { __instance.ViewDecrees(); });
                return false;
            }
            return true;
        }

        public static bool ViewDecrees(LordlyDecrees __instance)
        {
            if (DreamQuestConfig.IsZh)
            {
                __instance.game.physical.AddToVisualStackNoYield(__instance.game.physical, new object[]
                {
                    "当前禁令",
                    __instance.DecreeStrings()
                }, "BuildBulletTexts");
                return false;

            }
            return true;
            
        }
    }
}
