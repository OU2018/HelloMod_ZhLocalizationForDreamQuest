using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.Mono;
using UnityEngine;
using HarmonyLib;
using BepInEx.Logging;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;
using System.Linq;
using HelloMod.DungeonFeatureGroup;
using HelloMod.CombatAbilityGroup;
using System.Text.RegularExpressions;
using HelloMod.MonsterDetailOverride;
using UnityScript.Lang;
using System.IO;

namespace HelloMod
{
    [BepInPlugin("oz.gamePlugin.hellomod", "HelloMod", "0.0.1")]
    public class HelloMod:BaseUnityPlugin
    {
        public static ManualLogSource mLogger = null;
        public static CsvParagraphParser Csv = null;
        public static TextureDict textMgr = null;

        static Harmony harmony = null;
        public static Font loadFont = null;
        public static bool isLoadedFont = false;

        private readonly string jsonPath = $@"{Paths.PluginPath}\TransitionJSON";
        private readonly string csvDir = $@"{Paths.PluginPath}\TransitionParagraph";

        public static Action OnAfterFontLoaded;
        public static Action OnAfterTexLoaded;
        public static Action OnAfterProfessionBigLoaded;

        private static Queue<Action> modInitQueue = new Queue<Action>();
        public static void RegisterModInitMethod(Action init)
        {
            modInitQueue.Enqueue(init);
        }
#pragma warning disable IDE0051
        void Awake()
        {
            mLogger = new ManualLogSource("hellomod");
            BepInEx.Logging.Logger.Sources.Add(mLogger);
            harmony = new Harmony("oz.gamePlugin.hellomod");
            Csv = new CsvParagraphParser(csvDir);
            ReplaceStringPatch.SetHarmony(harmony);

            //载入设置
            LoadConfig();
            //如果阻断了Mod运行
            if(DreamQuestConfig.DontloadMod)
                return;
            TranslationManager.Initialize();

            LoadAssetBundle(15);
            if (DreamQuestConfig.IsUseOtherResource)
            {
                LoadLocalArtResource();
                StartCoroutine(LoadAudio());//TODO:可以弄成通用方法
                //添加按钮点击的音效
                PatchTargetPostfix(
                typeof(NiceButton).GetMethod("OnMouseUp", new Type[0]),
                typeof(HelloMod).GetMethod("NiceButton_OnMouseUp_Postfix"));
            }
            //修改背景音乐的方法，使得音量可以被控制
            PatchTargetPostfix(
                typeof(MusicManager).GetMethod("Awake"),
                typeof(HelloMod).GetMethod("MusicManager_Awake_Postfix"));
            //[基础区域]
            //让ShopDialogueButton创建出来后就自动重载字体
            PatchTargetPostfix(
                typeof(ShopDialogueButton).GetMethod("Create", new Type[] {typeof(Vector2)}),
                typeof(HelloMod).GetMethod("ButtonCreatePostfix"));
            PatchTargetPostfix(
                typeof(ShopDialogueButton).GetMethod("Create", new Type[] { typeof(Vector2) , typeof(Texture[])}),
                typeof(HelloMod).GetMethod("ButtonCreateWithTexPostfix"));
            //设置成 是手机，很多方面会变简单 ==>会导致意料之外的尺寸问题
            /*PatchTargetPostfix(
                typeof(GameManager).GetMethod("IsIPhone"),
                typeof(HelloMod).GetMethod("IsIPhonePostfix"));*/
            //TextBox Awake 覆盖，支持大部分中文文本
            try
            {
                PatchTargetPrefix(
                typeof(CardPhysical).GetMethod("CreateNewTO", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(CardPhysicalOverride).GetMethod("CreateNewTO"));
                /*PatchTargetPrefix(
                typeof(Textbox).GetMethod("Create", new Type[] { typeof(string), typeof(float) }),
                typeof(HelloMod).GetMethod("Textbox_Create_Override"));*/
                PatchTargetPrefix(
                typeof(Textbox).GetMethod("CreateNewTO", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(TextboxFormatTextAnalize).GetMethod("CreateInstanceToParent"));

                PatchTargetPrefix(
                typeof(Textbox).GetMethod("FormatText", new Type[] { typeof(string), typeof(float) }),
                typeof(TextboxFormatTextAnalize).GetMethod("FormatTextOverride"));

                PatchTargetPostfix(
                typeof(Textbox).GetMethod("FormatText", new Type[] { typeof(string), typeof(float) }),
                typeof(TextboxFormatTextAnalize).GetMethod("FormatTextPostfix"));
                //Textbox 运行时修改（关于中文换行）
                var FormatTextMethod = typeof(Textbox).GetMethod("FormatText", BindingFlags.Public | BindingFlags.Instance);
                ReplaceStringPatch.PatchMethodWithTranspiler_In_Textbox(FormatTextMethod);

                PatchTargetPrefix(
                typeof(CardPhysical).GetMethod("FormatText", new Type[] { typeof(string), typeof(float), typeof(float) }),
                typeof(CardPhysicalOverride).GetMethod("FormatTextOverride"));
                var CardPhysicalFormatTextMethod = typeof(CardPhysical).GetMethod("FormatText", BindingFlags.Public | BindingFlags.Instance);
                ReplaceStringPatch.PatchMethodWithTranspiler_In_CardPhysical(CardPhysicalFormatTextMethod);

                //角色名字 翻译
                PatchTargetPrefix(
                      typeof(ProfessionBase).GetMethod("GenerateName"),
                      typeof(ProfessionBaseOverride).GetMethod("GenerateName"));
                //[核心机制]涉及新添加的卡牌
                PatchTargetPrefix(
                      typeof(Game).GetMethod("StaticCreateMomentaryCard", new Type[] {typeof(string)}),
                      typeof(GameOverride).GetMethod("StaticCreateMomentaryCard"));
                PatchTargetPrefix(
                      typeof(Game).GetMethod("CreateCardAtTimeVirtualSplit"),
                      typeof(GameOverride).GetMethod("CreateCardAtTimeVirtualSplit"));
                PatchTargetPrefix(
                      typeof(Game).GetMethod("PopulateFromString"),
                      typeof(GameOverride).GetMethod("PopulateFromString"));
                PatchTargetPrefix(
                typeof(DungeonPhysical).GetMethod("RemoveCard"),
                typeof(DungeonPhysicalOverride).GetMethod("RemoveCard"));
                PatchTargetPrefix(
                typeof(DungeonPlayer).GetMethod("RemoveCardFromDeck"),
                typeof(DungeonPlayerOverride).GetMethod("RemoveCardFromDeck"));

                PatchTargetPostfix(
                    typeof(DungeonPlayerPhysical).GetMethod("LevelString"),
                    typeof(DungeonPlayerPhysicalOverride).GetMethod("LevelString"));
                PatchTargetPostfix(
                    typeof(DungeonPlayer).GetMethod("GetName"),
                    typeof(DungeonPlayerPhysicalOverride).GetMethod("DungeonPlayer_GetName"));

                //重载的卡牌图像，用正常的路径找不到图片
                PatchTargetPrefix(
                    typeof(MaterialManager).GetMethod("Fallback", BindingFlags.NonPublic | BindingFlags.Static),
                    typeof(HelloMod).GetMethod("MaterialManager_Fallback_Override"));
                //特殊的图片卡牌，覆盖
                PatchTargetPostfix(
                    typeof(MaterialManager).GetMethod("Fallback", BindingFlags.NonPublic|BindingFlags.Static),
                    typeof(HelloMod).GetMethod("MaterialManager_Fallback"));

                PatchTargetPrefix(
                    typeof(Resources).GetMethod("Load", new Type[] {typeof(string), typeof(Type)}),
                    typeof(HelloMod).GetMethod("Resource_Load_Prefix"));
            }
            catch (Exception ex)
            {
                base.Logger.LogError("Error while applying patch: " + ex.ToString());
            }
        }
        void Start()
        {
            //如果阻断了Mod运行
            if (DreamQuestConfig.DontloadMod)
                return;
            mLogger.LogInfo("Mod Start:" + modInitQueue.Count);
            //Mod 的 Init方法调用
            foreach (var item in modInitQueue)
            {
                item.Invoke();
            }
            mLogger.LogInfo("Mod Done Init");

            LoadAudioSetting();
            //Prefix测试
            PatchTargetPrefix(
                typeof(FontManager).GetMethod("SetFontSize", new Type[] { typeof(TextMesh), typeof(int) }),
                typeof(HelloMod).GetMethod("SetFontSizeOverride", new Type[] { typeof(TextMesh), typeof(int) }));

            //成就翻译
            PatchTargetPostfix(
                typeof(ShopDialogueAchievementViewer).GetMethod("AchievementName"),
                typeof(AchievementViewerOverride).GetMethod("AchievementNamePostfix"));
            PatchTargetPostfix(
                typeof(ShopDialogueAchievementViewer).GetMethod("AchievementValue"),
                typeof(AchievementViewerOverride).GetMethod("AchievementValuePostfix"));
            PatchTargetPostfix(
                typeof(ShopDialogueAchievementViewer).GetMethod("AchievementReq"),
                typeof(AchievementViewerOverride).GetMethod("AchievementReqPostfix"));
            PatchTargetPostfix(
                typeof(ShopDialogueAchievementViewer).GetMethod("AchievementReward"),
                typeof(AchievementViewerOverride).GetMethod("AchievementRewardPostfix"));
            PatchTargetPostfix(
                typeof(ShopDialogueAchievementViewer).GetMethod("AchievementProgress"),
                typeof(AchievementViewerOverride).GetMethod("AchievementProgressPostfix"));
            //怪物图鉴翻译
            PatchTargetPostfix(
                typeof(ShopDialogueBestiary).GetMethod("MonsterName"),
                typeof(ShopDialogueBestiaryOverride).GetMethod("MonsterNamePostfix"));
            PatchTargetPostfix(
                typeof(ShopDialogueBestiary).GetMethod("MonsterDescription"),
                typeof(ShopDialogueBestiaryOverride).GetMethod("MonsterDecriptionPostfix"));
            //难度翻译
            PatchTargetPostfix(
                                typeof(Difficulty).GetMethod("Easy"),
                                typeof(MainMenuOverride).GetMethod("EasyDifficultyPostfix"));
            PatchTargetPostfix(
                                typeof(Difficulty).GetMethod("Normal"),
                                typeof(MainMenuOverride).GetMethod("NormalDifficultyPostfix"));
            PatchTargetPostfix(
                                typeof(Difficulty).GetMethod("Hard"),
                                typeof(MainMenuOverride).GetMethod("HardDifficultyPostfix"));
            //成就 进度翻译
            PatchTargetPostfix(
                                typeof(DungeonUser).GetMethod("GetCompletedString"),
                                typeof(DungeonUserOverride).GetMethod("GetCompletedStringPostfix"));
            PatchTargetPostfix(
                                typeof(DungeonUser).GetMethod("BaseWinString"),
                                typeof(DungeonUserOverride).GetMethod("BaseWinStringPostfix"));
            PatchTargetPostfix(
                                typeof(DungeonUser).GetMethod("AltarString"),
                                typeof(DungeonUserOverride).GetMethod("AltarStringPostfix"));
            PatchTargetPostfix(
                                typeof(DungeonUser).GetMethod("AdvancedWinString"),
                                typeof(DungeonUserOverride).GetMethod("AdvancedWinStringPostfix"));
            PatchTargetPostfix(
                                typeof(DungeonUser).GetMethod("FirstFloorString"),
                                typeof(DungeonUserOverride).GetMethod("FirstFloorStringPostfix"));
            PatchTargetPostfix(
                                typeof(DungeonUser).GetMethod("AllMonstersProgress"),
                                typeof(DungeonUserOverride).GetMethod("AllMonstersProgressPostfix"));
            PatchTargetPrefix(
                                typeof(DungeonUser).GetMethod("UpdateHighScore"),
                                typeof(DungeonUserOverride).GetMethod("UpdateHighScoreOverride"));
            //Monster 相关信息
            PatchTargetPostfix(
                                typeof(Monster).GetMethod("Name"),
                                typeof(MonsterOverride).GetMethod("NamePostfix"));
            PatchTargetPrefix(
                                typeof(Monster).GetMethod("DevourString"),
                                typeof(MonsterOverride).GetMethod("DevourStringPostfix"));
            PatchTargetPostfix(
                                typeof(Monster).GetMethod("PowerString"),
                                typeof(MonsterOverride).GetMethod("PowerStringPostfix"));
            PatchTargetPostfix(
                                typeof(Monster).GetMethod("MonsterCounterString"),
                                typeof(MonsterOverride).GetMethod("MonsterCounterString"));
            PatchTargetPostfix(
                                typeof(AirElemental).GetMethod("MonsterCounterString"),
                                typeof(MonsterOverride).GetMethod("AEMonsterCounterString"));
            PatchTargetPostfix(
                                typeof(WaterElemental).GetMethod("MonsterCounterString"),
                                typeof(MonsterOverride).GetMethod("WEMonsterCounterString"));
            PatchTargetPostfix(
                                typeof(Monster).GetMethod("CombatPowerString"),
                                typeof(MonsterOverride).GetMethod("CombatPowerStringPostfix"));
            PatchTargetPostfix(
                                typeof(Monster).GetMethod("LevelString"),
                                typeof(MonsterOverride).GetMethod("LevelStringPostfix"));
            PatchTargetPostfix(
                                typeof(Monster).GetMethod("Defeated"),
                                typeof(MonsterOverride).GetMethod("DefeatedPostfix"));
            //希尔迪博士的快乐屋，喜欢的字符串，调整
            PatchTargetPostfix(
                 typeof(BrainsuckerPreference).GetMethod("LikeString"),
                 typeof(BrainsuckerPreferenceOverride).GetMethod("LikeString"));
            //Decree string
            PatchTargetPostfix(
                 typeof(Decree).GetMethod("DecreeString"),
                 typeof(DecreeOverride).GetMethod("DecreeString_Postfix"));
            //战斗中弃牌
            PatchTargetPostfix(
                typeof(ActionDiscard).GetMethod("SetTargetFinderParameters"),
                typeof(HelloMod).GetMethod("ActionDiscard_SetTargetFinderParameters_Postfix")
                );
            //战斗中需要选择装备或祷告进行操作的卡牌 的弹窗
            PatchTargetPostfix(
                typeof(ActionPilfer).GetMethod("SetTargetFinderParameters"),
                typeof(HelloMod).GetMethod("ActionPilfer_SetTargetFinderParameters_Postfix")
                );
            PatchTargetPostfix(
                typeof(ActionEngulf).GetMethod("SetTargetFinderParameters"),
                typeof(HelloMod).GetMethod("ActionEngulf_SetTargetFinderParameters_Postfix")
                );
            PatchTargetPostfix(
                typeof(ActionCrush).GetMethod("SetTargetFinderParameters"),
                typeof(HelloMod).GetMethod("ActionCrush_SetTargetFinderParameters_Postfix")
                );
            PatchTargetPostfix(
                typeof(ActionCrumble).GetMethod("SetTargetFinderParameters"),
                typeof(HelloMod).GetMethod("ActionCrumble_SetTargetFinderParameters_Postfix")
                );
            PatchTargetPostfix(
                typeof(ActionMime).GetMethod("SetTargetFinderParameters"),
                typeof(HelloMod).GetMethod("ActionMime_SetTargetFinderParameters_Postfix")
                );
            //Select a card ||TODO:等待替换
            //Achievement Viewer 运行时替换
            var SDAVBuildTextPaneMethod = typeof(ShopDialogueAchievementViewer).GetMethod("BuildTextPane", BindingFlags.Public | BindingFlags.Instance);
            var SDAVBuildTextPaneDict = new Dictionary<string, string> { 
                { "Unlock", TR.GetStr(MainMenuOverride.TableKey, "Unlock", "ACHIEVEMENT") } 
            };
            ReplaceStringPatch.PatchMethodWithTranspiler_In_SDAVBuildTextPane(SDAVBuildTextPaneMethod, SDAVBuildTextPaneDict);
            // High Score 运行时替换
            var highScoreBtnMethod = typeof(MainMenu).GetMethod("HighScoresButton", BindingFlags.Public | BindingFlags.Instance);
            var highScoreDict = new Dictionary<string, string> { { "High Scores", TR.GetStr(MainMenuOverride.TableKey, "High Scores") } };
            ReplaceStringPatch.PatchMethodWithTranspiler_In_HighScores(highScoreBtnMethod, highScoreDict);
            //History 运行时替换
            var SDHVCreateMethod = typeof(ShopDialogueHistoryViewer).GetMethod("Create", BindingFlags.Public | BindingFlags.Instance);
            var SDHVCreateDict = new Dictionary<string, string> { 
                { "You have no history!  Go make some!", TR.GetStr(MainMenuOverride.TableKey, "You have no history!  Go make some!") } 
            };
            ReplaceStringPatch.PatchMethodWithTranspiler_In_SDHVCreateMethod(SDHVCreateMethod, SDHVCreateDict);
            if (DreamQuestConfig.SkipLevelUpReward)//LevelUp Panel 重写，添加跳过的按钮
            {
                PatchTargetPrefix(
                typeof(DungeonPlayer).GetMethod("LevelUpPane", BindingFlags.Public | BindingFlags.Instance),
                typeof(DungeonPlayerOverride).GetMethod("LevelUpPane"));
            }
            //LevelUp Pane 运行时替换
            else
            {
                var LevelUpPaneMethod = typeof(DungeonPlayer).GetMethod("LevelUpPane", BindingFlags.Public | BindingFlags.Instance);
                var LevelUpPaneDict = new Dictionary<string, string> {
                { "Choose", TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose") },
                { "Level ", TR.GetStr(TranslationManager.specialTableKey, "Level") },
                { "You have gained ", TR.GetStr(DungeonPhysicalOverride.TableKey, "You have gained") },
                { " mana", TR.GetStr(TranslationManager.specialTableKey, "mana") },
                { " gold", TR.GetStr(TranslationManager.specialTableKey, "gold") },
                { " health", TR.GetStr(TranslationManager.specialTableKey, "health") },
                { "You have learned:", TR.GetStr(DungeonPhysicalOverride.TableKey, "You have learned") },
                { "Bonus! (Choose one)", TR.GetStr(DungeonPhysicalOverride.TableKey, "Bonus! (Choose one)") },
            };
                ReplaceStringPatch.PatchMethodWithTranspiler_In_LevelUpPane(LevelUpPaneMethod, LevelUpPaneDict);
            }

            PatchTargetPrefix(
                typeof(DungeonPlayer).GetMethod("AddCard", new Type[] { typeof(string), typeof(bool)}),
                typeof(DungeonPlayerOverride).GetMethod("AddCard"));
            //升级界面的特殊内容
            PatchTargetPrefix(
                typeof(LevelUpReward).GetMethod("SDOSpecial"),
                typeof(LevelUpRewardOverride).GetMethod("SDOSpecial"));
            //结算界面 运行时替换
            var CreateEndDialogueMethod = typeof(Dungeon).GetMethod("CreateEndDialogue", BindingFlags.Public | BindingFlags.Instance);
            var CreateEndDialogueDict = new Dictionary<string, string> {
                { "Congratulations!", TR.GetStr(DungeonPhysicalOverride.TableKey, "Congratulations!") },
                { "The ", TR.GetStr(DungeonPhysicalOverride.TableKey, "The") },
                { " only earns class specific achievements!", TR.GetStr(DungeonPhysicalOverride.TableKey, " only earns class specific achievements!") },
                { "Runs on Kitten difficulty do not earn achievements!", TR.GetStr(DungeonPhysicalOverride.TableKey, "Runs on Kitten difficulty do not earn achievements!") },
                { "Replays do not earn achievments!", TR.GetStr(DungeonPhysicalOverride.TableKey, "Replays do not earn achievments!") },
                { "New Achievements!", TR.GetStr(DungeonPhysicalOverride.TableKey, "New Achievements!") },
                { "Main Menu", TR.GetStr(DungeonPhysicalOverride.TableKey, "Main Menu") },
            };
            ReplaceStringPatch.PatchMethodWithTranspiler_In_CreateEndDialogue(CreateEndDialogueMethod, CreateEndDialogueDict);
            PatchTargetPrefix(
                typeof(DungeonStats).GetMethod("BuildEndAchievementViewer"),
                typeof(DungeonStatsOverride).GetMethod("BuildEndAchievementViewer")
                );
            //Infoblock 运行时替换
            var InfoblockMethod = typeof(Infoblock).GetMethod("BuildMyInfoBlock", BindingFlags.Public | BindingFlags.Instance);
            var InfoblockDict = new Dictionary<string, string> {
                { "Discard All", TR.GetStr(DungeonPhysicalOverride.TableKey, "Discard All") },
                { "Play\n All", TR.GetStr(DungeonPhysicalOverride.TableKey, "Play All") }
            };
            ReplaceStringPatch.PatchMethodWithTranspiler_In_InfoblockMethod(InfoblockMethod, InfoblockDict);
            //ShopDialogueTalentViewer 运行时替换
            var ShopDialogueTalentViewerMethod = typeof(ShopDialogueTalentViewer).GetMethod("BuildTextPane", BindingFlags.Public | BindingFlags.Instance);
            var ShopDialogueTalentViewerCreateMethod = typeof(ShopDialogueTalentViewer).GetMethod("Create", BindingFlags.Public | BindingFlags.Instance);
            var ShopDialogueTalentViewerDict = new Dictionary<string, string> {
                { "Choose a Talent", TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose a Talent") },
                { "Choose", TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose","TALENT") }
            };
            ReplaceStringPatch.PatchMethodWithTranspiler_In_ShopDialogueTalentViewerMethod(ShopDialogueTalentViewerMethod, ShopDialogueTalentViewerCreateMethod, ShopDialogueTalentViewerDict);
            //Choose Profile 运行时替换
            var ChooseProfileMethod = typeof(MainMenu).GetMethod("ChooseProfileDialogue", new Type[] {typeof(bool)});
            var ChooseProfileDict = new Dictionary<string, string> {
                { "Choose Profile", TR.GetStr(MainMenuOverride.TableKey, "Choose Profile") },
                { "New", TR.GetStr(MainMenuOverride.TableKey, "New", "PROFILE") },
                { "Delete", TR.GetStr(MainMenuOverride.TableKey, "Delete", "PROFILE") },
                { "Enter Name", TR.GetStr(MainMenuOverride.TableKey, "Enter Name", "PROFILE") },
                { "Submit", TR.GetStr(MainMenuOverride.TableKey, "Submit", "PROFILE") },
                { "Yes", TR.GetStr(MainMenuOverride.TableKey, "Yes", "PROFILE") },
                { "Cancel", TR.GetStr(MainMenuOverride.TableKey, "Cancel", "PROFILE") },
                { "Are you sure you want to delete ", TR.GetStr(MainMenuOverride.TableKey, "Are you sure you want to delete ", "PROFILE") },
            };
            var ChooseProfileSubnewMethod = typeof(MainMenu).GetMethod("CreateNewProfileDialogueBoolean");
            var ChooseProfileDeleteHandlerMethod = typeof(MainMenu).GetMethod("DeleteHandler");

            ReplaceStringPatch.PatchMethodWithTranspiler_In_ChooseProfileMethod( ChooseProfileMethod, ChooseProfileSubnewMethod, ChooseProfileDeleteHandlerMethod, ChooseProfileDict);
            PatchTargetPostfix(
                  typeof(MainMenu).GetMethod("SubmitNewProfile"),
                  typeof(MainMenuOverride).GetMethod("SubmitNewProfilePostfix"));
            //ReplayErrorMessage 翻译
            PatchTargetPostfix(
                  typeof(MainMenu).GetMethod("GetReplayErrorMessage"),
                  typeof(MainMenuOverride).GetMethod("GetReplayErrorMessagePostfix"));
            //[特殊]Choose Class 翻译
            PostPatchVirtualMethodAndOverrides(harmony, typeof(ProfessionBase), "ClassName",
                                typeof(ProfessionBaseOverride).GetMethod("ClassNamePostfix"));
            PostPatchVirtualMethodAndOverrides(harmony, typeof(ProfessionBase), "AbilityDescription",
                                typeof(ProfessionBaseOverride).GetMethod("AbilityDescriptionPostfix"));
            PostPatchVirtualMethodAndOverrides(harmony, typeof(ProfessionBase), "Description",
                                typeof(ProfessionBaseOverride).GetMethod("DescriptionPostfix"));
            //巨龙的特殊页面
            PatchTargetPrefix(
                typeof(ProfessionDragon).GetMethod("SmashPage"),
                typeof(ProfessionBaseOverride).GetMethod("SmashPage"));
            //巨龙的囤积能力
            PatchTargetPrefix(
                typeof(DungeonActionHoard).GetMethod("HoardPerform"),
                typeof(DungeonActionOverride).GetMethod("DungeonActionHoard_HoardPerform"));
            PatchTargetPrefix(
                typeof(DungeonActionDevour).GetMethod("Perform", new Type[] { typeof(Tile) }),
                typeof(DungeonActionOverride).GetMethod("DungeonActionDevour_Perform"));
            //刺客的刺杀能力（类似巨龙的吞噬）
            PatchTargetPrefix(
                typeof(DungeonActionMurder).GetMethod("Perform", new Type[] { typeof(Tile) }),
                typeof(DungeonActionOverride).GetMethod("DungeonActionMurder_Perform"));
            //Find Monster能力（不知道是谁的）
            PatchTargetPrefix(
                typeof(DungeonActionFindMonster).GetMethod("Perform"),
                typeof(DungeonActionOverride).GetMethod("DungeonActionFindMonster_Perform"));
            //地牢技能无法被释放时的提示
            PatchTargetPrefix(
                typeof(DungeonAction).GetMethod("OnClick", new Type[0]),
                typeof(DungeonActionOverride).GetMethod("OnClick"));
            //地牢技能准备就绪
            PatchTargetPrefix(
                typeof(DungeonPlayer).GetMethod("CoolingDownAbilities"),
                typeof(DungeonPlayerOverride).GetMethod("CoolingDownAbilities"));
            //哥布林机械师
            PatchTargetPrefix(
                typeof(GoblinMechanist).GetMethod("StartTurn",new Type[] {typeof(Player)}),
                typeof(GoblinMechanistOverride).GetMethod("StartTurn"));
            //卡牌上的额外词条
            PatchTargetPrefix(
                typeof(Card).GetMethod("BuildKeywordString"),
                typeof(CardOverride).GetMethod("BuildKeywordString"));
            //斯芬克斯的能力，禁止xx卡牌
            PatchTargetPrefix(
                typeof(Game).GetMethod("SetTopMessage", new Type[] {typeof(string)}),
                typeof(HelloMod).GetMethod("Game_SetTopMessage"));
            //混乱祷告，卡牌
            PatchTargetPrefix(
                typeof(ChaosPrayer).GetMethod("PlayEffect"),
                typeof(CardOverride).GetMethod("ChaosPrayer_PlayEffect"));
            //额外回合字符串的显示
            PatchTargetPrefix(
                typeof(Haste).GetMethod("PlayEffect"),
                typeof(CardOverride).GetMethod("Haste_PlayEffect"));
            PatchTargetPrefix(
                typeof(Storm).GetMethod("PlayEffect"),
                typeof(CardOverride).GetMethod("Storm_PlayEffect"));
            //马哈马特，卡牌
            PatchTargetPrefix(
                typeof(Mahamat).GetMethod("PlayEffect"),
                typeof(CardOverride).GetMethod("Mahamat_PlayEffect"));

            PatchTargetPostfix(
                typeof(PenaltyExtraTurn).GetMethod("PenaltyString"),
                typeof(CardOverride).GetMethod("PenaltyString"));
            //卡牌翻译 TODO
            /*PostPatchVirtualMethodAndOverrides(harmony, typeof(ActionCard), "PythonInitialize",
                                typeof(HelloMod).GetMethod("Card_PythonInitialize_Postfix"));
            PostPatchVirtualMethodAndOverrides(harmony, typeof(AttackCard), "PythonInitialize",
                                typeof(HelloMod).GetMethod("Card_PythonInitialize_Postfix"));
            PostPatchVirtualMethodAndOverrides(harmony, typeof(EquipmentCard), "PythonInitialize",
                                typeof(HelloMod).GetMethod("Card_PythonInitialize_Postfix"));
            PostPatchVirtualMethodAndOverrides(harmony, typeof(PrayerCard), "PythonInitialize",
                                typeof(HelloMod).GetMethod("Card_PythonInitialize_Postfix"));
            PostPatchVirtualMethodAndOverrides(harmony, typeof(ManaCard), "PythonInitialize",
                                typeof(HelloMod).GetMethod("Card_PythonInitialize_Postfix"));
            PostPatchVirtualMethodAndOverrides(harmony, typeof(ReactionCard), "PythonInitialize",
                                typeof(HelloMod).GetMethod("Card_PythonInitialize_Postfix"));
            PostPatchVirtualMethodAndOverrides(harmony, typeof(SpellCard), "PythonInitialize",
                                typeof(HelloMod).GetMethod("Card_PythonInitialize_Postfix"));*/
            PatchTargetPrefix(
                typeof(Card).GetMethod("Initialize", new Type[] { }),
                typeof(HelloMod).GetMethod("Card_Initialize_Prefix")
                );
            //卡牌图鉴双行可能导致的问题的修复==>这里是内部的卡牌名称显示，不需要进行修复
            /*PatchTargetPostfix(
                typeof(ShopDialogueCardViewer).GetMethod("CardName"),
                typeof(HelloMod).GetMethod("ShopDialogueCardViewer_CardNamePostfix")
                );*/
            PatchTargetPostfix(
                typeof(ShopDialogueCardViewerName).GetMethod("SetName"),
                typeof(HelloMod).GetMethod("ShopDialogueCardViewerName_SetName")
                );
            PatchTargetPostfix(
                typeof(ShopDialogueCardViewer).GetMethod("TypeToName"),
                typeof(HelloMod).GetMethod("ShopDialogueCardViewer_TypeToNamePostfix")
                );
            //修复 随机卡牌可能包含译文的问题
            PatchTargetPostfix(
                typeof(CardFinder).GetMethod("RandomName"),
                typeof(HelloMod).GetMethod("CardFinder_RandomName")
                );
            //Dungeon 卡组，最终Boss能力字符串
            PatchTargetPrefix(
                typeof(Dungeon).GetMethod("BasicDeckViewer"),
                typeof(DungeonOverride).GetMethod("BasicDeckViewer")
                );
            PatchTargetPrefix(
                typeof(Dungeon).GetMethod("EquipmentString"),
                typeof(DungeonOverride).GetMethod("EquipmentString")
                );
            PatchTargetPostfix(
                typeof(Dungeon).GetMethod("FinalBossPowers"),
                typeof(DungeonOverride).GetMethod("FinalBossPowers")
                );
            //此处不易进行修改
            /*PatchTargetPostfix(
                typeof(CardList).GetMethod("GetCardList"),
                typeof(HelloMod).GetMethod("CardList_GetCardListPostfix"));*/
            //CardData
            // 获取 CardData 构造函数的 ConstructorInfo
            ConstructorInfo[] constructors = typeof(CardData).GetConstructors();
            if (constructors != null && constructors.Length > 0)
            {
                harmony.Patch(constructors[0], null, new HarmonyMethod(typeof(HelloMod).GetMethod("CardData_CardData")));
            }
            else
            {
                mLogger.LogError("Can't Find Constructor");
            }
            //
            ConstructorInfo[] md_constructors = typeof(MonsterData).GetConstructors();
            if (md_constructors != null && md_constructors.Length > 0)
            {
                harmony.Patch(md_constructors[0], null, new HarmonyMethod(typeof(HelloMod).GetMethod("MonsterData_MonsterData")));
            }
            else
            {
                mLogger.LogError("Can't Find Constructor");
            }
            //CombatAbility
            PostPatchVirtualMethodAndOverrides(harmony, typeof(CombatAbility), "Name",
                                typeof(CombatAbilityNormalPatch).GetMethod("NamePostfix"));
            PostPatchVirtualMethodAndOverrides(harmony, typeof(CombatAbility), "Description",
                                typeof(CombatAbilityNormalPatch).GetMethod("DescriptionPostfix"));
            //DungeonAction
            PostPatchVirtualMethodAndOverrides(harmony, typeof(DungeonAction), "ButtonName",
                                typeof(DungeonActionOverride).GetMethod("ButtonNamePostfix"));
            //结算界面子界面重载 翻译
            PatchTargetPrefix(
                typeof(DungeonStats).GetMethod("CreateScoreDialogue"),
                typeof(DungeonStatsOverride).GetMethod("CreateScoreDialogueOverride"));
            //怪物图鉴全解锁
            if (DreamQuestConfig.UnlockAllMonster)
            {
                PatchTargetPrefix(
                typeof(Dungeon).GetMethod("StaticHasSeen"),
                typeof(HelloMod).GetMethod("DungeonHasSeenMonsterOverride"));
            }
            //教程汉化
            PatchTargetPrefix(
                typeof(DungeonUser).GetMethod("GetTutorialString"),
                typeof(DungeonUserOverride).GetMethod("GetTutorialStringOverride"));
            //MainMenu 重载(翻译 + 添加按钮)
            PatchTargetPrefix(
                typeof(MainMenu).GetMethod("Initialize"),
                typeof(MainMenuOverride).GetMethod("InitializeOverride"));
            PatchTargetPrefix(
                typeof(MainMenu).GetMethod("BuildCachedClassPickerDialogue"),
                typeof(MainMenuOverride).GetMethod("BuildCachedClassPickerDialogue"));
            //DungeonPhysical 重载(翻译 + 作弊按钮)==>UI
            PatchTargetPrefix(
                typeof(DungeonPhysical).GetMethod("InGameMenu"),
                typeof(DungeonPhysicalOverride).GetMethod("InGameMenuOverride"));
            PatchTargetPrefix(
                typeof(DungeonPhysical).GetMethod("ConstructSaveAndQuit"),
                typeof(DungeonPhysicalOverride).GetMethod("ConstructSaveAndQuitOverride"));
            //ShopDialogueTalentViewer Postfix
            PatchTargetPostfix(
                  typeof(ShopDialogueTalentViewer).GetMethod("TalentName"),
                  typeof(ShopDialogueTalentViewerOverride).GetMethod("TalentNamePostfix"));
            PatchTargetPostfix(
                  typeof(ShopDialogueTalentViewer).GetMethod("TalentEffect"),
                  typeof(ShopDialogueTalentViewerOverride).GetMethod("TalentEffectPostfix"));
            PatchTargetPostfix(
                  typeof(ShopDialogueTalentViewer).GetMethod("TalentRepeatable"),
                  typeof(ShopDialogueTalentViewerOverride).GetMethod("TalentRepeatablePostfix"));
            //AKMText Postfix
            PatchTargetPostfix(
                  typeof(DungeonPhysical).GetMethod("AKMText"),
                  typeof(DungeonPhysicalOverride).GetMethod("AKMTextPostfix"));
            //可以多选的情况下选择只进行不到最大值的选择的提示框 ==>天赋等相关的次级窗口
            PatchTargetPostfix(
                  typeof(DungeonActionTalentDeckTargetted).GetMethod("ConfirmText"),
                  typeof(DungeonActionDeckTargettedOverride).GetMethod("DungeonActionTalentDeckTargetted_ConfirmTextPostfix"));
            PatchTargetPrefix(
                  typeof(DungeonActionLevelUpDeckTargetted).GetMethod("BuildFromReward"),
                  typeof(DungeonActionDeckTargettedOverride).GetMethod("DungeonActionLevelUpDeckTargetted_BuildFromReward"));
            PatchTargetPrefix(
                  typeof(DungeonActionTalentDeckTargetted).GetMethod("CreateConfirmVerification"),
                  typeof(DungeonActionDeckTargettedOverride).GetMethod("DungeonActionTalentDeckTargetted_CreateConfirmVerification"));
            PatchTargetPrefix(
                  typeof(DungeonActionTalentDeckTargetted).GetMethod("BuildFromTalent"),
                  typeof(DungeonActionDeckTargettedOverride).GetMethod("DungeonActionTalentDeckTargetted_BuildFromTalent"));
            PatchTargetPrefix(
                  typeof(DungeonActionDynamicDeckTargetted).GetMethod("BuildFromString"),
                  typeof(DungeonActionDeckTargettedOverride).GetMethod("DungeonActionDynamicDeckTargetted_BuildFromString"));

            PatchTargetPostfix(
                  typeof(ActionModal).GetMethod("Choices"),
                  typeof(HelloMod).GetMethod("ActionModal_Choices"));
            //解锁所有卡牌的显示
            PatchTargetPrefix(
                  typeof(CardData).GetMethod("HasNever"),
                  typeof(HelloMod).GetMethod("CardData_HasNever"));
            //SDB 重载 技能相关的显示
            PatchTargetPrefix(
                typeof(SDB).GetMethod("CombatAbilityBig", new Type[] {typeof(CombatAbility), typeof(float), typeof(int) }),
                typeof(SDBOverride).GetMethod("CombatAbilityBig"));
            PatchTargetPrefix(
                typeof(SDB).GetMethod("CombatAbilityLittle"),
                typeof(SDBOverride).GetMethod("CombatAbilityLittle"));
            PatchTargetPrefix(
                typeof(SDB).GetMethod("ActionIconDescription"),
                typeof(SDBOverride).GetMethod("ActionIconDescription"));
            //地牢技能 Cancel TODO：后续可以考虑地牢的buttonName的翻译搬运到此处
            PatchTargetPrefix(
                typeof(SDB).GetMethod("ActionButton"),
                typeof(SDBOverride).GetMethod("ActionButton"));
            //开始界面的载入中汉化 Loading...
            PatchTargetPrefix(
                typeof(DeckBuilderTop).GetMethod("OnGUI"),
                typeof(DeckBuilderTopOverride).GetMethod("OnGUI"));
            //Infoblock重载
            PatchTargetPrefix(
                typeof(Infoblock).GetMethod("RestartBlock"),
                typeof(InfoblockOverride).GetMethod("RestartBlockPrefix"));
            PatchTargetPrefix(
                typeof(Infoblock).GetMethod("AskReallyConcede"),
                typeof(InfoblockOverride).GetMethod("AskReallyConcedePrefix"));
            PatchTargetPostfix(
                typeof(Infoblock).GetMethod("GetAbilityTextures"),
                typeof(InfoblockOverride).GetMethod("GetAbilityTexturesPostfix"));
            PatchTargetPrefix(
                typeof(Infoblock).GetMethod("AreYouSure"),
                typeof(InfoblockOverride).GetMethod("AreYouSurePrefix"));
            PatchTargetPrefix(
                typeof(Infoblock).GetMethod("EscapePhysical"),
                typeof(InfoblockOverride).GetMethod("EscapePhysical"));
            //游戏中卡牌显示
            PatchTargetPrefix(
                typeof(CardPhysical).GetMethod("InitializeTextNow"),
                typeof(CardPhysicalOverride).GetMethod("InitializeTextNow"));
            PatchTargetPrefix(
                typeof(CardPhysical).GetMethod("UpdateName"),
                typeof(CardPhysicalOverride).GetMethod("UpdateName"));
            PatchTargetPrefix(
                typeof(CardPhysical).GetMethod("RefreshTextBoxNow"),
                typeof(CardPhysicalOverride).GetMethod("RefreshTextBoxNow"));

            PatchTargetPrefix(
                typeof(TargetFinderBase).GetMethod("BuildGUI"),
                typeof(TargetFinderBaseOverride).GetMethod("BuildGUI"));
            PatchTargetPrefix(
                typeof(TargetFinderLine).GetMethod("OnGUI"),
                typeof(TargetFinderLineOverride).GetMethod("OnGUI"));

            PatchTargetPrefix(
                typeof(DungeonActionPortent).GetMethod("PortentPerform"),
                typeof(DungeonActionPortentOverride).GetMethod("PortentPerform"));
            PatchTargetPostfix(
                typeof(DungeonActionPortent).GetMethod("BuildText"),
                typeof(DungeonActionPortentOverride).GetMethod("BuildText"));
            //CheatMenu 运行时补丁
            var cheatMenuMethod = typeof(DungeonPhysical).GetMethod("CheatMenu", BindingFlags.Public | BindingFlags.Instance);
            var cheatMenuDict = new Dictionary<string, string> { 
                { "Reveal Map", TR.GetStr(DungeonPhysicalOverride.TableKey, "Reveal Map") },
                { "Level Up", TR.GetStr(DungeonPhysicalOverride.TableKey, "Level Up") },
                { "Gain Gold", TR.GetStr(DungeonPhysicalOverride.TableKey, "Gain Gold") },
                { "Uber Teleport", TR.GetStr(DungeonPhysicalOverride.TableKey, "Uber Teleport") },
                { "Gain 50 exp", TR.GetStr(DungeonPhysicalOverride.TableKey, "Gain 50 exp") },
                { "Gain Achievement Points", TR.GetStr(DungeonPhysicalOverride.TableKey, "Gain Achievement Points") },
                { "Reveal All Monsters", TR.GetStr(DungeonPhysicalOverride.TableKey, "Reveal All Monsters") },
            };
            ReplaceStringPatch.PatchMethodWithTranspiler_In_CheatMenu(cheatMenuMethod, cheatMenuDict);
            //Monster事件 运行时补丁，Fight按钮
            var MonsterDisplayMethod = typeof(Monster).GetMethod("DisplayInMini", new Type[] {typeof(bool)});
            var MonsterDisplayDict = new Dictionary<string, string> { 
                { "Fight!", TR.GetStr(DungeonPhysicalOverride.TableKey, "Fight!") } 
            };
            ReplaceStringPatch.PatchMethodWithTranspiler_In_MonsterDisplay(MonsterDisplayMethod, MonsterDisplayDict);
            //DungeonFeature DisplayInMini 中一般包含按钮 TODO，寻找 DungeonFeature子类，分别进行补丁？
            DungeonFeaturePatch();
            CombatAbilityPatch();
            //战斗中进行选择
            ActionBaseOverride.ActionPatch(harmony, this);
            //1.完善LevelUpRewardDescription 表格，比如添加其对 DungeonAbility等特殊关键词的引用，特别是 Channel 能力。


            //TODO:收尾时，需要检查所有表单，并将Translation中的 AutoAdd置为 false
            //收尾时需要检查 BardSong 和 BardSongDescription 等表
            //ProfessionDescription 和 ProfessionAbilityDescription 需要和职业名，等表格进行比对
            //TalentEffect 需要在卡牌和 buff之后重新检查
            //【需要脚本】，检查所有 TransitionSheet下面的表单，一行里面 必须有两个,, 同时也只能有两个！

            //1.优先汉化 buff， 卡牌；再围绕卡牌，buff，回到 CombatAbility等表格进行修改调整
            //SDB
            //Infoblock

        }

        private void CombatAbilityPatch()
        {
            List<CombatAbilityPatch> patches = new List<CombatAbilityPatch>() {
                new CombatAbilityOverride(),
                new CombatAbilityDesperatePrayerOverride(),
            };
            foreach (CombatAbilityPatch patch in patches)
            {
                patch.Patch(harmony, this);
                mLogger.LogMessage("Success Patch:" + patch.GetType().Name);
            }
        }

        private void DungeonFeaturePatch()
        {
            List<DungeonFeaturePatch> patches = new List<DungeonFeaturePatch>() {
                new HealthPackOverride(),
                new TreasureChestOverride(),
                new BlacksmithOverride(),
                new HealingPoolOverride(),
                new LichHunterOverride(),
                new MonasteryOverride(),
                new StairOverride(),
                new ShopOverride(),
                new SmoothieShackOverride(),
                new LevelStartOverride(),
                new ThroneOverride(),
                new AltarOverride(),
                new MushroomOverride(),
                new MushroomPatchOverride(),
                new BrainsuckerOverride(),
                new TavernOverride(),
                new TrapOverride(),
            };
            foreach (DungeonFeaturePatch patch in patches)
            {
                patch.Patch(harmony, this);
                mLogger.LogMessage("Success Patch:" + patch.GetType().Name);
            }
        }

        string configPath = $@"{Paths.PluginPath}\LocalizationConfig.ini";
        private void LoadConfig()
        {
            var parser = new IniParser();
            parser.Load(configPath);
            //语言区域
            string lang = parser.GetString("Lang", "Current", "en");
            //功能激活区域
            bool coverFont = parser.GetBool("Features", "CoverFont", false);
            bool unlockAll = parser.GetBool("Features", "UnlockAllMonster", false);
            bool activeCheat = parser.GetBool("Features", "ActiveCheatMenu", false);
            bool dontLoadMod = parser.GetBool("Features", "DontloadMod", false);
            bool overrideSQL = parser.GetBool("Features", "OverrideSQLDatabase", false);
            bool skipEndTurnConfirm = parser.GetBool("Features", "SkipEndTurnConfirm", false);
            bool seenAllCards = parser.GetBool("Features", "SeenAllCard", false);
            bool mixCardName = parser.GetBool("Features", "MixCardName", false);
            bool cmdtot = parser.GetBool("Features", "CardViewer_Mainly_display_the_original_text", false);
            bool rnct = parser.GetBool("Features", "RandomName_Not_Contain_Translation", true);
            bool nameCover = parser.GetBool("Features", "UsePlayerNameTransition", true);

            bool skipReward = parser.GetBool("Features", "SkipLevelUpReward", true);
            bool isUseOtherResource = parser.GetBool("Features", "IsUseOtherResource", true);
            //数值区域
            int skipRewardGold = parser.GetInt("Settings", "SkipLevelUpRewardGold", 10);
            float musicVolume = 0.01f * parser.GetInt("Settings", "MusicVolume", 5);
            float soundVolume = 0.01f * parser.GetInt("Settings", "SoundVolume", 50);
            /*int maxPlayers = parser.GetInt("Settings", "MaxPlayers", 4);
            int gameSpeed = parser.GetInt("Settings", "GameSpeed", 1);*/
            Logger.LogInfo(lang + "|" + coverFont + "|" + unlockAll);
            DreamQuestConfig.CurrentLang = lang;
            DreamQuestConfig.CoverFont = coverFont;
            DreamQuestConfig.UnlockAllMonster = unlockAll;
            DreamQuestConfig.ActiveCheatMenu = activeCheat;
            DreamQuestConfig.DontloadMod = dontLoadMod;
            DreamQuestConfig.OverrideSQLDatabase = overrideSQL;
            DreamQuestConfig.SkipEndTurnConfirm = skipEndTurnConfirm;
            DreamQuestConfig.SeenAllCard = seenAllCards;
            DreamQuestConfig.MixCardName = mixCardName;
            DreamQuestConfig.CardViewer_Mainly_display_the_original_text = cmdtot;
            DreamQuestConfig.RandomName_Not_Contain_Translation = rnct;
            DreamQuestConfig.UsePlayerNameTransition = nameCover;
            DreamQuestConfig.SkipLevelUpReward = skipReward;
            DreamQuestConfig.SkipLevelUpRewardGold = skipRewardGold;

            DreamQuestConfig.IsUseOtherResource = isUseOtherResource;
            DreamQuestConfig.MusicVolume = musicVolume;
            DreamQuestConfig.SoundVolume = soundVolume;

            if(DreamQuestConfig.IsZh)
            {
                DreamQuestConfig.CoverFont = true;
            }
        }

        private void LoadLocalArtResource()
        {
            RegisterTextureSourceFromLocal("Textures/ButtonBase", $"{Paths.PluginPath}\\ArtResource\\GUI\\ButtonBase.png");
            RegisterTextureSourceFromLocal("Textures/ButtonDisabled", $"{Paths.PluginPath}\\ArtResource\\GUI\\ButtonDisabled.png");
            RegisterTextureSourceFromLocal("Textures/ButtonPressed", $"{Paths.PluginPath}\\ArtResource\\GUI\\ButtonPressed.png");
            RegisterTextureSourceFromLocal("Textures/TextImageBorderless", $"{Paths.PluginPath}\\ArtResource\\GUI\\contentBG.png");
        }

        private static int totalRequireResource = 0;
        private void LoadAssetBundle(int version)
        {
            totalRequireResource = DreamQuestConfig.IsUseOtherResource ? 2 : 7;
            //载入字体
            StartCoroutine(LoadBundle($"file:///{Paths.PluginPath}\\AssetBundle\\Important\\fontPack.unity3d", version, OnLoadFont));
            //载入 许愿 卡牌
            StartCoroutine(LoadBundle($"file:///{Paths.PluginPath}\\AssetBundle\\Important\\texturePack.unity3d", version, OnLoadPenltyCardTexture));

            if (DreamQuestConfig.IsUseOtherResource) {
                //载入额外的 替换UI
                //StartCoroutine(LoadBundle($"file:///{Paths.PluginPath}\\AssetBundle\\GUITextures.unity3d", version, OnLoadOverrideGUITexture));
                //载入额外的 地形替换 
                StartCoroutine(LoadBundle($"file:///{Paths.PluginPath}\\AssetBundle\\DungeonTextures.unity3d", version, OnLoadOverrideDungeonTexture));
                //载入额外的 怪物替换 
                StartCoroutine(LoadBundle($"file:///{Paths.PluginPath}\\AssetBundle\\MonsterTextures.unity3d", version, OnLoadOverrideMonsterDungeonTexture));
                //载入额外的 角色大图替换 
                StartCoroutine(LoadBundle($"file:///{Paths.PluginPath}\\AssetBundle\\Professiontextures_big.unity3d", version, OnLoadOverrideProfessionTextureBig));
                //载入额外的 角色小图替换 
                StartCoroutine(LoadBundle($"file:///{Paths.PluginPath}\\AssetBundle\\Professiontextures_little.unity3d", version, OnLoadOverrideProfessionTextureLittle));
            }
        }
        #region PatchMethod
        public void PatchTargetPostfix(MethodInfo tm, MethodInfo pm)
        {
            if (tm == null)
            {
                base.Logger.LogError("Postfix Target method not found.");
                return;
            }
            if (pm == null)
            {
                base.Logger.LogError("Postfix method not found." );
                return;
            }
            var info = harmony.Patch(tm, postfix: new HarmonyMethod(pm));
            base.Logger.LogInfo("Patch result: " + info);
        }

        public void PatchTargetPrefix(MethodInfo tm, MethodInfo pm)
        {
            base.Logger.LogInfo(tm.ToString() + "|||" + pm.ToString());
            if (tm == null)
            {
                base.Logger.LogError("Prefix Target method not found.");
                return;
            }
            if (pm == null)
            {
                base.Logger.LogError("Prefix method not found.");
                return;
            }
            var info = harmony.Patch(tm, prefix: new HarmonyMethod(pm));
            base.Logger.LogInfo("Patch result: " + info);
        }

        public static void PostPatchVirtualMethodAndOverrides(Harmony harmony, Type baseType, string methodName, MethodInfo postfix)
        {
            // 获取基类的目标方法
            MethodInfo baseMethod = baseType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (baseMethod != null)
            {
                harmony.Patch(baseMethod, postfix: new HarmonyMethod(postfix));
            }

            // 获取所有子类并检查是否重写目标方法
            var subTypes = baseType.Assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
            foreach (var subType in subTypes)
            {
                MethodInfo overrideMethod = subType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (overrideMethod != null && overrideMethod.DeclaringType == subType)
                {
                    harmony.Patch(overrideMethod, postfix: new HarmonyMethod(postfix));
                }
            }
        }

        #endregion

        //[HarmonyPostfix, HarmonyPatch(typeof(SDB), nameof(SDB.Text), new Type[] { typeof(Vector2), typeof(string), typeof(MulticastDelegate) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
        ///用于界面中的中英文替换 
        ///Q:如果存在多个重载会怎么样
        //[HarmonyPostfix, HarmonyPatch(typeof(SDB), nameof(SDB.Text))]
        //缺少了一个Public
        //导致GetMethod找不到
        public static ShopDialogueText SDB_Text_Postfix(ShopDialogueText text)
        {
            mLogger.LogInfo("SDB_Text_Postfix executed.");
            return text;
        }

        //[HarmonyPrefix, HarmonyPatch(typeof(FontManager), nameof(FontManager.SetFontSize), new Type[] { typeof(TextMesh), typeof(string) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal })]
        //两个条件 public static
        //不要再添加 标签
        ///用于界面中的中英文替换 
        ///Q:如果存在多个重载会怎么样 <summary>
        /// 用于界面中的中英文替换 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        /// 
        private static List<TextMesh> recordMesh = new List<TextMesh>();
        private static List<int> recordFontSize = new List<int>();
        public static bool SetFontSizeOverride(TextMesh t, int x)
        {
            if (!isLoadedFont && DreamQuestConfig.CoverFont)
            {
                recordMesh.Add(t);
                recordFontSize.Add(x);
                return true;
            }
            if (DreamQuestConfig.CoverFont)
            {
                Color color = t.renderer.material.color;
                t.font = loadFont;
                Shader cache = t.renderer.sharedMaterial.shader;
                t.renderer.material = loadFont.material;
                t.renderer.material.shader = cache;
                t.fontSize = x;
                t.renderer.material.color = color;
                return false;
            }
            return true;
        }

        public static void SetTextMeshFont(TextMesh t, int size = 0)
        {
            if (size <= 0) size = 32;
            if (!isLoadedFont && DreamQuestConfig.CoverFont)
            {
                recordMesh.Add(t);
                recordFontSize.Add(size);
                return;
            }
            if (DreamQuestConfig.CoverFont)
            {
                Shader cache = t.renderer.sharedMaterial.shader;
                Color color = t.renderer.material.color;
                t.renderer.material = loadFont.material;
                t.renderer.material.shader = cache;
                t.font = loadFont;
                t.renderer.material.color = color;
                t.fontSize = size;
            }
        }

        private static  List<TextMesh> testMesh = new List<TextMesh>();
        public static void RegisterTextObj(TextMesh test)
        {
            testMesh.Add(test);
        }

        public static bool Textbox_Create_Override(Textbox __instance,string s,float lineWidth)
        {
            __instance.fontSize = 32;
            mLogger.LogInfo("Textbox Content:" + s );
            return true;
        }
        public static bool DungeonHasSeenMonsterOverride(ref bool __result, MonsterData m)
        {
            __result = true;
            return false;
        }

        private void CreateModalDialogue()
        {
            Vector2 vector = new Vector2(10f, 4.6f);//窗口大小
            ShopDialogueObject shopDialogueObject = SDB.ModalDialogue(vector, "This is just modal window test.", new string[] { "Yes", "No" }, (value) =>
            {
                //返回的value是选择选项的 index
                //Q：Debug.Log为何不生效？
                Debug.Log($"You Select Option {value}");
                if (value == 1)
                {
                    MainMenu.instance.gp.game.DestroyActiveShopNow();
                }
            });
            SDB.CancelButton(shopDialogueObject, () => { MainMenu.instance.gp.game.DestroyActiveShopNow(); });
            shopDialogueObject.CenterTo(new Vector3(0, 0, 6f));
            MainMenu.instance.gp.game.activeShop = shopDialogueObject;
        }

        private void BackToMainMenu()
        {
            GameManager.gameType = GameType.MainMenu;
            Application.LoadLevel(0);
        }

        private GameObject uiPrefab = null;
        //无效
        /*public void LoadAssetBundle()
        {
            var ab = AssetBundle.CreateFromFile($"{Paths.PluginPath}/fontPack.unity3d");
            if( ab == null)
            {
                Logger.LogInfo("无法正常载入AssetBundle：fontPack.unity3d");
                return;
            }
            
            uiPrefab = (GameObject)ab.Load("TestCanvas");
            if( uiPrefab == null)
            {
                Logger.LogInfo("无法正常载入GameObject: TestCanvas");
                return;
            }

            Instantiate(uiPrefab);
        }*/
        

        public static void IsIPhonePostfix(ref bool __result)
        {
            __result = true;
        }

        public static void ButtonCreatePostfix(Vector2 size, ref ShopDialogueButton __instance)
        {
            SetTextMeshFont(__instance.text);
        }

        public static void ButtonCreateWithTexPostfix(Vector2 size, Texture[] textures, ref ShopDialogueButton __instance)
        {
            SetTextMeshFont(__instance.text);
        }

        public static bool Card_Initialize_Prefix(Card __instance)
        {
            if (DreamQuestConfig.IsEn)
                return true;
            if(__instance.GetType() == typeof(Card))
            {
                return true;
            }
            string originCardName = string.Empty;
            //cardName
            CardNameReplace(ref __instance.cardName, ref originCardName);
            //text
            if (!(__instance.text.Equals(string.Empty) || __instance.text.Equals("")))
            {
                __instance.text = Csv.GetTranslationByID(_cardTextTableKey, "_" + __instance.GetType().ToString());
            }
            return true;
        }

        private const string _cardNameTableKey = "CardName";
        private const string _cardTextTableKey = "CardTextParagraph";
        public static void Card_PythonInitialize_Postfix(Card __instance)
        {
            string originCardName = string.Empty;
            //cardName
            CardNameReplace(ref __instance.cardName, ref originCardName);
            mLogger.LogMessage("Replace Card:" + __instance.GetType().Name +"||" + originCardName + "|" + __instance.cardName);
            //text
            /*if (!(__instance.text.Equals(string.Empty) || __instance.text.Equals(""))){
                __instance.text = Csv.GetTranslationByID(_cardTextTableKey, "_" + originCardName);
            }*/
        }

        public static void CardData_CardData(CardData __instance, string internalName, PreferenceWrapper[] biases, int gold,
                                  UserAttribute[] requirements, bool hasDynamicGold,
                                  Func<Dungeon> dynamicGoldFunction,
                                  float thief, float priest, float warrior, float wizard,
                                  int tier, int maxSpawns, string cardType, DamageTypes elementalAffinity, string longName)
        {
            string originCardName = string.Empty;
            CardNameReplace(ref __instance.realName, ref originCardName);
        }

        public static void CardFinder_RandomName(ref string __result)
        {
            if (DreamQuestConfig.MixCardName)
            {
                if (DreamQuestConfig.RandomName_Not_Contain_Translation)
                {
                    __result = __result.Split('\n')[0];
                }
            }
        }

        public static void MonsterData_MonsterData(MonsterData __instance, string internalName, Environments[] locations, int minLevel, int maxLevel, bool boss, int cryptin, int forestin, int dungeonin, int waterin, int volcanoin, int mountainin, string monsterName, string bestiaryEntry)
        {
            if (!DreamQuestConfig.IsEn)
            {
                __instance.bestiaryEntry = Csv.GetTranslationByID("MonsterBestiary", "_" + __instance.internalName);
            }
        }
        //TODO:通过读取配置，决定是否中英文同时显示
        public static void CardNameReplace(ref string cardName, ref string originName)
        {
            if (string.IsNullOrEmpty(cardName)) { return; }
            //cardName
            if (!(cardName.Equals(string.Empty) || cardName.Equals("")))
            {
                originName = cardName;
                if (originName.Equals("Dance, Puppets!"))
                {
                    cardName = TR.GetStr(_cardNameTableKey, "Dance Puppets!");
                }
                else
                {
                    cardName = TR.GetStr(_cardNameTableKey, originName);
                }
                if(!DreamQuestConfig.IsEn && DreamQuestConfig.MixCardName)
                {
                    cardName = originName + "\n" + cardName;
                }
            }
        }


        public static bool CardData_HasNever(ref bool __result,CardData __instance)
        {
            if (DreamQuestConfig.SeenAllCard)
            {
                __result = false;
                return false;
            }
            return true;
        }

        public static void ActionModal_Choices(ref string[] __result)
        {
            if(!(__result.Length == 0))
            {
                __result[0] = TR.GetStr(DungeonPhysicalOverride.TableKey, "Yes");
                __result[1] = TR.GetStr(DungeonPhysicalOverride.TableKey, "No");
            }
        }

        public static void ActionDiscard_SetTargetFinderParameters_Postfix(TargetFinderParameters tf,ActionDiscard __instance)
        {
            if (!DreamQuestConfig.IsEn) {
                tf.cardButtonText = TR.GetStr(DungeonPhysicalOverride.TableKey, "Discard this");
                tf.text = TR.GetStr(DungeonPhysicalOverride.TableKey, "DiscardContent").Replace(TR.PlaceHolder, __instance.strength.ToString());
            }
        }

        public static void ActionPilfer_SetTargetFinderParameters_Postfix(TargetFinderParameters tf, ActionPilfer __instance)
        {
            if (!DreamQuestConfig.IsEn)
            {
                tf.text = TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose an equipment to steal");
            }
        }
        public static void ActionEngulf_SetTargetFinderParameters_Postfix(TargetFinderParameters tf, ActionEngulf __instance)
        {
            if (!DreamQuestConfig.IsEn)
            {
                tf.text = TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose an equipment or prayer to exile");
            }
        }
        public static void ActionCrush_SetTargetFinderParameters_Postfix(TargetFinderParameters tf, ActionCrush __instance)
        {
            if (!DreamQuestConfig.IsEn)
            {
                tf.text = TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose an equipment to destroy");
            }
        }
        public static void ActionCrumble_SetTargetFinderParameters_Postfix(TargetFinderParameters tf, ActionCrumble __instance)
        {
            if (!DreamQuestConfig.IsEn)
            {
                tf.text = TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose an equipment or prayer to exile");
            }
        }

        public static void ActionMime_SetTargetFinderParameters_Postfix(TargetFinderParameters tf, ActionMime __instance)
        {
            if (!DreamQuestConfig.IsEn)
            {
                tf.text = TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose an action card to copy");
            }
        }

        public static void ShopDialogueCardViewer_CardNamePostfix(ref string __result)
        {
            if (DreamQuestConfig.MixCardName)
            {
                __result = __result.Split('\n')[0];
            }
        }

        public static void ShopDialogueCardViewer_TypeToNamePostfix(ref string __result)
        {
            if (!DreamQuestConfig.IsEn)
            {
                string cache = __result;
                string value = __result.Split(' ')[0];//例如: Other
                value = TR.GetStr(TR.SK, value.ToLower(), "CARD");
                string pattern = TR.GetStr(DungeonPhysicalOverride.TableKey, "CardType", "CARDVIEWER");
                __result = pattern.Replace("{value}", value);
            }
        }

        public static void ShopDialogueCardViewerName_SetName(CardData s, ShopDialogueCardViewerName __instance)
        {
            if (DreamQuestConfig.MixCardName)
            {
                if (DreamQuestConfig.CardViewer_Mainly_display_the_original_text)
                {
                    __instance.text.text = s.realName.Split('\n')[0];//原文
                }
                else
                {
                    __instance.text.text = s.realName.Split('\n')[1];//译文
                }
            }
        }

        public static void Dragon_SmashPage(ProfessionDragon __instance)
        {
            string text = "There was something here \n Now it's a smoking ruin \n Darn - stupid dragon";
            float x = __instance.dungeon.physical.WindowSize().x;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText("Dragon Smash!", 48, Color.black);
            ShopDialogueText shopDialogueText = SDB.CenteredText(x, text, 32, Color.black);
            SDB.Background(shopDialogueText);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueText }, "VP", 0.1f);
            SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
            SDB.CancelButton(shopDialogueAligned, __instance.dungeon.WindowBack);
            shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation());
            __instance.dungeon.activeShop = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
        }

        public static bool Game_SetTopMessage(ref string s)
        {
            //对要显示的信息进行处理
            if (!DreamQuestConfig.IsEn)
            {
                // 正则表达式匹配-斯芬克斯的显示信息，禁止某一类型的卡牌
                string pattern = @"I forbid (\w+) cards!";
                Regex regex = new Regex(pattern);

                Match match = regex.Match(s);

                if (match.Success)
                {
                    string value = match.Groups[1].Value;  // 获取捕获的值 禁止的卡牌类型
                    string valueTrans = TR.GetStr(TR.SK, value);
                    s = TR.GetStr(DungeonPhysicalOverride.TableKey, "forbidMsg").Replace(TR.PlaceHolder, valueTrans);
                }
                else//其他信息，比如法则
                {
                    if(s.StartsWith("Dance Puppet!"))
                    {
                        //Player PuppetString
                        s = TR.GetStr(DungeonPhysicalOverride.TableKey, "puppetMsg").Replace("\\n", "\n");
                    }
                    else
                    {

                    }
                }
            }
            return true;
        }

        public static void MaterialManager_Fallback(Renderer r, string s)
        {
            if (DreamQuestConfig.IsEn)
                return;

            if (s.Contains("Penalty"))
            {
                string k = s.Replace("Textures/", "");
                Texture tex = textMgr.FindTextureByKey(k);
                if (tex)
                {
                    r.material.mainTexture = tex;
                }
            }
        }
        //使用自己的卡面（不走正常获得卡牌的路径）
        public static bool MaterialManager_Fallback_Override(Renderer r, string s)
        {
            if (s.Contains("ModTexture"))
            {
                Texture tex = GlobalTextureManager.GetTextureByModTextureKey(s);//全局的自定义材质管理器
                if(tex == null)
                {
                    //使用某一张默认的卡牌材质 Heal 的默认材质
                    tex = (Texture)Resources.Load("Textures/Cards/Heal", typeof(Texture));
                }
                r.material.mainTexture = tex;
            }
            return true;
        }

        public static void CardList_GetCardListPostfix(CardList __instance)
        {
            CardList.cardList.Add(new CardData("HealingPotion", new PreferenceWrapper[0], 7, new UserAttribute[0], false, null, (float)3, (float)3, (float)3, (float)3, 2, 3, "ActionCard", DamageTypes.NONE, "Healing Potion"));//向全卡牌列表中添加数据
            CardList.allCards.Add("");//
        }

        public static bool Resource_Load_Prefix(ref UnityEngine.Object __result, string path, Type systemTypeInstance)
        {
            if (systemTypeInstance == typeof(Texture)) {
                mLogger.LogMessage("Try Load Texture:" + path);
                //查询 AssetBundle中是否存在相同路径的资源，如果有则进行替换，如果没有就沿用旧资源
                Texture tex = GlobalTextureManager.GetTextureByModTextureKey(path);//全局的自定义材质管理器
                if (tex != null)
                {
                    __result = tex;
                    return false;
                }
            }
            return true;
        }

        public static void RegisterTextureSourceFromLocal(string registerPath, string localPath)
        {
            if (File.Exists(localPath))
            {
                byte[] fileData = File.ReadAllBytes(localPath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                //注册
                GlobalTextureManager.RegisterModTexture(registerPath, (path) => { return tex; });
            }
            else
            {
                mLogger.LogError("图片文件未找到: " + localPath);
            }
        }

        private void OnLoadFont(AssetBundle assetBundle)
        {
            if (assetBundle.mainAsset != null)
            {
                GameObject gameObject = assetBundle.mainAsset as GameObject;

                Instantiate(gameObject);
                assetBundle.Unload(false);

                GameObject gobj = GameObject.Find("TestUI");
                if (gobj != null)
                {
                    loadFont = gobj.GetComponentInChildren<Text>().font;
                    loadFont.material.renderQueue = FontManager.instance.fontMaterials[0].renderQueue;
                    isLoadedFont = true;
                    //重新设置字体
                    for (int i = 0; i < recordMesh.Count; i++)
                    {
                        if (recordMesh[i] != null)
                            SetFontSizeOverride(recordMesh[i], recordFontSize[i]);
                    }
                    recordMesh.Clear();
                    recordFontSize.Clear();

                    gobj.SetActive(false);

                    OnAfterFontLoaded?.Invoke();
                }
            }
            else
            {
                base.Logger.LogInfo("AssetBundle：fontPack.unity3d  assetBundle.mainAsset 为空！");
            }
        }

        private void OnLoadPenltyCardTexture(AssetBundle assetBundle)
        {
            if (assetBundle.mainAsset != null)
            {
                GameObject gameObject = assetBundle.mainAsset as GameObject;

                GameObject instance = (GameObject)Instantiate(gameObject);
                assetBundle.Unload(false);
                mLogger.LogMessage(instance.name + "||child:" + instance.transform.childCount);

                GameObject gobj = instance;
                if (gobj != null)
                {
                    gobj.SetActive(false);
                    DontDestroyOnLoad(gobj);
                    mLogger.LogMessage(gobj.name + "||child:" + gobj.transform.childCount);
                    textMgr = new TextureDict(gobj);
                    textMgr.GenerateArr();
                    OnAfterTexLoaded?.Invoke();
                }
            }
            else
            {
                base.Logger.LogInfo("AssetBundle：texturePack.unity3d  assetBundle.mainAsset 为空！");
            }
        }

        private void OnLoadOverrideGUITexture(AssetBundle assetBundle)
        {
            if (assetBundle.mainAsset != null)
            {
                UnityEngine.Object[] overrideTexObjArr = assetBundle.LoadAll(typeof(Texture));
                for (int i = 0; i < overrideTexObjArr.Length; i++)
                {
                    Texture tex = overrideTexObjArr[i] as Texture;
                    mLogger.LogMessage("Register Texture:Textures/" + tex.name + "||" + tex.name);
                    GlobalTextureManager.RegisterModTexture("Textures/" + tex.name, (path) =>
                    {
                        return tex;
                    });
                }
            }
            else
            {
                base.Logger.LogInfo("AssetBundle：" + assetBundle.name + ".unity3d  assetBundle.mainAsset 为空！");
            }
        }

        private void OnLoadOverrideDungeonTexture(AssetBundle assetBundle)
        {
            if (assetBundle.mainAsset != null)
            {
                UnityEngine.Object[] overrideTexObjArr = assetBundle.LoadAll(typeof(Texture));
                for (int i = 0; i < overrideTexObjArr.Length; i++)
                {
                    Texture tex = overrideTexObjArr[i] as Texture;
                    mLogger.LogMessage("Register Texture:DungeonTextures/Little/" + tex.name + "||" + tex.name);
                    GlobalTextureManager.RegisterModTexture("DungeonTextures/Little/" + tex.name, (path) =>
                    {
                        return tex;
                    });
                    GlobalTextureManager.RegisterModTexture("DungeonTextures/Big/" + tex.name, (path) =>
                    {
                        return tex;
                    });
                }
            }
            else
            {
                base.Logger.LogInfo("AssetBundle：" + assetBundle.name + ".unity3d  assetBundle.mainAsset 为空！");
            }
        }

        private void OnLoadOverrideMonsterDungeonTexture(AssetBundle assetBundle)
        {
            if (assetBundle.mainAsset != null)
            {
                UnityEngine.Object[] overrideTexObjArr = assetBundle.LoadAll(typeof(Texture));
                for (int i = 0; i < overrideTexObjArr.Length; i++)
                {
                    Texture tex = overrideTexObjArr[i] as Texture;
                    mLogger.LogMessage("Register Texture:DungeonTextures/Little/" + tex.name + "||" + tex.name);
                    GlobalTextureManager.RegisterModTexture("DungeonTextures/Little/" + tex.name, (path) =>
                    {
                        return tex;
                    });
                    GlobalTextureManager.RegisterModTexture("DungeonTextures/Big/" + tex.name, (path) =>
                    {
                        return tex;
                    });
                }
            }
            else
            {
                base.Logger.LogInfo("AssetBundle：" + assetBundle.name + ".unity3d  assetBundle.mainAsset 为空！");
            }
        }

        private void OnLoadOverrideProfessionTextureLittle(AssetBundle assetBundle)
        {
            if (assetBundle.mainAsset != null)
            {
                UnityEngine.Object[] overrideTexObjArr = assetBundle.LoadAll(typeof(Texture));
                for (int i = 0; i < overrideTexObjArr.Length; i++)
                {
                    Texture tex = overrideTexObjArr[i] as Texture;
                    mLogger.LogMessage("Register Texture:ProfessionTextures/Little/" + tex.name + "||" + tex.name);
                    GlobalTextureManager.RegisterModTexture("ProfessionTextures/Little/" + tex.name, (path) =>
                    {
                        return tex;
                    });
                }
            }
            else
            {
                base.Logger.LogInfo("AssetBundle：" + assetBundle.name + ".unity3d  assetBundle.mainAsset 为空！");
            }
        }

        private void OnLoadOverrideProfessionTextureBig(AssetBundle assetBundle)
        {
            if (assetBundle.mainAsset != null)
            {
                UnityEngine.Object[] overrideTexObjArr = assetBundle.LoadAll(typeof(Texture));
                for (int i = 0; i < overrideTexObjArr.Length; i++)
                {
                    Texture tex = overrideTexObjArr[i] as Texture;
                    mLogger.LogMessage("Register Texture:ProfessionTextures/Big/" + tex.name + "||" + tex.name);
                    GlobalTextureManager.RegisterModTexture("ProfessionTextures/Big/" + tex.name, (path) =>
                    {
                        return tex;
                    });
                }

                OnAfterProfessionBigLoaded();
            }
            else
            {
                base.Logger.LogInfo("AssetBundle：" + assetBundle.name + ".unity3d  assetBundle.mainAsset 为空！");
            }
        }

        private static void AfterLoadAllBundle()
        {
            if (totalRequireResource > 0)
                return;
            //Time.timeScale = 1.0f;//无效
        }

        public IEnumerator LoadBundle(string url, int version,Action<AssetBundle> callback)
        {
            using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
            {
                base.Logger.LogInfo("尝试从 www 载入 " + url + "} {" + version + "}");
                yield return www;
                if (System.IO.File.Exists(url)) base.Logger.LogInfo("存在AssetBundle：" + url);
                AssetBundle assetBundle = www.assetBundle;
                base.Logger.LogInfo("成功载入AssetBundle：" + url);
                if (assetBundle == null)//此处为空
                {
                    base.Logger.LogInfo("AssetBundle:" + url + " 为空！");
                }
                else
                {
                    totalRequireResource--;
                    callback.Invoke(assetBundle);
                    AfterLoadAllBundle();
                }
            }
        }

        private static AudioClip btnSound = null;

        IEnumerator LoadAudio()
        {
            string fileUrl = $"file:///{Paths.PluginPath}\\SFX\\zipclick.wav"; // 使用 file 协议读取本地文件 需要 wav 或者 ogg
            mLogger.LogMessage("尝试 || 加载音频 ||" + fileUrl);
            WWW www = new WWW(fileUrl); // 创建 WWW 对象并加载音频
            yield return www; // 等待加载完成

            if (!string.IsNullOrEmpty(www.error))
            {
                mLogger.LogError("音频加载失败: " + www.error);
            }
            else
            {
                // 获取加载的音频文件
                btnSound = www.GetAudioClip(false);
                mLogger.LogMessage("按钮音频加载成功");
            }
        }

        public static void NiceButton_OnMouseUp_Postfix(NiceButton __instance)
        {
            if (btnSound != null) {
                FieldInfo fieldInfo = AccessTools.Field(typeof(NiceButton), "_clicked");
                bool clicked = (bool)fieldInfo.GetValue(__instance);
                //播放点击音效
                if (clicked)
                {
                    MusicManager.instance.effectsPlayer.volume = DreamQuestConfig.SoundVolume;
                    MusicManager.instance.effectsPlayer.PlayOneShot(btnSound);
                }
            }
        }

        public static void MusicManager_Awake_Postfix()
        {
            FieldInfo fieldInfo = AccessTools.Field( typeof(MusicManager),"baseVolume");
            fieldInfo.SetValue(MusicManager.instance, DreamQuestConfig.MusicVolume);
        }

        private static void LoadAudioSetting()
        {
            MusicManager.instance.musicPlayer.volume = DreamQuestConfig.MusicVolume;
            MusicManager.instance.effectsPlayer.volume = DreamQuestConfig.SoundVolume;
            mLogger.LogMessage("成功载入音频设置:" + MusicManager.instance.musicPlayer.volume + "||" + MusicManager.instance.effectsPlayer.volume);
        }
#pragma warning restore IDE0051
    }
}
