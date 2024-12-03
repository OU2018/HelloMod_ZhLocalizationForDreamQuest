using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace HelloMod
{
    public static class StringReplacePatcher
    {
        public static Dictionary<string, string> HighScoresDict = new Dictionary<string, string>();
        public static Dictionary<string, string> SDHVCreateDict = new Dictionary<string, string>();
        public static Dictionary<string, string> ChooseProfileDict = new Dictionary<string, string>();
        public static Dictionary<string, string> SDAVBuildTextPaneDict = new Dictionary<string, string>();
        public static Dictionary<string, string> CheatMenuDict = new Dictionary<string, string>();
        public static Dictionary<string, string> MonsterDisplayDict = new Dictionary<string, string>();

        public static Dictionary<string, string> InfoblockDict = new Dictionary<string, string>();

        public static Dictionary<string, string> LevelUpPaneDict = new Dictionary<string, string>();

        public static Dictionary<string, string> CreateEndDialogueDict = new Dictionary<string, string>();

        public static Dictionary<string, string> SDTVDict = new Dictionary<string, string>();
        /// <summary>
        /// 通用的 Transpiler 方法，用于替换字符串。
        /// </summary>
        /// <param name="instructions">原始指令集。</param>
        /// <param name="dict">字符串替换规则。</param>
        /// <returns>修改后的指令集。</returns>
        public static IEnumerable<CodeInstruction> HighScoresTranspiler(
            IEnumerable<CodeInstruction> instructions
            )
        {
            foreach (var instruction in instructions)
            {
                // 检查是否是加载字符串的指令
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string originalString)
                {
                    // 如果字典中存在替换规则，替换字符串
                    if (HighScoresDict.TryGetValue(originalString, out string newString))
                    {
                        instruction.operand = newString;
                    }
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> SDHVCreateTranspiler(
            IEnumerable<CodeInstruction> instructions
            )
        {
            foreach (var instruction in instructions)
            {
                // 检查是否是加载字符串的指令
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string originalString)
                {
                    // 如果字典中存在替换规则，替换字符串
                    if (SDHVCreateDict.TryGetValue(originalString, out string newString))
                    {
                        instruction.operand = newString;
                    }
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> ChooseProfileTranspiler(
            IEnumerable<CodeInstruction> instructions
            )
        {
            foreach (var instruction in instructions)
            {
                // 检查是否是加载字符串的指令
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string originalString)
                {
                    // 如果字典中存在替换规则，替换字符串
                    if (ChooseProfileDict.TryGetValue(originalString, out string newString))
                    {
                        instruction.operand = newString;
                    }
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> SDAVBuildTextPaneTranspiler(
            IEnumerable<CodeInstruction> instructions
            )
        {
            foreach (var instruction in instructions)
            {
                // 检查是否是加载字符串的指令
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string originalString)
                {
                    // 如果字典中存在替换规则，替换字符串
                    if (SDAVBuildTextPaneDict.TryGetValue(originalString, out string newString))
                    {
                        instruction.operand = newString;
                    }
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> CheatMenuTranspiler(
            IEnumerable<CodeInstruction> instructions
            )
        {
            int index = 0;
            var list = new List<CodeInstruction>(instructions);
            foreach (var instruction in instructions)
            {
                // 检查是否是加载字符串的指令
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string originalString)
                {
                    // 如果字典中存在替换规则，替换字符串
                    if (CheatMenuDict.TryGetValue(originalString, out string newString))
                    {
                        instruction.operand = newString;
                    }
                }
                //修改作弊菜单的高度
                if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float && (float)instruction.operand == 5f
                    && index + 1 < instructions.Count() &&
                    list[index + 1].opcode == OpCodes.Stloc_1)
                {
                    instruction.operand = 6f;
                }
                index++;
                yield return instruction;
            }
        }

        
            public static IEnumerable<CodeInstruction> MonsterDisplayTranspiler(
            IEnumerable<CodeInstruction> instructions
            )
        {
            foreach (var instruction in instructions)
            {
                // 检查是否是加载字符串的指令
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string originalString)
                {
                    // 如果字典中存在替换规则，替换字符串
                    if (MonsterDisplayDict.TryGetValue(originalString, out string newString))
                    {
                        instruction.operand = newString;
                    }
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> InfoblockTranspiler(
            IEnumerable<CodeInstruction> instructions
            )
        {
            foreach (var instruction in instructions)
            {
                // 检查是否是加载字符串的指令
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string originalString)
                {
                    // 如果字典中存在替换规则，替换字符串
                    if (InfoblockDict.TryGetValue(originalString, out string newString))
                    {
                        instruction.operand = newString;
                    }
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> LevelUpPaneTranspiler(
            IEnumerable<CodeInstruction> instructions
            )
        {
            foreach (var instruction in instructions)
            {
                // 检查是否是加载字符串的指令
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string originalString)
                {
                    // 如果字典中存在替换规则，替换字符串
                    if (LevelUpPaneDict.TryGetValue(originalString, out string newString))
                    {
                        instruction.operand = newString;
                    }
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> CreateEndDialogueTranspiler(
            IEnumerable<CodeInstruction> instructions
            )
        {
            foreach (var instruction in instructions)
            {
                // 检查是否是加载字符串的指令
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string originalString)
                {
                    // 如果字典中存在替换规则，替换字符串
                    if (CreateEndDialogueDict.TryGetValue(originalString, out string newString))
                    {
                        instruction.operand = newString;
                    }
                }

                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> SDTVTranspiler(
            IEnumerable<CodeInstruction> instructions
            )
        {
            foreach (var instruction in instructions)
            {
                // 检查是否是加载字符串的指令
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string originalString)
                {
                    // 如果字典中存在替换规则，替换字符串
                    if (SDTVDict.TryGetValue(originalString, out string newString))
                    {
                        instruction.operand = newString;
                    }
                }

                yield return instruction;
            }
        }
    }

}
