using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class DungeonOverride
    {
        public static bool BasicDeckViewer(Dungeon __instance)
        {
            if (__instance.player.equipSlots > 0)
            {
                __instance.DeckViewer(TR.GetStr("DungeonFeatureName", "Current Deck") + " \n " + __instance.player.CardsInDeck() + TR.GetStr("DungeonFeatureName", "Cards"), __instance.EquipmentString, __instance.BuildCombatAbilities());
                __instance.physical.AddEquippedButtons();
            }
            else
            {
                __instance.DeckViewer(TR.GetStr("DungeonFeatureName", "Current Deck") + " \n " + __instance.player.CardsInDeck() + TR.GetStr("DungeonFeatureName", "Cards"), __instance.BuildCombatAbilities());
            }
            return false;
        }

        public static void FinalBossPowers(ref string __result,Dungeon __instance)
        {
            string text = string.Empty;
            for (int i = 0; i < __instance.bossAttributes.Length; i++)
            {
                BossAttr bossAttr = __instance.bossAttributes[i];
                text += HelloMod.Csv.GetTranslationByID("MonsterPower", "_" + bossAttr.ToString());
                if (i < 2)
                {
                    text += "\n";
                }
            }
            HelloMod.mLogger.LogMessage(text);
            __result = text;
        }

        public static bool EquipmentString(TextMesh t,Dungeon __instance)
        {
            string text = string.Empty;
            if (__instance.player.equipSlots > 0)
            {
                text = "\n" + __instance.player.NumEquipped() + "/" + __instance.player.equipSlots + " " + TR.GetStr(DungeonPhysicalOverride.TableKey, "Equipment Worn");
            }
            if (__instance.player.NumEquipped() < __instance.player.equipSlots && __instance.physical.equipButtons != null && __instance.physical.equipButtons.Count > __instance.player.NumEquipped())
            {
                t.renderer.material.color = Color.red;
            }
            else
            {
                t.renderer.material.color = Color.white;
            }
            t.text = text;
            return false;
        }

        //TODO:弃用
        public static bool InternalDeckViewer(ShopDialogueObject p, string titleString, List<ShopDialogueButton> b, Action onCancel, bool hasDynamicTitle, Action<TextMesh> dynamicTitle, ShopDialogueObject extra, Dungeon __instance)
        {
            HelloMod.mLogger.LogInfo("内部卡组查看器-调用！");
            int num = 36;
            if (GameManager.IsIPhone())
            {
                num = 48;
            }
            ShopDialogueObject shopDialogueObject = SDB.CenteredText((float)6, titleString, num, Color.white);
            if (hasDynamicTitle)
            {
                shopDialogueObject = SDB.Align(new ShopDialogueObject[]
                {
                shopDialogueObject,
                SDB.FancyDynamicText((TextMesh t)=>{  dynamicTitle.Invoke(t); } , num)
                }, "VP", 0.1f);
            }
            ShopDialogueObject shopDialogueObject2 = null;
            if (b != null && b.Count > 0)
            {
                shopDialogueObject2 = SDB.Align(b.ToArray(), "HC", (float)6);
            }
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueObject, p }, "VP", 0.2f);
            if (extra)
            {
                shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, extra }, "VP", 0.2f);
            }
            if (shopDialogueObject2)
            {
                shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueObject2 }, "VP", 0.2f);
            }
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
            SDB.CancelButton(shopDialogueAligned,()=> { onCancel.Invoke(); } );
            shopDialogueAligned.UpperCenterTo(__instance.ShopLocation());
            __instance.MoveDeckbuilder(shopDialogueAligned);
            shopDialogueAligned.DoneBuilding();
            __instance.activeShop = shopDialogueAligned;
            HelloMod.mLogger.LogInfo("内部卡组查看器-显示完成！");

            return false;
        }
    }
}
