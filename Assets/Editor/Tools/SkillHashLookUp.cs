using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using XUtliPoolLib;
using System.Collections.Generic;
using System;

namespace XEditor
{
    public enum SearchType
    {
        Fast,
        SlowButFull
    }

    public class SkillHashLookUp : MonoBehaviour
    {
        [MenuItem(@"XEditor/LookUp skill hash")]
        static void LookUp()
        {
            EditorWindow.GetWindow(typeof(XLookUp));
        }
    }

    public class XLookUp : EditorWindow
    {
        private string hash = null;
        //private string name = null;

        private string m_name = "no match";
        private uint uhash = 0;

        private SearchType _type = SearchType.Fast;

        void OnGUI()
        {
            hash = EditorGUILayout.TextField("Hash Value", hash);
            EditorGUILayout.LabelField("Skill Name", m_name);
            _type = (SearchType)EditorGUILayout.EnumPopup(_type);

            if (GUILayout.Button("Match"))
            {
                m_name = "no match";
                Match();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            name = EditorGUILayout.TextField("Skill Name", name);
            uhash = XCommon.singleton.XHash(name);
            EditorGUILayout.LabelField("Hash Value", uhash.ToString());
        }

        void Match()
        {
            if (string.IsNullOrEmpty(hash)) return;

            switch (_type)
            {
                case SearchType.Fast:
                    {
                        SkillList.RowData[] list = XSkillListLibrary.AllList();
                        for (int i = 0; i < list.Length; i++)
                        {
                            if (XCommon.singleton.XHash(list[i].SkillScript).ToString() == hash)
                            {
                                m_name = list[i].SkillScript;
                            }
                        }
                    }break;
                case SearchType.SlowButFull:
                    {
                        DirectoryInfo TheFolder = new DirectoryInfo(@"Assets\Resources\SkillPackage");
                        ProcessFolder(TheFolder);
                    }break;
            }
        }

        void ProcessFolder(DirectoryInfo dir)
        {
            FileInfo[] fileInfo = dir.GetFiles();

            foreach (FileInfo file in fileInfo)
                ProcessFile(file);

            foreach (DirectoryInfo sub_dir in dir.GetDirectories())
                ProcessFolder(sub_dir);
        }

        void ProcessFile(FileInfo file)
        {
            if (file.FullName.IndexOf(".meta") < 0)
            {
                try
                {
                    XSkillData data = XDataIO<XSkillData>.singleton.DeserializeData(file.FullName.Substring(file.FullName.IndexOf(@"Assets\Resources\SkillPackage\")));
                    Process(data);
                }
                catch (Exception e)
                {
                    Debug.Log("Parse " + file.Name + " " + e.Message);
                }
            }
        }

        void Process(XSkillData data)
        {
            //EditorGUILayout.LabelField("Processing...", data.Name);
            if (XCommon.singleton.XHash(data.Name).ToString() == hash)
            {
                m_name = data.Name;
            }
        }
    }
}