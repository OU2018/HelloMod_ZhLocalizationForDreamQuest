using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BepInEx;

namespace HelloMod
{
    internal class MainMenuOverride
    {
        public const string TableKey = "MainMenu";
        private static Game currentGame = null;
        public static bool InitializeOverride(MainMenu __instance)
        {
            HelloMod.mLogger.LogInfo("MainMenuOverride Success Initialized");
            GameManager.activeDungeon = null;
            Resources.UnloadUnusedAssets();
            MainMenu.instance = __instance;
            __instance.gp = GamePhysical.instance;
            currentGame = GamePhysical.instance.game;
            Vector2 vector = new Vector2((float)3, 0.6f);
            if (GameManager.IsIPhone())
            {
                vector *= 1.1f;
            }
            float btnScale = 1f;
            int fontSize = 32;
            //Play!
            
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(vector * 1.35f, TR.GetStr(TableKey, "Play!"),
                ()=> { Play(); });
            shopDialogueButton.FontSize(48);
            //View Achievements || View Bestiary
            ShopDialogueButton shopDialogueButton2 = SDB.BasicButton(vector, TR.GetStr(TableKey, "View Achievements"), ()=> {__instance.ViewAchievements(); });
            shopDialogueButton2.FontSize(fontSize);
            ShopDialogueButton shopDialogueButton3 = SDB.BasicButton(vector, TR.GetStr(TableKey, "View Bestiary"), ()=> {__instance.ViewBestiary(); });
            shopDialogueButton3.FontSize(fontSize);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueButton[] { shopDialogueButton2, shopDialogueButton3 }, "HP", 0.2f);
            //High Scores || History
            ShopDialogueButton shopDialogueButton4 = SDB.BasicButton(vector, TR.GetStr(TableKey, "High Scores"), ()=> {
                __instance.HighScoresButton(); 
            });
            shopDialogueButton4.FontSize(fontSize);
            ShopDialogueButton shopDialogueButton5 = SDB.BasicButton(vector, TR.GetStr(TableKey, "History"), ()=> {
                HistoryButton();
                /*ActionBaseOnLang(new Dictionary<string, Action>()
                {
                {DreamQuestConfig.EN,  __instance.HistoryButton},
                {DreamQuestConfig.ZH,  HistoryButton},
                });*/
            });
            shopDialogueButton5.FontSize(fontSize);
            ShopDialogueAligned shopDialogueAligned2 = SDB.Align(new ShopDialogueButton[] { shopDialogueButton4, shopDialogueButton5 }, "HP", 0.2f);
            //[Mod] View Cards || Change Lang
            ShopDialogueButton shopDialogueButton10 = SDB.BasicButton(vector, TR.GetStr(TableKey, "View Cards"), () => { __instance.ViewCards(); });
            shopDialogueButton10.FontSize(fontSize);
            //TODO:语言从外部的配置文件进行配置 || 该方法改为解锁怪物图鉴或其他
            ShopDialogueButton shopDialogueButton11 = SDB.BasicButton(vector, TR.GetStr(TableKey, "Placeholder"), () => { ViewTalent(); });
            shopDialogueButton11.FontSize(fontSize);

