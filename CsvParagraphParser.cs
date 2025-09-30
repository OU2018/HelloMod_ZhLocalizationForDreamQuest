using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    using HarmonyLib;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class CsvParagraphParser
    {
        private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>();
        private string baseDirectory;

        // 构造函数接收一个目录来管理多个表单
        public CsvParagraphParser(string directory)
        {
            baseDirectory = directory;
            ParseCsvFiles();
        }

        // 解析所有 CSV 文件
        private void ParseCsvFiles()
        {
            var files = Directory.GetFiles(baseDirectory, "*.csv");
            foreach (var file in files)
            {
                ParseCsvFile(file);
            }
        }

        // 解析单个 CSV 文件
        private void ParseCsvFile(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            var csvLines = File.ReadAllLines(filePath);
            string currentKey = null;
            string currentLanguage = null;
            string currentId = null;
            List<string> currentTranslation = new List<string>();
            foreach (var line in csvLines)
            {
                if (IsFirstTwoLetters(line))
                {
                    if (currentKey != null && currentTranslation.Count > 0)
                    {
                        if (!translations.ContainsKey(fileName))
                        {
                            translations[fileName] = new Dictionary<string, string>();
                        }
                        translations[fileName][currentLanguage + currentId] = string.Join("", currentTranslation.ToArray());
                    }
                    // Now parse the new line to set the new key and translation data
                    var parts = line.Split(',');
                    currentLanguage = parts[0].Substring(0, 2);  // Extract the language part (e.g., "en" or "zh")
                    currentId = parts[0].Substring(2); // Extract the ID (e.g., "0001") 也有可能是 _tutorial_basic
                    currentKey = parts[0];

                    currentTranslation.Clear();
                    currentTranslation.Add(string.Join(",", parts.Skip(1).ToArray()));  // Add the first part of the translation
                }
                else
                {
                    currentTranslation.Add(line.Trim());
                }
            }

            // Save the last entry
            if (currentKey != null && currentTranslation.Count > 0)
            {
                if (!translations.ContainsKey(fileName))
                {
                    translations[fileName] = new Dictionary<string, string>();
                }
                translations[fileName][currentLanguage + currentId] = string.Join("", currentTranslation.ToArray());
            }


        }

        public string GetTranslationByID(string formName, string id)
        {
            string language = DreamQuestConfig.CurrentLang;
            return GetTranslationByID( formName, language, id);
        }

        public string GetTranslationByID(string formName, string language, string id)
        {
            if (!translations.ContainsKey(formName))
            {
                return "未找到表格:" + formName;
            }
            if (!translations[formName].ContainsKey(language + id))
            {
                return "未找到对应行:" + language + id;
            }
            string input = translations[formName][language + id];
            return TranslationManager.ReplaceAllTableKeyword(input);
        }
        // 获取某表单中指定语言的 ID
        public string GetTranslationIDByContent(string formName, string language, string text)
        {
            if (translations.ContainsKey(formName))
            {
                foreach (var entry in translations[formName])
                {
                    if (entry.Value.Contains(text))
                    {
                        return entry.Key; // 返回对应的 ID
                    }
                }
            }
            return null;
        }

        // 如果没有找到，生成新的 ID 并写入 CSV 文件
        public string AddTranslation(string formName, string language, string text)
        {
            string newId = GenerateNewId(formName);
            string newKey = language + newId;
            string newTranslation = text;

            // 更新内存中的翻译
            if (!translations.ContainsKey(formName))
            {
                translations[formName] = new Dictionary<string, string>();
            }
            translations[formName][newId] = newTranslation;

            // 写入 CSV 文件
            string filePath = Path.Combine(baseDirectory, formName + ".csv");
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{newKey},{newTranslation}");
                writer.WriteLine($"zh{newId},{newTranslation}-未翻译");
            }

            return newId;
        }

        // 生成新的 ID
        private string GenerateNewId(string formName)
        {
            int maxId = 0;

            if (translations.ContainsKey(formName))
            {
                maxId = Mathf.FloorToInt( translations[formName].Count * 0.5f) + 1;
            }
            // 返回自增后的 ID
            return (maxId + 1).ToString("D5");
        }

        public static bool IsFirstTwoLetters(string str)
        {
            if (str.Length < 2) return false;
            string pattern = "^[a-zA-Z]{2}"; // 正则表达式，匹配前两位是字母
            return Regex.IsMatch(str, pattern);
        }
    }

}
