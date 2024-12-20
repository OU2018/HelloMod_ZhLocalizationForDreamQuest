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
            this.cooldown = 0;
        }

        public override string ButtonName()//按钮名称
        {
            return TR.GetStr("DungeonAction", "Random Alchemy");
        }

        public override void Perform()//点击后的效果
        {
            this.dungeon.player.AddCard("HealingPotion");
            this.dungeon.player.AddCard("Pierce3");
            this.dungeon.player.AddCard("Mod_TemplatePotion");

            foreach (var item in this.dungeon.player.deck)
            {
                HelloMod.mLogger.LogMessage(item);
            }
        }
    }
}
