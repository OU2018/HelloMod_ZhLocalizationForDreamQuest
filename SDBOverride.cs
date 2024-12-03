using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class SDBOverride
    {
        //战斗中右下角的技能显示
        public static bool ActionIconDescription(ref ShopDialogueObject __result, ActionBase a, string text, bool good)
        {
            string _noteKey = "ActionIconDescription";
            HelloMod.mLogger.LogMessage(_noteKey);
            ShopDialogueObject shopDialogueObject = SDB.CenteredText((float)3, text, 32, Color.black);
            if (good && a.combatAbility.cooldown > 0)
            {
                string text2 = TR.GetStr(DungeonPhysicalOverride.TableKey, "(Cooldown: ", _noteKey) + a.combatAbility.cooldown + ")";
                ShopDialogueText shopDialogueText = SDB.CenteredText((float)3, text2, 32, Color.black);
                shopDialogueObject = SDB.Align(new ShopDialogueObject[] { shopDialogueObject, shopDialogueText }, "VP", 0.02f);
            }
            ShopDialogueObject shopDialogueObject2;
            if (!good)
            {
                shopDialogueObject2 = SDB.CenteredText((float)3, a.combatAbility.FailText(), 32, Color.red);
            }
            else
            {
                shopDialogueObject2 = SDB.BasicButton(new Vector2(1.6f, 0.35f), TR.GetStr(DungeonPhysicalOverride.TableKey, "Use", _noteKey), SDB.ActionIconHelper(a));
            }
            ShopDialogueObject shopDialogueObject3 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject, shopDialogueObject2 }, "VP", 0.3f);
            SDB.Background(shopDialogueObject3, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
            __result = shopDialogueObject3;
            return false;
        }
        //在升级界面等处出现的大的技能信息
        public static bool CombatAbilityBig(ref ShopDialogueObject __result, CombatAbility c, float width, int fontSize)
        {
            string _noteKey = "CombatAbilityBig";
            HelloMod.mLogger.LogMessage(_noteKey);

            float num = 0.7125f;
            ShopDialogueObject shopDialogueObject = SDB.ShopTexture(num, num, c.GetTexture());
            ShopDialogueText shopDialogueText = SDB.CenteredText(width * 0.75f, c.Name() + TR.GetStr(DungeonPhysicalOverride.TableKey, " (Combat Ability)", _noteKey), fontSize, Color.blue);
            ShopDialogueText shopDialogueText2 = SDB.CenteredText(width * 0.75f, c.Description(), fontSize - 4, Color.black);
            string text = TR.GetStr(DungeonPhysicalOverride.TableKey, "combats", _noteKey);
            if (c.cooldown == 1)
            {
                text = TR.GetStr(DungeonPhysicalOverride.TableKey, "combat", _noteKey);
            }
            string text2 = c.cooldown + " " + text;
            if (c.cooldown == 0)
            {
                text2 = TR.GetStr(DungeonPhysicalOverride.TableKey, "None", _noteKey);
            }
            ShopDialogueText shopDialogueText3 = SDB.CenteredText(width * 0.75f, TR.GetStr(DungeonPhysicalOverride.TableKey, "Cooldown: ", _noteKey) + text2, fontSize - 4, Color.black);
            ShopDialogueObject shopDialogueObject2 = SDB.Align(new ShopDialogueText[] { shopDialogueText, shopDialogueText3 }, "VP", 0.2f);
            shopDialogueObject2 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject2, shopDialogueText2 }, "VP", 0.3f);
            shopDialogueObject2 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject, shopDialogueObject2 }, "HP", 0.2f);
            SDB.Background(shopDialogueObject2, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            __result = shopDialogueObject2;
            return false;
        }
        //显示在打开的牌组菜单中
        public static bool CombatAbilityLittle(ref ShopDialogueObject __result, CombatAbility c, float width, int fontSize)
        {
            string _noteKey = "CombatAbilityLittle";
            HelloMod.mLogger.LogMessage(_noteKey);

            float num = 0.7f;
            ShopDialogueObject shopDialogueObject = SDB.ShopTexture(num, num, c.GetTexture());
            ShopDialogueObject shopDialogueObject2;
            if (c.currentCooldown > 0)
            {
                shopDialogueObject2 = SDB.CenteredText(width * 0.75f, c.Name(), fontSize, Color.grey);
            }
            else
            {
                shopDialogueObject2 = SDB.CenteredText(width * 0.75f, c.Name(), fontSize, Color.blue);
            }
            ShopDialogueText shopDialogueText = SDB.CenteredText(width * 0.75f, c.Description(), fontSize - 4, Color.black);
            ShopDialogueObject shopDialogueObject3;
            if (c.currentCooldown > 0)
            {
                shopDialogueObject3 = SDB.CenteredText(width * 0.75f, TR.GetStr(DungeonPhysicalOverride.TableKey, "Cooldown Remaining: ", _noteKey) + c.currentCooldown + "/" + c.cooldown, fontSize - 4, Color.black);
            }
            else
            {
                shopDialogueObject3 = SDB.CenteredText(width * 0.75f, TR.GetStr(DungeonPhysicalOverride.TableKey, "Ready! (Cooldown ", _noteKey) + c.cooldown + ")", fontSize - 4, Color.black);
            }
            ShopDialogueObject shopDialogueObject4 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject2, shopDialogueObject3 }, "VP", 0.1f);
            shopDialogueObject4 = SDB.Align(new ShopDialogueObject[] { shopDialogueObject4, shopDialogueText }, "VP", 0.1f);
            __result = SDB.Align(new ShopDialogueObject[] { shopDialogueObject, shopDialogueObject4 }, "HP", 0.2f);
            return false;
        }
        
    }
}
