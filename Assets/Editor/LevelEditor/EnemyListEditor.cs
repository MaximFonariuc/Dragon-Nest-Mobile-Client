﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XUtliPoolLib;
using System.IO;
using XEditor;

public class EnemyListEditor : EditorWindow
{
    //static EnemyListEditor _windowInstance; 

    private XEntityStatistics _data_info = null;

    public static int width = 100;
    public static int height = 125;

    private Dictionary<uint ,Texture2D>  ID_ICON = new Dictionary<uint, Texture2D>();
    //public Rect _rect;

    private static GUIContent
        SearchButtonContent = new GUIContent("Search", "Search");

    private static GUIContent
        BuffButtonContent = new GUIContent("Doodad", "Doodad");

    private string _searchText = "";
    private string _buff = "";

    public void OnEnable()
    {
        //if (_data_info == null)
        //{
        //    _data_info = new XEnemyInfo();

        //    FileStream stream = new FileStream(@"Assets\Resources\Table\XEnemyList.txt", FileMode.Open);
        //    if (!_data_info.LoadFile(stream, false))
        //    {
        //        Debug.Log("<color=red>Error occurred when loading associate data file.</color>");
        //    }
        //    stream.Close();

        //    PrepareIcons();
        //}

        LevelEditor _levelInstance = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        if (_levelInstance == null) return;

        _data_info = _levelInstance.LevelMgr.EnemyList;

        PrepareIcons();
    }

    private void PrepareIcons()
    {
        foreach (XEntityStatistics.RowData _row in _data_info.Table)
        {
            string strPrefab = XAnimationLibrary.AssociatedAnimations(_row.PresentID).Prefab;

            GameObject _prefab = Resources.Load("Prefabs/" + strPrefab) as GameObject;

            Texture2D icon128 = AssetPreview.GetAssetPreview(_prefab);
            Texture2D icon64 = new Texture2D(64, 64);
            WaveWindow.Compress128To64(icon128, icon64);

            ID_ICON.Add(_row.ID, icon64);
        }
    }

    private void OnGUI()
    {
       // GUILayout.Label("enemy list", EditorStyles.boldLabel);
        
        int windowPerRow = 10;
        int pageCount = 40;

        GUILayout.BeginHorizontal();

        _searchText = GUILayout.TextField(_searchText, GUILayout.Width(120f));

        if (GUILayout.Button(SearchButtonContent, GUILayout.Width(120f)))
        {
            OnSearch();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        _buff = GUILayout.TextField(_buff, GUILayout.Width(120f));
        if (GUILayout.Button(BuffButtonContent, GUILayout.Width(120f)))
        {
            OnSelectDoodad();
        }
        GUILayout.EndHorizontal();


        GUILayout.Space(50);
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });

        BeginWindows();
        int i = 0;
        foreach (XEntityStatistics.RowData _row in _data_info.Table)
        {
            if (_searchText != null && _searchText.Length > 0)
            {
                if (_searchText != _row.ID.ToString()) continue;
            }

            Rect _rect = new Rect((i % windowPerRow) * width, ((i / windowPerRow) * height + 100), width, height);

            GUI.Window((int)_row.ID, _rect, doWindow, _row.Name);

            i++;

            if (i >= pageCount) break;
        }

        EndWindows();
    }

    public void doWindow(int id)
    {
        XEntityStatistics.RowData npcInfo = _data_info.GetByID((uint)id);
        
        GUILayout.BeginVertical();

        Texture2D _icon;
        if (ID_ICON.TryGetValue((uint)id, out _icon))
        {
            GUILayout.Box(_icon);
        }
        
        if (GUILayout.Button(npcInfo.ID.ToString(), GUILayout.Width(80f)))
        {
            OnEnemyClick((uint)id);
        }
        GUILayout.EndVertical();
    }

    public void OnEnemyClick(uint id)
    {
        LevelEditor _levelInstance = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        int curWave = _levelInstance.LevelMgr.CurrentEdit;

        LevelWave wv = _levelInstance.LevelMgr.GetWave(curWave);
        wv.EnemyID = id;

        EnemyListEditor _window = (EnemyListEditor)EditorWindow.GetWindow(typeof(EnemyListEditor));
        _window.Close();
    }

    public void OnSelectDoodad()
    {
        if (_buff == "") return;

        LevelEditor _levelInstance = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        int curWave = _levelInstance.LevelMgr.CurrentEdit;

        LevelWave wv = _levelInstance.LevelMgr.GetWave(curWave);
        wv.SpawnType = LevelWave.LevelSpawnType.Spawn_Source_Buff;
        wv.EnemyID = uint.Parse(_buff);

        EnemyListEditor _window = (EnemyListEditor)EditorWindow.GetWindow(typeof(EnemyListEditor));
        _window.Close();
    }
    protected void OnSelectScript()
    {
        string path = EditorUtility.OpenFilePanel("Select script", "./Temp", "txt");

        LevelEditor _levelInstance = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        int curWave = _levelInstance.LevelMgr.CurrentEdit;

        LevelWave wv = _levelInstance.LevelMgr.GetWave(curWave);
        wv.LevelScript = ExtractScriptName(path);

        EnemyListEditor _window = (EnemyListEditor)EditorWindow.GetWindow(typeof(EnemyListEditor));
        _window.Close();
    }

    protected void OnSearch()
    {
        
    }

    protected string ExtractScriptName(string path)
    {
        int pos = path.IndexOf("Table/");
        return path.Substring(pos);
    }

}

