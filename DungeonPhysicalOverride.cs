using HelloMod.ModFeature;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using UnityEngine;
using UnityScript.Lang;

namespace HelloMod
{
    public class ModMenuButtonData
    {
        private Func<string, string, string> nameFunc = null;
        private string enName = string.Empty;
        private Action<DungeonPlayer> onClick = null;
        public ModMenuButtonData(Func<string, string, string> nameFunc, string enName, Action<DungeonPlayer> onClick)
        {
            this.nameFunc = nameFunc;
            this.enName = enName;
            this.onClick = onClick;
        }

        public ShopDialogueButton GetButtonInstance(Vector2 btnSize, DungeonPlayer currentPlayer)
        {
            string btnName = nameFunc.Invoke(DreamQuestConfig.CurrentLang, enName);
            ShopDialogueButton btn = SDB.BasicButton(btnSize, btnName, () => { onClick(currentPlayer); });
            return btn;
        }
    }


    public class DungeonPhysicalOverride
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

        private static Vector2 defSize = new Vector2(4f, 0.4f);

        public static void AddBasicButton(Func<string,string, string> nameFunc, string enName, Action<DungeonPlayer> onClick)
        {
            btnDataList.Add(new ModMenuButtonData(nameFunc, enName, onClick));
        }

        private static List<ShopDialogueObject> GenerateButtonListFromData(DungeonPlayer currentPlayer)
        {
            List<ShopDialogueObject> result = new List<ShopDialogueObject>();
            foreach (var item in btnDataList)
            {
                result.Add(item.GetButtonInstance(defSize, currentPlayer));
            }
            return result;  
        }

        private static List<ModMenuButtonData> btnDataList = new List<ModMenuButtonData>();

        private static bool isInit = false;
        public static void ModMenu(DungeonPhysical __instance)
        {
            float win_width = 4.5f;
            float win_height = 5f;
            float btn_height = defSize.y;
            float space_y = 0.3f;
            int maxWinItemCount = 10;
            if (!isInit)
            {
                isInit = true;
                AddBasicButton((lang, name) => {
                    return TR.GetStr(TableKey, name);
                }, "Random Alchemy", RandomAlchemy);
                AddBasicButton((lang, name) => {
                    return TR.GetStr(TableKey, name);
                }, "Find Monster", (player) =>
                {
                    player.AddAction(new DungeonActionFindMonster());
                });
                //测试 25/09/27
                AddBasicButton((lang, name) => {
                    return TR.GetStr(TableKey, name);
                }, "CopySacrifice", (player) =>
                {
                    player.AddAction(new DungeonActionCopySacrifice());
                });
                AddBasicButton((lang, name) => {
                    return TR.GetStr(TableKey, name);
                }, "ActionDream", (player) =>
                {
                    player.AddAction(new DungeonActionDream());
                });
                AddBasicButton((lang, name) => {
                    return TR.GetStr(TableKey, name);
                }, "ActionSacrifice", (player) =>
                {
                    player.AddAction(new DungeonActionSacrifice());
                });
                AddBasicButton((lang, name) => {
                    return TR.GetStr(TableKey, name);
                }, "AllCardViewer", (player) =>
                {
                    __instance.dungeon.AllCardsDeckViewer("全卡图鉴", new List<ShopDialogueButton> { SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), "确认", null, null) }, () => { __instance.dungeon.WindowBack(); });
                });
                //测试 模组 按钮数量
                /*for (int i = 0; i < maxWinItemCount * 2 + 3; i++) {
                    btnList.Add(SDB.BasicButton(new Vector2((float)4, 0.4f), TR.GetStr(TableKey, "Random Alchemy"), () => { RandomAlchemy(__instance.dungeon.player); }));
                }*/
                //限制单页数量为10个最好
            }
            List<ShopDialogueObject> btnList = GenerateButtonListFromData(__instance.dungeon.player);


            float total_height = 0;
            int pageItemCount = Mathf.Min(maxWinItemCount, btnList.Count);
            total_height = btn_height * pageItemCount + space_y * (pageItemCount + 1);
            win_height = total_height;

            if (btnList.Count > maxWinItemCount)
            {
                ShopDialogueObject[] pagedItemArr = new ShopDialogueObject[btnList.Count/maxWinItemCount + 1];
                int currentPageIndex = 0;
                List<ShopDialogueObject> pageBtnList = new List<ShopDialogueObject>();
                for (int i = 0; i < btnList.Count; i++)
                {
                    if((i + 1) % maxWinItemCount == 0)
                    {
                        pageBtnList.Add(btnList[i]);
                        //完成一页
                        ShopDialogueObject align = SDB.Align(pageBtnList.ToArray(), "VP", space_y);
                        align = SDB.Padded(align, new Vector2(win_width, win_height), ShopDialogueCardinal.center);

                        pagedItemArr[currentPageIndex] = align;
                        currentPageIndex++;
                        pageBtnList.Clear();
                        continue;
                    }
                    pageBtnList.Add(btnList[i]);
                }
                if (btnList.Count % maxWinItemCount != 0) {
                    //完成最后一页
                    ShopDialogueObject align = SDB.Align(pageBtnList.ToArray(), "VP", space_y);
                    align = SDB.Padded(align, new Vector2(win_width, win_height), ShopDialogueCardinal.uppercenter);
                    pagedItemArr[currentPageIndex] = align;
                }
                //在单页承载量过大时使用分页加载  超过10为过大
                ShopDialogueObject shopDialogueObject = null;//pageContainer
                ShopDialoguePaged shopDialoguePaged = SDB.Paged(pagedItemArr);
                shopDialogueObject = SDB.Padded(shopDialoguePaged, new Vector2(win_width, win_height), ShopDialogueCardinal.center);
                SDB.Background(shopDialogueObject, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
                SDB.CancelButton(shopDialogueObject, __instance.dungeon.WindowBack);
                shopDialogueObject.UpperCenterTo(__instance.dungeon.ShopLocation());
                __instance.dungeon.activeShop = shopDialogueObject;
            }
            else
            {
                //单页的情况下
                ShopDialogueObject shopDialogueObject = SDB.Align(btnList.ToArray(), "VP", space_y);
                shopDialogueObject = SDB.Padded(shopDialogueObject, new Vector2(win_width, win_height), ShopDialogueCardinal.center);
                SDB.Background(shopDialogueObject, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
                SDB.CancelButton(shopDialogueObject, __instance.dungeon.WindowBack);
                shopDialogueObject.UpperCenterTo(__instance.dungeon.ShopLocation());
                __instance.dungeon.activeShop = shopDialogueObject;
            }
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
