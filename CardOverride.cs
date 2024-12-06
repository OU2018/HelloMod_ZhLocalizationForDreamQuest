using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class CardOverride
    {
        public static bool BuildKeywordString(ref string __result, Card __instance)
        {
            List<string> list = new List<string>();
            if (__instance is Creature)
            {
                Creature creature = __instance as Creature;
                if (creature.GetAttribute(CreatureAttributes.HERO) > 0)
                {
                    list.Add("<Hero>");
                }
                if (creature.GetAttribute(CreatureAttributes.GRIXILLUSION) > 0)
                {
                    list.Add("<Hero>");
                }
                if (creature.GetAttribute(CreatureAttributes.LEVEL) > 0)
                {
                    list.Add("<Level " + creature.Level() + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.CHAMPION) > 0)
                {
                    list.Add("<Champion>");
                }
                if (creature.GetAttribute(CreatureAttributes.TOKEN) > 0)
                {
                    list.Add("<Token>");
                }
                if (creature.GetAttribute(CreatureAttributes.BLOODLUST) > 0)
                {
                    list.Add("<Bloodlust>");
                }
                if (creature.GetAttribute(CreatureAttributes.RALLY) > 0)
                {
                    list.Add("<Rally>");
                }
                if (creature.GetAttribute(CreatureAttributes.SLUGGISH) > 0)
                {
                    list.Add("<Sluggish>");
                }
                if (creature.GetAttribute(CreatureAttributes.GRACE) > 0)
                {
                    list.Add("<Grace>");
                }
                if (creature.GetAttribute(CreatureAttributes.TAUNT) > 0)
                {
                    list.Add("<Taunt>");
                }
                if (creature.GetAttribute(CreatureAttributes.DETAIN) > 0)
                {
                    list.Add("<Detain>");
                }
                if (creature.GetAttribute(CreatureAttributes.TAUNTED) > 0)
                {
                    list.Add("<Taunted>");
                }
                if (creature.GetAttribute(CreatureAttributes.DETAINED) > 0)
                {
                    list.Add("<Detained>");
                }
                if (creature.GetAttribute(CreatureAttributes.INVULNERABLE) > 0)
                {
                    list.Add("<Invulnerable>");
                }
                if (creature.GetAttribute(CreatureAttributes.RANGE) != 1)
                {
                    list.Add("<Range " + creature.GetAttribute(CreatureAttributes.RANGE) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.DAMAGE_REDUCTION) > 0)
                {
                    list.Add("<Damage Reduction " + creature.GetAttribute(CreatureAttributes.DAMAGE_REDUCTION) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.BASE_MOVES) != 1)
                {
                    list.Add("<Speed " + creature.GetAttribute(CreatureAttributes.BASE_MOVES) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.WEAKNESS) > 0)
                {
                    list.Add("<Weakness " + creature.GetAttribute(CreatureAttributes.WEAKNESS) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.POISON) > 0)
                {
                    list.Add("<Poison " + creature.GetAttribute(CreatureAttributes.POISON) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.POISONED) > 0)
                {
                    list.Add("<Poisoned " + creature.GetAttribute(CreatureAttributes.POISONED) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.STUNNED) > 0)
                {
                    list.Add("<Stunned>");
                }
                if (creature.GetAttribute(CreatureAttributes.MAXSHIELD) > 0)
                {
                    list.Add("<Shield " + creature.GetAttribute(CreatureAttributes.SHIELD) + " / " + creature.GetAttribute(CreatureAttributes.MAXSHIELD) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.BACKSTAB) > 0)
                {
                    list.Add("<Backstab>");
                }
                if (creature.GetAttribute(CreatureAttributes.BLINK) > 0)
                {
                    list.Add("<Blink>");
                }
                if (creature.GetAttribute(CreatureAttributes.TELEPORT) > 0)
                {
                    list.Add("<Teleport>");
                }
                if (creature.GetAttribute(CreatureAttributes.DODGE) > 0)
                {
                    list.Add("<Dodge>");
                }
                if (creature.GetAttribute(CreatureAttributes.ENRAGED) > 0)
                {
                    list.Add("<Enraged " + creature.GetAttribute(CreatureAttributes.ENRAGED) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.REBIRTH) > 0)
                {
                    list.Add("<Rebirth>");
                }
                if (creature.GetAttribute(CreatureAttributes.REGENERATION) > 0)
                {
                    list.Add("<Regeneration " + creature.GetAttribute(CreatureAttributes.REGENERATION) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.NOMOVE) > 0)
                {
                    list.Add("May not move");
                }
                if (creature.GetAttribute(CreatureAttributes.NOATTACK) > 0)
                {
                    list.Add("May not attack");
                }
                if (creature.GetAttribute(CreatureAttributes.UNIQUE) > 0)
                {
                    list.Add("<Unique>");
                }
                if (creature.GetAttribute(CreatureAttributes.STONESKIN) > 0)
                {
                    list.Add("<Stoneskin>");
                }
                if (creature.GetAttribute(CreatureAttributes.FLYING) > 0)
                {
                    list.Add("<Flying>");
                }
                if (creature.GetAttribute(CreatureAttributes.HASTE) > 0)
                {
                    list.Add("<Haste>");
                }
                if (creature.GetAttribute(CreatureAttributes.PERMSHIELD) > 0)
                {
                    list.Add("<Shield " + creature.GetAttribute(CreatureAttributes.PERMSHIELD) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.WEAKENED) > 0)
                {
                    list.Add("<Weakened " + creature.GetAttribute(CreatureAttributes.WEAKENED) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.REFLECT_DAMAGE) > 0)
                {
                    list.Add("<Reflect Damage>");
                }
                if (creature.GetAttribute(CreatureAttributes.LOOT) > 0)
                {
                    list.Add("<Loot " + creature.GetAttribute(CreatureAttributes.LOOT) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.INSPIRE) > 0)
                {
                    list.Add("<Inspire " + creature.GetAttribute(CreatureAttributes.INSPIRE) + ">");
                }
                if (creature.GetAttribute(CreatureAttributes.INSPIREX) > 0)
                {
                    list.Add("<Inspire X>");
                }
                if (creature.GetAttribute(CreatureAttributes.STEALTH) > 0)
                {
                    list.Add("<Stealth>");
                }
                if (creature.GetAttribute(CreatureAttributes.BURNING) > 0)
                {
                    list.Add("<Burning " + creature.GetAttribute(CreatureAttributes.BURNING) + ">");
                }
                for (int i = 0; i < creature.GetAttribute(CreatureAttributes.FLANKING); i++)
                {
                    list.Add("<Flanking>");
                }
                for (int i = 0; i < creature.GetAttribute(CreatureAttributes.CASCADE); i++)
                {
                    list.Add("<Cascade>");
                }
            }
            if (__instance is Shrine && !(__instance is WallBase))
            {
                Shrine shrine = __instance as Shrine;
                if (shrine.counters == 1)
                {
                    list.Add("<" + shrine.counters + " Counter>");
                }
                else
                {
                    list.Add("<" + shrine.counters + " Counters>");
                }
                if (shrine.damageReduction > 0)
                {
                    list.Add("<Damage Reduction " + shrine.damageReduction + ">");
                }
            }
            if (__instance is Enchantment)
            {
                Enchantment enchantment = __instance as Enchantment;
                if (enchantment.counters == 1)
                {
                    list.Add("<" + enchantment.counters + " Counter>");
                }
                else if (enchantment.counters >= 0)
                {
                    list.Add("<" + enchantment.counters + " Counters>");
                }
                if (enchantment.tactic)
                {
                    list.Add("<Tactic>");
                }
            }
            if (__instance is EquipmentCard)
            {
                EquipmentCard equipmentCard = __instance as EquipmentCard;
                if (equipmentCard.ShowCharge())
                {
                    list.Add(TR.GetStr(DungeonPhysicalOverride.TableKey, "Charge") + " " + equipmentCard.charge);
                }
            }
            if (__instance.IsLatent())
            {
                list.Add("<Latent>");
            }
            if (__instance.temporary)
            {
                list.Add("<Temporary>");
            }
            if (__instance.illusionary)
            {
                list.Add("<Illusionary>");
            }
            if (__instance.theft)
            {
                list.Add("<Theft>");
            }
            if (__instance.isFreeOnce)
            {
                list.Add("<Free>");
            }
            if (__instance.basic)
            {
                list.Add("<Basic>");
            }
            if (__instance.influence)
            {
                list.Add("<Influence>");
            }
            __result = string.Join(" ^, ", list.ToArray());
            return false;
        }

        public static bool ChaosPrayer_PlayEffect(ChaosPrayer __instance)
        {
            __instance.PythonPlayEffect();
            int num = __instance.game.InGameRandomRange(0, 3);
            string text = string.Empty;
            if (num == 0)
            {
                text = "Healing!";
            }
            else if (num == 1)
            {
                text = "Flame!";
            }
            else if (num == 2)
            {
                text = "Madness!";
            }
            else if (num == 3)
            {
                text = "Sloth!";
            }
            text = TR.GetStr(TR.SK, text);
            if (__instance.IsPhysical())
            {
                __instance.game.physical.AddToVisualStackNoYield(__instance.physical, new object[]
                {
                text,
                3,
                Color.blue
                }, "ScrollTextDurationColor");
            }
            if (num == 0)
            {
                __instance.Heal(5);
            }
            else if (num == 1)
            {
                if (__instance.IsPhysical())
                {
                    __instance.player.DealDamage(3, DamageTypes.FIRE, __instance.player);
                }
                else
                {
                    __instance.player.DealDamage(3, DamageTypes.FIRE, __instance.player.Enemy());
                }
            }
            else if (num == 2)
            {
                __instance.ForceDiscard(2);
            }
            else if (num == 3)
            {
                __instance.LoseActions(2);
            }
            return false;
        }
    }
}
