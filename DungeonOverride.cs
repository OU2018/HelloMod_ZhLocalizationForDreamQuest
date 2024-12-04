using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
