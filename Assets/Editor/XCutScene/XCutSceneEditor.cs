using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using XUtliPoolLib;
using System.Collections.Generic;

namespace XEditor
{
    public class XCutSceneEditor : MonoBehaviour
	{
        [MenuItem(@"XEditor/Cut Scene")]
        static void CutScene()
        {
            EditorWindow.GetWindowWithRect(typeof(XCutSceneWindow), new Rect(0, 0, 600, 800), true, @"CutScene");
        }
	}

    public class XCutSceneAddationWindow : EditorWindow
    {
        public XClip _clip = null;
        public float _total_frame = 0;
        public float _play_at_frame = 0;
        private bool _ok = false;

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            _play_at_frame = EditorGUILayout.FloatField("Play at Frame", _play_at_frame);
            GUILayout.Label("(frame)");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time At", (_play_at_frame * (1 / 30.0f)).ToString("F2"));
            GUILayout.Label("(s)");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            _play_at_frame = EditorGUILayout.Slider("Ratio", _play_at_frame, 0, _total_frame);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                _ok = true;
                Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        void OnDestroy()
        {
            if (_ok)
            {
                XCutSceneWindow window = EditorWindow.GetWindow<XCutSceneWindow>(@"Cut Scene");

                if (_clip != null)
                {
                    window.RemoveClip(_clip);
                    _clip.TimeLine = _play_at_frame;
                    window.AddClip(_clip);
                }
                else
                {
                    window.AddClip(_play_at_frame);
                }

                window.Focus();
            }

            _clip = null;
        }
    }

    public class XCutSceneWindow : EditorWindow
    {
        public enum Trigger
        {
            ToEffect,
            ToIdle,
            ToWarriorSelect,
            ToWarriorShow,
            ToWarriorTurn,
            ToArcherSelect,
            ToArcherShow,
            ToArcherTurn,
            ToSorceressSelect,
            ToSorceressShow,
            ToSorceressTurn,
            ToClericSelect,
            ToClericShow,
            ToClericTurn,
            ToAcademicSelect,
            ToAcademicShow,
            ToAcademicTurn,
            ToAssassinSelect,
            ToAssassinShow,
            ToAssassinTurn,
            ToKaliSelect,
            ToKaliShow,
            ToKaliTurn,
            ToLogin,
            ToLoginEnter,
            ToSelectChar
        }

        public enum EntitySpecies
        {
            Species_Boss = 1,
            Species_Opposer = 2,
            Species_Puppet = 3,
            Species_Ally = 4,
            Species_Npc = 7,
            Species_Role = 10,
            Species_Empty = 8,
            Species_Dummy = 9,
            Species_Neutral = 5,
            Species_Affiliate = 11,
            Species_Elite = 6
        }

        private bool _open_scene = false;
        private string _file = null;

        private SortedList<XClip, XClip> _clips = new SortedList<XClip, XClip>(new XClipComparer());

        private XClipType _type = XClipType.Actor;
        private GUIContent _content_add = new GUIContent("+");

        private AnimationClip _camera = null;
        private string _name = null;
        private string _script = null;
        private string _scene = null;
        private int _type_mask = -1;
        private bool _mourningborder = true;
        private bool _auto_end = true;
        private bool _general_show = true;
        private bool _general_bigguy = false;
        private bool _override_bgm = true;

        private float _fov = 45;
        private float _length = 0;
        private Trigger _trigger = Trigger.ToEffect;

        private XCutSceneData _run_data = null;

        private GUIStyle _labelstyle = null;

        Vector2 scrollPosition = Vector2.zero;

        public static List<string> ActorList = new List<string>();

        XCutSceneWindow()
        {
            EditorApplication.playmodeStateChanged += OnQuit;
        }

        void OnEnable()
        {
            if (_run_data != null)
            {
                InnerLoad(_run_data);
            }
        }

