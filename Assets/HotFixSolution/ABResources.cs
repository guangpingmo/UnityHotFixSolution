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
            Debug.LogFormat("ABResources.Load assetPath:{0} type:{1}", assetPath, typeof(T).FullName);
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                if(EmulateLoadAssetBundleInEditor) {
                    return LoadAssetFromAssetBundle<T>(assetPath);
                } else {
                    return AssetDatabase.LoadAssetAtPath<T>(assetPath);
                }
#else
                return null;
#endif
            } else {
                return LoadAssetFromAssetBundle<T>(assetPath);
            }
        }

        private static T LoadAssetFromAssetBundle<T>(string assetPath) where T : UnityEngine.Object
        {
            string abName = AssetPathToAssetBundleName(assetPath);
            AssetBundle assetBundle = ABManager.LoadAssetBundle(abName);
            if (assetBundle == null)
                throw new Exception(string.Format("fail to Load AssetBundle For {0}!", assetPath));
            T asset = assetBundle.LoadAsset<T>(assetPath);
            return asset;
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
