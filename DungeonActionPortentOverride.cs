using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class DungeonActionPortentOverride
    {
        public static string DungeonText(string[] floorBoss, Environments[] floorEnvironments)
        {
            string text = HelloMod.Csv.GetTranslationByID("DungeonActionPortentText", "_dungeonText") + "\n";
            for (int i = 0; i < 3; i++)
            {
                text += "\t" + OneDungeonText(MonsterFinder.GetMonsterName(floorBoss[i]), floorEnvironments[i]);
                if (i < 2)
                {
                    text += "\n";
                }
            }
            return text;
        }

        public static string OneDungeonText(string boss, Environments environment)
        {
            string text = HelloMod.Csv.GetTranslationByID("DungeonActionPortentText", "_Environments" + "_" + environment);
            //text = text.Replace(TR.PlaceHolder, "<" + TR.GetStr("MonsterName", boss) + ">");//会有错位问题，需要对Format方法再进行改造||疑似是原生问题
            text = text.Replace(TR.PlaceHolder, TR.GetStr("MonsterName", boss));
            return text;
        }

        public static string FinalBossText(BossAttr[] bossAttributes)
        {
            string text = HelloMod.Csv.GetTranslationByID("DungeonActionPortentText", "_finalBossText") + "\n";
            for (int i = 0; i < bossAttributes.Length; i++)
            {
                BossAttr bossAttr = bossAttributes[i];
                text += "\t" + HelloMod.Csv.GetTranslationByID("DungeonActionPortentText", "_BossAttr" + "_" + bossAttr);
                if (i < 2)
                {
                    text += "\n";
                }
            }
            return text;
        }

        public static void BuildText(ref string __result, DungeonActionPortent __instance)
        {
            if (DreamQuestConfig.IsEn)
                return;
            string text = DungeonText(__instance.dungeon.floorBoss, __instance.dungeon.floorEnvironments);
            if (__instance.dungeon.MaxDepth() == 4)
            {
                text += "\n\n" + FinalBossText(__instance.dungeon.bossAttributes);
            }
            __result = text;
        }

        public static bool PortentPerform(DungeonActionPortent __instance)
        {
            float x = __instance.dungeon.physical.WindowSize().x;
            int num = 0;
            if (GameManager.IsIPhone())
            {
                num = 8;
            }
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(TR.GetStr(DungeonPhysicalOverride.TableKey, "A Vision"), (int)((float)48 + (float)num * 1.5f), Color.black);
            ShopDialogueText shopDialogueText = SDB.Text(x, __instance.BuildText(), 32 + num, Color.black);
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
