using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using XUtliPoolLib;
using XEditor;

[Serializable]
public class SerializeLevel : ScriptableObject
{
	[SerializeField]
	public List<LevelWave> _waves;

    [NonSerialized]
	private LevelWave _toBeRemoved;

    private GameObject _markPrefab;

    public List<GameObject> _markGO;

    public Dictionary<uint, int> _preLoadInfo;
    public Dictionary<uint, int> _lastCachePreloadInfo;

    [SerializeField]
    private XEntityStatistics _data_info = null;

    [NonSerialized]
    private LevelLayoutManager _layout;

    public static int maxSpawnTime = 180;

    private float _markGOHeight;
    public float MarkHeight
    {
        get { return _markGOHeight; }
        set { _markGOHeight = value; }
    }

    private float _goStep = 0.0001f;

    public string current_level = "";
	
	private LevelEditor _editor;
	public LevelEditor Editor
	{
		get { return _editor; }
		set { _editor = value; }
	}

    public int WaveCount
    {
        get { return _waves.Count; }
    }

    public XEntityStatistics EnemyList
    {
        get { return _data_info; }
    }
	
	int _currentEdit = -1;
	public int CurrentEdit 
	{
		get { return _currentEdit; }
	    set
	    {
	        int preID = _currentEdit;
	        _currentEdit = value;

	        if (preID != _currentEdit)
	        {
	            MarkEditWave();
	        }
	    }
	}
	
	public void OnEnable ()
	{
		hideFlags = HideFlags.HideAndDontSave;

		if (_waves == null)
			_waves = new List<LevelWave> ();

        if (_markGO == null)
            _markGO = new List<GameObject>();

        if (_preLoadInfo == null)
            _preLoadInfo = new Dictionary<uint, int>();

        if (_lastCachePreloadInfo == null)
            _lastCachePreloadInfo = new Dictionary<uint, int>();

        if (_layout == null)
            _layout = new LevelLayoutManager(this);

        _markPrefab = Resources.Load("Common/EditorRes/EditorArrow") as GameObject;

	    _currentEdit = -1;

	    _markGOHeight = 2.0f;

        if (_data_info == null)
        {
            _data_info = new XEntityStatistics();

            if (!XTableReader.ReadFile("Table/XEntityStatistics", _data_info))
            {
                Debug.Log("<color=red>Error occurred when loading associate data file.</color>");
            }
        }
	}
	
	public void OnGUI()
	{
		_layout.OnGUI();
        
        //foreach (LevelWave _wave in _waves)
        //    _wave.OnGUI ();

        if (_toBeRemoved != null)
        {
            if (_toBeRemoved._id == CurrentEdit)
            {
                CurrentEdit = -1;
            }

            _waves.Remove(_toBeRemoved);
            _toBeRemoved = null;
        }
	}

    public void Update()
    {
        float minh = 1.8f;
        float maxh = 2.2f;

        _markGOHeight += _goStep;

        if (_markGOHeight > maxh)
        {
            _markGOHeight = maxh;
            _goStep = -_goStep;
        }
        else if (_markGOHeight < minh)
        {
            _markGOHeight = minh;
            _goStep = -_goStep;
        }

        foreach (GameObject go in _markGO)
        {
            go.transform.localPosition = new Vector3(0, _markGOHeight, 0);
        }
    }

	public void SaveToFile()
	{
        string path = EditorUtility.SaveFilePanel("Select a file to save", XEditorPath.Lev, "temp.txt", "txt");

		SaveToFile (path, false);
	}

