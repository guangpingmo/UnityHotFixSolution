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

        [MenuItem("BuildAssetBundlesEditor/BuildAllAssetBundles")]
        public static void BuildAllAssetBundles()
        {
            Generator.ClearAll();
            Generator.GenAll();
            ClearAllAssetBundlesName();
            MarkAllAssetBundles();
            string currentDateDesc = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            String outputPath = FileUtils.CombinePath("Build", "AssetBundles", EditorUserBuildSettings.activeBuildTarget.ToString(), currentDateDesc);
            Directory.CreateDirectory(outputPath);
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
            Application.OpenURL("file:///" + Path.GetFullPath(outputPath));
            BuildAssetBundlesXMLManifest(outputPath);
            string zipFile = Path.Combine(Path.GetDirectoryName(outputPath), currentDateDesc + ".zip");
            FileUtils.CreateZipFile(zipFile, outputPath);
            Debug.LogFormat("Zip Folder:{0} To ZipFile:{1}", outputPath, zipFile);
            string versionConfigFile = Path.Combine(Path.GetDirectoryName(outputPath), ABDownloader.versionConfig);
            File.WriteAllText(versionConfigFile, currentDateDesc + ".zip");
            //FileUtils.ExtractZipFile(zipFile, outputPath+"-unzip");
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

        [MenuItem("BuildAssetBundlesEditor/EmulateLoadAssetBundleInEditor", true)]
        public static bool EmulateLoadAssetBundleInEditorValidate()
        {
            Menu.SetChecked("BuildAssetBundlesEditor/EmulateLoadAssetBundleInEditor", ABResources.EmulateLoadAssetBundleInEditor);
            return true;
        }

        [MenuItem("BuildAssetBundlesEditor/EmulateLoadAssetBundleInEditor")]
        public static void EmulateLoadAssetBundleInEditor()
        {           
            ABResources.EmulateLoadAssetBundleInEditor = !ABResources.EmulateLoadAssetBundleInEditor;
            Debug.LogFormat("EmulateLoadAssetBundleInEditor:{0}", ABResources.EmulateLoadAssetBundleInEditor);
        }
    }
}
