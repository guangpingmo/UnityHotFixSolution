using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HotFixSolution
{
    public class ABResources
    {
        public static T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            Debug.LogFormat("ABResources.Load assetPath:{0} type:{1}", assetPath, typeof(T).FullName);
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
            string abName = AssetPathToAssetBundleName(assetPath);
                AssetBundle assetBundle = ABManager.LoadAssetBundle(abName);
                if (assetBundle == null)
                    throw new Exception(string.Format("fail to Load AssetBundle For {0}!", assetPath));
                T asset = assetBundle.LoadAsset<T>(assetPath);
                return asset;
#endif
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
