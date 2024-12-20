using HelloMod.ModFeature;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityScript.Lang;

namespace HelloMod
{
    internal class DungeonPhysicalOverride
    {
        private static Dungeon mDungeon = null;

        public const string TableKey = "DungeonPhysical";

        public static bool InGameMenuOverride(DungeonPhysical __instance)
        {
            mDungeon = __instance.dungeon;
            float win_width = 4.5f;
            float win_height = 2.5f;
            float space_y = 0.3f;
            float btn_height = 0.4f;
            if (GameManager.IsIPhone())
            {
                win_height = 3f;
                space_y = 0.5f;
                btn_height = 0.5f;
            }
            List<ShopDialogueObject> list = new List<ShopDialogueObject>();
            if (DreamQuestConfig.ActiveCheatMenu)
            {
                list.Add(SDB.BasicButton(new Vector2((float)4, btn_height), TR.GetStr(TableKey, "Cheats"), __instance.CallCheatMenu));
            }
            if (DreamQuestConfig.AddModItem)
            {
                list.Add(SDB.BasicButton(new Vector2((float)4, btn_height), TR.GetStr(TableKey, "Mod Item"), ()=> { CallModMenu(__instance); }));
            }
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2((float)4, btn_height), string.Empty, __instance.SoundToggle);
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(4f, SoundButtonLabel, Color.white);
            HelloMod.SetTextMeshFont(shopDialogueDynamicText.text, 32);
            shopDialogueDynamicText.CenterTo(shopDialogueButton.Center() + new Vector3((float)0, (float)0, 0.05f));
            ShopDialogueAligned shopDialogueAligned = SDB.Group(new ShopDialogueObject[] { shopDialogueButton, shopDialogueDynamicText });
            list.Add(shopDialogueAligned);
            list.Add(SDB.BasicButton(new Vector2((float)4, btn_height), TR.GetStr(TableKey, "View Bestiary"), __instance.Bestiary));
            list.Add(SDB.BasicButton(new Vector2((float)4, btn_height), TR.GetStr(TableKey, "Save and Exit"), __instance.SaveAndQuit));

            float total_height = 0;
            //total_height += space_y;
            total_height = btn_height * list.Count + space_y * (list.Count - 1);
            win_height = total_height;

            ShopDialogueObject shopDialogueObject = SDB.Align(list.ToArray(), "VP", space_y);
            shopDialogueObject = SDB.Padded(shopDialogueObject, new Vector2(win_width, win_height), ShopDialogueCardinal.center);
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

        public static void CallModMenu(DungeonPhysical __instance)
        {
            __instance.dungeon.SubWindow(() => { ModMenu(__instance); });
        }

        public static void ModMenu(DungeonPhysical __instance)
        {
            float num = 4.5f;
            float num2 = 5f;
            ShopDialogueObject shopDialogueObject = SDB.Align(new System.Collections.Generic.List<ShopDialogueObject>
            {
                SDB.BasicButton(new Vector2((float)4, 0.4f), TR.GetStr(TableKey, "Random Alchemy"), ()=>{ RandomAlchemy(__instance.dungeon.player); }),
            }.ToArray(), "VP", 0.3f);
            shopDialogueObject = SDB.Padded(shopDialogueObject, new Vector2(num, num2), ShopDialogueCardinal.center);
            SDB.Background(shopDialogueObject, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            SDB.CancelButton(shopDialogueObject, __instance.dungeon.WindowBack);
            shopDialogueObject.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueObject;
        }

        public static void RandomAlchemy(DungeonPlayer player)
        {
            player.AddAction(new DungeonActionRandomAlchemy());
        }
        //只是复制到这里作为参考 TODO：
        public static void CallCheatMenu(DungeonPhysical __instance)
        {
            __instance.dungeon.SubWindow(__instance.CheatMenu);
        }
        //只是复制到这里作为参考 TODO：
        public virtual void CheatMenu(DungeonPhysical __instance)
        {
            float num = 4.5f;
            float num2 = 5f;
            ShopDialogueObject shopDialogueObject = SDB.Align(new System.Collections.Generic.List<ShopDialogueObject>
            {
                SDB.BasicButton(new Vector2((float)4, 0.4f), "Reveal Map", __instance.RevealMap),
                SDB.BasicButton(new Vector2((float)4, 0.4f), "Level Up", __instance.LevelUp),
                SDB.BasicButton(new Vector2((float)4, 0.4f), __instance.AKMText(), __instance.AutoKillMonsters),
                SDB.BasicButton(new Vector2((float)4, 0.4f), "Gain Gold", __instance.CheatGainGold),
                SDB.BasicButton(new Vector2((float)4, 0.4f), "Uber Teleport", __instance.UberTeleport),
                SDB.BasicButton(new Vector2((float)4, 0.4f), "Gain 50 exp", __instance.GainExp),
                SDB.BasicButton(new Vector2((float)4, 0.4f), "Gain Achievement Points", __instance.GainAchievements),
                SDB.BasicButton(new Vector2((float)4, 0.4f), "Reveal All Monsters", __instance.RevealMonsters)
            }.ToArray(), "VP", 0.3f);
            shopDialogueObject = SDB.Padded(shopDialogueObject, new Vector2(num, num2), ShopDialogueCardinal.center);
            SDB.Background(shopDialogueObject, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            SDB.CancelButton(shopDialogueObject, __instance.dungeon.WindowBack);
            shopDialogueObject.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueObject;
        }

        //mod相关的改造
        public static bool RemoveCard(ref IEnumerator __result, string s, DungeonPhysical __instance)
        {
            ShopDialogueCard[] array = new ShopDialogueCard[__instance.cachedCardObjects.Length - 1];
            int num = 0;
            bool searchDone = false;
            for (int i = 0; i < __instance.cachedCardObjects.Length; i++)
            {
                HelloMod.mLogger.LogMessage("Try Remove Card==>" + s + "||Current==>" + __instance.cachedCardObjects[i].card.internalName);
                if (!searchDone && __instance.cachedCardObjects[i].card.internalName == s)
                {
                    searchDone = true;
                }
                else
                {
                    array[num] = __instance.cachedCardObjects[i];
                    num++;
                }
            }
            if (searchDone)
            {
                __instance.cachedCardObjects = array;//
                __instance.StartCoroutine_Auto(__instance.BuildDeckBuilder());
            }
            __result = UnityRuntimeServices.EmptyEnumerator;
            return false;
        }
    }
}
