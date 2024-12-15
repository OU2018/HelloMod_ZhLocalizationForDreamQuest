using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod.ModFeature
{

    [Serializable]
    public class TemplatePotion : ActionCard
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
            this.PotionSelfDestruct();//药水永久删除
        }

        public virtual void PythonInitialize()
        {
            this.cardName = "Healing Potion";
            this.text = "Completely heal. This card is removed from the game and your deck permanently after use.";
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

        // Token: 0x06000848 RID: 2120 RVA: 0x0002C63C File Offset: 0x0002A83C
        public virtual void PythonPlayEffect()
        {
        }
    }
}
