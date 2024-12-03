using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod.CombatAbilityGroup
{
    internal abstract class CombatAbilityPatch
    {
        protected static string _abilityTableName = "AbilityName";
        protected static string _abilityDesTableName = "AbilityDescription";

        public abstract void Patch(Harmony harmony, HelloMod modCenter);
    }
}
