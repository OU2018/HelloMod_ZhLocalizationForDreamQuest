using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static UnityEngine.UI.Image;
using UnityEngine;

namespace HelloMod
{
    public static class DreamQuestConfig
    {
        public const string EN = "en";
        public const string ZH = "zh";
        //只是预留，未实际使用
        public const string JP = "jp";
        public const string KR = "kr";

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

        public static bool UsePlayerNameTransition = true;//是否使用玩家名字的翻译（对局内的名字）

        //24/12/17 新增项
        public static bool SkipLevelUpReward = true;//是否可以跳过升级界面二选一的奖励

        public static int SkipLevelUpRewardGold = 10;//跳过时获得的金币数
        //24/12/19 新增项
        public static bool AddModItem = true;//增加mod新增的技能


        //24/12/30 新增项 卡面及部分美术资源替换
        public static bool IsUseOtherResource = true;

        //25/1/1
        public static float MusicVolume = 0.1f;
        public static float SoundVolume = 1.0f;

        public static bool IsZh { get { return CurrentLang == "zh"; } }
        public static bool IsEn { get { return CurrentLang == "en"; } }//一般用来判断是否执行原方法
    }
}
