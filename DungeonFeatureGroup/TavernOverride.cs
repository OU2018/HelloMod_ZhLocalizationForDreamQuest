using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class TavernOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(Tavern).GetMethod("TextP1"),
                typeof(TavernOverride).GetMethod("TextP1"));
            modCenter.PatchTargetPostfix(
                typeof(Tavern).GetMethod("TextP2"),
                typeof(TavernOverride).GetMethod("TextP2"));
            modCenter.PatchTargetPostfix(
                typeof(Tavern).GetMethod("Name"),
                typeof(TavernOverride).GetMethod("Name"));

            modCenter.PatchTargetPrefix(
                typeof(Tavern).GetMethod("DisplayInMini"),
                typeof(TavernOverride).GetMethod("DisplayInMini"));
            modCenter.PatchTargetPrefix(
                typeof(Tavern).GetMethod("Enter"),
                typeof(TavernOverride).GetMethod("Enter"));
            modCenter.PatchTargetPrefix(
                typeof(Tavern).GetMethod("BuildSongObject"),
                typeof(TavernOverride).GetMethod("BuildSongObject"));
        }

        public static void TextP1(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_tavern_textp1");
        }

        public static void TextP2(ref string __result)
        {
            __result = HelloMod.Csv.GetTranslationByID(_dungeonFeaturePragraphTableName, "_tavern_textp2");
        }
        public static void Name(ref string __result)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Tolga's Tavern");
        }

        public static bool DisplayInMini(Tavern __instance)
        {
            DungeonPlayerPhysical physical = __instance.dungeon.player.physical;
            float x = physical.renderer.bounds.size.x;
            float y = physical.renderer.bounds.size.y;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), 32, Color.white);
            ShopDialogueClickableIcon shopDialogueClickableIcon = SDB.ClickableIcon(new Vector2(x * 0.5f, x * 0.5f), __instance.Portrait(), string.Empty);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(x * 0.6f, 0.35f), TR.GetStr(DungeonPhysicalOverride.TableKey, "Enter"), __instance.Enter);
            shopDialogueButton.FontSize(24);
            shopDialogueButton.ColliderMod(1.2f, 1.5f);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueClickableIcon }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", 0.2f);
            shopDialogueAligned.CenterTo(new Vector3(physical.transform.position.x, physical.transform.position.y - 0.35f * y, physical.transform.position.z + 0.2f));
            physical.miniDisplay = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }

        public static bool Enter( Tavern __instance)
        {
            __instance.EnteringShop();
            if (__instance.ShouldSmash())
            {
                __instance.SmashPage();
            }
            else
            {
                float num;
                if (GameManager.IsIPhone())
                {
                    num = __instance.dungeon.physical.WindowSize().x;
                }
                else
                {
                    num = __instance.dungeon.physical.WindowSize().x * 1.2f;
                }
                int num2 = 0;
                if (GameManager.IsIPhone())
                {
                    num2 = 8;
                }
                ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), (int)((float)48 + (float)num2 * 1.5f), Color.black);
                ShopDialogueObject shopDialogueObject;
                if (GameManager.IsIPhone())
                {
                    ShopDialogueText[] array = new ShopDialogueText[]
                    {
                    SDB.Text(num * 0.9f, __instance.TextP1(), 32 + num2, Color.black),
                    SDB.Text(num * 0.9f, __instance.TextP2(), 32 + num2, Color.black)
                    };
                    shopDialogueObject = SDB.Paged(array);
                    (shopDialogueObject as ShopDialoguePaged).SetSyncPoint(ShopDialogueCardinal.center);
                }
                else
                {
                    shopDialogueObject = SDB.Text(num, __instance.text, 32 + num2, Color.black);
                }
                SDB.Background(shopDialogueObject);
                ShopDialogueObject shopDialogueObject2 = __instance.BuildSongObjects();
                ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueObject }, "VP", 0.1f);
                if (shopDialogueObject2)
                {
                    shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueObject2 }, "VP", 0.4f);
                }
                SDB.Background(shopDialogueAligned, (Texture)Resources.Load("Textures/TextImageBorderless", typeof(Texture)));
                if (GameManager.IsIPhone())
                {
                    shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation());
                }
                else
                {
                    shopDialogueAligned.UpperCenterTo(__instance.dungeon.ShopLocation() + new Vector3((float)0, 0.5f, (float)0));
                }
                __instance.dungeon.activeShop = shopDialogueAligned;
                shopDialogueAligned.DoneBuilding();
            }
            return false;
        }
        public static bool BuildSongObject(ref ShopDialogueObject __result,SongBase s,Tavern __instance)
        {
            Vector2 vector = __instance.dungeon.physical.DefaultButtonSize() + new Vector2(0.5f, (float)0);
            if (GameManager.IsIPhone())
            {
                vector += new Vector2((float)0, 0.2f);
            }
            string btnName = s.name;
            btnName = TR.GetStr("SongName", btnName);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(vector, btnName, __instance.ChooseSongFactory(s));
            if (GameManager.IsIPhone())
            {
                shopDialogueButton.FontSize(36);
            }
            float num = __instance.dungeon.physical.WindowSize().x * 1.2f;
            int num2 = 28;
            if (GameManager.IsIPhone())
            {
                num2 = 40;
            }
            string songDescription = s.description;
            songDescription = HelloMod.Csv.GetTranslationByID("SongDescription", "_" + s.GetType().ToString());
            ShopDialogueText shopDialogueText = SDB.LeftText(num * 0.65f, songDescription, num2, Color.black);
            ShopDialogueObject shopDialogueObject = SDB.Align(new ShopDialogueObject[] { shopDialogueButton, shopDialogueText }, "HP", 0.6f);
            __result= SDB.Padded(shopDialogueObject, new Vector2(num * 0.65f + 0.6f + shopDialogueButton.mywidth, shopDialogueObject.myheight));
            return false;
        }

        public static string[] ALL_SONGS = new string[]
    {
        "BellicoseBallad", "SoothingSerenade", "KnavishNocturne", "MiserlyMinuet", "BlazingBeat", "CadenzaOfCelerity", "AnthemOfAmbush", "DreadfulDirge", "ArmingAria", "HeroicHymn",
        "HealingHarmony", "PiercingPaean", "RollickingRondo", "RhapsodyOfRecall", "VileVibrato", "StoutSonata", "ChoraleOfChaos", "ChantOfClarity", "EnlightenedElegy", "PugilistsPolka"
    };
    }
}
