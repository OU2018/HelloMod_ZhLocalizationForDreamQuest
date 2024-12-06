using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class TargetFinderBaseOverride
    {
        public static bool BuildGUI(TargetFinderBase __instance)
        {
            __instance.action.card.game.SuppressTopMessage();
            ShopDialogueText shopDialogueText = SDB.Text((float)8, __instance.@params.text, 32, Color.black);
            float width = shopDialogueText.r.width;
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2((float)1, 0.35f), TR.GetStr(DungeonPhysicalOverride.TableKey, "Done"),__instance.EarlyDone, __instance.CanBeDone);
            ShopDialogueButton shopDialogueButton2;
            ShopDialogueObject shopDialogueObject;

            if (__instance.@params.canCancel)
            {
                shopDialogueButton2 = SDB.BasicButton(new Vector2((float)1, 0.35f), "None", __instance.NoneButton);
                shopDialogueObject = SDB.Align(new ShopDialogueButton[] { shopDialogueButton, shopDialogueButton2 }, "HP", 0.8f);
            }
            else
            {
                shopDialogueObject = shopDialogueButton;
            }
            shopDialogueObject = SDB.Align(new ShopDialogueObject[] { shopDialogueText, shopDialogueObject }, "VP", 0.3f);
            SDB.Background(shopDialogueObject, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            if (GameManager.IsIPhone())
            {
                shopDialogueObject.Grow(1.3f);
            }
            shopDialogueObject.UpperCenterTo(new Vector3(-1.4f, 3.8f, (float)4));
            __instance.action.card.game.activeShop = shopDialogueObject;
            shopDialogueObject.DoneBuilding();
            return false;
        }
    }
}
