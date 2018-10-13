using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

namespace HotFixSolution
{
    public class ABManifest
    {
        public readonly static string FileName = "AssetBundlesManifest.xml";
        public List<ABItem> abItems = new List<ABItem>();

        public void Serialize(string fileName)
        {
            using(TextWriter tw = new StreamWriter(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ABManifest));
                serializer.Serialize(tw, this);
            }
        }

        public static ABManifest Deserialize(string fileName)
        {
            using(TextReader tr = new StringReader(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ABManifest));
                ABManifest manifest = (ABManifest)serializer.Deserialize(tr);
                return manifest;
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
