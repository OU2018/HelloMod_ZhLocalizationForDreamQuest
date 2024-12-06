using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class TargetFinderLineOverride
    {
        public static bool OnGUI(TargetFinderLine __instance)
        {
            if (__instance.enabled)
            {
                if (__instance.action != null)
                {
                    GUIStyle guistyle = new GUIStyle();

                    Font font = Resources.Load("Fonts/ARLRDBD") as Font;
                    font.material.color = Color.black;
                    guistyle.font = font;
                    //由于按钮等空间都需要用到中文字体，所以将字体样式提出，并应用到所有控件上
                    if (HelloMod.loadFont != null)//字体用加载进来的字体覆盖
                    {
                        CoverFontOrNot(guistyle);
                    }
                    else
                    {
                        HelloMod.OnAfterFontLoaded += () =>
                        {
                            CoverFontOrNot(guistyle);
                        };
                    }

                    if (__instance.action.game.CardSelected)
                    {
                        GUI.enabled = false;
                    }
                    GUI.enabled = GamePhysical.GlobalGUIOn();
                    string text = __instance.@params.text;
                    int num = 0;
                    if (!string.IsNullOrEmpty(text) && text != "None")
                    {
                        //获得私有变量
                        // 获取字段的反射信息，'ExampleClass' 是类名，'hourglassBase' 是字段名
                        FieldInfo linefieldInfo = AccessTools.Field(typeof(TargetFinderLine), "guiTextLines");
                        // 使用反射获取字段值
                        var guiTextLines = (int)linefieldInfo.GetValue(__instance);
                        //获得私有变量
                        // 获取字段的反射信息，'ExampleClass' 是类名，'hourglassBase' 是字段名
                        FieldInfo textfieldInfo = AccessTools.Field(typeof(TargetFinderLine), "myguiText");
                        // 使用反射获取字段值
                        var myguiText = (string)textfieldInfo.GetValue(__instance);

                        if (guiTextLines < 0)
                        {
                            __instance.BuildGuiText(text);
                        }

                        Rect rect = GUIHelper.MyRect((float)0, (float)num, (float)__instance.TEXT_WIDTH, (float)(guiTextLines * 30), myguiText, guistyle);
                        num += guiTextLines * 30;
                        GUI.Label(rect, myguiText, guistyle);
                    }
                    int num2 = 0;
                    int num3 = 120;
                    if (__instance.@params.canCancel && !__instance.action.game.physical.noCancel)
                    {
                        if (GUI.Button(GUIHelper.MyRect((float)num2, (float)num, (float)80, (float)50, "Cancel", "button"), "Cancel", guistyle))
                        {
                            __instance.Cancel();
                        }
                        num2 += num3;
                    }
                    GUI.enabled = true;
                }
            }
            return false;
        }

        private static void CoverFontOrNot(GUIStyle uIStyle)
        {
            if (DreamQuestConfig.CoverFont)
            {
                uIStyle.font = HelloMod.loadFont;
            }
        }
    }
}
