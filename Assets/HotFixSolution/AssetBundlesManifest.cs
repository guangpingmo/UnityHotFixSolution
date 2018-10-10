using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundlesManifest
{
    public List<AssetBundleItem> assetBundleItems;
}


public class AssetBundleItem
{
    public string name;
    public string path;
    public long fileSizeInBytes;
    public string md5;
    public string[] dependencies;
}
