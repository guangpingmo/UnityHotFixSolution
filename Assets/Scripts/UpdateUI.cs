using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HotFixSolution;

public class UpdateUI : MonoBehaviour {
    public Text tip;
    public Slider downloadSlider;

	// Use this for initialization
	void Start () {
        ABDownloader.Instance.OnDownloadABZipFile += OnDownload;
        ABDownloader.Instance.OnUnZipAB += OnUnZipAB;
        ABDownloader.Instance.OnSuccUnZipAB += OnSuccUnZipAB;
        ABDownloader.Instance.DownloadABZipFile();
	}

    private void OnDownload(float progress)
    {
        downloadSlider.value = progress;
        tip.text = "正在下载资源包，请稍等！";
    }

    private void OnUnZipAB()
    {
        downloadSlider.value = 1f;
        tip.text = "正在解压资源包，请稍等！";
    }

    private void OnSuccUnZipAB()
    {
        downloadSlider.value = 1f;
        tip.text = "解压资源包成功！游戏开始！";
        LuaVM.Instance.RunScript("Assets/ABResources/LuaScript/Entry.lua.txt");
    }
}