        public void AddClip(float timeline)
        {
            switch (_type)
            {
                case XClipType.Actor:
                    {
                        XClip clip = new XActorClip(timeline);
                        _clips.Add(clip, clip);
                    } break;
                case XClipType.Player:
                    {
                        XClip clip = new XPlayerClip(timeline);
                        _clips.Add(clip, clip);
                    } break;
                case XClipType.Fx:
                    {
                        XClip clip = new XFxClip(timeline);
                        _clips.Add(clip, clip);
                    } break;
                case XClipType.Audio:
                    {
                        XClip clip = new XAudioClip(timeline);
                        _clips.Add(clip, clip);
                    } break;
                case XClipType.SubTitle:
                    {
                        XClip clip = new XSubTitleClip(timeline);
                        _clips.Add(clip, clip);
                    } break;
                case XClipType.Slash:
                    {
                        XClip clip = new XSlashClip(timeline);
                        _clips.Add(clip, clip);
                    } break;
            }
        }

        public void AddClip(XClip clip)
        {
            _clips.Add(clip, clip);
        }

        public XClip AddClip(XCutSceneClip clip)
        {
            XClip xclip = null;

            switch (clip.Type)
            {
                case XClipType.Actor:
                    {
                        xclip = new XActorClip(clip);
                    } break;
                case XClipType.Player:
                    {
                        xclip = new XPlayerClip(clip);
                    } break;
                case XClipType.Fx:
                    {
                        xclip = new XFxClip(clip);
                    } break;
                case XClipType.Audio:
                    {
                        xclip = new XAudioClip(clip);
                    } break;
                case XClipType.SubTitle:
                    {
                        xclip = new XSubTitleClip(clip);
                    } break;
                case XClipType.Slash:
                    {
                        xclip = new XSlashClip(clip);
                    } break;
            }

            if (xclip != null)
            {
                xclip.Flush();
                _clips.Add(xclip, xclip);
            }

            return xclip;
        }

        public void RemoveClip(XClip clip)
        {
            int idx = _clips.IndexOfValue(clip);
            if (idx >= 0 && idx < _clips.Count) _clips.RemoveAt(idx);
        }

        void OnDestroy()
        {
            if(EditorUtility.DisplayDialog("Save or not",
                        "Do you want to save your changes?",
                        "Ok", "No"))
            {
                Save();
            }
        }

        void OnQuit()
        {
            if (!EditorApplication.isPlaying &&
                !EditorApplication.isUpdating &&
                !EditorApplication.isCompiling &&
                !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GameObject _cameraObject = GameObject.Find(@"Main Camera");
                DestroyImmediate(_cameraObject.GetComponent<XScriptStandalone>());
            }
        }

        Trigger ConverStrToTrigger(string name)
        {
            if (Trigger.ToIdle.ToString() == name) return Trigger.ToIdle;
            if (Trigger.ToWarriorSelect.ToString() == name) return Trigger.ToWarriorSelect;
            if (Trigger.ToWarriorShow.ToString() == name) return Trigger.ToWarriorShow;
            if (Trigger.ToWarriorTurn.ToString() == name) return Trigger.ToWarriorTurn;
            if (Trigger.ToArcherSelect.ToString() == name) return Trigger.ToArcherSelect;
            if (Trigger.ToArcherShow.ToString() == name) return Trigger.ToArcherShow;
            if (Trigger.ToArcherTurn.ToString() == name) return Trigger.ToArcherTurn;
            if (Trigger.ToSorceressSelect.ToString() == name) return Trigger.ToSorceressSelect;
            if (Trigger.ToSorceressShow.ToString() == name) return Trigger.ToSorceressShow;
            if (Trigger.ToSorceressTurn.ToString() == name) return Trigger.ToSorceressTurn;
            if (Trigger.ToClericSelect.ToString() == name) return Trigger.ToClericSelect;
            if (Trigger.ToClericShow.ToString() == name) return Trigger.ToClericShow;
            if (Trigger.ToClericTurn.ToString() == name) return Trigger.ToClericTurn;
            if (Trigger.ToAcademicSelect.ToString() == name) return Trigger.ToAcademicSelect;
            if (Trigger.ToAcademicShow.ToString() == name) return Trigger.ToAcademicShow;
            if (Trigger.ToAcademicTurn.ToString() == name) return Trigger.ToAcademicTurn;
            if (Trigger.ToAssassinSelect.ToString() == name) return Trigger.ToAssassinSelect;
            if (Trigger.ToAssassinShow.ToString() == name) return Trigger.ToAssassinShow;
            if (Trigger.ToAssassinTurn.ToString() == name) return Trigger.ToAssassinTurn;
            if (Trigger.ToKaliSelect.ToString() == name) return Trigger.ToKaliSelect;
            if (Trigger.ToKaliShow.ToString() == name) return Trigger.ToKaliShow;
            if (Trigger.ToKaliTurn.ToString() == name) return Trigger.ToKaliTurn;
            if (Trigger.ToLogin.ToString() == name) return Trigger.ToLogin;
            if (Trigger.ToLoginEnter.ToString() == name) return Trigger.ToLoginEnter;
            if (Trigger.ToSelectChar.ToString() == name) return Trigger.ToSelectChar;

            return Trigger.ToEffect;
        }

