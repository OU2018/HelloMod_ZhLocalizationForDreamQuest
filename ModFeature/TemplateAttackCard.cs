using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod
{
    public class TemplateAttackCard : AttackCard
    {
        //可能在 CardList  GetCardList Postfix 添加, HashSet
        public override void Initialize()
        {
            this.PythonInitialize();
        }

        // Token: 0x0600086E RID: 2158 RVA: 0x0002CAE4 File Offset: 0x0002ACE4
        public override void PlayEffect()
        {
            this.PythonPlayEffect();
        }

        // Token: 0x0600086F RID: 2159 RVA: 0x0002CAEC File Offset: 0x0002ACEC
        public virtual void PythonInitialize()
        {
            this.cardName = "TemplateAttackCard";
            this.text = "Deal 1 @atk";
            this.flavorText = string.Empty;
            this.cost = 0;
            this.goldCost = 30;
            this.manaCost = 0;
            this.level = 0;
            this.maxLevel = 0;
            this.tier = 4;
            this.decayTo = "Attack3";
            base.Initialize();
            this.AIKeepValue = 30;
            this.AIPlaySequence = 50;
        }

        // Token: 0x06000870 RID: 2160 RVA: 0x0002CB68 File Offset: 0x0002AD68
        public virtual void PythonPlayEffect()
        {
            this.PoisonPlayer(3);
            this.DealDamage(1);
            this.ForceDiscard(2);
        }
    }
}
