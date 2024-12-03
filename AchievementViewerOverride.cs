using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HelloMod
{
    internal class AchievementViewerOverride
    {
        private static string _achievementName = "AchievementName";
        private static string _achievementReq = "AchievementReq";
        private static string _achievementRew = "AchievementRew";
        private static string _achievementProcess = "AchievementProcess";
        public static void AchievementNamePostfix(ref string __result)
        {
            if (__result != null && !__result.Equals(string.Empty))
            {
                //结果不为空时进行调整
                __result = TR.GetStr(_achievementName, __result);
            }
        }

        public static void AchievementValuePostfix(ref string __result)
        {
            if (__result != null && !__result.Equals(string.Empty))
            {
                bool containsLetters = Regex.IsMatch(__result, @"\p{L}");
                if (containsLetters)
                {
                    __result = __result.Replace("Value", TR.GetStr(MainMenuOverride.TableKey, "Value"));
                }
            }
        }

        public static void AchievementReqPostfix(ref string __result)
        {
            if (__result != null && !__result.Equals(string.Empty))
            {
                //结果不为空时进行调整
                __result = TR.GetStr(_achievementReq, __result);
            }
        }

        public static void AchievementRewardPostfix(ref string __result)
        {
            if (__result != null && !__result.Equals(string.Empty))
            {
                //结果不为空时进行调整
                __result = TR.GetStr(_achievementRew, __result);
            }
        }

        public static void AchievementProgressPostfix(ref string __result)
        {
            if (__result != null && !__result.Equals(string.Empty))
            {
                //结果不为空时进行调整
                bool containsLetters = Regex.IsMatch(__result, @"\p{L}");
                if (containsLetters)
                {
                    //TODO
                }
            }
        }
    }
}
