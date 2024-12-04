using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static UnityEngine.UI.Image;
using UnityEngine;

namespace HelloMod
{
    internal static class DreamQuestConfig
    {
        public const string EN = "en";
        public const string ZH = "zh";

        public static string CurrentLang = "zh"; 
        public static bool CoverFont = true; 
        public static bool UnlockAllMonster = true; 
        public static bool ActiveCheatMenu = true; 
        //这一项配置可以彻底阻止该mod运行
        public static bool DontloadMod = false; 
        //TODO:将所有csv表格的变化重写到SQLLite数据库中
        public static bool OverrideSQLDatabase = false;

        public static bool SkipEndTurnConfirm = false;

        public static bool SeenAllCard = false;//解锁所有卡牌，包括本不能解锁的那些

        public static bool MixCardName = false;//卡牌名中英结合显示

        public static bool CardViewer_Mainly_display_the_original_text = false;//卡牌图鉴中，当混合显示时，优先显示原文

        public static bool RandomName_Not_Contain_Translation = true;//混合显示的情况下，随机卡牌名不会包含译文

        public static bool IsZh { get { return CurrentLang == "zh"; } }
        public static bool IsEn { get { return CurrentLang == "en"; } }//一般用来判断是否执行原方法
    }
}
