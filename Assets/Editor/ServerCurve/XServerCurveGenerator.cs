using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using XEditor;
using System.IO;
using XUtliPoolLib;

internal class XServerCurveGenerator
{
    public static bool GenerateCurve(GameObject prefab)
    {
        if(prefab == null) return false;

        IXCurve curve = prefab.GetComponent("XCurve") as IXCurve;
        return curve != null && RecordCurve(prefab, curve, null);
    }

    public static bool GenerateCurve(GameObject prefab, string fullname)
    {
        if (prefab == null) return false;

        IXCurve curve = prefab.GetComponent("XCurve") as IXCurve;
        return curve != null && RecordCurve(prefab, curve, fullname);
    }

    static bool RecordCurve(GameObject prefab, IXCurve curve, string fullname)
    {
        string path = fullname == null ? AssetDatabase.GetAssetPath(prefab) : fullname;
        int idx = path.IndexOf("/Curve/");

        if (idx < 0) return false;

        path = path.Substring(idx + 7);

		idx = path.LastIndexOf('/');
		if (idx < 0)
			return false;
		string file = XEditorPath.GetEditorBasedPath("Server/Curve" + "/" + path.Substring(0, path.LastIndexOf('/'))) + prefab.name + ".txt";

        using (FileStream writer = new FileStream(file, FileMode.Create))
        {
            //using Encoding
            StreamWriter sw = new StreamWriter(writer, Encoding.ASCII);
            WriteData(sw, curve);
            sw.Close();
        }

        AssetDatabase.Refresh();
        return true;
    }

    static void WriteData(StreamWriter sw, IXCurve curve)
    {
        sw.WriteLine("{0}\t{1}", curve.GetMaxValue(), curve.GetLandValue());

		for(int i = 0 ;i < curve.length;++i)
		{
			sw.WriteLine("{0}\t{1}", curve.GetTime(i), curve.GetValue(i));
		}
		/*
        foreach (Keyframe key in curve.GetCurve().keys)
        {
            sw.WriteLine("{0}\t{1}", key.time, key.value);
        }
        */
    }

	public static void GenerateCurveTable(UnityEngine.Object[] os)
	{


		SortedDictionary<uint,XCurve> dic = new SortedDictionary<uint, XCurve>();
		
		for(int i = 0 ; i < os.Length;++i)
		{
			GameObject prefab = os[i] as GameObject;
			XCurve curve = prefab.GetComponent("XCurve") as XCurve;

			if(curve == null)
				continue;
			
			string path = AssetDatabase.GetAssetPath(prefab);

			int idx = path.IndexOf("/Curve/");
			
			if (idx < 0) continue;
			
			path = path.Substring(idx + 7);

			idx = path.LastIndexOf('/');
			if(idx < 0)
			{
				path = "Curve/"+prefab.name;
				//continue;
			}else
			{
				path = "Curve/"+path.Substring(0, idx) + "/" + prefab.name;
			}


			uint hash = XCommon.singleton.XHash(path);
			if( dic.ContainsKey(hash) )
			{
				Debug.LogError("generate curve table error:hash conflic");
			}
			
			dic[hash] = curve;
		}

		string file = XEditorPath.GetPath("Table")+"Curve.bytes";
		using (FileStream writer = new FileStream(file, FileMode.Create)) 
		{
			BinaryWriter bw = new BinaryWriter (writer);

			bw.Write(dic.Count);

			foreach(KeyValuePair<uint,XCurve> item in dic)
			{

				bw.Write(item.Key);
				XCurve curve = item.Value;
                float maxvalue = curve.GetMaxValue() * 100;
                short usMaxValue = (short)maxvalue;
                bw.Write(usMaxValue);
                float landvalue = curve.GetLandValue() * 100;
                short usLandValue = (short)landvalue;
                bw.Write (usLandValue);

				float time = curve.Curve.keys[curve.Curve.length-1].time;
				float time30 = time * 30.0f;
				int count = Mathf.CeilToInt(time30);
				bw.Write(count+1);

				for(int i = 0;i<count+1;++i)
				{
                    float value = curve.Curve.Evaluate(i / 30.0f) * 100;
                    short usvalue = (short)value;
                    bw.Write(usvalue);
				}
				if(time - (count/30.0f) > 0.0f)
				{
					Debug.Log("Curve Error:"+curve.name+"|"+(time - (count/30.0f) )+"|Time:"+time+"|Count:"+count);
				}
			}

		}

		Debug.Log ("Generate Curve Table Success!");
	}
}