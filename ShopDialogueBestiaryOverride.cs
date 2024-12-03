using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HelloMod
{
    internal class ShopDialogueBestiaryOverride
    {
        private static string _monsterName = "MonsterName";
        private static string _monsterDescription = "MonsterName";
        public static void MonsterNamePostfix(ref string __result)
        {
            if (__result != null && !__result.Equals(string.Empty))
            {
                //结果不为空时进行调整
                __result = TR.GetStr(_monsterName, __result);
            }
        }

        public static void MonsterDecriptionPostfix(ref string __result)
        {
            if (__result != null && !__result.Equals(string.Empty))
            {
                bool containsLetters = Regex.IsMatch(__result, @"\p{L}");
                if (containsLetters)
                {
                    //__result = TR.GetStr(_monsterDescription, __result);
                }
            }
        }
    }
}
