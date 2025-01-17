using UnityEditor;
using UnityEngine;

public class ChangeShader :  EditorWindow {
	
  static TextAsset textAsset;
  static  string [] lineArray;

    [MenuItem(@"XEditor/Tools/ChangerShader")]
    static void AddWindow ()
	{       
		textAsset = AssetDatabase.LoadAssetAtPath("Assets/Editor/Tools/shader.txt",typeof(TextAsset))as TextAsset;
		
		lineArray = textAsset.text.Split ("&"[0]);
		
		Rect  wr = new Rect (0,0,500,500);
        ChangeShader window = (ChangeShader)EditorWindow.GetWindowWithRect (typeof (ChangeShader),wr,true,"widow name");	
		window.Show();

    }
	
	void OnGUI ()
	{
		for(int i =0; i<lineArray.Length; i++)
		{
			string shaderName = lineArray[i].Trim();
			if(GUILayout.Button(shaderName))
			{
				Change(Shader.Find(shaderName));		
			}
		}
	}
	
	void Change(Shader shader)
	{
		if(Selection.activeGameObject != null)
		{
			foreach(GameObject g in Selection.gameObjects)
			{
				Renderer []renders = g.GetComponentsInChildren<Renderer>();
				foreach(Renderer r in renders)
				{
					if(r  !=  null)
					{
						foreach(Object o in r.sharedMaterials)
						{
								string path = AssetDatabase.GetAssetPath(o);
								Material m = AssetDatabase.LoadAssetAtPath(path,typeof(Material)) as Material;
								m.shader = shader;				
						}
					}
				}
			}	
			this.ShowNotification(new GUIContent("选择的对象批量修改shader成功"));
		}else
		{
			this.ShowNotification(new GUIContent("没有在Hierarchy视图中选择对象"));

		}
	}
	
}
