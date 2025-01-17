using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;

internal class XServerCurveMgr : MonoBehaviour
{
    [MenuItem(@"Assets/Generate Server Curve (Selected)")]
    static void ServerCurve()
    {
        UnityEngine.Object[] os = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);

        foreach (UnityEngine.Object o in os)
            XServerCurveGenerator.GenerateCurve(o as GameObject);
    }

    [MenuItem(@"Assets/Generate Server Curve (All)")]
    static void ServerCurveAll()
    {
        UnityEngine.Object[] os = Resources.LoadAll("Curve", typeof(GameObject));

        foreach (UnityEngine.Object o in os)
            XServerCurveGenerator.GenerateCurve(o as GameObject);

		//ServerCurveTable ();

    }

	[MenuItem(@"Assets/Generate Server Curve (Client Table)")]
	static public void ServerCurveTable()
	{
		UnityEngine.Object[] os = Resources.LoadAll("Curve", typeof(GameObject));
		
		XServerCurveGenerator.GenerateCurveTable (os);
		
		
		
	}

}
