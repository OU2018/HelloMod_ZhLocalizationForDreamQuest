using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod.ModFeature
{

    [Serializable]
    public class Mod_TemplatePotion : ActionCard
    {
        public override void Initialize()
        {
            this.PythonInitialize();
            this.usedUp = true;
        }

        public override void PlayEffect()
        {
            this.PythonPlayEffect();
            this.player.FullHeal();
            this.DealDamage(3, DamageTypes.PHYSICAL);
            this.PotionSelfDestruct();//药水永久删除
        }

        public virtual void PythonInitialize()
        {
            this.cardName = "Template Potion";
            this.text = "Deal 10 @atk .Full Heal.";
            this.flavorText = string.Empty;
            this.cost = 1;
            this.goldCost = 7;
            this.manaCost = 0;
            this.level = 0;
            this.maxLevel = 0;
            this.tier = 2;
            this.decayTo = string.Empty;
            base.Initialize();
            this.AIKeepValue = 90;
            this.AIPlaySequence = 50;
        }

        public virtual void PythonPlayEffect()
        {
        }

        public override string ImageName()
        {
            //return base.ImageName();
            return "Textures/Cards/" + "HealingPotion";
        }
    }
}
