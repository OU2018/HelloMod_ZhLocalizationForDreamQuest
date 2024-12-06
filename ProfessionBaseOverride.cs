using HelloMod.DungeonFeatureGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class ProfessionBaseOverride
    {
        private static string classNameCache = string.Empty;
        public static void ClassNamePostfix(ref string __result)
        {
            if (!string.IsNullOrEmpty(__result))
            {
                classNameCache = __result;
                __result = TR.GetStr("ChooseClass", __result);
            }
        }
        //能力描述，属于 长段 的文字
        public static void AbilityDescriptionPostfix(ref string __result,ProfessionBase __instance)
        {
            if (!string.IsNullOrEmpty(__result))
            {
                __instance.ClassName();
                __result = HelloMod.Csv.GetTranslationByID("ProfessionAbilityDescription", "_" + classNameCache);
            }
        }
        //人物描述，属于 长段 的文字
        public static void DescriptionPostfix(ref string __result, ProfessionBase __instance)
        {
            if (!string.IsNullOrEmpty(__result))
            {
                __instance.ClassName();
                __result = HelloMod.Csv.GetTranslationByID("ProfessionDescription", "_" + classNameCache);
            }
        }
        //名字留到最后，考虑是否进行修改
        public static void GetPossibleNamesPostfix(ref string[] __result)
        {
            //TODO
        }
        //ProfessionDragon SmashPage 巨龙毁灭页面
        public static bool SmashPage(ProfessionDragon __instance)
        {
            string text = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_SmashPage");
            float x = __instance.dungeon.physical.WindowSize().x;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(TR.GetStr("DungeonFeatureName", "Dragon Smash!"), 48, Color.black);
            ShopDialogueText shopDialogueText = SDB.CenteredText(x, text, 32, Color.black);
            SDB.Background(shopDialogueText);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueText }, "VP", 0.1f);
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
            SDB.CancelButton(shopDialogueAligned, __instance.dungeon.WindowBack);
            shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }
    }
}
