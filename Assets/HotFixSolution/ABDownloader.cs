using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;

namespace HotFixSolution
{
    public class ABDownloader : MonoBehaviour
    {
        private static readonly string abServerUrl = "http://localhost:8080/";
        private static readonly string versionConfig = "versionConfig.txt";


        private static string _DownloadZipFileRootPathh = null;
        private static string DownloadZipFileRootPath {
            get {
                if(_DownloadZipFileRootPathh == null) {
                    _DownloadZipFileRootPathh = Path.Combine(Application.persistentDataPath, "DownloadAssetBundlesZipFile");
                    if(!Directory.Exists(_DownloadZipFileRootPathh)) {
                        Directory.CreateDirectory(_DownloadZipFileRootPathh);
                    }
                }
                return _DownloadZipFileRootPathh;
            }
        }

        private static readonly string LastestUnzipSuccFileNameKey = "LastestUnzipSuccFileName";
        private static string LastestUnzipSuccFileName {
            get {
                return PlayerPrefs.GetString(LastestUnzipSuccFileNameKey, "");
            }
            set {
                PlayerPrefs.SetString(LastestUnzipSuccFileNameKey, value);
            }
        }

        public static ABDownloader _instance;
        public static ABDownloader Instance
        {
            get {
                if(_instance == null) {
                    GameObject gameObject = new GameObject(typeof(ABDownloader).FullName);
                    _instance = gameObject.AddComponent<ABDownloader>();
                }
                return _instance;
            }
        }

        public event Action<float> OnDownloadABZipFile;
        public event Action OnUnZipAB;
        public event Action OnSuccUnZipAB;

        public void DownloadABZipFile()
        {
            StartCoroutine(DownloadABZipFileCo());
        }

        private IEnumerator DownloadABZipFileCo()
        {
            string versionUrl = abServerUrl + versionConfig;
            WWW downloadVersion = new WWW(versionUrl);
            yield return downloadVersion;
            string abZipFileName = downloadVersion.text;
            if(abZipFileName == LastestUnzipSuccFileName) {
                if (OnSuccUnZipAB != null)
                {
                    OnSuccUnZipAB();
                }
                yield break;
            }
            string abZipFileUrl = abServerUrl + abZipFileName;
            WWW downloadABZipFile = new WWW(abZipFileUrl);
            while(!downloadABZipFile.isDone) {
                if(OnDownloadABZipFile != null) {
                    OnDownloadABZipFile(downloadABZipFile.progress);
                }
                yield return null;
            }
            string saveABZipFile = Path.Combine(DownloadZipFileRootPath, abZipFileName);
            File.WriteAllBytes(saveABZipFile, downloadABZipFile.bytes);
            if(OnUnZipAB != null)
            {
                OnUnZipAB();
            }
            FileUtils.ExtractZipFile(saveABZipFile, ABManager.ABRootPath);
            LastestUnzipSuccFileName = abZipFileName;
            if(OnSuccUnZipAB != null)
            {
                OnSuccUnZipAB();
            }
        }
    }
}
