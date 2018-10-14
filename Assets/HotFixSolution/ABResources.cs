using System;
using UnityEngine;
using XLua;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HotFixSolution
{
    [LuaCallCSharp]
    public class ABResources
    {
        public static bool EmulateLoadAssetBundleInEditor
        {
            get
            {
                bool emulateLoadABInEditorSetting = PlayerPrefs.GetInt("EmulateLoadAssetBundleInEditor", 0) == 1;
                return emulateLoadABInEditorSetting;
            }
            set
            {
                PlayerPrefs.SetInt("EmulateLoadAssetBundleInEditor", value ? 1 : 0);
            }
        }

        public static T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            UnityEngine.Object obj = Load(assetPath, typeof(T));
            return obj as T;
        }

        public static UnityEngine.Object Load(string assetPath, Type type)
        {
            Debug.LogFormat("ABResources.Load assetPath:{0} type:{1}", assetPath, type.FullName);
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                if (EmulateLoadAssetBundleInEditor)
                {
                    return LoadAssetFromAssetBundle(assetPath, type);
                }
                else
                {
                    return AssetDatabase.LoadAssetAtPath(assetPath, type);
                }
#else
                return null;
#endif
            }
            else
            {
                return LoadAssetFromAssetBundle(assetPath, type);
            }
        }

        private static UnityEngine.Object LoadAssetFromAssetBundle(string assetPath, Type type)
        {
            string abName = AssetPathToAssetBundleName(assetPath);
            AssetBundle assetBundle = ABManager.LoadAssetBundle(abName);
            if (assetBundle == null)
                throw new Exception(string.Format("fail to Load AssetBundle For {0}!", assetPath));
            return assetBundle.LoadAsset(assetPath, type);
        }

        private static T LoadAssetFromAssetBundle<T>(string assetPath) where T : UnityEngine.Object
        {
            UnityEngine.Object obj = LoadAssetFromAssetBundle(assetPath, typeof(T));
            return obj as T;
        }

        public static string AssetPathToAssetBundleName(string assetPath)
        {
            return assetPath.ToLower();
        }

        public static void UnloadAllAsset()
        {
            ABManager.UnloadAllAssetBundles();
            Resources.UnloadUnusedAssets();
        }
    }
}
