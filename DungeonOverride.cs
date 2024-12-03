using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    internal class DungeonOverride
    {
        public static void BasicDeckViewer(Dungeon __instance)
        {
            if (__instance.player.equipSlots > 0)
            {
                __instance.DeckViewer("Current Deck \n " + __instance.player.CardsInDeck() + " Cards", __instance.EquipmentString, __instance.BuildCombatAbilities());
                __instance.physical.AddEquippedButtons();
            }
            else
            {
                __instance.DeckViewer("Current Deck \n " + __instance.player.CardsInDeck() + " Cards", __instance.BuildCombatAbilities());
            }
        }
    }
}
