using System;
using UnityEngine;
using XLua;

namespace HotFixSolution
{
    [LuaCallCSharp]
    public class LuaVM : LuaEnv
    {
        private static LuaVM _instance;
        public static LuaVM Instance {
            get {
                if(_instance == null) {
                    _instance = new LuaVM();
                }
                return _instance;
            }
        }

        public LuaVM()
        {
            this.AddLoader((ref string filepath) =>
            {
                //Debug.LogFormat("LuaVM Load filepath:{0}", filepath);
                TextAsset luaText = ABResources.Load<TextAsset>(filepath);
                //Debug.LogFormat("LuaVM Load file content:\n{0}", luaText.text);
                return luaText.bytes;
            });
        }

        public object[] RunScript(string scriptAssetpath)
        {
            TextAsset luaText = ABResources.Load<TextAsset>(scriptAssetpath);
            //Debug.LogFormat("LuaVM RunScript luaText:\n{0}", luaText.text);
            return DoString(luaText.bytes, scriptAssetpath);
        }
    }
}
