using System.Collections.Generic;
using System;
using XLua;
using System.Reflection;
using System.Linq;

public static class XLuaGenConfig
{
    [LuaCallCSharp]
    public static IEnumerable<Type> LuaCallCSharp
    {
        get
        {
            var unityTypes = new List<Type> {
                typeof(UnityEngine.UI.Text),
                typeof(UnityEngine.UI.Button),
                typeof(UnityEngine.MonoBehaviour),
                typeof(UnityEngine.GameObject),
                typeof(UnityEngine.Vector2),
                typeof(UnityEngine.Vector3),
            };

            return unityTypes;
        }
    }

    ///自动把LuaCallCSharp涉及到的delegate加到CSharpCallLua列表，后续可以直接用lua函数做callback
    [CSharpCallLua]
    public static List<Type> CSharpCallLua
    {
        get
        {
            var delegate_types = new List<Type> {
                typeof(UnityEngine.Events.UnityAction),
            };
            return delegate_types;
        }
    }

    [DoNotGen]
    public static Dictionary<Type, List<string>> DontGenMember = new Dictionary<Type, List<string>>  {
        {typeof(UnityEngine.MonoBehaviour), new List<string>(){"runInEditMode"}},
    };

    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
        //new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
        new List<string>(){"UnityEngine.UI.Text", "OnRebuildRequested"},
    };
}