using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;

namespace HotFixSolution
{
    public class ABManifest
    {
        public readonly static string FileName = "AssetBundlesManifest.xml";
        public List<ABItem> abItems = new List<ABItem>();

        public void Serialize(string fileName)
        {
            using(Stream sr = File.Create(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ABManifest));
                serializer.Serialize(sr, this);
            }
        }

        public static ABManifest Deserialize(string fileName)
        {
            using(Stream sr = File.OpenRead(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ABManifest));
                ABManifest manifest = (ABManifest)serializer.Deserialize(sr);
                return manifest;
            }
        }

        public override string ToString()
        {
            using(MemoryStream ms = new MemoryStream()) {
                XmlSerializer serializer = new XmlSerializer(typeof(ABManifest));
                serializer.Serialize(ms, this);
                return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }


    public class ABItem
    {
        public string name;
        public long fileSizeInBytes;
        public string md5;
        public string[] dependencies;
    }
}
