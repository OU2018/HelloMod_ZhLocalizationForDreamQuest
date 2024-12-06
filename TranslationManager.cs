using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HelloMod
{
    public static class TR
    {
        public const string SK = "SpecialKeyword";
        public const string PlaceHolder = "{value}";
        public static string GetStr(string tableKey, string str, string note = null)
        {
            return TranslationManager.GetTranslation(tableKey, str, note);
        }
    }
    public class TranslationManager
    {
        public static bool AutoAdd = true;

        private static Dictionary<string, List<TranslationEntry>> translations = new Dictionary<string, List<TranslationEntry>>();
        public static void Initialize()
        {
            HelloMod.mLogger.LogInfo("TranslationManager Success Initialized");
            var filePaths = Directory.GetFiles($@"{Paths.PluginPath}\TranslateSheet", "*.csv");
            LoadTranslations(filePaths);
            //TODO:载入长段的翻译表
        }
        //载入短语翻译表
        public static void LoadTranslations(string[] tablePaths)
        {
            foreach (var path in tablePaths)
            {
                string tableKey = Path.GetFileNameWithoutExtension(path);
                try
                {
                    var entries = LoadTable(path); // 加载单张表格的方法
                    translations[tableKey] = entries;
                }
                catch (Exception ex)
                {
                    HelloMod.mLogger.LogError("Error while applying patch: " + ex.ToString());
                }
            }
        }

        private const string SpecialKey = "SK";
        public static string ReplaceAllTableKeyword(string input)
        {
            // 正则表达式
            string pattern = @"<<([a-zA-Z0-9_| !@#$%^&*,'\-]+)>>";
            MatchCollection matches = Regex.Matches(input, pattern);
            if (matches.Count > 0)//包含 查找其他表单的内容 <<OtherTable|Key>>
            {
                // 输出所有匹配的结果
                foreach (Match match in matches)
                {
                    /*Console.WriteLine("Found: " + match.Value); // 匹配的整个内容
                    Console.WriteLine("Number: " + match.Groups[1].Value); // 提取的数字（不包括<<和>>）*/
                    //
                    string[] parms = match.Groups[1].Value.Split('|');
                    //parms[0];//表单名
                    //parms[1];//键值
                    string tableName = parms[0];
                    string note = string.Empty;
                    if (parms[0].Equals(SpecialKey))
                    {
                        tableName = specialTableKey;
                    }
                    if(parms.Length > 2)
                    {
                        note = parms[2];
                        input = input.Replace(match.Value, TR.GetStr(tableName, parms[1], note));
                    }
                    else
                    {
                        input = input.Replace(match.Value, TR.GetStr(tableName, parms[1]));
                    }
                }
                return input;
            }
            return input;
        }

        private static string dirPath = $@"{Paths.PluginPath}\TranslateSheet";
        private static readonly string specialKey = "[KEYWORD]";
        public static readonly string specialTableKey = "SpecialKeyword";
        //短语，词汇 翻译
        public static string GetTranslation(string tableKey, string str, string note = null)
        {
            if (translations.TryGetValue(tableKey, out var table))
            {
                var result = table.FirstOrDefault(entry => entry.En == str &&
                                                            (note == null || entry.Note == note));
                //HelloMod.mLogger.LogMessage(tableKey + "|" +  str + "|" + note + ":" + result.GetTranslate(DreamQuestConfig.CurrentLang));
                if (result == null)
                {
                    if (AutoAdd)
                    {
                        string filePath = Path.Combine(dirPath, tableKey + ".csv");
                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine($"{str},待翻译,{note}");

                            HelloMod.mLogger.LogMessage("Create new line:" + str);
                        }
                        return str;
                    }
                }
                else
                {
                    string currentResult = result.GetTranslate(DreamQuestConfig.CurrentLang);
                    if (currentResult.Contains(specialKey) 
                        && !tableKey.Equals(specialTableKey))//包含特殊关键词的情况下-且不是特殊表单
                    {
                        string specialKeyword = GetTranslation(specialTableKey, str, note);
                        return result.GetTranslate(DreamQuestConfig.CurrentLang).Replace(specialKey, specialKeyword);
                    }
                    return ReplaceAllTableKeyword(currentResult);
                }
                return result?.GetTranslate(DreamQuestConfig.CurrentLang) ?? str; // 找不到翻译时返回原文
            }
            
            HelloMod.mLogger.LogError("Table " + tableKey + " not found.");
            throw new KeyNotFoundException($"Table {tableKey} not found.");
        }
        //长段 翻译（将不同语句进行编号，不同语言的版本分别在不同的 csv文件中，第一列是文本编号 比如 00001；第二列开始就是文本）
        public static string GetLongSegmentTranslation(string tableKey, string en, string note = null)
        {
            //TODO
            HelloMod.mLogger.LogError("Table " + tableKey + " not found.");
            throw new KeyNotFoundException($"Table {tableKey} not found.");
        }

        public static List<TranslationEntry> LoadTable(string filePath)
        {
            List<TranslationEntry> entries = new List<TranslationEntry>();
            List<string[]> data = CSVReader.LoadCSV(filePath);

            int line = 0;
            foreach (var row in data)
            {
                line++;
                if (line == 1)
                    continue;
                string en = row[0];  // 第一列: en
                string zh = row[1];  // 第二列: zh
                string note = row[2]; // 第三列: comment
                if (row[0].Length > 0 && row[1].Length > 0) {
                    if (
                        char.IsDigit(row[0][row[0].Length - 1])
                        &&
                        char.IsDigit(row[1][0])
                        )//出现这种情况 abs 10,000 e.g.,把这两段接起来
                    {
                        en = row[0] + "," +row[1];
                        zh = row[2];
                        note = row[3];
                    }
                }
                TranslationEntry entry = new TranslationEntry(en, zh, note);
                entries.Add(entry);
            }
            return entries;
        }

    }
    public class TranslationEntry
    {
        public string En { get; set; }    // 原文
        public string Zh { get; set; }    // 中文翻译
        public string Note { get; set; } // 注释

        public TranslationEntry(string en, string zh, string note)
        {
            En = en;
            Zh = zh;
            Note = note;
        }

        public override string ToString()
        {
            return $"EN: {En}, ZH: {Zh}, Note: {Note}";
        }

        public string GetTranslate(string key)
        {
            switch (key)
            {
                case "en":
                    return En;
                case "zh":
                    return Zh;
                default:
                    break;
            }
            return En;
        }
    }

}
