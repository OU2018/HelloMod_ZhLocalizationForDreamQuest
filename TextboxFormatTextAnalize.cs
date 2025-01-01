using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityScript.Lang;

namespace HelloMod
{
    internal class TextboxFormatTextAnalize:MonoBehaviour
    {
        
        public static bool FormatTextOverride(ref GameObject __result,ref string s, float lineWidth, ref Textbox __instance)
        {
            s = Regex.Replace(s, @"(?<=[\u4e00-\u9fa5])(?=[\u4e00-\u9fa5])|(?<=[\u4e00-\u9fa5])(?=[^\u4e00-\u9fa5])|(?<=[^\u4e00-\u9fa5])(?=[\u4e00-\u9fa5])", " ");
            s = s.Replace("\\\\n", " \n ");//需要 回车换行的位置，需要把字符"\n" 切换成回车=>"\\n"
            s = s.Replace("\\\\t", " \t ");//
            return true;
        }

        public static void FormatTextPostfix(ref GameObject __result, string s, float lineWidth)
        {
            TextMesh[] texts = __result.GetComponentsInChildren<TextMesh>();
            foreach (var item in texts)
            {
                //item.text = RemoveSpacesBetweenChineseCharacters(item.text);
                HelloMod.mLogger.LogInfo("Text:" + item.text + "||" + item.font);
                //HelloMod.SetTextMeshFont(item, 32, item.renderer.material.color);
                //重新居中，换行，整理
            }
        }

        static string RemoveSpacesBetweenChineseCharacters(string input)
        {
            return Regex.Replace(input, @"(?<=[\u4e00-\u9fa5]) (?=[\u4e00-\u9fa5])", "");
        }
        //Version 2
        /*public static bool FormatTextOverride(ref GameObject __result, ref string s, float lineWidth, ref Textbox __instance)
        {
            if (DreamQuestConfig.IsEn)//英文版不走这个步骤
                return true;

            TextMesh originText = __instance.textObjectNormal.GetComponent<TextMesh>();
            TextMesh specialText = __instance.textObjectSpecial.GetComponent<TextMesh>();
            HelloMod.SetTextMeshFont(originText);
            HelloMod.SetTextMeshFont(specialText);
            // 将字符串中的汉字字符间加上空格
            s = Regex.Replace(s, @"(?<=[\u4e00-\u9fa5])(?=[^\u4e00-\u9fa5])|(?<=[^\u4e00-\u9fa5])(?=[\u4e00-\u9fa5])", " ");
            HelloMod.mLogger.LogInfo(">Word Content:text:" + s);

            GameObject textInstance = CreateInstanceToParent(__instance.textObjectNormal, __instance.gameObject, __instance);
            TextMesh textMesh = textInstance.GetComponent<TextMesh>();
            textMesh.text = s;
            return false;
        }*/

        //Version 1
        /*public static bool FormatTextOverride(ref GameObject __result, string s, float lineWidth, ref Textbox __instance)
        {
            if (DreamQuestConfig.IsEn)//英文版不走这个步骤
                return true;

            Traverse.Create(__instance).Field("tempFontColor").SetValue(__instance.fontColor);

            // 将字符串预处理，替换符号并按空格分割
            s = s.Replace(">", " > ");
            string[] wordSegment = s.Split(' ');

            // 将字符串中的汉字字符间加上空格
            List<string> words = new List<string>();
            List<bool> needSpace = new List<bool>();
            foreach (var segment in wordSegment)
            {
                char[] chars = segment.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    if (chars[i] >= 0x4e00 && chars[i] <= 0x9fbb)
                    {
                        // 如果是汉字
                        words.Add(chars[i].ToString());
                        needSpace.Add(false);
                    }
                    else
                    {
                        words.Add(segment);
                        needSpace.Add(true);
                        break;
                    }
                }
            }
            wordSegment = words.ToArray();

            GameObject root = __instance.gameObject;
            GameObject textInstance = CreateInstanceToParent(__instance.textObjectNormal, root, __instance);
            TextMesh textMesh = textInstance.GetComponent<TextMesh>();

            float blankLineHeight = textInstance.GetComponent<Renderer>().bounds.size.y * 0.3f;
            float charWidth = MeasureCharWidth(textMesh);
            float lineHeight = textInstance.GetComponent<Renderer>().bounds.size.y;
            Traverse.Create(__instance).Field("_textHeight").SetValue(lineHeight * 2f);

            float currentLineWidth = 0f;
            List<GameObject> lineObjects = new List<GameObject>();
            List<List<GameObject>> allLines = new List<List<GameObject>>();

            for (int i = 0; i < wordSegment.Length; i++)
            {
                string word = wordSegment[i].Replace(" ", string.Empty);
                if (string.IsNullOrEmpty(word))
                    continue;

                bool isLineBreak = word == "\n";
                bool isSpecialWord = word.StartsWith("^") || word.StartsWith("@") || word.StartsWith("<") || word.EndsWith(">");
                bool needsSpace = needSpace[i];

                // 添加单词到当前行
                if (!isLineBreak)
                {
                    if (needsSpace)
                        textMesh.text += " " + word;
                    else
                        textMesh.text += word;

                    // 检查是否需要换行
                    if (textMesh.GetComponent<Renderer>().bounds.size.x + currentLineWidth > lineWidth)
                    {
                        textMesh.text = textMesh.text.Remove(textMesh.text.LastIndexOf(word));
                        isLineBreak = true;
                    }
                }

                // 处理换行逻辑
                if (isLineBreak)
                {
                    lineObjects.Add(textInstance);
                    allLines.Add(lineObjects);

                    currentLineWidth = 0f;
                    textInstance = CreateInstanceToParent(__instance.textObjectNormal, root, __instance);
                    textMesh = textInstance.GetComponent<TextMesh>();

                    Vector3 position = textInstance.transform.position;
                    position.y -= lineHeight;
                    textInstance.transform.position = position;

                    lineObjects = new List<GameObject>();
                }

                if (!isLineBreak)
                {
                    currentLineWidth += textMesh.GetComponent<Renderer>().bounds.size.x;
                }
            }
            if (textMesh.text.Length > 0)
            {
                lineObjects.Add(textInstance);
            }
            // 最后一行加入
            if (lineObjects.Count > 0)
            {
                allLines.Add(lineObjects);
            }
            //总共几行
            HelloMod.mLogger.LogInfo(">>>All Lines:" + allLines.Count);
            int line_index = 0;
            foreach (var line in allLines)
            {
                HelloMod.mLogger.LogInfo(">Line:" + line_index);
                foreach (var word in line)
                {
                    TextMesh currentMesh = word.GetComponent<TextMesh>();
                    if (currentMesh != null)
                    {
                        HelloMod.mLogger.LogInfo(">Word Content:text:" + currentMesh.text + "||font:" + currentMesh.font + "||fontSize:" + currentMesh.fontSize);
                    }
                }
                line_index++;
            }
            
            // 调整整体位置
            AdjustAlignmentAndPosition(root, allLines, lineWidth, __instance);
            __result = root;
            return false;
        }*/

