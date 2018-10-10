using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;

public static class BuildAssetBundlesEditor {
    public static readonly string assetBundleResourceFolder = "Assets/AssetBundles";

    [MenuItem("BuildAssetBundlesEditor/MarkAllAssetBundles")]
    public static void MarkAllAssetBundles()
    {
        var assetFiles = Directory.GetFiles(assetBundleResourceFolder, "*", SearchOption.AllDirectories);
        var assetFilesToBeAB = assetFiles.Where(f => !f.EndsWith(".meta"))
                                          .Where(f => !f.EndsWith(".cs"));
        Debug.LogFormat("assetFilesToBeAB:\n{0}", string.Join("\n", assetFilesToBeAB.ToArray()));
        foreach(string assetPath in assetFilesToBeAB)
        {
            var assetImp = AssetImporter.GetAtPath(assetPath);
            if(assetImp != null) {
                assetImp.assetBundleName = AssetPathToAssetBundleName(assetPath);
            }
        }
    }

    public static string AssetPathToAssetBundleName(string assetPath)
    {
        return assetPath.ToLower();
    }

    private static string CombinePath(params string[] pathComponent)
    {
        string dstPath = pathComponent[0];
        for (int i = 1; i < pathComponent.Length; i++) {
            dstPath = Path.Combine(dstPath, pathComponent[i]);
        }
        return dstPath;
    }

    [MenuItem("BuildAssetBundlesEditor/BuildAllAssetBundles")]
    public static void BuildAllAssetBundles()
    {
        string currentDateDesc = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        String outputPath = CombinePath("Build", "AssetBundles", EditorUserBuildSettings.activeBuildTarget.ToString(), currentDateDesc);
        Directory.CreateDirectory(outputPath);
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
        Application.OpenURL("file:///" + Path.GetFullPath(outputPath));
    }
}
