using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class IniParser
    {
        private readonly Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

        public void Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"INI file not found: {path}");

            string currentSection = null;
            foreach (var line in File.ReadAllLines(path))
            {
                string trimmedLine = line.Trim();
                HelloMod.mLogger.LogInfo(trimmedLine);
                // 跳过空行和注释行
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";"))
                    continue;

                // 检测到新节
                if (trimmedLine.StartsWith("[") && trimmedLine.Contains("]"))
                {
                    var cut = trimmedLine.Split(']')[0];
                    currentSection = cut.Substring(1, cut.Length - 1).Trim();
                    if (!data.ContainsKey(currentSection))
                    {
                        data[currentSection] = new Dictionary<string, string>();
                    }
                    HelloMod.mLogger.LogInfo("new section||" + currentSection);
                }
                else if (currentSection != null)
                {
                    // 解析键值对
                    var keyValue = trimmedLine.Split(new[] { '='}, 2);
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Split(new[] {';','#'})[0].Trim();
                        data[currentSection][key] = value;
                        HelloMod.mLogger.LogInfo("new data||" + key + ":" + value);
                    }
                }
            }
        }

        public string GetString(string section, string key, string defaultValue = null)
        {
            if (data.ContainsKey(section) && data[section].ContainsKey(key))
            {
                return data[section][key];
            }
            return defaultValue;
        }

        public bool GetBool(string section, string key, bool defaultValue = false)
        {
            if (data.ContainsKey(section) && data[section].ContainsKey(key))
            {
                return bool.TryParse(data[section][key], out bool result) ? result : defaultValue;
            }
            return defaultValue;
        }

        public int GetInt(string section, string key, int defaultValue = 0)
        {
            if (data.ContainsKey(section) && data[section].ContainsKey(key))
            {
                return int.TryParse(data[section][key], out int result) ? result : defaultValue;
            }
            return defaultValue;
        }

        public void SetValue(string section, string key, object value)
        {
            if (!data.ContainsKey(section))
            {
                data[section] = new Dictionary<string, string>();
            }
            data[section][key] = value.ToString();
        }

        public void Save(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                foreach (var section in data)
                {
                    writer.WriteLine($"[{section.Key}]");
                    foreach (var pair in section.Value)
                    {
                        writer.WriteLine($"{pair.Key}={pair.Value}");
                    }
                    writer.WriteLine();
                }
            }
        }
    }

}
