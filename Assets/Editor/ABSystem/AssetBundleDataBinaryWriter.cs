using System.Collections.Generic;
using System.IO;

namespace ABSystem
{
    public class AssetBundleDataBinaryWriter : AssetBundleDataWriter
    {
        public override void Save(Stream stream, AssetTarget[] targets)
        {
            BinaryWriter sw = new BinaryWriter(stream);
            //写入文件头判断文件类型用，ABDB 意思即 Asset-Bundle-Data-Binary
            sw.Write(new char[] { 'A', 'B', 'D', 'B' });

            List<string> bundleNames = new List<string>();

            for (int i = 0; i < targets.Length; i++)
            {
                AssetTarget target = targets[i];
                bundleNames.Add(target.bundleName);
            }

            //写入文件名池
            //sw.Write(bundleNames.Count);
            //for (int i = 0; i < bundleNames.Count; i++)
            //{
            //    string name = bundleNames[i].Replace(".ab", "");
            //    uint parseName = uint.Parse(name);
            //    sw.Write(parseName);
            //}

            //写入详细信息
            for (int i = 0; i < targets.Length; i++)
            {
                AssetTarget target = targets[i];
                HashSet<AssetTarget> deps = new HashSet<AssetTarget>();
                target.GetDependencies(deps);

                //debug name
                //sw.Write(target.assetPath);
                //bundle name
                int index = bundleNames.IndexOf(target.bundleName);
                if (index >= 0 && index < bundleNames.Count)
                {
                    string name = bundleNames[index];
                    name = name.Replace(".ab", "");
                    uint uintName = uint.Parse(name);
                    sw.Write(uintName);
                }
                else
                {
                    int uintName = 0;
                    sw.Write(uintName);
                }
                uint shortName = XUtliPoolLib.XCommon.singleton.XHashLowerRelpaceDot(0, target.bundleShortName);
                //File Name
                sw.Write(shortName);
                //hash
                //sw.Write(target.bundleCrc);
                //type
                byte type = (byte)target.compositeType;
                sw.Write(type);
                //写入依赖信息
                if(deps.Count>255)
                {
                    Debugger.LogError("deps count more than 255:" + target.bundleShortName);
                }
                byte count = (byte)deps.Count;
                sw.Write(count);

                foreach (AssetTarget item in deps)
                {
                    index = bundleNames.IndexOf(item.bundleName);
                    if (index >= 0 && index < bundleNames.Count)
                    {
                        string name = bundleNames[index];
                        name = name.Replace(".ab", "");
                        uint uintName = uint.Parse(name);
                        sw.Write(uintName);
                    }
                    else
                    {
                        int hash = 0;
                        sw.Write(hash);
                    }
                }
            }
            sw.Close();
        }
    }
}