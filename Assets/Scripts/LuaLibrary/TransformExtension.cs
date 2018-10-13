using System;
using UnityEngine;
using XLua;

namespace LuaLibrary
{
    [LuaCallCSharp]
    public static class TransformExtension
    {
        public static Component GetChildComponent(this Transform rootTrans, string childName, Type compType)
        {
            Transform child = rootTrans.Find(childName);
            return child.GetComponent(compType);
        }
    }
}
