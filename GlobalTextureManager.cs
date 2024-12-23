using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelloMod
{
    public class GlobalTextureManager
    {
        private static Dictionary<string, Func<string, Texture>> m_tex_dict = new Dictionary<string, Func<string, Texture>>();
        public static bool RegisterModTexture(string modTextureName, Func<string, Texture> func)
        {
            if (m_tex_dict.ContainsKey(modTextureName)) { 
                HelloMod.mLogger.LogError("注册的 modTextureName = " +  modTextureName + "已经存在！请勿重复注册！");
                return false;
            }
            m_tex_dict.Add(modTextureName, func);
            return true;
        }
        public static Texture GetTextureByModTextureKey(string modTextureKey)
        {
            if (m_tex_dict.ContainsKey(modTextureKey)) { 
                return m_tex_dict[modTextureKey].Invoke(modTextureKey);
            }
            return null;
        }
    }
}