	public void SaveToFile(string path, bool bAutoSave)
	{
        SavePreprocess();
		StreamWriter sw = File.CreateText (path);
		sw.WriteLine ("{0}", _waves.Count);

        CalEnemyNum cen = new CalEnemyNum();
        CalEnemyNum.PrintLog = true;
        Dictionary<uint, int> suggest = cen.CalNum(_waves);

        int preLoadCount = 0;
        foreach (KeyValuePair<uint, int> keyValuePair in _preLoadInfo)
        {
            int sugCount = 0;
            suggest.TryGetValue(keyValuePair.Key, out sugCount);
            if (keyValuePair.Value > 0 || sugCount > 0) ++preLoadCount;
        }

        sw.WriteLine("{0}", preLoadCount);
        foreach (KeyValuePair<uint, int> keyValuePair in _preLoadInfo)
	    {
            int sugCount = 0;
            suggest.TryGetValue(keyValuePair.Key, out sugCount);
            if (keyValuePair.Value > 0 || sugCount > 0)
            {
                sw.WriteLine("pi:" + keyValuePair.Key + "," + keyValuePair.Value);
            }
	    }
        
		foreach (LevelWave _wave in _waves)
		{
			_wave.WriteToFile (sw);
		}
        sw.Flush();
		sw.Close ();

        AssetDatabase.Refresh();
	}

    public void SavePreprocess()
    {
        List<LevelWave> _wavesToRemove = new List<LevelWave>();
        foreach (LevelWave _wave in _waves)
        {
            if (!_wave.ValidWave())
                _wavesToRemove.Add(_wave);
        }

        foreach (LevelWave _remove in _wavesToRemove)
        {
            _waves.Remove(_remove);
        }
    }

	public void LoadFromFile()
	{
	    current_level = "";
        string path = EditorUtility.OpenFilePanel("Select a file to load", XEditorPath.Lev, "txt");

		RemoveSceneViewInstance ();
		_waves.Clear ();
        _preLoadInfo.Clear();
        _lastCachePreloadInfo.Clear();

        _currentEdit = -1;

	    int xiegang = path.LastIndexOf("Level/");
	    int dot = path.LastIndexOf(".");

	    current_level = path.Substring(xiegang + 6, dot - xiegang - 6);

		LoadFromFile (path);

        CalEnemyNum cen = new CalEnemyNum();
        CalEnemyNum.PrintLog = false;
        _lastCachePreloadInfo = cen.CalNum(_waves);
    }
    
	public void LoadFromFile(string path)
	{
		if (!File.Exists (path)) 	return;

		StreamReader sr = File.OpenText (path);
		
		string line = sr.ReadLine ();
		
		int totalWave = int.Parse (line);

	    line = sr.ReadLine();

	    int PreloadWave = int.Parse(line);

	    for (int i = 0; i < PreloadWave; i++)
        {
            line = sr.ReadLine();
            line = line.Replace("pi:", "");
            string[] preloadInfo = line.Split(',');
            _preLoadInfo.Add(uint.Parse(preloadInfo[0]), int.Parse(preloadInfo[1]));
        }
				
		for(int id = 0; id < totalWave; id++)
		{
			LevelWave newWave =  ScriptableObject.CreateInstance<LevelWave> ();

            newWave.LevelMgr = this;
			newWave.ReadFromFile(sr);
						
			_waves.Add (newWave);
			
			CurrentEdit = -1;
		}
		sr.Close ();
		
		_editor.Repaint ();
	}

    public void ClearWaves()
    {
        RemoveSceneViewInstance();

        _waves.Clear();
        _preLoadInfo.Clear();
        _lastCachePreloadInfo.Clear();
        _editor.Repaint();
    }

    public void OpenLevelScriptFile()
    {
        if (string.IsNullOrEmpty(current_level)) return;

        System.Diagnostics.Process.Start("notepad.exe", "./" + XEditorPath.Lev + current_level + "_sc.txt");
    }