        void InnerLoad(XCutSceneData data)
        {
            _run_data = data;

            _name = data.Name;
            _script = data.Script;
            _scene = data.Scene;
            _camera = Resources.Load(data.CameraClip, typeof(AnimationClip)) as AnimationClip;
            _type_mask = data.TypeMask;
            _auto_end = data.AutoEnd;
            _general_show = data.GeneralShow;
            _general_bigguy = data.GeneralBigGuy;
            _override_bgm = data.OverrideBGM;
            _mourningborder = data.Mourningborder;
            _fov = data.FieldOfView;
            _length = data.Length;
            _trigger = ConverStrToTrigger(data.Trigger);

            _clips.Clear();

            ActorList.Clear();
            ActorList.Add("None");
            foreach (XActorDataClip clip in data.Actors)
            {
                TimeChecker(clip, data);

                XClip xclip = AddClip(clip);
                ActorList.Add(xclip.Name);
            }

            foreach (XPlayerDataClip clip in data.Player)
            {
                TimeChecker(clip, data);

                XClip xclip = AddClip(clip);
                ActorList.Add(xclip.Name);
            }

            foreach (XFxDataClip clip in data.Fxs)
            {
                TimeChecker(clip, data);

                AddClip(clip);
            }

            foreach (XAudioDataClip clip in data.Audios)
            {
                TimeChecker(clip, data);

                AddClip(clip);
            }

            foreach (XSubTitleDataClip clip in data.SubTitle)
            {
                TimeChecker(clip, data);

                AddClip(clip);
            }

            foreach (XSlashDataClip clip in data.Slash)
            {
                TimeChecker(clip, data);

                AddClip(clip);
            }

            if (_open_scene && _scene != null && _scene.Length != 0)
            {
                //string current = EditorApplication.currentScene;
                Scene scene = EditorSceneManager.GetActiveScene();
                //string current = EditorApplication.currentScene;

                if (scene.name.Length > 0 && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    return;
                }
                else
                {
                    EditorSceneManager.OpenScene(_scene + ".unity");
                }

                _open_scene = false;
            }
        }

        void TimeChecker(XCutSceneClip clip, XCutSceneData data)
        {
            if (clip.TimeLineAt >= data.TotalFrame)
            {
                EditorUtility.DisplayDialog("Confirm your configuration.",
                            "clip play-at time bigger than cutscene length!",
                            "Ok");

                throw new Exception("clip time bigger than cutscene time!");
            }
        }

        void Load(XCutSceneData data)
        {
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;

            InnerLoad(data);
        }

