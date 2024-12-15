using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloMod.ModFeature
{

    [Serializable]
    public class DungeonActionRandomAlchemy : DungeonAction
    {
        public override void Initialize()
        {
            base.Initialize();
            this.cooldown = 1;
        }

        public override string ButtonName()//按钮名称
        {
            return "Alchemy";
        }

        public override void Perform()//点击后的效果
        {
            this.dungeon.player.AddCard("HealingPotion");
        }
    }

}
