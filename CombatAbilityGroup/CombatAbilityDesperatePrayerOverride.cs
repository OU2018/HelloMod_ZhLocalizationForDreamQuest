using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.CombatAbilityGroup
{
    internal class CombatAbilityDesperatePrayerOverride : CombatAbilityPatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPrefix(
                typeof(CombatAbilityDesperatePrayer).GetMethod("DoMe", new Type[] { typeof(Player) }),
                typeof(CombatAbilityDesperatePrayerOverride).GetMethod("DoMe")
                );
        }

        public static bool DoMe(Player p,CombatAbilityDesperatePrayer __instance)
        {
            Game game = p.game;
            int num = game.InGameRandomRange(0, 4);
            string text = string.Empty;
            int num2 = num;
            if (num2 == 0)
            {
                text = "Knowledge!";
            }
            else if (num2 == 1)
            {
                text = "Protection!";
            }
            else if (num2 == 2)
            {
                text = "Death!";
            }
            else if (num2 == 3)
            {
                text = "Speed!";
            }
            else if (num2 == 4)
            {
                text = "Armored!";
            }
            text = TR.GetStr(TR.SK, text, "CARDEFFECT");
            if (game.IsPhysical() && p.infoblock)
            {
                game.physical.AddToVisualStackNoYield(p.infoblock, new object[]
                {
                text,
                3,
                Color.blue
                }, "ScrollTextDurationColor");
            }
            int num3 = num;
            if (num3 == 0)
            {
                p.Draw(5);
            }
            else if (num3 == 1)
            {
                p.GainShield(25);
            }
            else if (num3 == 2)
            {
                int num4 = p.Enemy().health / 2;
                if (num4 > 60)
                {
                    num4 = 60;
                }
                p.Enemy().SetHealth(p.Enemy().health - num4);
            }
            else if (num3 == 3)
            {
                p.game.AddExtraTurn(p);
            }
            else if (num3 == 4)
            {
                p.CreateEquipment("CelestialPlate");
                p.CreateEquipment("DeckOfWonder");
                p.CreateEquipment("Pendant");
            }
            return false;
        }
    }
}
