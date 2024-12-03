using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HelloMod
{
    internal class ReplaceStringPatch
    {
        static Harmony harmony = null;

        public static void SetHarmony(Harmony value)
        {
            harmony = value;
        }

        public static void PatchMethodWithTranspiler_In_SDAVBuildTextPane(MethodInfo targetMethod, Dictionary<string, string> dict)
        {
            StringReplacePatcher.SDAVBuildTextPaneDict = dict;

            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
                StringReplacePatcher.SDAVBuildTextPaneTranspiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }

        public static void PatchMethodWithTranspiler_In_MonsterDisplay(MethodInfo targetMethod, Dictionary<string, string> dict)
        {
            StringReplacePatcher.MonsterDisplayDict = dict;

            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
                StringReplacePatcher.MonsterDisplayTranspiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }
        public static void PatchMethodWithTranspiler_In_CheatMenu(MethodInfo targetMethod, Dictionary<string, string> dict)
        {
            StringReplacePatcher.CheatMenuDict = dict;

            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
                StringReplacePatcher.CheatMenuTranspiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }
        public static void PatchMethodWithTranspiler_In_HighScores(MethodInfo targetMethod, Dictionary<string, string> dict)
        {
            StringReplacePatcher.HighScoresDict = dict;

            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
                StringReplacePatcher.HighScoresTranspiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }
        public static void PatchMethodWithTranspiler_In_SDHVCreateMethod(MethodInfo targetMethod, Dictionary<string, string> dict)
        {
            StringReplacePatcher.SDHVCreateDict = dict;

            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
                StringReplacePatcher.SDHVCreateTranspiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }
        public static void PatchMethodWithTranspiler_In_LevelUpPane(MethodInfo targetMethod, Dictionary<string, string> dict)
        {
            StringReplacePatcher.LevelUpPaneDict = dict;

            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
                StringReplacePatcher.LevelUpPaneTranspiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }
        public static void PatchMethodWithTranspiler_In_CreateEndDialogue(MethodInfo targetMethod, Dictionary<string, string> dict)
        {
            StringReplacePatcher.CreateEndDialogueDict = dict;

            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
                StringReplacePatcher.CreateEndDialogueTranspiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }
        public static void PatchMethodWithTranspiler_In_InfoblockMethod(MethodInfo targetMethod, Dictionary<string, string> dict)
        {
            StringReplacePatcher.InfoblockDict = dict;

            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
                StringReplacePatcher.InfoblockTranspiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }

        public static void PatchMethodWithTranspiler_In_ShopDialogueTalentViewerMethod(MethodInfo targetMethod, MethodInfo secondMethod, Dictionary<string, string> dict)
        {
            StringReplacePatcher.SDTVDict = dict;

            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
                StringReplacePatcher.SDTVTranspiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
            harmony.Patch(secondMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }

        public static void PatchMethodWithTranspiler_In_ChooseProfileMethod(MethodInfo targetMethod, MethodInfo secondMethod, MethodInfo thirdMethod, Dictionary<string, string> dict)
        {
            StringReplacePatcher.ChooseProfileDict = dict;

            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
                StringReplacePatcher.ChooseProfileTranspiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
            harmony.Patch(secondMethod, transpiler: new HarmonyMethod(transpilerMethod));
            harmony.Patch(thirdMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }

        public static void PatchMethodWithTranspiler_In_Textbox(MethodInfo targetMethod)
        {
            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
            FormatTextTranpiler.Transpiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }

        public static void PatchMethodWithTranspiler_In_CardPhysical(MethodInfo targetMethod)
        {
            // 使用 Transpiler 方法，传递替换规则
            var transpilerMethod = SymbolExtensions.GetMethodInfo(() =>
            FormatTextTranpiler.CardPhysical_Transpiler(null));

            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }
    }
}
