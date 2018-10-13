using System;
using UnityEngine;
using HotFixSolution;
using XLua;

namespace LuaLibrary
{
    public class LuaMonoBehaviour : MonoBehaviour
    {
        public string luaScriptAssetPath;

        private LuaFunction awakeLuaFunc;
        private LuaFunction startLuaFunc;
        private LuaFunction updateLuaFunc;
        private LuaFunction onEnableLuaFunc;
        private LuaFunction onDisableLuaFunc;
        private LuaFunction onDestroyLuaFunc;

        private void Awake()
        {
            var luaObjs = LuaVM.Instance.RunScript(luaScriptAssetPath);
            LuaTable luaTable = luaObjs[0] as LuaTable;
            awakeLuaFunc = luaTable.GetInPath<LuaFunction>("Awake");
            startLuaFunc = luaTable.GetInPath<LuaFunction>("Start");
            updateLuaFunc = luaTable.GetInPath<LuaFunction>("Update");
            onEnableLuaFunc = luaTable.GetInPath<LuaFunction>("OnEnable");
            onDisableLuaFunc = luaTable.GetInPath<LuaFunction>("OnDisable");
            onDestroyLuaFunc = luaTable.GetInPath<LuaFunction>("OnDestroy");
            if(awakeLuaFunc != null) {
                awakeLuaFunc.Call(transform);
            }
        }

        private void Start()
        {
            if (startLuaFunc != null)
            {
                startLuaFunc.Call();
            }
        }

        private void Update()
        {
            if (updateLuaFunc != null)
            {
                updateLuaFunc.Call();
            }
        }

        private void OnEnable()
        {
            if (onEnableLuaFunc != null)
            {
                onEnableLuaFunc.Call();
            }
        }

        private void OnDisable()
        {
            if (onDisableLuaFunc != null)
            {
                onDisableLuaFunc.Call();
            }
        }

        private void OnDestroy()
        {
            if (onDestroyLuaFunc != null)
            {
                onDestroyLuaFunc.Call();
            }
        }
    }
}