    public void LoadWallInfo()
    {
        if (string.IsNullOrEmpty(current_level)) return;

        string fileName = "./" + XEditorPath.Lev + current_level + "_sc.txt";

        if (!File.Exists(fileName)) return;
        //StreamReader sr = File.OpenText(fileName);

        string levelConfig = "Level/" + current_level;

        string content = File.ReadAllText(fileName);
        string[] commands = content.Split(new char[] { '\n' });

        string dynamicNode = XSceneLibrary.GetDynamicString(levelConfig);
        GameObject dynamic = GameObject.Find(dynamicNode);
        if (dynamic == null) return;

        for (int i = 0; i < commands.Length; i++)
        {
            if (commands[i].StartsWith("info"))
            {
                if (commands[i][commands[i].Length - 1] == '\r')
                    commands[i] = commands[i].Substring(0, commands[i].Length - 1);

                string[] s = commands[i].Split(' ');

                string wallName = s[0].Substring(5);

                Transform t = XCommon.singleton.FindChildRecursively(dynamic.transform, wallName);
                if (t != null)
                {
                    //string strPosition = s[1];
                    string[] sp = s[1].Split('|');
                    t.position = new Vector3(XParse.Parse(sp[0]), XParse.Parse(sp[1]), XParse.Parse(sp[2]));
                    t.rotation = Quaternion.Euler(new Vector3(t.rotation.eulerAngles.x, XParse.Parse(sp[3]), t.rotation.eulerAngles.z));
                    //t.rotation.SetEulerAngles(new Vector3(t.rotation.eulerAngles.x, XParse.Parse(sp[3]), t.rotation.eulerAngles.z));

                    //t.rotation.eulerAngles = new Vector3(t.rotation.eulerAngles.x, XParse.Parse(sp[3]), t.rotation.eulerAngles.z);
                    
                    string strState = s[2];

                    if (strState == "on")
                        t.gameObject.SetActive(true);
                    else
                        t.gameObject.SetActive(false);

                    if (s.Length > 5)
                    {
                        string strBoxX = s[3];
                        string strBoxY = s[4];
                        string strBoxZ = s[5];

                        BoxCollider c = t.GetComponent<BoxCollider>();
                        if (c != null)
                        {
                        //    append += " " + (c.size.x * t.localScale.x);
                        //append += " " + (t.position.y + c.size.y / 2* t.localScale.y);
                        //append += " " + (c.size.z * t.localScale.z);
                            float x = XParse.Parse(strBoxX) / t.localScale.x;
                            float y = (XParse.Parse(strBoxY) - t.position.y) / t.localScale.y * 2;
                            float z = XParse.Parse(strBoxZ) / t.localScale.z;

                            c.size = new Vector3(x, y, z);
                        }
                    }
                    

                    if (s.Length > 6)
                    {
                        string strHorseIndex = s[6];

                        XHorseWall horseWall = t.GetComponent<XHorseWall>();
                        horseWall.index = int.Parse(strHorseIndex);
                    }
                    
                }
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    public void GenerateWallInfo()
    {
        if (string.IsNullOrEmpty(current_level)) return;

        string fileName = "./" + XEditorPath.Lev + current_level + "_sc.txt";

        if (!File.Exists(fileName)) return;
        //StreamReader sr = File.OpenText(fileName);
        
        string levelConfig = "Level/" + current_level;
        
        string content = File.ReadAllText(fileName);

        string final = "";

        string[] commands = content.Split(new char[] { '\n' });

        string append = "";

        //List<int> toberemove = new List<int>();
        bool HasPreInfo = false;

        List<string> RecordWalls = new List<string>();

        for (int i = 0; i < commands.Length; i++)
        {
            if (commands[i].StartsWith("info"))
            {
                HasPreInfo = true;
                continue;
            }

            if (!commands[i].StartsWith("opendoor"))
            {
                final += commands[i] + '\n';
                continue;
            }

            if (commands[i][commands[i].Length-1] == '\r')
                commands[i] = commands[i].Substring(0, commands[i].Length - 1);

            string[] s = commands[i].Split(' ');
            
            //if (s.Length == 2) commands[i] += " off";

            s = commands[i].Split(' ');

            string wallName = s[1];
            string dynamicNode = XSceneLibrary.GetDynamicString(levelConfig);

            GameObject dynamic = GameObject.Find(dynamicNode);

            if (dynamic != null)
            {
                Transform t = XCommon.singleton.FindChildRecursively(dynamic.transform, wallName);
                if (t != null)
                {
                    XSpawnWall spawnWall = t.GetComponent<XSpawnWall>();
                    XTransferWall transferWall = t.GetComponent<XTransferWall>();
		            XHorseWall horseWall = t.GetComponent<XHorseWall>();
                    if(spawnWall != null || transferWall != null )
                    {
                        final += commands[i] + "\r\n";
                        continue;
                    }

                    if (RecordWalls.Contains(wallName))
                    {
                        final += commands[i] + "\r\n";
                        continue;
                    }

                    RecordWalls.Add(wallName);
                    Vector3 pos = t.position;
                    float r = t.rotation.eulerAngles.y;

                    append += ("info:" + wallName);

                    append += " ";
                    append += pos.x;
                    append += "|";
                    append += pos.y;
                    append += "|";
                    append += pos.z;
                    append += "|";
                    append += r;

                    if (t.gameObject.activeInHierarchy)
                        append += " on";
                    else
                        append += " off";

                    BoxCollider c = t.GetComponent<BoxCollider>();
                    if (c != null)
                    {
                        append += " " + (c.size.x * t.localScale.x);
                        append += " " + (t.position.y + c.size.y / 2* t.localScale.y);
                        append += " " + (c.size.z * t.localScale.z);
                    }

                    if (horseWall != null)
                    {
                        append += " " + horseWall.index;
                    }
                
                    append += "\r\n";
                }
            }

            commands[i] += '\r';

            final += commands[i] + '\n';
        }

        if (!HasPreInfo) final += "\r\n";

        final += append;

        File.WriteAllText(fileName, final);
    }

	public void RemoveSceneViewInstance()
	{
		foreach (LevelWave _wave in _waves)
		{
			_wave.RemoveSceneViewInstance ();
		}

        _markGO.Clear();
	}
	
	public void AddWave()
	{
		LevelWave newWave =  ScriptableObject.CreateInstance<LevelWave> ();

	    int newid = GetEmptySlot(0, 100);

	    if (newid >= 0)
	    {
            newWave._id = newid;
            newWave.LevelMgr = this;
            _waves.Add(newWave);

            CurrentEdit = newWave._id;
	    }
	    else
	    {
	        Debug.Log("More than 100 waves?");
	    }
	}

    public void AddScript()
    {
        LevelWave newWave = ScriptableObject.CreateInstance<LevelWave>();

        int newid = GetEmptySlot(1000, 1100);

        if (newid >= 0)
        {
            newWave._id = newid;
            newWave.LevelMgr = this;
            _waves.Add(newWave);

            CurrentEdit = newWave._id;
        }
    }

    protected int GetEmptySlot(int startIndex, int endIndex)
    {
        for (int i = startIndex; i < endIndex; i++)
        {
            bool bOccupied = false;
            foreach (LevelWave _wave in _waves)
            {
                if (_wave._id == i)
                {
                    bOccupied = true;
                    break;
                }
            }

            if (bOccupied == false)
                return i;
        }

        return -1;
    }
	
	public void RemoveWave(int id)
	{
		foreach (LevelWave _wave in _waves)
		{
			if(_wave._id == id)
			{
				_toBeRemoved = _wave;
			}
		}
	}

    public LevelWave GetWave(int id)
    {
        foreach (LevelWave _wave in _waves)
        {
            if (_wave._id == id)
            {
                return _wave;
            }
        }

        return null;
    }
	
	public void AddMonster(Vector3 pos)
	{
		foreach (LevelWave _wave in _waves)
		{
			if(_wave._id == _currentEdit)
			{
				_wave.AddMonsterAtPos(pos, _markPrefab);
			}
		}
	}
	
	public void RemoveMonster(GameObject go)
    {
        string[] wave_info = go.name.Replace("Wave", "").Replace("monster", "").Split('_');
        int wave = int.Parse(wave_info[0]);
        //int index = int.Parse(wave_info[1]);

        foreach (LevelWave _wave in _waves)
		{
			if(_wave._id == wave)
			{
				_wave.RemoveMonster(go);
			}
		}
	}

    protected void MarkEditWave()
    {
        foreach (GameObject go in _markGO)
        {
            DestroyImmediate(go);
        }

        _markGO.Clear();

        if (_markPrefab != null)
        {
            foreach (LevelWave _wave in _waves)
            {
                if (_wave._id == CurrentEdit)
                {
                    _wave.SetWaveMarked(_markPrefab);
                }
            } 
        }
        
    }
}


