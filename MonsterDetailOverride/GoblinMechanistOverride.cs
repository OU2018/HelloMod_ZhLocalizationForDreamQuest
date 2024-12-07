
using UnityEngine;

namespace HelloMod.MonsterDetailOverride
{
    internal class GoblinMechanistOverride
    {
        public static bool StartTurn(Player p, GoblinMechanist __instance)
        {
            if (p == p.game.them)
            {
                if (__instance.IsPhysical())
                {
                    p.game.physical.AddToVisualStackNoYield(p.infoblock, new object[]
                    {
                    TR.GetStr(MainMenuOverride.TableKey, "Tinkering...","GoblinMechanist"),
                    3,
                    Color.blue
                    }, "ScrollTextDurationColor");
                }
                if (p.game.InGameRandomRange(0, 4) == 0)
                {
                    if (__instance.IsPhysical())
                    {
                        p.game.physical.AddToVisualStackNoYield(p.infoblock, new object[]
                        {
                        TR.GetStr(MainMenuOverride.TableKey, "EXPLOSION!","GoblinMechanist"),
                        3,
                        Color.red
                        }, "ScrollTextDurationColor");
                    }
                    p.TakeDamage(5, DamageTypes.FIRE);
                }
                else
                {
                    Card card = __instance.BuildRandomEquipment(p);
                    if (__instance.IsPhysical())
                    {
                        p.game.physical.AddToVisualStackNoYield(p.infoblock, new object[]
                        {
                        TR.GetStr(MainMenuOverride.TableKey, "Built ","GoblinMechanist") + card.cardName,
                        3,
                        Color.green
                        }, "ScrollTextDurationColor");
                    }
                }
            }
            return false;
        }
    }

}