        XCutSceneData GetCurrentData()
        {
            if (_name == null || _name.Length == 0 || _camera == null) return null;

            XCutSceneData data = new XCutSceneData();
            data.CameraClip = XClip.LocateRes(_camera);
            data.Name = _name;
            data.Script = _script;
            data.Scene = _scene;
            data.TotalFrame = _camera.length * 30.0f;
            data.TypeMask = _type_mask;
            data.AutoEnd = _auto_end;
            data.GeneralShow = _general_show;
            data.GeneralBigGuy = _general_bigguy;
            data.OverrideBGM = _override_bgm;
            data.Mourningborder = _mourningborder;
            data.FieldOfView = _fov;
            data.Length = _length;
            data.Trigger = _trigger.ToString();

            foreach (XClip clip in _clips.Values)
            {
                if (clip.Valid)
                {
                    if (_camera != null)
                    {
                        clip.Dump();
                        switch (clip.ClipType)
                        {
                            case XClipType.Actor: data.Actors.Add(clip.CutSceneClip as XActorDataClip); break;
                            case XClipType.Player: data.Player.Add(clip.CutSceneClip as XPlayerDataClip); break;
                            case XClipType.Audio: data.Audios.Add(clip.CutSceneClip as XAudioDataClip); break;
                            case XClipType.Fx: data.Fxs.Add(clip.CutSceneClip as XFxDataClip); break;
                            case XClipType.SubTitle: data.SubTitle.Add(clip.CutSceneClip as XSubTitleDataClip); break;
                            case XClipType.Slash: data.Slash.Add(clip.CutSceneClip as XSlashDataClip); break;
                        }
                    }
                }
            }

            return data;
        }

