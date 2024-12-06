using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityScript.Lang;

namespace HelloMod
{
    internal class CardPhysicalOverride
    {
        public static bool InitializeTextNow(CardPhysical __instance)
        {
            //获得私有变量
            // 获取字段的反射信息，'ExampleClass' 是类名，'hourglassBase' 是字段名
            FieldInfo fieldInfo = AccessTools.Field(typeof(CardPhysical), "cachedTransform");
            // 使用反射获取字段值
            var cachedTransform = (Transform)fieldInfo.GetValue(__instance);

            Quaternion rotation = cachedTransform.rotation;
            cachedTransform.rotation = Quaternion.identity;
            __instance.nameplate = cachedTransform.Find("Nameplate").gameObject;
            if (__instance.nameplate)
            {
                __instance.nameplate.transform.localScale = __instance.origNameSize;
                TextMesh cardName_TextMesh = __instance.nameplate.GetComponent<TextMesh>();
                HelloMod.SetTextMeshFont(cardName_TextMesh);
            }

            Transform transform = cachedTransform.Find("Levelplate");
            if (transform)
            {
                __instance.levelplate = transform.gameObject;
                if (__instance.levelplate)
                {
                    TextMesh level_TextMesh = __instance.levelplate.GetComponent<TextMesh>();
                    HelloMod.SetTextMeshFont(level_TextMesh);
                }
            }
            return true;
        }

        public static bool UpdateName(ref IEnumerator __result, CardPhysical __instance)
        {
            TextMesh textMesh = __instance.nameplate.GetComponent<TextMesh>();
            Color colorCache = textMesh.renderer.material.color;
            HelloMod.SetTextMeshFont(textMesh);
            textMesh.color = colorCache;
            HelloMod.mLogger.LogMessage("__instance.card.BuildName():" + __instance.card.BuildName());
            return true;
        }

