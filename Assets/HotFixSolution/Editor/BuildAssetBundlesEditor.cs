using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using CSObjectWrapEditor;

namespace HotFixSolution
{
    public static class BuildAssetBundlesEditor
    {
        public static readonly string assetBundleResourceFolder = "Assets/ABResources";

        public static void MarkAllAssetBundles()
        {
            var assetFiles = Directory.GetFiles(assetBundleResourceFolder, "*", SearchOption.AllDirectories);
            var assetFilesToBeAB = assetFiles.Where(f => !f.EndsWith(".meta"))
                                              .Where(f => !f.EndsWith(".cs"));
            //make sure the path is assetPath(path are relative to the project folder, and path is sperated by "/", for example: "Assets/MyTextures/hello.png").
            assetFilesToBeAB = assetFilesToBeAB.Select(path => path.Replace("\\", "/")); 
            Debug.LogFormat("assetFilesToBeAB:\n{0}", string.Join("\n", assetFilesToBeAB.ToArray()));
            foreach (string assetPath in assetFilesToBeAB)
            {
                var assetImp = AssetImporter.GetAtPath(assetPath);
                if (assetImp != null)
                {
                    assetImp.assetBundleName = ABResources.AssetPathToAssetBundleName(assetPath);
                }
            }
        }

        private static string CombinePath(params string[] pathComponent)
        {
            string dstPath = pathComponent[0];
            for (int i = 1; i < pathComponent.Length; i++)
            {
                dstPath = Path.Combine(dstPath, pathComponent[i]);
            }
            return dstPath;
        }

        [MenuItem("BuildAssetBundlesEditor/BuildAllAssetBundles")]
        public static void BuildAllAssetBundles()
        {
            Generator.ClearAll();
            Generator.GenAll();
            ClearAllAssetBundlesName();
            MarkAllAssetBundles();
            string currentDateDesc = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            String outputPath = CombinePath("Build", "AssetBundles", EditorUserBuildSettings.activeBuildTarget.ToString(), currentDateDesc);
            Directory.CreateDirectory(outputPath);
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
            Application.OpenURL("file:///" + Path.GetFullPath(outputPath));
            BuildAssetBundlesXMLManifest(outputPath);
        }

        private static void BuildAssetBundlesXMLManifest(string rootPath)
        {
            ABManifest manifest = new ABManifest();
            foreach(var abName in AssetDatabase.GetAllAssetBundleNames())
            {
                ABItem aBItem = new ABItem();
                aBItem.name = abName;
                string abFile = Path.Combine(rootPath, abName);
                aBItem.fileSizeInBytes = new FileInfo(abFile).Length;
                aBItem.md5 = FileUtils.GetMD5HashFromFile(abFile);
                aBItem.dependencies = AssetDatabase.GetAssetBundleDependencies(abName, false);
                manifest.abItems.Add(aBItem);
            }
            string manifestXmlFile = Path.Combine(rootPath, ABManifest.FileName);
            manifest.Serialize(manifestXmlFile);
        }

        [MenuItem("BuildAssetBundlesEditor/ClearAllAssetBundlesName")]
        public static void ClearAllAssetBundlesName()
        {
            foreach (var abName in AssetDatabase.GetAllAssetBundleNames())
            {
                AssetDatabase.RemoveAssetBundleName(abName, true);
            }
        }

        [MenuItem("BuildAssetBundlesEditor/OpenEditorDownloadABRootPath")]
        public static void OpenEditorDownloadABRootPath()
        {
            Debug.LogFormat("ABManager.ABRootPath:{0}", ABManager.ABRootPath);
            Application.OpenURL("file:///" + ABManager.ABRootPath);
        }
    }
}
