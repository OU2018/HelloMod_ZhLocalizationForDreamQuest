using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    internal class DungeonPhysicalOverride
    {
        private static Dungeon mDungeon = null;

        public const string TableKey = "DungeonPhysical";

        public static bool InGameMenuOverride(DungeonPhysical __instance)
        {
            mDungeon = __instance.dungeon;
            float num = 4.5f;
            float num2 = 2.5f;
            float num3 = 0.3f;
            float num4 = 0.4f;
            if (GameManager.IsIPhone())
            {
                num2 = 3f;
                num3 = 0.5f;
                num4 = 0.5f;
            }
            List<ShopDialogueObject> list = new List<ShopDialogueObject>();
            if (DreamQuestConfig.ActiveCheatMenu)
            {
                list.Add(SDB.BasicButton(new Vector2((float)4, num4), TR.GetStr(TableKey, "Cheats"), __instance.CallCheatMenu));
            }
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2((float)4, num4), string.Empty, __instance.SoundToggle);
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(4f, SoundButtonLabel, Color.white);
            HelloMod.SetTextMeshFont(shopDialogueDynamicText.text, 32);
            shopDialogueDynamicText.CenterTo(shopDialogueButton.Center() + new Vector3((float)0, (float)0, 0.05f));
            ShopDialogueAligned shopDialogueAligned = SDB.Group(new ShopDialogueObject[] { shopDialogueButton, shopDialogueDynamicText });
            list.Add(shopDialogueAligned);
            list.Add(SDB.BasicButton(new Vector2((float)4, num4), TR.GetStr(TableKey, "View Bestiary"), __instance.Bestiary));
            list.Add(SDB.BasicButton(new Vector2((float)4, num4), TR.GetStr(TableKey, "Save and Exit"), __instance.SaveAndQuit));
            ShopDialogueObject shopDialogueObject = SDB.Align(list.ToArray(), "VP", num3);
            shopDialogueObject = SDB.Padded(shopDialogueObject, new Vector2(num, num2), ShopDialogueCardinal.center);
            SDB.Background(shopDialogueObject, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            SDB.CancelButton(shopDialogueObject, mDungeon.WindowBack);
            shopDialogueObject.UpperCenterTo(mDungeon.ShopLocation());
            mDungeon.activeShop = shopDialogueObject;
            return false;
        }

        public static string SoundButtonLabel()
        {
            return (!MusicManager.IsSoundOn()) ? TR.GetStr(TableKey, "Turn Sound ON") : TR.GetStr(TableKey, "Turn Sound OFF");
        }

        public static bool ConstructSaveAndQuitOverride(DungeonPhysical __instance)
        {
            Vector2 vector = new Vector2((float)5, (float)2);
            if (GameManager.IsIPhone())
            {
                vector = new Vector2((float)7, (float)3);
            }
            ShopDialogueObject shopDialogueObject = SDB.ModalDialogue(vector, TR.GetStr(TableKey, "Are you sure you want to exit?"), new string[] { TR.GetStr(TableKey, "Yes"), TR.GetStr(TableKey, "No") }, (x)=> { __instance.SaveAndQuitCallback(x); });
            shopDialogueObject.UpperCenterTo(mDungeon.ShopLocation() + new Vector3((float)0, -1f, (float)0));
            mDungeon.activeShop = shopDialogueObject;
            return false;
        }

        public static void AKMTextPostfix(ref string __result)
        {
            __result = TR.GetStr(TableKey, __result);
        }
    }
}