        void Save()
        {
            XCutSceneData data = GetCurrentData();
            if (data != null)
            {
                _run_data = data;

                string file = XEditorPath.Cts + data.Name + ".txt";
                XDataIO<XCutSceneData>.singleton.SerializeData(file, data);
            }
        }
        //GameObject o = null;
        void OnGUI()
        {
            if (_labelstyle == null)
            {
                _labelstyle = new GUIStyle(EditorStyles.boldLabel);
                _labelstyle.fontSize = 11;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            EditorGUILayout.Space();
            GUILayout.Label("CutScene Editor:", _labelstyle);
            EditorGUILayout.Space();
            _name = EditorGUILayout.TextField("Name", _name);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scene", _scene);
            if (GUILayout.Button("Browser", GUILayout.MaxWidth(80)))
            {
                string file = EditorUtility.OpenFilePanel("Select unity scene file", XEditorPath.Sce, "unity");

                if (file.Length != 0)
                {
                    file = file.Remove(file.LastIndexOf("."));
                    _scene = file.Remove(0, file.IndexOf(XEditorPath.Sce));

                    Scene scene = EditorSceneManager.GetActiveScene();
                    //string current = EditorApplication.currentScene;

                    if (scene.name.Length == 0 || !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(_scene + ".unity");
                        //EditorApplication.OpenScene(_scene + ".unity");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            _auto_end = EditorGUILayout.Toggle("Auto End", _auto_end);
            EditorGUILayout.Space();
            _general_show = EditorGUILayout.Toggle("General Clip", _general_show);
            if (_general_show)
            {
                _general_bigguy = EditorGUILayout.Toggle("General Big Guy", _general_bigguy);

                if (_general_bigguy)
                    _camera = Resources.Load("Animation/Main_Camera/Cut_Scene/cutscene_generalshow_bigguy", typeof(AnimationClip)) as AnimationClip;
                else
                    _camera = Resources.Load("Animation/Main_Camera/Cut_Scene/cutscene_generalshow", typeof(AnimationClip)) as AnimationClip;

                EditorGUILayout.ObjectField("Camera Clip", _camera, typeof(AnimationClip), true);
            }
            else
                _camera = EditorGUILayout.ObjectField("Camera Clip", _camera, typeof(AnimationClip), true) as AnimationClip;
            if (_camera != null)
            {
                _length = _camera.length;
                EditorGUILayout.LabelField("CutScene Length", _length.ToString("F3") + "s" + "\t" + (_length * 30.0f).ToString("F1") + "(frame)");
            }

            EditorGUILayout.Space();
            _type_mask = (int)(EntitySpecies)EditorGUILayout.EnumMaskField("Type Mask", (EntitySpecies)_type_mask);
            _mourningborder = EditorGUILayout.Toggle("Mourningborder", _mourningborder);

            EditorGUILayout.Space();
            _fov = EditorGUILayout.FloatField("FieldOfVeiw", _fov);
            _trigger = (Trigger)EditorGUILayout.EnumPopup("Trigger", _trigger);
            EditorGUILayout.Space();
            _override_bgm = EditorGUILayout.Toggle("Override BGM", _override_bgm);
            EditorGUILayout.Space();

            UpdateScript();

            GUILayout.Label("TimeLine:", _labelstyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            _type = (XClipType)EditorGUILayout.EnumPopup("Add Clip", _type);
            if (GUILayout.Button(_content_add, GUILayout.MaxWidth(25), GUILayout.MaxHeight(15)))
            {
                if (_camera != null && _name != null && _name.Length > 0)
                {
                    XCutSceneAddationWindow window = EditorWindow.GetWindow<XCutSceneAddationWindow>(@"Timeline At:");
                    window._total_frame = _camera.length * 30.0f;
                    window._clip = null;
                }
                else
                {
                    EditorUtility.DisplayDialog("Confirm your selection.",
                        "Please select camera clip or name the cutscene",
                        "Ok");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            ActorList.Clear();
            ActorList.Add("None");

            foreach (XClip clip in _clips.Values)
            {
                if (clip.Valid)
                {
                    if (_camera != null)
                    {
                        clip.TimeLineTotal = _camera.length * 30.0f;
                        clip.OnGUI(GetCurrentData());

                        if (clip.ClipType == XClipType.Actor || clip.ClipType == XClipType.Player)
                        {
                            int all = ActorList.FindAll(delegate(string s)
                            {
                                return s == clip.Name;
                            }).Count + 1;

                            if(all > 1)
                                ActorList.Add(clip.Name + " " + all);
                            else
                                ActorList.Add(clip.Name);
                        }
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Run"))
            {
                if (_name != null && _name.Length > 0 && _camera != null && !EditorApplication.isPlaying)
                {
                    GameObject _cameraObject = GameObject.Find(@"Main Camera");
                    XScriptStandalone xss = _cameraObject.AddComponent<XScriptStandalone>();
                    _run_data = GetCurrentData();
                    xss._cut_scene_data = _run_data;

                    EditorApplication.ExecuteMenuItem("Edit/Play");
                }
            }

            if (GUILayout.Button("Pause"))
            {
                if (EditorApplication.isPlaying)
                    EditorApplication.isPaused = !EditorApplication.isPaused;
            }

            if (GUILayout.Button("Quit"))
            {
                if (EditorApplication.isPlaying)
                    EditorApplication.isPlaying = false;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                Save();
            }

            if (GUILayout.Button("Open"))
            {
                _file = EditorUtility.OpenFilePanel("Select cutscene file", XEditorPath.Cts, "txt");

                if (_file.Length != 0)
                {
                    _open_scene = true;
                    Load(XDataIO<XCutSceneData>.singleton.DeserializeData(_file.Substring(_file.IndexOf("Assets/"))));
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        void UpdateScript()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Script", _script);

            if (GUILayout.Button("Browser", GUILayout.MaxWidth(80)))
            {
                _script = ScriptFileBrowser();
            }

            EditorGUILayout.EndHorizontal();
        }

        private string ScriptFileBrowser()
        {
            string file = EditorUtility.OpenFilePanel("Select script file", XSkillScriptGen.singleton.ScriptPath, "cs");

            if (file == null || file.Length == 0) return "";

            file = file.Remove(file.LastIndexOf('.'));
            return file.Substring(file.LastIndexOf('/') + 1);
        }
    }
}
