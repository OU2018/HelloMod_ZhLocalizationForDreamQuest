using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class DungeonActionOverride
    {
        public static void ButtonNamePostfix(ref string __result,DungeonAction __instance)//地牢技能名称
        {
            if (__instance.GetType() == typeof(DungeonActionHoard)) {
                if (!DreamQuestConfig.IsEn)
                {
                    __result = TR.GetStr("DungeonAction", "Hoard").Replace(TR.PlaceHolder, (__instance as DungeonActionHoard).Cost().ToString());
                }
            }
            else
            {
                __result = TR.GetStr("DungeonAction", __result);
            }
        }

        public static bool DungeonActionHoard_HoardPerform(DungeonActionHoard __instance)
        {
            int num = __instance.Cost();
            int num2 = __instance.Cost() * 2;
            DungeonUser dungeonUser = GameManager.LoadCurrentUser();
            if (dungeonUser != null && dungeonUser.GetAttribute(UserAttribute.WINDRAGON2) != 0)
            {
                num = __instance.Cost() * 2;
                num2 = __instance.Cost() * 3;
            }
            string[] array = CardFinder.TwoRandomAvailableCardGoldRange(num, num2);
            float x = __instance.dungeon.physical.WindowSize().x;
            LevelUpReward levelUpReward = new LevelUpReward(array[0], 1, LevelUpRewardType.CARD, __instance.dungeon);
            LevelUpReward levelUpReward2 = new LevelUpReward(array[1], 1, LevelUpRewardType.CARD, __instance.dungeon);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose"), __instance.dungeon.player.LevelUpChooseFactory(levelUpReward));
            ShopDialogueButton shopDialogueButton2 = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose"), __instance.dungeon.player.LevelUpChooseFactory(levelUpReward2));
            ShopDialogueObject shopDialogueObject = levelUpReward.GetShopDialogueObject();
            ShopDialogueObject shopDialogueObject2 = levelUpReward2.GetShopDialogueObject();
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueObject, shopDialogueObject2 }, "HC", x * 0.9f);
            shopDialogueButton.UpperCenterTo(new Vector3(shopDialogueObject.Center().x, shopDialogueAligned.LowerLeft().y - 0.1f, shopDialogueObject.Center().z));
            shopDialogueButton2.UpperCenterTo(new Vector3(shopDialogueObject2.Center().x, shopDialogueAligned.LowerLeft().y - 0.1f, shopDialogueObject2.Center().z));
            shopDialogueAligned = SDB.Group(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton, shopDialogueButton2 });
            ShopDialogueText shopDialogueText = SDB.CenteredText(x, TR.GetStr(DungeonPhysicalOverride.TableKey, "New Power!"), 48, Color.black);
            ShopDialogueText shopDialogueText2 = SDB.CenteredText(x, TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose One:"), 32, Color.black);
            ShopDialogueObject shopDialogueObject3 = SDB.Align(new ShopDialogueText[] { shopDialogueText, shopDialogueText2 }, "VP", 0.5f);
            shopDialogueObject3 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject3, shopDialogueAligned }, "VP", 0.2f);
            shopDialogueObject3 = SDB.Padded(shopDialogueObject3, new Vector2(x, shopDialogueObject3.r.height), ShopDialogueCardinal.center);
            SDB.Background(shopDialogueObject3, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.white);
            shopDialogueObject3.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueObject3;
            shopDialogueObject3.DoneBuilding();
            return false;
        }

        public static bool DungeonActionDevour_Perform(Tile t, DungeonActionDevour __instance)
        {
            Monster monster = t.DestroyNormalMonster();
            t.Refresh();
            if (monster != null)
            {
                int num = monster.ExperienceValue();
                monster.Devour();
                __instance.dungeon.player.PlayerSpriteTextQueued("+" + num + " " + TR.GetStr(DungeonPhysicalOverride.TableKey, "EXP"), 3, Utility.darkGreen);
                __instance.dungeon.player.ProcessKill(monster);
            }
            return false;
        }
    }
}