            ShopDialogueAligned shopDialogueAligned4 = SDB.Align(new ShopDialogueButton[] { shopDialogueButton10, shopDialogueButton11 }, "HP", 0.2f);
            //布局
            ShopDialogueAligned shopDialogueAligned3 = SDB.Align(new ShopDialogueAligned[] { shopDialogueAligned, shopDialogueAligned2 , shopDialogueAligned4 }, "VP", 0.25f);
            shopDialogueAligned3 = SDB.Align(new ShopDialogueObject[] { shopDialogueButton, shopDialogueAligned3 }, "VP", 0.75f);
            //Exit Game
            ShopDialogueButton shopDialogueButton6 = SDB.BasicButton(vector, TR.GetStr(TableKey, "Exit Game"), ()=> {
                __instance.ExitGame(); 
            });
            shopDialogueButton6.FontSize(fontSize);
            shopDialogueAligned3 = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned3, shopDialogueButton6 }, "VP", (float)0.25f);
            //整体定位
            shopDialogueAligned3.UpperCenterTo(new Vector3((float)0, 0.6f, (float)0));
            //左下角 用户区域
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(() => {return __instance.CurrentUserName(); }, fontSize, Color.white);
            ShopDialogueButton shopDialogueButton7 = SDB.BasicButton(new Vector2(1.3f, 0.4f) * btnScale, TR.GetStr(TableKey, "Change","USER"), ()=> {__instance.ChooseProfileDialogue(); });
            shopDialogueButton7.FontSize(fontSize - 4);
            shopDialogueAligned3 = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueButton7 }, "VP", 0.1f);
            Vector3 vector2 = new Vector3(5.4f, -4.4f, (float)0);
            shopDialogueAligned3.LowerCenterTo(vector2);

            if (GameManager.LoadCurrentUser() ==  null)
            {
                if (!__instance.ExistUsers())
                {
                    __instance.CreateNewProfileDialogueBoolean(false);
                }
                else
                {
                    __instance.ChooseProfileDialogue();
                }
            }
            //右下角 制作人员 和 更改游戏难度 区域
            ShopDialogueButton shopDialogueButton8 = SDB.BasicButton(new Vector2(1.3f, 0.4f) * btnScale, TR.GetStr(TableKey, "Credits"), ()=> {
                ActionBaseOnLang(new Dictionary<string, Action>()
                {
                {DreamQuestConfig.EN,  __instance.CreditsButton},
                {DreamQuestConfig.ZH,  CreditsButton},
                });
            });
            shopDialogueButton8.FontSize(fontSize - 4);
            shopDialogueButton8.CenterTo(new Vector3(-4.08f, -4.63f, (float)0));
            shopDialogueDynamicText = SDB.DynamicText(() => { return CurrentDifficultyName(); }, fontSize, Color.white);
            shopDialogueDynamicText.SDDTCenterText();
            ShopDialogueButton shopDialogueButton9 = SDB.BasicButton(new Vector2(1.3f, 0.4f) * btnScale, TR.GetStr(TableKey, "Change","DIFF"), ()=> {ChooseDifficulty(); });
            shopDialogueButton9.FontSize(fontSize - 4);
            shopDialogueAligned3 = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueButton9 }, "VP", 0.2f);
            shopDialogueAligned3.CenterTo(new Vector3(-4.08f, -3.63f, (float)0));
            //先预存 构建角色页面
            if (HelloMod.isLoadedFont)
            {
                __instance.BuildCachedClassPickerDialogue();
            }
            else
            {
                HelloMod.OnAfterFontLoaded += () =>
                {
                    __instance.BuildCachedClassPickerDialogue();
                };
            }
            //播放音乐
            MusicManager.PlayMusic(Music.MAINMENU);

            __instance.soundButton.SetToggleCallback((x) => {
                MusicManager.SetSoundOn(x);
            });
            __instance.soundButton.SilentToggleTo(MusicManager.IsSoundOn());
            return false;
        }

        public static void EasyDifficultyPostfix(ref Difficulty __result)
        {
            string name = "Kitten";
            string description = "You receive significant bonuses but are unable to earn achievements or achievement points.";//en00003
            if (DreamQuestConfig.IsZh)
            {
                name = TR.GetStr(TableKey, "Kitten");
                description = HelloMod.Csv.GetTranslationByID("UIextra", DreamQuestConfig.CurrentLang, "00003");
            }
            __result = new Difficulty(name, description, Difficulty.EASY);
        }

        public static void NormalDifficultyPostfix(ref Difficulty __result)
        {
            string name = "Grizzly Bear";
            string description = "The default difficulty for the game.";//en00004
            if (DreamQuestConfig.IsZh)
            {
                name = TR.GetStr(TableKey, "Grizzly Bear");
                description = HelloMod.Csv.GetTranslationByID("UIextra", DreamQuestConfig.CurrentLang, "00004");
            }
            __result = new Difficulty(name, description, Difficulty.NORMAL);
        }

        public static void HardDifficultyPostfix(ref Difficulty __result)
        {
            string name = "Velociraptor";
            string description = "The game is much more difficult, but you receive double the number of achievement points and find additional content at the end of the game.";//en00005
            if (DreamQuestConfig.IsZh)
            {
                name = TR.GetStr(TableKey, "Velociraptor");
                description = HelloMod.Csv.GetTranslationByID("UIextra", DreamQuestConfig.CurrentLang, "00005");
            }
            __result = new Difficulty(name, description, Difficulty.HARD);
        }

        public static string CurrentDifficultyName()
        {
            return TR.GetStr(TableKey, "Difficulty:") + "\n" + MainMenu.instance.GetDifficulty().name;
        }

        public static void ChooseDifficulty()
        {
            ChooseDifficulty(canCancel: true);
        }

        public static void ChooseDifficulty(bool canCancel)
        {
            ShopDialogueObject shopDialogueObject = MainMenu.instance.BuildDifficultyObjects();
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(TR.GetStr(TableKey, "Choose a difficulty"), 36, Color.black);
            shopDialogueObject = SDB.Align(new ShopDialogueObject[2] { shopDialogueDynamicText, shopDialogueObject }, "VP", 0.5f);
            SDB.Background(shopDialogueObject, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            if (canCancel)
            {
                SDB.CancelButton(shopDialogueObject, currentGame.DestroyActiveShopNow);
            }

            shopDialogueObject.UpperCenterTo(new Vector3(0f, 3f, 6f));
            currentGame.activeShop = shopDialogueObject;
        }
        private static void ViewTalent()
        {
            /*ShopDialogueTalentViewer shopDialogueTalentViewer = SDB.TalentViewer(null, 0, null);
            SDB.CancelButton(shopDialogueTalentViewer, currentGame.DestroyActiveShopNow);
            SDB.Background(shopDialogueTalentViewer, Vector2.zero, Vector2.one * 0.05f, Color.black);
            currentGame.activeShop = shopDialogueTalentViewer;
            ..*/
                //TODO:如何实现全天赋的浏览器
        }

        public static bool BuildCachedClassPickerDialogue()
        {
            Vector2 professionIconSize = new Vector2(11f, 9f);

            List<ShopDialogueObject> list = new List<ShopDialogueObject>();
            float descriptionLeftPadding = professionIconSize.x * 0.7f;

            int num = 0;

            int i = 0;
            string[] professionList = ProfessionList.GetProfessionList(allowRandom: true);
            for (int length = professionList.Length; i < length; i++)
            {
                ProfessionBase professionBase = ProfessionBase.Build(professionList[i], currentGame);
                ShopDialogueTexture shopDialogueTexture = SDB.ShopTexture(5.25f, 3.75f, professionBase.GetBigTexture());
                ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(professionBase.ClassName(), 42 + num / 8 * 18, Color.black);
                //TODO:这里的 x1.2f 只是为了使文本和下面的按钮不重合
                ShopDialogueText shopDialogueText = SDB.Text(descriptionLeftPadding * 1.2f, professionBase.Description(), 32 + num, Color.black);
                ShopDialogueText shopDialogueText2 = SDB.LeftText(descriptionLeftPadding * 1.2f, professionBase.AbilityDescription(), 32 + num, Color.black);

                ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(4f, 0.4f), TR.GetStr(TableKey, "Choose ") + professionBase.ClassName(), MainMenu.instance.CreateClassFunction(professionBase));
                shopDialogueButton.FontSize(DreamQuestConfig.IsZh ? 36 : shopDialogueButton.text.fontSize);
                if (GameManager.IsIPhone())
                {
                    shopDialogueButton.FontSize(36);
                }

                ShopDialogueObject shopDialogueObject = SDB.Align(new ShopDialogueObject[2] { shopDialogueDynamicText, shopDialogueTexture }, "VP", 0.2f);
                if (!professionBase.EarnsAchievements())
                {
                    shopDialogueObject = SDB.Align(new ShopDialogueObject[2]
                    {
                    shopDialogueObject,
                    SDB.DynamicText(TR.GetStr(TableKey, "The ") + professionBase.ClassName() + TR.GetStr(TableKey, " only earns class specific achievements"), (int)(36f + (float)num * 1.5f), Color.black)
                    }, "VP", 0.1f);
                }

                shopDialogueObject = SDB.Align(new ShopDialogueObject[2] { shopDialogueObject, shopDialogueText }, "VP", 0.1f);
                shopDialogueObject = SDB.Align(new ShopDialogueObject[2] { shopDialogueObject, shopDialogueText2 }, "VP", 0.2f);
                shopDialogueObject = ((!GameManager.IsIPhone()) ? SDB.Align(new ShopDialogueObject[2] { shopDialogueObject, shopDialogueButton }, "VT", 8f) : SDB.Align(new ShopDialogueObject[2] { shopDialogueObject, shopDialogueButton }, "VT", 8.5f));
                list.Add(shopDialogueObject);
            }

            ShopDialoguePaged shopDialoguePaged = SDB.CircularPaged(list.ToArray());
            ShopDialogueDynamicText shopDialogueDynamicText2 = SDB.DynamicText(TR.GetStr(TableKey, "Choose your class"), (int)(36f + (float)num * 1.5f), Color.black);
            ShopDialogueObject shopDialogueObject2 = SDB.Align(new ShopDialogueObject[2] { shopDialogueDynamicText2, shopDialoguePaged }, "VP", 0.3f);
            ShopDialogueTexture shopDialogueTexture2 = SDB.ShopTexture(professionIconSize.x, professionIconSize.y, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
            shopDialogueObject2 = SDB.Center(new ShopDialogueObject[2] { shopDialogueTexture2, shopDialogueObject2 });
            shopDialogueObject2.debugName = "Total";
            SDB.CancelButton(shopDialogueObject2, MainMenu.instance.KillClassPicker);
            shopDialogueObject2.CenterTo(new Vector3(0f, 0f, 4f));
            shopDialogueObject2.Disable();
            MainMenu.instance.cachedChoosePlayerDialogue = shopDialogueObject2;
            return false;
        }

        public static void CreditsButton()
        {
            float num = 4f;
            string text = "<Artwork (the good stuff)> \n Andrew Whalen \n Kristine Whalen \n \n <Test/Design/Awesome People> \n Albert Bush \n Steven Ehrlich \n Robert Krone \n Kristine Whalen";
            string text2 = "<Music> \n Vertex Studios \n Evil Mind Entertainment \n Lucky Lion Studios \n Water Prelude Kevin MacLeod (incompetech.com) \n Final Count Kevin MacLeod (incompetech.com)";
            string text3 = "<Editing> \n Robert Burke \n <Beta Testers> \n Alvin Chen \n Steve Nichols \n Chris Pryby \n Ruidong Wang \n Daniel Whalen";
            string text4 = "Version " + (object)1.1f + "" + " \n \n Copyright Peter Whalen 2015 \n All rights reserved";
            if (DreamQuestConfig.IsZh)
            {
                text = "<美术 (好伙计)> \n Andrew Whalen \n Kristine Whalen \n \n <测试/设计/妙人儿> \n Albert Bush \n Steven Ehrlich \n Robert Krone \n Kristine Whalen";
                text2 = "<音乐> \n Vertex Studios \n Evil Mind Entertainment \n Lucky Lion Studios \n Water Prelude Kevin MacLeod (incompetech.com) \n Final Count Kevin MacLeod (incompetech.com)";
                text3 = "<校队/润色/编辑> \n Robert Burke \n <Beta 测试者们> \n Alvin Chen \n Steve Nichols \n Chris Pryby \n Ruidong Wang \n Daniel Whalen";
                text4 = "版本 " + (object)1.1f + "" + " \n \n Copyright Peter Whalen 2015 \n All rights reserved \n <感谢以下朋友的帮助：>\nDreamQuest贴吧吧友\n二叶叶\nIcetric冰介 及 群友\n宵夜97、贴吧吧友无限\nBepInEx和Dnspy等工具的开发者们\n 攻略推荐：Skyphantom的视频";
            }

            string[] array = new string[4] { text, text2, text3, text4 };
            ShopDialogueObject[] array2 = new ShopDialogueObject[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(TR.GetStr(TableKey, "Dream Quest"), 48, Color.blue);
                ShopDialogueDynamicText shopDialogueDynamicText2 = SDB.DynamicText(TR.GetStr(TableKey, "By Peter Whalen"), 36, Color.black);
                ShopDialogueText shopDialogueText = SDB.CenteredText(num, array[i], 32, Color.black);
                ShopDialogueObject shopDialogueObject = SDB.Align(new ShopDialogueDynamicText[2] { shopDialogueDynamicText, shopDialogueDynamicText2 }, "VP", 0.05f);
                shopDialogueObject = SDB.Align(new ShopDialogueObject[2] { shopDialogueObject, shopDialogueText }, "VP", 0.3f);
                shopDialogueObject = SDB.Padded(shopDialogueObject, new Vector2(num, 5f), ShopDialogueCardinal.uppercenter);
                array2[i] = shopDialogueObject;
            }

            ShopDialoguePaged shopDialoguePaged = SDB.Paged(array2);
            float y = shopDialoguePaged.myheight * 1.1f;
            ShopDialoguePadded shopDialoguePadded = SDB.Padded(shopDialoguePaged, new Vector2(num * 1.1f + 0.5f, y), ShopDialogueCardinal.center);
            SDB.Background(shopDialoguePadded, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            SDB.CancelButton(shopDialoguePadded, currentGame.DestroyActiveShopNow);
            shopDialoguePadded.UpperCenterTo(new Vector3(0f, 3f, 2f));
            currentGame.activeShop = shopDialoguePadded;
        }

        public static void HistoryButton()
        {
            ShopDialogueHistoryViewer sh = null;
            sh = SDB.HistoryViewer();
            SDB.CancelButton(sh, currentGame.DestroyActiveShopNow);
            SDB.Background(sh, new Vector2(0f, 0f), new Vector2(0.05f, 0.05f), Color.black);
            if (GameManager.IsIPhone())
            {
                sh.UpperCenterTo(new Vector3(0f, 4f, 2f));
            }
            else
            {
                sh.UpperCenterTo(new Vector3(0f, 3.8f, 2f));
            }
            //前面是 按钮按下时的反应，后者是 检测按钮是否启用的Function
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(2f, 0.5f), TR.GetStr(TableKey, "Replay"),()=> {
                ReplayButton(sh);
                }, ()=> {
                 return MainMenu.instance.CanReplay(sh);
                });
            ShopDialogueButton shopDialogueButton2 = SDB.BasicButton(new Vector2(2f, 0.5f), TR.GetStr(TableKey, "Enter Code"), EnterCodeButton);
            ShopDialogueButton sdb_copy_code = SDB.BasicButton(new Vector2(2f, 0.5f), TR.GetStr(TableKey, "Copy Code"), () => {
                CopyCodeButton(sh);
            }, () => {
                return MainMenu.instance.CanReplay(sh);
            });
            shopDialogueButton.FontSize(32);
            shopDialogueButton2.FontSize(32);
            sdb_copy_code.FontSize(32);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueButton[3] { shopDialogueButton, shopDialogueButton2,sdb_copy_code }, "HP", 2f);
            //TODO:考虑这个翻译的具体意思
            ShopDialogueText shopDialogueText = SDB.Text(9f, TR.GetStr(TableKey, "Replayed games do not earn achievements."), 24, Color.black);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[2] { shopDialogueAligned, shopDialogueText }, "VP", 0.3f);
            shopDialogueAligned.LowerCenterTo(sh.LowerCenter() + new Vector3(0f, 0.3f, 0.3f));
            ShopDialogueAligned activeShop = SDB.Group(new ShopDialogueObject[2] { sh, shopDialogueAligned });
            currentGame.activeShop = activeShop;
        }

        public static void EnterCodeButton()
        {
            if (Dungeon.IsSavedDungeon())
            {
                currentGame.DestroyActiveShopNow();
                ShopDialogueObject shopDialogueObject = SDB.ModalDialogue(new Vector2(8f, 2f), 
                    HelloMod.Csv.GetTranslationByID("UIextra", DreamQuestConfig.CurrentLang, "00002")
                    , new string[2] { TR.GetStr(TableKey, "Delete", "DUNGEON"), TR.GetStr(TableKey, "Cancel", "DUNGEON") }, MainMenu.instance.ModalHandlerEnterCode);
                shopDialogueObject.UpperCenterTo(new Vector3(0f, 2f, 6f));
                currentGame.activeShop = shopDialogueObject;
            }
            else
            {
                EnterReplayString();
            }
        }
        //翻译预览
        //en00001,A saved dungeon already exists.  Would you like to resume that game?  If you do not, it will be lost forever
        //en00002,A saved dungeon already exists.  If you continue, it will be lost forever.
        public static void ReplayButton(ShopDialogueHistoryViewer h)
        {
            if (Dungeon.IsSavedDungeon())
            {
                currentGame.DestroyActiveShopNow();
                ShopDialogueObject shopDialogueObject = SDB.ModalDialogue(new Vector2(8f, 2f), HelloMod.Csv.GetTranslationByID("UIextra", DreamQuestConfig.CurrentLang, "00002"), new string[2] { TR.GetStr(TableKey, "Delete", "DUNGEON"), TR.GetStr(TableKey, "Cancel", "DUNGEON") }, (choice) =>
                {
                    MainMenu.instance.ModalHandlerReplay(choice, h.GetActiveHistory().code);
                }
                    );
                shopDialogueObject.UpperCenterTo(new Vector3(0f, 2f, 6f));
                currentGame.activeShop = shopDialogueObject;
            }
            else
            {
                MainMenu.instance.Replay(h.GetActiveHistory().code);
            }
        }

        public static void CopyCodeButton(ShopDialogueHistoryViewer h)
        {
            //弹出询问框，询问玩家确认进行复制吗？将会覆盖当前剪贴板的内容
            if (!ClipboardUtility.GetFromClipboard().IsNullOrWhiteSpace())
            {
                currentGame.DestroyActiveShopNow();
                ShopDialogueObject shopDialogueObject = SDB.ModalDialogue(new Vector2(8f, 2f), HelloMod.Csv.GetTranslationByID("UIextra", DreamQuestConfig.CurrentLang, "_copy_code") + "\n" + h.GetActiveHistory().code, new string[2] { TR.GetStr(TableKey, "Copy to clipboard"), TR.GetStr(TableKey, "Cancel", "DUNGEON") }, (choice) =>
                {
                    ModalHandlerCopyCode(choice, h.GetActiveHistory().code);
                }
                    );
                shopDialogueObject.UpperCenterTo(new Vector3(0f, 2f, 6f));
                currentGame.activeShop = shopDialogueObject;
            }
            else
            {
                CopyCode(h.GetActiveHistory().code);
            }
        }

        public static void EnterReplayString()
        {
            currentGame.DestroyActiveShopNow();
            MainMenu.instance.replayErrorMessage = string.Empty;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(TR.GetStr(TableKey, "Enter Code"), 36, Color.black);
            ShopDialogueDynamicText shopDialogueDynamicText2 = SDB.DynamicText(MainMenu.instance.GetReplayErrorMessage, 24, Color.red);
            ShopDialogueTextInput shopDialogueTextInput = SDB.TextInput(25, 36);
            if (GameManager.IsIPhone())
            {
                shopDialogueTextInput.SetSize(48);
            }

            shopDialogueTextInput.SetSubmit(MainMenu.instance.SubmitReplayCode);
            if (GameManager.IsIPhone())
            {
                SDB.Background(shopDialogueTextInput, new Vector2(0f, 0.06f), new Vector2(0.01f, 0.01f), Color.black);
            }
            else
            {
                SDB.Background(shopDialogueTextInput, new Vector2(0f, 0.03f), new Vector2(0.01f, 0.01f), Color.black);
            }
            //TODO:到底执行什么？
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(1.3f, 0.4f), TR.GetStr(TableKey, "Submit"), shopDialogueTextInput.Submit, () =>
            {
                return shopDialogueTextInput.text.text.Length > 0;
            });
            //将剪贴板的内容粘贴到输入框中
            ShopDialogueButton pasteBtn = SDB.BasicButton(new Vector2(1.3f, 0.4f), TR.GetStr(TableKey, "Paste"), () =>
            {
                PasteCode(shopDialogueTextInput);
            }, () =>
            {
                return !ClipboardUtility.GetFromClipboard().IsNullOrWhiteSpace();
            });
            ShopDialogueAligned sdAlign = SDB.Align(new ShopDialogueObject[2] { shopDialogueButton, pasteBtn }, "HP", 0.1f);
            //更新字体 中文时字号需要变大

            shopDialogueButton.FontSize(DreamQuestConfig.IsZh ?  36 : shopDialogueButton.text.fontSize);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueDynamicText[2] { shopDialogueDynamicText, shopDialogueDynamicText2 }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[2] { shopDialogueAligned, shopDialogueTextInput }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[2] { shopDialogueAligned, sdAlign }, "VP", 0.1f);
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)), Color.black);
            SDB.CancelButton(shopDialogueAligned, currentGame.DestroyActiveShopNow);
            if (GameManager.IsIPhone())
            {
                shopDialogueAligned.transform.localScale = shopDialogueAligned.transform.localScale * 2f;
            }

            shopDialogueAligned.UpperCenterTo(new Vector3(0f, 3f, 2f));
            currentGame.activeShop = shopDialogueAligned;
        }

        public static void ActionBaseOnLang(Dictionary<string, Action> langActions)
        {
            if (langActions.ContainsKey(DreamQuestConfig.CurrentLang))
            {
                langActions[DreamQuestConfig.CurrentLang].Invoke();
            }
            else
            {
                //如果查找不到，就默认执行第一个
                langActions.First().Value.Invoke();
            }
        }

        public static void SubmitNewProfilePostfix(string name, ref MainMenu __instance)
        {
            if (__instance.errorMessage.Length > 0)
            {
                if (DreamQuestConfig.IsZh)
                {
                    if(__instance.errorMessage.Equals("A user by that name already exists"))
                    {
                        __instance.errorMessage = "存在同名用户！";
                    }else if (__instance.errorMessage.Equals("Player names may only contain letters, numbers, underscore, and spaces"))
                    {
                        __instance.errorMessage = "玩家名称仅可包含英文字母，数字，下划线及空格！";
                    }
                }
                //如果有其他语言，可以在别处进行修改
            }
        }

        public static void Play()
        {
            if (Dungeon.IsSavedDungeon())
            {
                Vector2 vector = new Vector2(5f, 2.3f);
                if (GameManager.IsIPhone())
                {
                    vector *= 1.5f;
                }
                ShopDialogueObject shopDialogueObject = SDB.ModalDialogue(vector, HelloMod.Csv.GetTranslationByID("UIextra", DreamQuestConfig.CurrentLang, "00001"), new string[] { TR.GetStr(TableKey, "Resume", "DUNGEON"), TR.GetStr(TableKey, "Delete", "DUNGEON") }, (x)=> { MainMenu.instance.SavedDungeonCallback(x); });
                SDB.CancelButton(shopDialogueObject, ()=> {
                    currentGame.DestroyActiveShopNow();
                });
                shopDialogueObject.CenterTo(new Vector3(0, 0, 6f));
                currentGame.activeShop = shopDialogueObject;
            }
            else
            {
                MainMenu.instance.ClassPickerDialogue();
            }
        }
        //TODO:好像是清除云端保存的记录的，暂时不动……
        public static bool ClearCheckPrefix(MainMenu __instance)
        {
            List<string> list = __instance.CloudNames();
            currentGame.DestroyActiveShopNow();
            if (list.Count > 0)
            {
                int count = list.Count;
                string text = count + " profiles";
                if (count == 1)
                {
                    text = count + " profile";
                }
                ShopDialogueObject shopDialogueObject = SDB.ModalDialogue(new Vector2((float)6, (float)2), "Are you sure you want to delete " + text + "?", new string[] { "Yes", "Cancel" }, __instance.ModalHandlerClear);
                shopDialogueObject.UpperCenterTo(new Vector3((float)0, (float)2, (float)6));
                currentGame.activeShop = shopDialogueObject;
            }
            return false;
        }
        //ShopDialogueDynamicText 这个类会在 Update中不断调用 Setter事件来获取字符串并进行更新
        //应该用委托响应之类的方法[Placeholder]
        public static void GetReplayErrorMessagePostfix(ref string __result)
        {
            if ( !string.IsNullOrEmpty(__result))
            {
                __result = TR.GetStr("ReplayErrorMessage", __result);
            }
        }

        public static void ModalHandlerCopyCode(int x,string s)
        {
            currentGame.DestroyActiveShopNow();
            if(x == 0)
            {
                CopyCode(s);
                return;
            }

            if(x != 1)
            {

            }
        }

        public static void CopyCode(string s) { 
            ClipboardUtility.CopyToClipboard(s);
        }

        public static void PasteCode(ShopDialogueTextInput input)
        {
            input.text.text += ClipboardUtility.GetFromClipboard();
        }
    }
}
