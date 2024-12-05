using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class MushroomPatchOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(MushroomPatch).GetMethod("Name"),
                typeof(MushroomPatchOverride).GetMethod("Name")
                );
            modCenter.PatchTargetPostfix(
                typeof(MushroomPatch).GetMethod("InitialQueryText"),
                typeof(MushroomPatchOverride).GetMethod("InitialQueryText")
                );
            modCenter.PatchTargetPostfix(
                typeof(MushroomPatch).GetMethod("StartQuestText"),
                typeof(MushroomPatchOverride).GetMethod("StartQuestText")
                );
            modCenter.PatchTargetPostfix(
                typeof(MushroomPatch).GetMethod("UnfinishedQuestText"),
                typeof(MushroomPatchOverride).GetMethod("UnfinishedQuestText")
                );
            modCenter.PatchTargetPostfix(
                typeof(MushroomPatch).GetMethod("CompleteQuestText"),
                typeof(MushroomPatchOverride).GetMethod("CompleteQuestText")
                );

            modCenter.PatchTargetPrefix(
                typeof(MushroomPatch).GetMethod("DisplayInMini"),
                typeof(MushroomPatchOverride).GetMethod("DisplayInMini")
                );
            modCenter.PatchTargetPrefix(
                typeof(MushroomPatch).GetMethod("CompleteQuestQuery"),
                typeof(MushroomPatchOverride).GetMethod("CompleteQuestQuery")
                );
            modCenter.PatchTargetPrefix(
                typeof(MushroomPatch).GetMethod("InitialQuery"),
                typeof(MushroomPatchOverride).GetMethod("InitialQuery")
                );
            modCenter.PatchTargetPrefix(
                typeof(MushroomPatch).GetMethod("ConstructDeckViewer"),
                typeof(MushroomPatchOverride).GetMethod("ConstructDeckViewer")
                );
            modCenter.PatchTargetPrefix(
                typeof(MushroomPatch).GetMethod("CompleteQuest"),
                typeof(MushroomPatchOverride).GetMethod("CompleteQuest")
                );
        }

        public static void Name(ref string __result,MushroomPatch __instance)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Mushroom Patch");
        }

        public static void InitialQueryText(ref string __result, MushroomPatch __instance)
        {
            __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_mushroompatch_init");
        }

        public static void StartQuestText(ref string __result, MushroomPatch __instance)
        {
            __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_mushroompatch_startquest");
        }

        public static void UnfinishedQuestText(ref string __result, MushroomPatch __instance)
        {

            __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_mushroompatch_unfinished").Replace(TR.PlaceHolder, Utility.NameFromNum(__instance.mushroomsLeft).ToLower());
            if (!DreamQuestConfig.IsEn)
            {
                __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_mushroompatch_unfinished").Replace(TR.PlaceHolder, __instance.mushroomsLeft.ToString());
            }
        }

        public static void CompleteQuestText(ref string __result, MushroomPatch __instance)
        {
            __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_mushroompatch_complete");
        }

        public static bool DisplayInMini(MushroomPatch __instance)
        {
            DungeonPlayerPhysical physical = __instance.dungeon.player.physical;
            float x = physical.renderer.bounds.size.x;
            float y = physical.renderer.bounds.size.y;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), 32, Color.white);
            ShopDialogueClickableIcon shopDialogueClickableIcon = SDB.ClickableIcon(new Vector2(x * 0.5f, x * 0.5f), __instance.Portrait(), string.Empty);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(x * 0.6f, 0.35f), TR.GetStr(DungeonPhysicalOverride.TableKey, "Stomp"), __instance.Investigate);
            shopDialogueButton.FontSize(24);
            shopDialogueButton.ColliderMod(1.2f, 1.5f);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueClickableIcon }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", 0.2f);
            shopDialogueAligned.CenterTo(new Vector3(physical.transform.position.x, physical.transform.position.y - 0.35f * y, physical.transform.position.z + 0.2f));
            physical.miniDisplay = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }
        public static bool CompleteQuestQuery(MushroomPatch __instance)
        {
            BasicFeatureText basicFeatureText = new BasicFeatureText(__instance.dungeon, __instance.Name(), __instance.CompleteQuestText(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Expand Your Mind"), __instance.ResolveCopy, false);
            basicFeatureText.buttonSize *= 1.2f;
            basicFeatureText.Build();
            return false;
        }

        public static bool CompleteQuest(string c, MushroomPatch __instance)
        {
            __instance.dungeon.player.AddCard(c);
            __instance.dungeon.player.stats.FinishMushrooms();
            __instance.dungeon.player.physical.WipeMiniDisplayNow();
            __instance.dungeon.WindowFinished();
            __instance.Destroy();
            return false;
        }

        public static bool InitialQuery(MushroomPatch __instance)
        {
            BasicFeatureText basicFeatureText = new BasicFeatureText(__instance.dungeon, __instance.Name(), __instance.InitialQueryText(), HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_ok_i_guess"), __instance.StartQuest, true);
            basicFeatureText.Build();
            return false;
        }
        public static bool ConstructDeckViewer(MushroomPatch __instance)
        {
            __instance.dungeon.activeDungeonFeature = __instance;
            __instance.dungeon.DeckViewer(TR.GetStr(DungeonPhysicalOverride.TableKey, "Choose a card to copy"), DeckButtons(__instance), __instance.CancelDeckViewer);
            return false;
        }

        public static List<ShopDialogueButton> DeckButtons(MushroomPatch __instance)
        {
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Confirm"), __instance.ConfirmPressed, __instance.AllowDone);
            return new List<ShopDialogueButton> { shopDialogueButton };
        }
    }
}
