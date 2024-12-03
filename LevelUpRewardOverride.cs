using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class LevelUpRewardOverride
    {
        public static bool SDOSpecial(ref ShopDialogueObject __result, LevelUpReward __instance)
        {

            float num = __instance.dungeon.physical.PreferredCardWidth() * 1.2f;
            string text = null;
            string text2 = null;
            string text3 = __instance.rewardName;
            if (text3 == "Delete")
            {
                if (__instance.rewardAmount == 1)
                {
                    text = "Cleanse";
                    text2 = "Delete one card from your deck";
                }
                else
                {
                    text = "Sanctity";
                    text2 = "Delete " + __instance.rewardAmount + " cards from your deck";
                }
            }
            else if (text3 == "Upgrade")
            {
                if (__instance.rewardAmount == 1)
                {
                    text = "Upgrade";
                    text2 = "Upgrade one card from your deck";
                }
                else
                {
                    text = "Upgrade";
                    text2 = "Upgrade " + __instance.rewardAmount + " cards from your deck";
                }
            }
            else if (text3 == "Card")
            {
                text = "+Card";
                text2 = "Draw one additional card each turn";
            }
            else if (text3 == "Action")
            {
                text = "+Action";
                text2 = "Gain one additional action per round";
            }
            else if (text3 == "EquipmentSlot")
            {
                text = "+Equipment";
                text2 = "Gain one additional equipment slot";
            }
            else if (text3 == "Gold")
            {
                text = "Rich!";
                text2 = "Gain " + __instance.rewardAmount + " gold";
            }
            else if (text3 == "Health")
            {
                text = "Healthy";
                text2 = "Gain " + __instance.rewardAmount + " health";
            }
            else if (text3 == "Mana")
            {
                text = "Magical";
                text2 = "Gain " + __instance.rewardAmount + " mana";
            }
            else if (text3 == "Talent")
            {
                text = "Talented";
                text2 = "Choose an additional talent";
            }
            else if (text3 == "DodgeAbility")
            {
                text = "Dexterous";
                text2 = "Dodge 20% of attacks in combat";
            }
            else if (text3 == "UpgradeAll")
            {
                text = "Upgrade All";
                text2 = "Upgrade all upgradable cards in your deck";
            }
            else if (text3 == "RegenAbility")
            {
                text = "Perfect Body";
                text2 = "Heal 2 damage each turn";
            }
            else if (text3 == "DRAbility")
            {
                text = "Thick Skin";
                text2 = "Prevent one point of physical damage from each source";
            }
            else if (text3 == "FireShieldAbility")
            {
                text = "Flame Aura";
                text2 = "Deal 2 fire damage for each attack card an enemy plays";
            }
            else if (text3 == "FearAbility")
            {
                text = "Fear";
                text2 = "Opponent's maximum hand size is reduced by 1";
            }
            else if (text3 == "DungeonAbilityMurder")
            {
                num *= (float)2;
                text = "Murder";
                text2 = "Dungeon Ability (cooldown 4):  Kill a non-boss monster, gaining experience but not gold";
            }
            else if (text3 == "DungeonAbilityInvisibility")
            {
                num *= (float)2;
                text = "Invisibility";
                text2 = "Dungeon Ability (cooldown 3):  May move through monsters until your next combat";
            }
            else if (text3 == "DungeonAbilityAlchemy")
            {
                num *= (float)2;
                text = "Alchemy";
                text2 = "Dungeon Ability (cooldown 1):  Create a healing potion to add to your deck";
            }
            else if (text3 == "ChannelUpgrade")
            {
                text = "Channel Upgrade";
                text2 = "Your channel ability now gives 4 mana each use";
            }
            else if (text3 == "KaiUpgrade")
            {
                text = "Kai Blast";
                text2 = "Your Kai Strike ability is reset and now has a cooldown of 2";
            }
            else if (text3 == "GraceAbility")
            {
                text = "Grace";
                text2 = "Gain 5 life, 5 mana, and 1 action";
            }
            else if (text3 == "ChannelUp3")
            {
                text = "Channel Upgrade";
                text2 = "Your channel ability now gives three mana per action card";
            }
            else if (text3 == "ChannelUp4")
            {
                text = "Channel Upgrade";
                text2 = "Your channel ability now gives four mana per action card";
            }
            else if (text3 == "DiamondFist")
            {
                text = "Diamond Fist";
                text2 = "Your damage is piercing";
            }
            else if (text3 == "IronWill")
            {
                text = "Iron Will";
                text2 = "You are immune to most deck and hand manipulation";
            }
            else if (text3 == "BoneShieldPassive")
            {
                text = "Bone Shield";
                text2 = "You start combats with a 5 point shield";
            }
            else if (text3 == "BoastAbility")
            {
                text = "Boast";
                text2 = "Defeating a higher level monster reduces your dungeon ability cooldowns by an additional 1";
            }
            else if (text3 == "DungeonAbilityFindMonster")
            {
                num *= (float)2;
                text = "Make Camp";
                text2 = "Dungeon Ability (cooldown 4):  Create a makeshift camp to rest, recover, and take notes";
            }
            else if (text3 == "PassiveAmbush")
            {
                text = "Ambush";
                text2 = "Draw two cards at the start of each fight";
            }
            int num2 = 0;
            if (GameManager.IsIPhone())
            {
                num2 = 4;
            }
            SolveRewardNameTransition(ref text,ref text2, text3, __instance);

            ShopDialogueText shopDialogueText = SDB.CenteredText(num, text2, 32 + num2, Color.black);
            ShopDialogueText shopDialogueText2 = SDB.CenteredText(num, text, 36 + num2, Color.black);

            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueText[] { shopDialogueText2, shopDialogueText }, "VP", 0.5f);
            if (__instance.SpawnsChild() && __instance.primary)
            {
                ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Perform","LEVELUP"), () =>
                {
                    // 获取 MyClass 类型的私有方法 PrivateMethod
                    MethodInfo privateMethod = AccessTools.Method(typeof(LevelUpReward), "GainInternalFalse");
                    // 调用私有方法
                    privateMethod.Invoke(__instance, null);
                },
                    () =>
                    {
                        return !__instance.handled;
                    }
                    );
                shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", 0.2f);
            }
            ShopDialogueObject shopDialogueObject = SDB.Padded(shopDialogueAligned, new Vector2(num, shopDialogueAligned.r.height), ShopDialogueCardinal.center);
            SDB.Background(shopDialogueObject, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            shopDialogueObject.DoneBuilding();

            __result = shopDialogueObject;
            return false;
        }

        private static void SolveRewardNameTransition(ref string text,ref string text2,string rewardName, LevelUpReward __instance)
        {
            if (rewardName == "Delete")
            {
                if (__instance.rewardAmount == 1)
                {
                    text2 = TR.GetStr("LevelUpRewardDescription", text2);
                }
                else
                {
                    text2 = TR.GetStr("LevelUpRewardDescription", "Delete").Replace("{value}", __instance.rewardAmount.ToString());
                }
            }
            else if (rewardName == "Upgrade")
            {
                if (__instance.rewardAmount == 1)
                {
                    text2 = TR.GetStr("LevelUpRewardDescription", text2);
                }
                else
                {
                    text2 = TR.GetStr("LevelUpRewardDescription", "Upgrade").Replace("{value}", __instance.rewardAmount.ToString());
                }
            }
            else if (rewardName == "Gold")
            {
                text2 = TR.GetStr("LevelUpRewardDescription", "GainGold").Replace("{value}", __instance.rewardAmount.ToString());
            }
            else if (rewardName == "Health")
            {
                text2 = TR.GetStr("LevelUpRewardDescription", "GainHealth").Replace("{value}", __instance.rewardAmount.ToString()) ;
            }
            else if (rewardName == "Mana")
            {
                text2 = TR.GetStr("LevelUpRewardDescription", "GainMana").Replace("{value}", __instance.rewardAmount.ToString());
            }
            else
            {
                text2 = TR.GetStr("LevelUpRewardDescription", text2);
            }
            HelloMod.mLogger.LogMessage("LevelUpReward" + text);
            text = TR.GetStr("LevelUpReward", text);
            HelloMod.mLogger.LogMessage("LevelUpReward" + text);
            //TODO:LevelUpRewardDescription 的 空格对齐存在问题

        }
    }
}