        // 计算单个字符宽度
        private static float MeasureCharWidth(TextMesh textMesh)
        {
            textMesh.text = "m";
            float charWidth = textMesh.GetComponent<Renderer>().bounds.size.x;
            textMesh.text = string.Empty;
            return charWidth;
        }

        // 调整对齐方式并计算位置
        private static void AdjustAlignmentAndPosition(GameObject root, List<List<GameObject>> allLines, float lineWidth, Textbox __instance)
        {
            float totalHeight = 0f;
            float maxWidth = 0f;

            foreach (var line in allLines)
            {
                float lineWidthCurrent = line[line.Count - 1].transform.position.x - line[0].transform.position.x;
                maxWidth = Mathf.Max(maxWidth, lineWidthCurrent);
                totalHeight += line[0].GetComponent<Renderer>().bounds.size.y;
            }

            Vector3 rootPosition = root.transform.position;
            rootPosition = Vector3.zero;
            HelloMod.mLogger.LogInfo(">Root Position:" + rootPosition);
            rootPosition.x -= maxWidth / 2f;
            rootPosition.y += totalHeight / 2f;
            root.transform.position = rootPosition;

            __instance.width = maxWidth;
            __instance.height = totalHeight;
        }


        public static bool CreateInstanceToParent(ref GameObject __result, GameObject gotype, GameObject textRoot,ref Textbox __instance)
        {
            if (DreamQuestConfig.IsEn)
            {
                return true;
            }
            // 检查输入参数
            if (gotype == null)
            {
                HelloMod.mLogger.LogError("gotype is null! Cannot create an instance.");
                __result = null;
                return false;
            }

            if (textRoot == null)
            {
                HelloMod.mLogger.LogError("textRoot is null! Cannot set position.");
                __result = null;
                return false;
            }

            // 实例化模板对象
            GameObject instance = (GameObject)UnityEngine.GameObject.Instantiate(gotype);
            if (instance == null)
            {
                HelloMod.mLogger.LogError("Failed to instantiate the gotype.");
                __result = null;
                return false;
            }

            // 设置实例的位置和父级
            Transform instanceTransform = instance.transform;
            instanceTransform.SetParent(null, false);
            instanceTransform.position = new Vector3(
                textRoot.transform.position.x,
                textRoot.transform.position.y,
                __instance.transform.position.z + 0.01f
            );

            // 隐藏渲染器
            Renderer instanceRenderer = instance.GetComponent<Renderer>();
            if (instanceRenderer != null)
                instanceRenderer.enabled = false;

            // 设置文本和字体
            TextMesh textMesh = instance.GetComponent<TextMesh>();
            if (textMesh != null)
            {
                Renderer textRenderer = textMesh.GetComponent<Renderer>();
                if (textRenderer != null)
                    textRenderer.material.color = (Color)Traverse.Create(__instance).Field("tempFontColor").GetValue();

                //textMesh.color = (Color)Traverse.Create(__instance).Field("tempFontColor").GetValue();
                HelloMod.SetTextMeshFont(textMesh, __instance.fontSize);
                textRenderer.material.color = (Color)Traverse.Create(__instance).Field("tempFontColor").GetValue();
            }
            else
            {
                HelloMod.mLogger.LogWarning("TextMesh component not found on instance!");
            }
            __result = instance;
            return false;
        }

        public static bool ContainsChinese(string input)
        {
            // 定义一个正则表达式，匹配中文字符
            string pattern = @"[\u4e00-\u9fa5]";

            // 使用正则表达式判断是否包含中文字符
            return Regex.IsMatch(input, pattern);
        }
    }
}
