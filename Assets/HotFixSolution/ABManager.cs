using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using XLua;

namespace HotFixSolution
{
    [LuaCallCSharp]
    public static class ABManager
    {
        public static Dictionary<String, ABItem> configs = new Dictionary<string, ABItem>();
        public static Dictionary<String, AssetBundle> loadedAB = new Dictionary<string, AssetBundle>();

        private static string _ABRootPath = null;
        public static string ABRootPath {
            get {
                if(_ABRootPath == null) {
                    if(Application.isEditor) {
                        _ABRootPath = FileUtils.CombinePath(FileUtils.GetUnityProjectPath(),
                                                            "EditorEmulateLoadAssetBundle", 
                                                            "AssetBundles");
                    } else {
                        _ABRootPath = Path.Combine(Application.persistentDataPath, "AssetBundles");
                    }
                    if(!Directory.Exists(_ABRootPath)) {
                        Directory.CreateDirectory(_ABRootPath);
                    }
                }
                return _ABRootPath;
            }
        }

        static ABManager() {
            string manifestFile = Path.Combine(ABRootPath, ABManifest.FileName);
            if(File.Exists(manifestFile)) {
                Debug.LogFormat("ABManager(), manifestFile:{0} content:\n{1}", manifestFile, File.ReadAllText(manifestFile));
                ABManifest manifest = ABManifest.Deserialize(manifestFile);
                foreach(var item in manifest.abItems) {
                    configs.Add(item.name, item);
                }
            }
        }

        public static AssetBundle LoadAssetBundle(string abName)
        {
            Debug.LogFormat("ABManager.LoadAssetBundleabName:{0}", abName);
            if(loadedAB.ContainsKey(abName)) {
                return loadedAB[abName];
            }
            if(!configs.ContainsKey(abName)) {
                return null;
            }
            ABItem abItemConfig = configs[abName];
            string abFile = Path.Combine(ABRootPath, abItemConfig.name);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(abFile);
            Debug.LogFormat("ABManager.LoadAssetBundle real abName:{0} File:{1}", abName, abFile);
            loadedAB[abName] = assetBundle;
            foreach(var dependABName in abItemConfig.dependencies)
            {
                LoadAssetBundle(abName);
            }
            return assetBundle;
        }

        public static void UnloadAllAssetBundles()
        {
            AssetBundle.UnloadAllAssetBundles(false);
        }
    }
}
