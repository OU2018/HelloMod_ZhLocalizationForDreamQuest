using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HelloMod
{
    internal class CSVReader
    {
        public static List<string[]> LoadCSV(string filePath)
        {
            var rows = new List<string[]>();
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    // 使用逗号分隔数据
                    string[] values = line.Split(',');
                    rows.Add(values);
                }
            }
            return rows;
        }
    }

}
