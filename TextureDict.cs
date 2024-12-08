using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HelloMod {
    public class TextureDict
    {
        public Texture[] textures;

        public Dictionary<string, Texture> textureDict = new Dictionary<string, Texture> ();

        private GameObject root;
        public TextureDict(GameObject texRoot)
        {
            this.root = texRoot;
            HelloMod.mLogger.LogMessage("Save Root " + texRoot.name);
        }
        public void InitTextureDict()
        {
            textureDict.Clear();
            for (int i = 0; i < textures.Length; i++)
            {
                textureDict.Add(textures[i].name, textures[i]);
            }
        }

        public Texture FindTextureByKey(string key)
        {
            if (textureDict.ContainsKey(key))
            {
                return textureDict[key];
            }
            return null;
        }

        public void GenerateArr()
        {
            HelloMod.mLogger.LogMessage("root childCount:" + root.transform.childCount);
            textures = new Texture[root.transform.childCount];
            textureDict.Clear();
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Transform child = root.transform.GetChild(i);
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                textures[i] = spriteRenderer.sprite.texture;
                textureDict.Add(child.gameObject.name, textures[i]);
            }
        }
    }
}

