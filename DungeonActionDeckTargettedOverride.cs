using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class DungeonActionDeckTargettedOverride
    {
        public static void DungeonActionTalentDeckTargetted_ConfirmTextPostfix(ref string __result)
        {
            if (!__result.Equals(string.Empty))
            {
                string noteKey = "ConfirmTextPostfix";
                __result = __result.Replace("You may choose up to", TR.GetStr(DungeonPhysicalOverride.TableKey, "You may choose up to", noteKey));
                __result = __result.Replace("cards.  Are you sure you want to only choose", TR.GetStr(DungeonPhysicalOverride.TableKey, " cards.  Are you sure you want to only choose ", noteKey));
                __result = __result.Replace("?", TR.GetStr(DungeonPhysicalOverride.TableKey, "?", noteKey));
            }
        }

        public virtual void DungeonActionLevelUpDeckTargetted_BuildFromReward(LevelUpReward r, DungeonActionLevelUpDeckTargetted __instance)
        {
            __instance.requiredCards = 0;
            __instance.reward = r;
            string rewardName = r.rewardName;
            if (rewardName == "Upgrade")
            {
                if (r.rewardAmount == 1)
                {
                    __instance.titleString = "Choose up to 1 card to upgrade";
                }
                else
                {
                    __instance.titleString = "Choose up to " + r.rewardAmount + " cards to upgrade";
                }
                __instance.confirmName = "Upgrade";
                __instance.shouldCull = true;
                __instance.allowFunction = __instance.AllowOnlyUpgradable;
                __instance.maxCards = r.rewardAmount;
                if (r.rewardAmount > 1)
                {
                    __instance.multiCards = true;
                }
            }
            else if (rewardName == "Delete")
            {
                if (r.rewardAmount == 1)
                {
                    __instance.titleString = "Choose up to 1 card to delete";
                }
                else
                {
                    __instance.titleString = "Choose up to " + r.rewardAmount + " cards to delete";
                }
                __instance.confirmName = "Delete";
                __instance.maxCards = r.rewardAmount;
                if (r.rewardAmount > 1)
                {
                    __instance.multiCards = true;
                }
            }
        }

        public virtual void DungeonActionTalentDeckTargetted_CreateConfirmVerification(DungeonActionTalentDeckTargetted __instance)
        {
            Vector2 vector = new Vector2((float)5, (float)2);
            if (GameManager.IsIPhone())
            {
                vector = new Vector2((float)7, (float)3);
            }
            ShopDialogueObject shopDialogueObject = SDB.ModalDialogue(vector, __instance.ConfirmText(), new string[] { "Yes", "Cancel" }, __instance.ModalHandlerConfirm);
            shopDialogueObject.UpperCenterTo(new Vector3(__instance.dungeon.ShopLocation().x, (float)2, (float)6));
            __instance.dungeon.activeShop = shopDialogueObject;
        }

        public virtual void DungeonActionTalentDeckTargetted_BuildFromTalent(DungeonTalent t, DungeonActionTalentDeckTargetted __instance)
        {
            __instance.talent = t;
            string internalName = t.internalName;
            if (internalName == "Upgrade1")
            {
                __instance.titleString = "Choose a card to upgrade";
                __instance.confirmName = "Upgrade";
                __instance.shouldCull = true;
                __instance.allowFunction = __instance.AllowOnlyUpgradable;
            }
            else if (internalName == "Purity")
            {
                __instance.titleString = "Choose up to 3 cards to delete";
                __instance.confirmName = "Delete";
                __instance.maxCards = 3;
                __instance.multiCards = true;
            }
            else if (internalName == "CopyTalent")
            {
                __instance.titleString = "Choose a card to copy";
                __instance.confirmName = "Copy";
            }
            else if (internalName == "Adventurous")
            {
                __instance.titleString = "Choose a card to acquire";
                __instance.confirmName = "Acquire";
                __instance.allCards = true;
            }
        }

        public virtual void DungeonActionDynamicDeckTargetted_BuildFromString(string s,DungeonActionDynamicDeckTargetted __instance)
        {
            __instance.requiredCards = 0;
            __instance.ddtName = s;
            string text = __instance.ddtName;
            if (text == "Upgrade")
            {
                __instance.titleString = "Choose a card to upgrade";
                __instance.confirmName = "Upgrade";
                __instance.shouldCull = true;
                __instance.allowFunction = __instance.AllowOnlyUpgradable;
            }
            else if (text == "Delete")
            {
                __instance.titleString = "Choose a card to delete";
                __instance.confirmName = "Delete";
                __instance.maxCards = 1;
            }
            else if (text == "Delete2")
            {
                __instance.titleString = "Choose up to TWO cards to delete";
                __instance.confirmName = "Delete";
                __instance.maxCards = 2;
                __instance.multiCards = true;
            }
        }

    }
}
