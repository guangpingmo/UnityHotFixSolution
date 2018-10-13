using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

public static class UnityEditorUtility{
    #region menu extension.
    [MenuItem("Utility/Prefs/CleanPlayerPrefs")]
    public static void CleanPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Utility/Prefs/CleanEditorPrefs")]
    public static void CleanEditorPrefs()
    {
        EditorPrefs.DeleteAll();
    }
    #endregion

    #region Hierarchy view extension
    [MenuItem("GameObject/Utility/CopyHierarchyName", false, 10)]
    public static void CopyHierarchyName()
    {
        GameObject selectedGO = Selection.activeGameObject;
        Debug.Assert(selectedGO != null, "must select a gameObject in hierarchy view to copy its hierarchyName!");
        string hierarchyName = selectedGO.name;
        Transform node = selectedGO.transform.parent;
        while(node != null) {
            hierarchyName = node.name + "/" + hierarchyName;
            node = node.parent;
        }
        CopyStringToPasteboard(hierarchyName);
        Debug.LogFormat(selectedGO, "Current selected gameObject hierarchy name :{0}", hierarchyName);
    }
    #endregion

    #region Project view extension
    [MenuItem("Assets/Utility/CopyResourcePath")]
    public static void CopyResourcePath()
    {
        Object obj = Selection.activeObject;
        Debug.Assert(obj != null, "must select a asset in project view to copy its resourcePath!");
        string assetPath = AssetDatabase.GetAssetPath(obj);
        Debug.LogFormat("assetPath is :{0}", assetPath);
        const string resourcesFolderName = "Resources";
        int startIndex = assetPath.LastIndexOf(resourcesFolderName);
        if(startIndex >= 0) {
            startIndex = startIndex + resourcesFolderName.Length + 1;
            int endIndex = assetPath.LastIndexOf(".");
            int length = endIndex - startIndex;
            if (length > 0) {
                string resourcePath = assetPath.Substring(startIndex, length);
                CopyStringToPasteboard(resourcePath);
                Debug.LogFormat("resourcePath is :{0}", resourcePath);
            } else {
                Debug.LogErrorFormat("invalid resource Path!");
            }
        } else {
            Debug.LogErrorFormat("selected asset is not within the Resources Folder!");
        }
    }

    [MenuItem("Assets/Utility/CopyAssetPath")]
    public static void CopyAssetPath()
    {
        Object obj = Selection.activeObject;
        Debug.Assert(obj != null, "must select a asset in project view to copy its assetPath!");
        string assetPath = AssetDatabase.GetAssetPath(obj);
        Debug.LogFormat("assetPath is :{0}", assetPath);
        CopyStringToPasteboard(assetPath);
    }
    #endregion

    #region other
    public static void CopyStringToPasteboard(string content)
    {
        GUIUtility.systemCopyBuffer = content;
    }
    #endregion
}
