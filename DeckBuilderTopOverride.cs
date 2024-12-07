using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using UnityEngine;

namespace HelloMod
{
    internal class DeckBuilderTopOverride
    {
        public static bool OnGUI(DeckBuilderTop __instance)
        {
            if (DreamQuestConfig.IsEn)
            {
                return true;
            }
            GUIStyle guistyle = new GUIStyle();
            //由于按钮等空间都需要用到中文字体，所以将字体样式提出，并应用到所有控件上
            if (HelloMod.loadFont != null)//字体用加载进来的字体覆盖
            {
                guistyle.font = HelloMod.loadFont;
            }
            
            if (!__instance.setup)
            {
                GUI.Label(GUIHelper.MyRect((float)0, (float)0, (float)0, (float)0, TR.GetStr(DungeonPhysicalOverride.TableKey, "Loading..."), guistyle),TR.GetStr(DungeonPhysicalOverride.TableKey, "Loading..."), guistyle);
            }
            if (__instance.setup)
            {
                if (__instance.db.gp.game.CardSelected)
                {
                    GUI.enabled = false;
                }
                GUI.enabled = GamePhysical.GlobalGUIOn();
                if (true && __instance.actualCount > __instance.page * __instance.cPerCol * __instance.cPerRow && GUI.Button(GUIHelper.MyRect((float)1365, (float)250, (float)64, (float)40), string.Empty, __instance.rightArrow))
                {
                    __instance.page++;
                    __instance.StartCoroutine_Auto(__instance.ViewCards());
                }
                if (true && __instance.page > 1 && GUI.Button(GUIHelper.MyRect((float)5, (float)250, (float)64, (float)40), string.Empty, __instance.leftArrow))
                {
                    __instance.page--;
                    __instance.StartCoroutine_Auto(__instance.ViewCards());
                }
                GUI.enabled = true;
            }
            return false;
        }
    }
}