        private static float _contentYOffset = 0.3f;
        public static bool RefreshTextBoxNow(CardPhysical __instance)
        {
            if (DreamQuestConfig.MixCardName && !DreamQuestConfig.IsEn)
                _contentYOffset = 0.3f;
            else
                _contentYOffset = 0f;
            if (__instance.ctophysical)
            {
                __instance.ctophysical.card = __instance.card;
                __instance.StartCoroutine_Auto(__instance.ctophysical.RefreshTextBox());
                __instance.StartCoroutine_Auto(__instance.ctophysical.RefreshInfluenceText());
                if (__instance.counters)
                {
                    if (__instance.card.ShownCounters() > 0)
                    {
                        TextMesh textMesh = (TextMesh)__instance.counters.GetComponent(typeof(TextMesh));
                        textMesh.text = __instance.card.ShownCounters() + string.Empty;
                    }
                    else
                    {
                        TextMesh textMesh = (TextMesh)__instance.counters.GetComponent(typeof(TextMesh));
                        textMesh.text = string.Empty;
                    }
                }
            }
            else
            {
                // 获取字段的反射信息，'ExampleClass' 是类名，'hourglassBase' 是字段名
                FieldInfo fieldInfo_cTR = AccessTools.Field(typeof(CardPhysical), "cachedTransform");
                // 使用反射获取字段值
                var cachedTransform = (Transform)fieldInfo_cTR.GetValue(__instance);
                // 获取字段的反射信息，'ExampleClass' 是类名，'hourglassBase' 是字段名
                FieldInfo fieldInfo_tSM = AccessTools.Field(typeof(CardPhysical), "textSizeMod");
                // 使用反射获取字段值
                var textSizeMod = (float)fieldInfo_tSM.GetValue(__instance);
                textSizeMod = 1f;
                Transform parent = cachedTransform.parent;
                cachedTransform.parent = null;
                Quaternion rotation = cachedTransform.rotation;
                Vector3 localScale = cachedTransform.localScale;
                cachedTransform.rotation = Quaternion.identity;
                cachedTransform.localScale = __instance.baseScale;
                if (__instance.currentTextRoot)
                {
                    UnityEngine.Object.Destroy(__instance.currentTextRoot);
                }
                GameObject textBoxGobj = cachedTransform.Find("TextBox").gameObject;
                if (string.IsNullOrEmpty(__instance.card.text))
                {
                    __instance.card.text = string.Empty;
                }
                float x = textBoxGobj.renderer.bounds.size.x;
                float y = textBoxGobj.renderer.bounds.size.y;
                GameObject contentGobj = __instance.FormatText(__instance.card.BuildCardText(), x, y - _contentYOffset);
                Transform transform = __instance.transform.Find("Type");
                GameObject cardTypeGobj = null;
                if (transform)
                {
                    // 获取卡片类型字符串
                    string text = __instance.card.CardTypeString();
                    //卡牌类型汉化
                    text = TR.GetStr(TR.SK, text);
                    // 实例化 gameObject3
                    cardTypeGobj = (GameObject)UnityEngine.Object.Instantiate(__instance.textObjectSpecial);

                    // 设置位置
                    cardTypeGobj.transform.position = new Vector3(transform.position.x, transform.position.y, contentGobj.transform.position.z);

                    // 启用渲染器并设置文本
                    cardTypeGobj.renderer.enabled = __instance.renderer.enabled;
                    TextMesh textMesh = cardTypeGobj.GetComponent<TextMesh>();
                    HelloMod.SetTextMeshFont(textMesh);
                    textMesh.text = text;
                    textMesh.anchor = TextAnchor.UpperLeft;


                    // 设置 parent
                    cardTypeGobj.transform.parent = contentGobj.transform;

                    // 获取 KeywordManager 并进行初始化
                    KeywordManager keywordManager = cardTypeGobj.GetComponent<KeywordManager>();
                    keywordManager.Finalize(new List<GameObject> { cardTypeGobj }, text, x, __instance, true);

                    // 如果存在 myTextRoot，更新位置
                    if (keywordManager.myTextRoot)
                    {
                        keywordManager.myTextRoot.transform.position = new Vector3(cardTypeGobj.renderer.bounds.center.x, keywordManager.myTextRoot.transform.position.y, keywordManager.myTextRoot.transform.position.z);
                    }

                    // 禁用碰撞器
                    cardTypeGobj.collider.enabled = false;
                }
                contentGobj.transform.position = textBoxGobj.renderer.bounds.max + Vector3.down * _contentYOffset;
                contentGobj.transform.parent = __instance.transform;
                if (cardTypeGobj && transform)
                {
                    // 使用 transform 的 x 和 y 位置，并保留 gameObject2 的 z 位置
                    cardTypeGobj.transform.position = new Vector3(transform.position.x, transform.position.y, contentGobj.transform.position.z);
                }
                __instance.currentTextRoot = contentGobj;
                __instance.transform.rotation = rotation;
                __instance.transform.localScale = localScale;
                __instance.transform.parent = parent;
            }
            if (__instance.IsSelected())
            {
                __instance.OnSelect();
            }
            __instance.StartCoroutine_Auto(__instance.RefreshIcons());
            __instance.RefreshParentInfo();
            return false;
        }

        public static bool FormatTextOverride(ref GameObject __result, ref string s, float lineWidth, float maxHeight, Textbox __instance)
        {
            if (DreamQuestConfig.IsEn)//英文版不走这个步骤
                return true;
            s = Regex.Replace(s, @"(?<=[\u4e00-\u9fa5])(?=[\u4e00-\u9fa5])|(?<=[\u4e00-\u9fa5])(?=[^\u4e00-\u9fa5])|(?<=[^\u4e00-\u9fa5])(?=[\u4e00-\u9fa5])", " ");
            s = s.Replace("\\\\n", "\n");//需要 回车换行的位置，需要把字符"\n" 切换成回车=>"\\n"
            return true;
        }

        public static bool CreateNewTO(ref GameObject __result,GameObject gotype, GameObject textRoot,CardPhysical __instance)
        {
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(gotype);
            gameObject.transform.parent = null;
            gameObject.transform.position = textRoot.transform.position;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, textRoot.transform.position.z + 0.01f);
            //获得私有变量
            // 获取字段的反射信息，'ExampleClass' 是类名，'hourglassBase' 是字段名
            FieldInfo fieldInfo = AccessTools.Field(typeof(CardPhysical), "textSizeMod");
            // 使用反射获取字段值
            var textSizeMod = (float)fieldInfo.GetValue(__instance);

            gameObject.transform.localScale = new Vector3(
                gameObject.transform.localScale.x * textSizeMod,
                gameObject.transform.localScale.y * textSizeMod,
                gameObject.transform.localScale.z
                );
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            HelloMod.SetTextMeshFont( textMesh );
            gameObject.renderer.enabled = __instance.renderer.enabled;
            __result = gameObject;
            return false;
        }
    }
}
