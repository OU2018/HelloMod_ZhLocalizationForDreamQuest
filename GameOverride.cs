using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityScript.Lang;

namespace HelloMod
{
    internal class GameOverride
    {
        //因为原本的 Type.GetType无法访问到新的卡牌类，所以重写该方法
        public static bool StaticCreateMomentaryCard(ref Card __result, string name, Game __instance)
        {
            string tex_Name = string.Empty;
            if (name.StartsWith("Mod_"))
            {
                Type type = Type.GetType("HelloMod.ModFeature." + name + ",HelloMod");
                if (type == null)
                {
                    Debug.Log(name);
                    HelloMod.mLogger.LogMessage("Not Found Type ==>" + name);
                }
                HelloMod.mLogger.LogMessage("Found Moded Type ==>" + name);
                Card card = Activator.CreateInstance(type) as Card;
                card.internalName = name;
                card.Initialize();
                __result = card;
                return false;
            }
            return true;
        }

        public static bool CreateCardAtTimeVirtualSplit(ref Card __result, string name, Player player, Vector3 location, Quaternion rotation, bool now, bool isvirtual, Card splitCard, CardPhysical myPhysical, Game __instance)
        {
            GameObject gameObject = null;
            bool flag = false;
            bool flag2 = false;
            CardPhysical cardPhysical = null;
            if (name.StartsWith("Mod_"))
            {
                if (__instance.IsPhysical() && !isvirtual)
                {
                    if (__instance.currentDungeon != null && !__instance.isDungeon)
                    {
                        if (__instance.IsPhysical() && myPhysical)
                        {
                            gameObject = myPhysical.gameObject;
                            flag2 = true;
                            cardPhysical = myPhysical;
                        }
                        else if (player.Equals(__instance.me))
                        {
                            cardPhysical = __instance.currentDungeon.physical.GetPlayerDeckObject(name);
                            if (cardPhysical)
                            {
                                gameObject = cardPhysical.gameObject;
                                flag2 = true;
                            }
                        }
                        if (!flag2)
                        {
                            gameObject = __instance.currentDungeon.physical.GetCardObject();
                        }
                        if (!now)
                        {
                            __instance.physical.SafeBuild(gameObject, location, rotation);
                        }
                        else
                        {
                            gameObject.transform.position = location;
                            gameObject.transform.rotation = rotation;
                            ((CardPhysical)gameObject.GetComponent(typeof(CardPhysical))).turnedOn = true;
                        }
                    }
                    else if (!now)
                    {
                        gameObject = __instance.physical.SafeInstantiate(__instance.cardCreated, location, rotation);
                    }
                    else
                    {
                        gameObject = (GameObject)UnityEngine.Object.Instantiate(__instance.cardCreated, location, rotation);
                        ((CardPhysical)gameObject.GetComponent(typeof(CardPhysical))).turnedOn = true;
                    }
                }
                Type type = Type.GetType("HelloMod.ModFeature." + name + ",HelloMod");
                if (type == null)
                {
                    __instance.print("Failed to find " + name);
                }
                Card card = Activator.CreateInstance(type) as Card;
                if (__instance.IsPhysical() && !isvirtual && !flag2)
                {
                    card.go = gameObject;
                }
                card.game = __instance;
                card.cardName = name;
                card.objectType = ObjectTypes.CARD;
                card.player = player;
                if (splitCard != null)
                {
                    card.splitCard = splitCard;
                }
                card.internalName = name;
                card.Initialize();
                if (flag2 && __instance.IsPhysical() && !isvirtual)
                {
                    card.go = gameObject;
                    card.physical = cardPhysical;
                    cardPhysical.card = card;
                    cardPhysical.CIN.card = card;
                }
                else if (__instance.IsPhysical() && !isvirtual && player.Equals(__instance.me) && __instance.currentDungeon != null && !__instance.isDungeon)
                {
                    __instance.currentDungeon.physical.AddPlayerDeckObject(card.physical);
                }
                if (flag)
                {
                    card.ChampionToHero();
                }
                if (__instance.user != null)
                {
                    __instance.user.ModifyCard(card);
                }
                __result = card;
                return false;
            }
            return true;
        }

        public static bool PopulateFromString(string s,Game __instance)
        {
            string[] array = s.Split(new char[] { "|"[0] });
            Queue<string> queue = new Queue<string>(array);
            string text = queue.Dequeue();
            int num = int.Parse(text);
            Game.SetSeed(num);
            int num2 = int.Parse(queue.Dequeue());
            for (int i = 0; i < num2; i++)
            {
                text = queue.Dequeue();
                if (__instance.AllObjects.Count <= i)
                {
                    if (text == "null")
                    {
                        __instance.AllObjects.Add(null);
                        __instance.unusedIndices.Push(i);
                    }
                    else
                    {
                        HelloMod.mLogger.LogMessage("Try Load Type:" + text);
                        Type type = Type.GetType(text + ",Assembly-UnityScript");//从原生的程序集中寻找
                        if (text.Contains('.')){//更加通用的写法
                            string[] split = text.Split(new char[] { '.' });
                            type = Type.GetType(text + "," + split[0]);//分解出程序集名称
                            HelloMod.mLogger.LogMessage("Success Load Mod Type:" + type);
                        }
                        //if (text.Contains(".ModFeature."))//仅适用与我的写法
                        //{
                        //    type = Type.GetType(text + ",HelloMod");
                        //    HelloMod.mLogger.LogMessage("Success Load Mod Type:" + type);
                        //}
                        if (type == null)
                        {
                            __instance.print("Failed to find " + text);
                        }
                        VirtualObject virtualObject = Activator.CreateInstance(type) as VirtualObject;
                        virtualObject.game = __instance;
                        virtualObject.index = i;
                        __instance.AllObjects.Add(virtualObject);
                    }
                }
            }
            return false;
        }
    }
}
