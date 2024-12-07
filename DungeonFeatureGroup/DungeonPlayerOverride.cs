using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SocialPlatforms;
using UnityEngine;
using System.Collections;
using UnityScript.Lang;

namespace HelloMod
{
    internal class DungeonPlayerOverride
    {
        public static bool CoolingDownAbilities(ref string __result,int x,DungeonPlayer __instance)
        {
            string text = string.Empty;

            // 假设 GetActions() 返回一个集合，这里使用 List<T> 举例
            var actions = __instance.GetActions(); // 获取动作列表

            foreach (var obj in actions)
            {
                // 强制转换为 DungeonAction 类型
                if (obj is DungeonAction dungeonAction)
                {
                    // 检查当前冷却时间
                    if (dungeonAction.currentCooldown != 0)
                    {
                        // 检查冷却时间为 1 或符合重置条件
                        if (dungeonAction.currentCooldown == 1 || __instance.profession.WillReset(dungeonAction, x))
                        {
                            if (text != string.Empty)
                            {
                                text += "\n";  // 如果不是第一个动作，换行
                            }
                            text += dungeonAction.ButtonName() + TR.GetStr(DungeonPhysicalOverride.TableKey, " is ready!");  // 添加动作名称
                        }
                    }
                }
            }
            __result = text;
            return false;
        }
    }
}
