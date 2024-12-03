using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace HelloMod
{
    public static class FormatTextTranpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            return ReplaceIfCondition(instructionsList);
        }

        public static IEnumerable<CodeInstruction> CardPhysical_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            return ReplaceIfCondition(instructionsList);
        }

        // 判断是否为 `flag7 = flag;` 赋值语句
        private static bool IsFlagAssignment(CodeInstruction instruction, List<CodeInstruction> instructionsList, int index)
        {
            // 检查 ldloc.0（flag）加载到操作栈，然后 stloc.s V_32(32) 将其存储到 flag7
            if (instruction.opcode == OpCodes.Ldloc_0 &&
                instructionsList[index + 1].opcode == OpCodes.Stloc_S &&
                instructionsList[index + 1].operand.ToString() == "V_32")
            {
                return true;
            }
            return false;
        }

        private static MethodInfo textPropertySetter = null;
        public static List<CodeInstruction> ReplaceIfCondition(List<CodeInstruction> instructionsList)
        {
            CodeInstruction loadArr2_0 = null;
            CodeInstruction loadArr2_1 = null;
            CodeInstruction loadArr2_2 = null;
            // 找到 textMesh.text = text + " " + array2[i]; 所在的指令位置
            int index = FindTextMeshTextAssignmentIndex(instructionsList);
            if (index != -1)
            {
                int startIndex = index;

                var loadTextMesh = instructionsList[index];
                var setText = instructionsList[index + 8];
                var addStr = instructionsList[index + 3];
                var loadText = instructionsList[index + 1];//24
                var brLabel = instructionsList[index + 9];

                loadArr2_0 = instructionsList[index + 4];
                loadArr2_1 = instructionsList[index + 5];
                loadArr2_2 = instructionsList[index + 6];

                // 获取 TextMesh 类的 'text' 属性的 setter 方法
                textPropertySetter = AccessTools.PropertySetter(typeof(UnityEngine.TextMesh), "text");

                var skipLabel = instructionsList[index - 1];
                instructionsList.RemoveRange(index, 9);
                // 定义跳转标签
                var skipSpaceLabel = new Label();
                var skipElseLabel = new Label();
                // 插入新的条件判断逻辑（如果是汉字字符则不加空格，否则加空格）
                instructionsList.Insert(startIndex++, loadArr2_0);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_1);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_2);  // 加载 array2[i]

                MethodInfo method = AccessTools.Method(typeof(FormatTextTranpiler), "IsChineseCharacter");
                MethodInfo log = AccessTools.Method(typeof(FormatTextTranpiler), "LogMessage");

                // 调用 IsChineseCharacter 方法
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Call, method));  // 调用 IsChineseCharacter 方法
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Brtrue_S, skipSpaceLabel));  // 如果是汉字字符，跳过空格拼接


                // 如果是汉字字符的处理
                instructionsList.Insert(startIndex++, loadTextMesh);  // 加载 TextMesh
                instructionsList.Insert(startIndex++, loadText);  // 加载 text
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Ldstr, " "));  // 加载空格
                instructionsList.Insert(startIndex++, addStr); // 拼接 text + " "
                instructionsList.Insert(startIndex++, loadArr2_0);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_1);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_2);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, addStr); // 拼接 text + " " + array2[i]
                instructionsList.Insert(startIndex++, setText);  // 设置 textMesh.text

                /*instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Ldstr, "First Path<<<"));  // 加载 text
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Call, log));  // 调用 IsChineseCharacter 方法*/

                instructionsList.Insert(startIndex++, brLabel);  // 标记跳转点
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Nop));  // 标记跳转点
                instructionsList[startIndex - 1].labels.Add(skipSpaceLabel); // 跳过空格拼接的部分

                // 如果不是汉字字符的处理
                instructionsList.Insert(startIndex++, loadTextMesh);  // 加载 TextMesh
                instructionsList.Insert(startIndex++, loadText);  // 加载 text
                instructionsList.Insert(startIndex++, loadArr2_0);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_1);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_2);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, addStr);  // 拼接 text + array2[i]
                instructionsList.Insert(startIndex++, setText);  // 设置 textMesh.text

                /*instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Ldstr, "Second Path>>>"));  // 加载 text
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Call, log));  // 调用 IsChineseCharacter 方法*/
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Nop));  // 标记跳转点
                instructionsList[startIndex - 1].labels.Add(skipElseLabel); // 跳过空格拼接的部分
            }
            /*int second_index = FindTextMeshTextAignIndex(instructionsList);//搞错了，这个是图标里面的逻辑，根本不会执行到
            if (second_index != -1)
            {
                int startIndex = second_index;

                var endLabel = instructionsList[startIndex - 1].operand;
                //instructionsList.RemoveRange(index, 8);
                // 定义跳转标签
                var skipSpaceLabel = new Label();
                // 插入新的条件判断逻辑（如果是汉字字符则不加空格，否则加空格）
                instructionsList.Insert(startIndex++, loadArr2_0);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_1);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_2);  // 加载 array2[i]

                MethodInfo method = AccessTools.Method(typeof(FormatTextTranpiler), "IsChineseCharacter");
                // 调用 IsChineseCharacter 方法
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Call, method));  // 调用 IsChineseCharacter 方法
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Brtrue_S, endLabel));  // 如果是汉字字符，不需要再计算空格长度
                // 如果不是汉字字符才执行原来的操作
                //原本的操作
            }*/

            return instructionsList;
        }

        public static List<CodeInstruction> CardPhysical_ReplaceIfCondition(List<CodeInstruction> instructionsList)
        {
            CodeInstruction loadArr2_0 = null;
            CodeInstruction loadArr2_1 = null;
            CodeInstruction loadArr2_2 = null;
            // 找到 textMesh.text = text + " " + array2[i]; 所在的指令位置
            int index = FindTextMeshTextAssignmentIndex(instructionsList);
            if (index != -1)
            {
                int startIndex = index;

                var loadTextMesh = instructionsList[index];
                var setText = instructionsList[index + 8];
                var addStr = instructionsList[index + 3];
                var loadText = instructionsList[index + 1];//24
                var brLabel = instructionsList[index + 9];

                loadArr2_0 = instructionsList[index + 4];
                loadArr2_1 = instructionsList[index + 5];
                loadArr2_2 = instructionsList[index + 6];

                // 获取 TextMesh 类的 'text' 属性的 setter 方法
                textPropertySetter = AccessTools.PropertySetter(typeof(UnityEngine.TextMesh), "text");

                var skipLabel = instructionsList[index - 1];
                instructionsList.RemoveRange(index, 9);
                // 定义跳转标签
                var skipSpaceLabel = new Label();
                var skipElseLabel = new Label();
                // 插入新的条件判断逻辑（如果是汉字字符则不加空格，否则加空格）
                instructionsList.Insert(startIndex++, loadArr2_0);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_1);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_2);  // 加载 array2[i]

                MethodInfo method = AccessTools.Method(typeof(FormatTextTranpiler), "IsChineseCharacter");
                MethodInfo log = AccessTools.Method(typeof(FormatTextTranpiler), "LogMessage");

                // 调用 IsChineseCharacter 方法
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Call, method));  // 调用 IsChineseCharacter 方法
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Brtrue_S, skipSpaceLabel));  // 如果是汉字字符，跳过空格拼接
                // 如果是汉字字符的处理
                instructionsList.Insert(startIndex++, loadTextMesh);  // 加载 TextMesh
                instructionsList.Insert(startIndex++, loadText);  // 加载 text
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Ldstr, " "));  // 加载空格
                instructionsList.Insert(startIndex++, addStr); // 拼接 text + " "
                instructionsList.Insert(startIndex++, loadArr2_0);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_1);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_2);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, addStr); // 拼接 text + " " + array2[i]
                instructionsList.Insert(startIndex++, setText);  // 设置 textMesh.text

                instructionsList.Insert(startIndex++, brLabel);  // 标记跳转点
                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Nop));  // 标记跳转点
                instructionsList[startIndex - 1].labels.Add(skipSpaceLabel); // 跳过空格拼接的部分

                // 如果不是汉字字符的处理
                instructionsList.Insert(startIndex++, loadTextMesh);  // 加载 TextMesh
                instructionsList.Insert(startIndex++, loadText);  // 加载 text
                instructionsList.Insert(startIndex++, loadArr2_0);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_1);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, loadArr2_2);  // 加载 array2[i]
                instructionsList.Insert(startIndex++, addStr);  // 拼接 text + array2[i]
                instructionsList.Insert(startIndex++, setText);  // 设置 textMesh.text

                instructionsList.Insert(startIndex++, new CodeInstruction(OpCodes.Nop));  // 标记跳转点
                instructionsList[startIndex - 1].labels.Add(skipElseLabel); // 跳过空格拼接的部分
            }
            return instructionsList;
        }

        private static int FindTextMeshTextAssignmentIndex(List<CodeInstruction> instructionsList)
        {
            // 检查 instructionsList 是否为 null 或空
            if (instructionsList == null || instructionsList.Count == 0)
            {
                return -1;  // 如果为空，返回 -1 或其他合适的默认值
            }
            // 遍历指令列表，确保有足够的指令来匹配
            for (int i = 0; i < instructionsList.Count - 9; i++)
            {
                if (!(instructionsList[i + 2].opcode == OpCodes.Ldstr && (string)instructionsList[i + 2].operand == " "))
                {
                    continue;
                }
                /*HelloMod.mLogger.LogInfo("LineInfo:");
                for (int j = 0; j < 9; j++)
                {
                    if (instructionsList[i + j].operand == null)
                    {
                        HelloMod.mLogger.LogInfo("opcode:[" + j + "]" + instructionsList[i + j].opcode);
                        continue;
                    }
                    HelloMod.mLogger.LogInfo("opcode:" + instructionsList[i + j].opcode + "||operand:" + instructionsList[i + j].operand.ToString());
                }*/
                // 确保我们匹配正确的局部变量类型（string）
                if (instructionsList[i].opcode == OpCodes.Ldloc_S &&  // ldloc.s V_4 (text)
                    instructionsList[i + 1].opcode == OpCodes.Ldloc_S &&  // ldloc.s V_24 (array2[i])
                    instructionsList[i + 2].opcode == OpCodes.Ldstr &&  // ldstr ""
                    instructionsList[i + 3].opcode == OpCodes.Call && ((MethodInfo)instructionsList[i + 3].operand).Name == "op_Addition" &&  // call op_Addition(string,string)
                    instructionsList[i + 4].opcode == OpCodes.Ldloc_S &&  // ldloc.s V_45
                    instructionsList[i + 5].opcode == OpCodes.Ldloc_S &&  // ldloc.s V_44
                    instructionsList[i + 6].opcode == OpCodes.Ldelem_Ref &&  // ldelem.ref
                    instructionsList[i + 7].opcode == OpCodes.Call && ((MethodInfo)instructionsList[i + 7].operand).Name == "op_Addition" &&  // call op_Addition(string,string)
                    instructionsList[i + 8].opcode == OpCodes.Callvirt && ((MethodInfo)instructionsList[i + 8].operand).Name == "set_text" &&  // callvirt set_text(string)
                    instructionsList[i + 9].opcode == OpCodes.Br)  // br 指令
                {
                    // 如果找到匹配的代码段，返回起始位置的索引
                    HelloMod.mLogger.LogMessage("Find index:" + i);
                    return i;
                }
            }

            // 如果没有找到匹配的代码，返回 -1
            return -1;
        }

        // 判断字符是否是汉字
        public static bool IsChineseCharacter(string str)
        {
            if (str == null || str.Length == 0)
                return false;
            return str[0] >= 0x4e00 && str[0] <= 0x9fff; // 汉字的 Unicode 范围
            //return str >= 0x4e00 && str <= 0x9fff; // 汉字的 Unicode 范围
        }

        public static void LogMessage(string msg) {
            HelloMod.mLogger.LogMessage(msg);
        }

        //-----------------------------------------修复偏移不正常的问题
        private static int FindTextMeshTextAignIndex(List<CodeInstruction> instructionsList)
        {
            // 检查 instructionsList 是否为 null 或空
            if (instructionsList == null || instructionsList.Count == 0)
            {
                return -1;  // 如果为空，返回 -1 或其他合适的默认值
            }
            // 遍历指令列表，确保有足够的指令来匹配
            for (int i = 0; i < instructionsList.Count; i++)
            {
                if (!(instructionsList[i].opcode == OpCodes.Ldloc_S))
                    continue;
                if(instructionsList[i].operand == null)
                    continue;
                if (!(instructionsList[i].operand is LocalBuilder local && local.LocalIndex == 30))
                {
                    continue;
                }
                // 确保我们匹配正确的局部变量类型（string）
                if (
                    instructionsList[i].opcode == OpCodes.Ldloc_S && 
                    instructionsList[i + 1].opcode == OpCodes.Ldloc_S && instructionsList[i + 1].operand is LocalBuilder slocal && slocal.LocalIndex == 12 &&
                    instructionsList[i + 3].opcode == OpCodes.Stloc_S &&
                    instructionsList[i + 4].opcode == OpCodes.Ldloc_S &&
                    instructionsList[i + 5].opcode == OpCodes.Ldloc_S &&
                    instructionsList[i + 7].opcode == OpCodes.Stloc_S
                    )  // br 指令
                {
                    // 如果找到匹配的代码段，返回起始位置的索引
                    HelloMod.mLogger.LogMessage("Find index Text Align:" + i);
                    return i;
                }
            }

            // 如果没有找到匹配的代码，返回 -1
            return -1;
        }

    }

}
