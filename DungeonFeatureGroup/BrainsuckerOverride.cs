using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod.DungeonFeatureGroup
{
    internal class BrainsuckerOverride : DungeonFeaturePatch
    {
        public override void Patch(Harmony harmony, HelloMod modCenter)
        {
            modCenter.PatchTargetPostfix(
                typeof(Brainsucker).GetMethod("Name"),
                typeof(BrainsuckerOverride).GetMethod("Name")
                );
            modCenter.PatchTargetPostfix(
                typeof(Brainsucker).GetMethod("InitialQueryText"),
                typeof(BrainsuckerOverride).GetMethod("InitialQueryText")
                );
            modCenter.PatchTargetPostfix(
                typeof(Brainsucker).GetMethod("LikeDislikeSign"),
                typeof(BrainsuckerOverride).GetMethod("LikeDislikeSign")
                );
            modCenter.PatchTargetPostfix(
                typeof(Brainsucker).GetMethod("BaseEatText"),
                typeof(BrainsuckerOverride).GetMethod("BaseEatText")
                );
            modCenter.PatchTargetPostfix(
                typeof(Brainsucker).GetMethod("SupplementalEatText"),
                typeof(BrainsuckerOverride).GetMethod("SupplementalEatText")
                );

            modCenter.PatchTargetPrefix(
                typeof(Brainsucker).GetMethod("DisplayInMini"),
                typeof(BrainsuckerOverride).GetMethod("DisplayInMini")
                );
            modCenter.PatchTargetPrefix(
                typeof(Brainsucker).GetMethod("Investigate"),
                typeof(BrainsuckerOverride).GetMethod("Investigate")
                );
            modCenter.PatchTargetPrefix(
                typeof(Brainsucker).GetMethod("BrainsuckResolve"),
                typeof(BrainsuckerOverride).GetMethod("BrainsuckResolve")
                );
            modCenter.PatchTargetPrefix(
                typeof(Brainsucker).GetMethod("ConstructDeckViewer"),
                typeof(BrainsuckerOverride).GetMethod("ConstructDeckViewer")
                );
        }
        public static void Name(ref string __result, Brainsucker __instance)
        {
            __result = TR.GetStr(_dungeonFeatureTableName, "Hillditi's Happy House");
        }

        public static void InitialQueryText(ref string __result, Brainsucker __instance)
        {
            __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_brainsucker_init").Replace(TR.PlaceHolder, __instance.dungeon.player.GetName());
        }

        public static void LikeDislikeSign(ref string __result, Brainsucker __instance)
        {
            __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_brainsucker_likedislike")
                .Replace("{like1}", __instance.LikeString(__instance.like1))
                .Replace("{like2}", __instance.LikeString(__instance.like2))
                .Replace("{dislike1}", __instance.LikeString(__instance.dislike1))
                ;
        }

        public static void BaseEatText(ref string __result,int mod)
        {
            string text = "ok";
            if (mod >= 3)
            {
                text = "phenomenal";
            }
            else if (mod == 2)
            {
                text = "delicious";
            }
            else if (mod == 1)
            {
                text = "tasty";
            }
            else if (mod == -1)
            {
                text = "gross";
            }
            else if (mod == -2)
            {
                text = "horrendous";
            }
            else if (mod == -3)
            {
                text = "inedible";
            }
            text = TR.GetStr(_dungeonFeatureTableName, text, "BRAIN");
            __result = HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_brainsucker_baseeat").Replace(TR.PlaceHolder, text);
        }

        public static void SupplementalEatText(ref string __result, Brainsucker __instance)
        {
            __result = (__instance.usesLeft != 0) ? __instance.LikeDislikeSign() : HelloMod.Csv.GetTranslationByID("DungeonFeatureParagraph", "_brainsucker_supplementalEat");
        }

        public static bool DisplayInMini(Brainsucker __instance)
        {
            DungeonPlayerPhysical physical = __instance.dungeon.player.physical;
            float x = physical.renderer.bounds.size.x;
            float y = physical.renderer.bounds.size.y;
            ShopDialogueDynamicText shopDialogueDynamicText = SDB.DynamicText(__instance.Name(), 32, Color.white);
            ShopDialogueClickableIcon shopDialogueClickableIcon = SDB.ClickableIcon(new Vector2(x * 0.5f, x * 0.5f), __instance.Portrait(), string.Empty);
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(new Vector2(x * 0.6f, 0.35f), TR.GetStr(_dungeonFeatureTableName, "Investigate"), __instance.Investigate);
            shopDialogueButton.FontSize(24);
            shopDialogueButton.ColliderMod(1.2f, 1.5f);
            ShopDialogueAligned shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueDynamicText, shopDialogueClickableIcon }, "VP", 0.1f);
            shopDialogueAligned = SDB.Align(new ShopDialogueObject[] { shopDialogueAligned, shopDialogueButton }, "VP", 0.2f);
            shopDialogueAligned.CenterTo(new Vector3(physical.transform.position.x, physical.transform.position.y - 0.35f * y, physical.transform.position.z + 0.2f));
            physical.miniDisplay = shopDialogueAligned;
            shopDialogueAligned.DoneBuilding();
            return false;
        }

        public static bool Investigate(Brainsucker __instance)
        {
            __instance.EnteringShop();
            if (__instance.ShouldSmash())
            {
                __instance.SmashPage();
            }
            else
            {
                BasicFeatureText basicFeatureText = new BasicFeatureText(__instance.dungeon, __instance.Name(), __instance.InitialQueryText() + " \\n " + __instance.LikeDislikeSign(),
                    TR.GetStr(_dungeonFeatureTableName, "Ok ({value} Left)").Replace(TR.PlaceHolder, __instance.usesLeft.ToString())
                    ,
                    __instance.Brainsuck, true);
                if (GameManager.IsIPhone())
                {
                    basicFeatureText.PagedText(new string[]
                    {
                    __instance.InitialQueryText(),
                    __instance.LikeDislikeSign()
                    });
                }
                basicFeatureText.Build();
            }
            return false;
        }

        public static bool BrainsuckResolve(Card c, Brainsucker __instance)
        {
            int num = c.tier;
            int num2 = -1;
            if (num == 0)
            {
                num2 = -3;
            }
            else
            {
                int i = 0;
                BrainsuckerPreference[] array = new BrainsuckerPreference[] { __instance.like1, __instance.like2, __instance.dislike1 };
                int length = array.Length;
                while (i < length)
                {
                    num2 += array[i].TierMod(c);
                    i++;
                }
            }
            num += num2;
            string text = string.Empty;
            if (num < 1)
            {
                text = "Curse";
            }
            else
            {
                int num3 = 1;
                if (num > 10)
                {
                    num = 10;
                }
                if (num2 >= 2)
                {
                    num3 = 5;
                }
                text = CardFinder.ChooseCardReward(__instance.dungeon.player.profession.ClassBiases(), num, __instance.dungeon, num3);
            }
            __instance.dungeon.player.AddCard(text);
            if (__instance.usesLeft > 0)
            {
                BasicFeatureText basicFeatureText = new BasicFeatureText(__instance.dungeon, __instance.Name(), __instance.EatText(num2),
                    TR.GetStr(_dungeonFeatureTableName, "Again! ({value} Left)").Replace(TR.PlaceHolder, __instance.usesLeft.ToString())
                    ,
                    __instance.Brainsuck, true);
                basicFeatureText.card = text;
                basicFeatureText.cardPadding = 0.2f;
                basicFeatureText.buttonPadding = 0.2f;
                if (GameManager.IsIPhone())
                {
                    basicFeatureText.PagedText(__instance.EatTextPaged(num2));
                }
                basicFeatureText.Build();
            }
            else
            {
                BasicFeatureText basicFeatureText = new BasicFeatureText(__instance.dungeon, __instance.Name(), __instance.EatText(num2), TR.GetStr(_dungeonFeatureTableName, "Huh"), __instance.Finished, false);
                basicFeatureText.card = text;
                basicFeatureText.cardPadding = 0.2f;
                basicFeatureText.buttonPadding = 0.2f;
                if (GameManager.IsIPhone())
                {
                    basicFeatureText.PagedText(__instance.EatTextPaged(num2));
                }
                basicFeatureText.Build();
            }
            return false;  
        }

        public static bool ConstructDeckViewer(Brainsucker __instance)
        {
            __instance.dungeon.activeDungeonFeature = __instance;
            __instance.dungeon.DeckViewer(TR.GetStr(_dungeonFeatureTableName, "Choose a card to give Dr. Hillditi"), DeckButtons(__instance),__instance.CancelDeckViewer);
            return false;
        }
        public static List<ShopDialogueButton> DeckButtons(Brainsucker __instance)
        {
            ShopDialogueButton shopDialogueButton = SDB.BasicButton(__instance.dungeon.physical.DefaultButtonSize(), TR.GetStr(DungeonPhysicalOverride.TableKey, "Confirm"), __instance.ConfirmPressed, __instance.AllowDone);
            return new List<ShopDialogueButton> { shopDialogueButton };
        }
    }
}
