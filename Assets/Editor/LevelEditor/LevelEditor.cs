using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[ExecuteInEditMode]
public class LevelEditor : EditorWindow {

	public int toolbarOption = 0;  

	public string[] toolbarTexts = {"SingleMonster"}; 

	[SerializeField]
	static LevelEditor _windowInstance; 

	RaycastHit _hitInfo;

	private float _lastLBClickedTime = 0;

	private SerializeLevel _SerialziedLevel;

	private static string _autoSaveFile = "./Temp/__auto__leveleditor.txt";

	//static bool bOnOpen = false;

    public Vector2 scrollPosition;

    public SerializeLevel LevelMgr
    {
        get { return _SerialziedLevel; }
        set { _SerialziedLevel = value; }
    }

	[MenuItem("Window/LevelEditor %L")]  
	static void Init()  
	{  
		//Debug.Log ("init start");
		//bOnOpen = true;
		_windowInstance = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
	}  

	// Window has been selected
	void OnFocus() {
		//Debug.Log ("on focus start");
		// Remove delegate listener if it has previously
		// been assigned.
        SceneView.onSceneGUIDelegate -= LevelEditor.OnSceneFunc;
		
		// Add (or re-add) the delegate.
        SceneView.onSceneGUIDelegate += LevelEditor.OnSceneFunc;

		EditorApplication.playmodeStateChanged -= StateChange;
		EditorApplication.playmodeStateChanged += StateChange;
	}
	
	void OnDestroy() {
		// When the window is destroyed, remove the delegate
		// so that it will no longer do any drawing.
		_SerialziedLevel.RemoveSceneViewInstance ();
        SceneView.onSceneGUIDelegate -= LevelEditor.OnSceneFunc;
	}

	public void OnEnable()
	{
		hideFlags = HideFlags.HideAndDontSave;
		if (_SerialziedLevel == null)
		{
			_SerialziedLevel = ScriptableObject.CreateInstance<SerializeLevel> ();
			_SerialziedLevel.Editor = this;
		}

        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;

        //if(bOnOpen)
        //{
        //    _SerialziedLevel.LoadFromFile (_autoSaveFile);
        //    bOnOpen = false;
        //}
    }

	void StateChange()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
		{
			_SerialziedLevel.RemoveSceneViewInstance ();
		}
	}

	public void OnDisable()
	{
		_SerialziedLevel.SaveToFile (_autoSaveFile, true);
	}
	
	static public void OnSceneFunc(SceneView sceneView)  
	{  
		if(_windowInstance == null)
		{
			_windowInstance = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
		}

			_windowInstance.CustomSceneUpdate(sceneView);  
	}

    void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        Event e = Event.current;

        if (e.isKey && e.keyCode == KeyCode.Delete)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                GameObject monster = obj;
                while (monster.transform.parent != null) monster = monster.transform.parent.gameObject;
                if (monster.name.IndexOf("Wave") != -1)
                {
                    _SerialziedLevel.RemoveMonster(monster);
                }
            }
        }
    }

    void CustomSceneUpdate(SceneView sceneView)  
	{ 
		Event e = Event.current;

		if (e.type == EventType.MouseDown && e.button == 0) 
		{
			if(Time.realtimeSinceStartup - _lastLBClickedTime < 0.2f)
			{
				//Camera cameara = sceneView.camera;  
				Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition); 

				int layerMask = (1 << 9 | 1);
				if (Physics.Raycast (ray, out _hitInfo, Mathf.Infinity, layerMask)) 
				{  
					_SerialziedLevel.AddMonster(_hitInfo.point + new Vector3(0, 0.05f, 0));

					this.Repaint();

					_lastLBClickedTime = 0.0f;
				}
			}
			else
			{
				_lastLBClickedTime = Time.realtimeSinceStartup;
			}
		}

		if (e.isKey && e.keyCode == KeyCode.Delete) 
		{
		    foreach (GameObject obj in Selection.gameObjects)
		    {
                GameObject monster = obj;
                while (monster.transform.parent != null) monster = monster.transform.parent.gameObject;
		        if (monster.name.IndexOf("Wave") != -1)
		        {
                    _SerialziedLevel.RemoveMonster(monster);
		        }
		    }
		}
	}

	void OnGUI ()   
	{
        CreateSingleMonsterWindow();
        //toolbarOption = GUILayout.Toolbar (toolbarOption, toolbarTexts);  
        //switch (toolbarOption)
        //{
        //case 0:
        //    CreateSingleMonsterWindow();
        //    break;
        //}
	}

	void CreateSingleMonsterWindow()
	{
        //scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) });
        _SerialziedLevel.OnGUI();

        //ttest();
       // GUILayout.EndScrollView();
	}
}
