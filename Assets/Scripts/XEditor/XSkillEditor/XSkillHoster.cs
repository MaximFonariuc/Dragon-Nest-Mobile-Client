#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using XUtliPoolLib;
using UnityEditorInternal;

namespace XEditor
{
	public class XSkillHoster : MonoBehaviour
	{
        public class XChargeSetting
        {
            public XChargeData data;
            public float offset;
        }

        [SerializeField]
        private XSkillData _xData = null;
        [SerializeField]
        private XSkillDataExtra _xDataExtra = null;
        [SerializeField]
        private XEditorData _xEditorData = null;
        [SerializeField]
        private XConfigData _xConfigData = null;

        private XSkillData _xOuterData = null;

        private enum DummyState { Idle, Move, Fire };
        private DummyState _state = DummyState.Idle;

        private Animator _ator = null;
        private XSkillCamera _camera = null;

        private int _combined_id = 0;

        private float _to = 0;
        private float _from = 0;

        private float _delta = 0;
        private float _fire_time = 0;
        private float _time_offset = 0;

        private string _trigger = null;
        private bool _execute = false;
        private bool _anim_init = false;
        //private bool _effectual = false;
        private bool _freezed = false;
        private XSkillCharge _update = null;
        private XSkillManipulate _manipulate = null;

        GameObject _target = null;
        List<GameObject> _mob_unit = new List<GameObject>();

        AudioSource _audio_motion = null;
        AudioSource _audio_action = null;
        AudioSource _audio_skill = null;
        AudioSource _audio_behit = null;

        XFmod _emitter = null;

        private XCameraShake _xcamera_effect = null;

        private List<uint> _combinedToken = new List<uint>();
        private List<uint> _presentToken = new List<uint>();
        private List<uint> _logicalToken = new List<uint>();
        private List<XSkillData> _combinedlist = new List<XSkillData>();

        private XSkillData _current = null;

        private float _last_swype_time = 0;
        private int _jaCount = 0;
        private bool _skill_when_move = false;

        public float defaultFov = 45;

        [HideInInspector]
        public XSkillCamera Camera { get { return _camera; } }
        [HideInInspector]
        public GameObject Target { get { return _target; } }
        [HideInInspector]
        public XFmod Emitter { get { return _emitter; } set { _emitter = value; } }
        [HideInInspector]
        public static bool Quit { get; set; }
        [HideInInspector]
        public static XSerialized<XSkillData> sData = new XSerialized<XSkillData>();
        [HideInInspector]
        public static XSerialized<XEditorData> sEditorData = new XSerialized<XEditorData>();
        [HideInInspector]
        public static XSerialized<XConfigData> sConfigData = new XSerialized<XConfigData>();
        [HideInInspector]
        public List<XSkillData> ComboSkills = new List<XSkillData>();
        [HideInInspector]
        public int nHotID = 0;
        [HideInInspector]
        public Vector3 nResultForward = Vector3.zero;
        [HideInInspector]
        public Transform ShownTransform = null;
        [HideInInspector]
        public AnimatorOverrideController oVerrideController = null;
        [HideInInspector]
        public float ir = 0;
        [HideInInspector]
        public float or = 0;

        [HideInInspector]
        public Vector3 ControlDir = Vector3.zero;

        private XEntityPresentation.RowData _present_data = null;

        void Awake()
        {
            ShownTransform = transform;
        }

        void Start()
        {
            _state = DummyState.Idle;

            if (oVerrideController == null) BuildOverride();

            _camera = new XSkillCamera(gameObject);

            _camera.Initialize();
            _camera.UnityCamera.fieldOfView = defaultFov;

            AudioListener audiolistener = _camera.UnityCamera.gameObject.GetComponent<AudioListener>();
            Component.DestroyImmediate(audiolistener);

            _camera.UnityCamera.gameObject.AddComponent<FMOD_Listener>();

            Light light = _camera.UnityCamera.gameObject.AddComponent<Light>() as Light;
            light.type = LightType.Directional;
            light.intensity = 0.5f;

            RebuildSkillAniamtion();

            Application.targetFrameRate = 60;

            if (!string.IsNullOrEmpty(SkillData.CameraPostEffect.Effect))
            {
                //ImageEffectBase iebase = _camera.UnityCamera.gameObject.AddComponent<RadialBlur>() as ImageEffectBase;
                //ImageEffectBase iebase = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(_camera.UnityCamera.gameObject, "Assets/Scripts/XEditor/XSkillEditor/XSkillHoster.cs (135,42)", SkillData.CameraPostEffect.Effect) as ImageEffectBase;
                //iebase.shader = SkillDataExtra.PostEffectEx.Shader;

                Behaviour o = _camera.UnityCamera.GetComponent(SkillData.CameraPostEffect.Effect) as Behaviour;
                if(o!=null)
                o.enabled = false;
            }
        }

        public void RebuildSkillAniamtion()
        {
            AnimationClip clip = Resources.Load(SkillData.ClipName) as AnimationClip;

            if (oVerrideController == null) BuildOverride();

            if (SkillData.TypeToken == 0)
            {
                string motion = XSkillData.JaOverrideMap[SkillData.SkillPosition];
                oVerrideController[motion] = clip;

                foreach (XJADataExtraEx ja in SkillDataExtra.JaEx)
                {
                    if (SkillData.SkillPosition == 15)  //ToJA_QTE
                        continue;

                    if (ja.Next != null && ja.Next.Name.Length > 0) oVerrideController[XSkillData.JaOverrideMap[ja.Next.SkillPosition]] = Resources.Load(ja.Next.ClipName) as AnimationClip;
                    if (ja.Ja != null && ja.Ja.Name.Length > 0) oVerrideController[XSkillData.JaOverrideMap[ja.Ja.SkillPosition]] = Resources.Load(ja.Ja.ClipName) as AnimationClip;
                }
            }
            else if (SkillData.TypeToken == 3)
            {
                for (int i = 0; i < SkillData.Combined.Count; i++)
                {
                    oVerrideController[XSkillData.CombinedOverrideMap[i]] = SkillDataExtra.CombinedEx[i].Clip;
                }
            }
            else
            {
                oVerrideController["Art"] = clip;
            }

            _present_data = XAnimationLibrary.AssociatedAnimations((uint)_xConfigData.Player);

            oVerrideController["Idle"] = Resources.Load("Animation/" + _present_data.AnimLocation + _present_data.AttackIdle) as AnimationClip;
            oVerrideController["Run"] = Resources.Load("Animation/" + _present_data.AnimLocation + _present_data.AttackRun) as AnimationClip;
            oVerrideController["Walk"] = Resources.Load("Animation/" + _present_data.AnimLocation + _present_data.AttackWalk) as AnimationClip;
        }

        private void BuildOverride()
        {
            oVerrideController = new AnimatorOverrideController();

            _ator = GetComponent<Animator>();
            if (_ator == null)
            {
                _ator = gameObject.AddComponent<Animator>();
                _ator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/Resources/Controller/XAnimator.controller");
            }
            oVerrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
            _ator.runtimeAnimatorController = oVerrideController;

        }

        void OnDrawGizmos()
        {
            DrawManipulationFileds();

            if (nHotID < 0 || CurrentSkillData.Result == null || nHotID >= CurrentSkillData.Result.Count) return;

            if (ShownTransform == null) ShownTransform = transform;

            float offset_x = CurrentSkillData.Result[nHotID].LongAttackEffect ? CurrentSkillData.Result[nHotID].LongAttackData.At_X : CurrentSkillData.Result[nHotID].Offset_X;
            float offset_z = CurrentSkillData.Result[nHotID].LongAttackEffect ? CurrentSkillData.Result[nHotID].LongAttackData.At_Z : CurrentSkillData.Result[nHotID].Offset_Z;

            Vector3 offset = ShownTransform.rotation * new Vector3(offset_x, 0, offset_z);

            Color defaultColor = Gizmos.color;
            Gizmos.color = Color.red;

            Matrix4x4 defaultMatrix = Gizmos.matrix;
            if (ShownTransform == transform)
            {
                ShownTransform.position += offset;
                Gizmos.matrix = ShownTransform.localToWorldMatrix;
                ShownTransform.position -= offset;
            }
            else    //bullet
                Gizmos.matrix = ShownTransform.localToWorldMatrix;

            if (CurrentSkillData.Result[nHotID].LongAttackEffect)
            {
                if (CurrentSkillData.Result[nHotID].LongAttackData.TriggerAtEnd)
                {
                    float m_Theta = 0.01f;

                    Vector3 beginPoint = Vector3.zero;
                    Vector3 firstPoint = Vector3.zero;

                    for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                    {
                        float x = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Cos(theta);
                        float z = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Sin(theta);
                        Vector3 endPoint = new Vector3(x, 0, z);
                        if (theta == 0)
                        {
                            firstPoint = endPoint;
                        }
                        else
                        {
                            Gizmos.DrawLine(beginPoint, endPoint);
                        }
                        beginPoint = endPoint;
                    }

                    Gizmos.DrawLine(firstPoint, beginPoint);

                    if (CurrentSkillData.Result[nHotID].Low_Range > 0)
                    {
                        m_Theta = 0.01f;

                        beginPoint = Vector3.zero;
                        firstPoint = Vector3.zero;

                        for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                        {
                            float x = CurrentSkillData.Result[nHotID].Low_Range / ShownTransform.localScale.y * Mathf.Cos(theta);
                            float z = CurrentSkillData.Result[nHotID].Low_Range / ShownTransform.localScale.y * Mathf.Sin(theta);
                            Vector3 endPoint = new Vector3(x, 0, z);
                            if (theta == 0)
                            {
                                firstPoint = endPoint;
                            }
                            else
                            {
                                Gizmos.DrawLine(beginPoint, endPoint);
                            }
                            beginPoint = endPoint;
                        }

                        Gizmos.DrawLine(firstPoint, beginPoint);
                    }
                }
                else
                {
                    if (CurrentSkillData.Result[nHotID].LongAttackData.Type == XResultBulletType.Ring)
                    {
                        float m_Theta = 0.01f;

                        Vector3 beginPoint = Vector3.zero;
                        Vector3 firstPoint = Vector3.zero;

                        for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                        {
                            float x = ir / ShownTransform.localScale.y * Mathf.Cos(theta);
                            float z = ir / ShownTransform.localScale.y * Mathf.Sin(theta);
                            Vector3 endPoint = new Vector3(x, 0, z);
                            if (theta == 0)
                            {
                                firstPoint = endPoint;
                            }
                            else
                            {
                                Gizmos.DrawLine(beginPoint, endPoint);
                            }
                            beginPoint = endPoint;
                        }

                        Gizmos.DrawLine(firstPoint, beginPoint);

                        Vector3 beginPoint2 = Vector3.zero;
                        Vector3 firstPoint2 = Vector3.zero;

                        for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                        {
                            float x = or / ShownTransform.localScale.y * Mathf.Cos(theta);
                            float z = or / ShownTransform.localScale.y * Mathf.Sin(theta);
                            Vector3 endPoint = new Vector3(x, 0, z);
                            if (theta == 0)
                            {
                                firstPoint2 = endPoint;
                            }
                            else
                            {
                                Gizmos.DrawLine(beginPoint2, endPoint);
                            }
                            beginPoint2 = endPoint;
                        }

                        Gizmos.DrawLine(firstPoint2, beginPoint2);
                    }
                    else
                    {
                        float m_Theta = 0.01f;

                        Vector3 beginPoint = Vector3.zero;
                        Vector3 firstPoint = Vector3.zero;

                        for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                        {
                            float x = CurrentSkillData.Result[nHotID].LongAttackData.Radius / ShownTransform.localScale.y * Mathf.Cos(theta);
                            float z = CurrentSkillData.Result[nHotID].LongAttackData.Radius / ShownTransform.localScale.y * Mathf.Sin(theta);
                            Vector3 endPoint = new Vector3(x, 0, z);
                            if (theta == 0)
                            {
                                firstPoint = endPoint;
                            }
                            else
                            {
                                Gizmos.DrawLine(beginPoint, endPoint);
                            }
                            beginPoint = endPoint;
                        }

                        Gizmos.DrawLine(firstPoint, beginPoint);
                    }
                }
            }
            else
            {
                if (CurrentSkillData.Result[nHotID].Sector_Type)
                {
                    float m_Theta = 0.01f;

                    Vector3 beginPoint = Vector3.zero;
                    Vector3 firstPoint = Vector3.zero;

                    for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                    {
                        float x = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Cos(theta);
                        float z = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Sin(theta);
                        Vector3 endPoint = new Vector3(x, 0, z);
                        if (theta == 0)
                        {
                            firstPoint = endPoint;
                        }
                        else
                        {
                            Gizmos.DrawLine(beginPoint, endPoint);
                        }
                        beginPoint = endPoint;
                    }

                    Gizmos.DrawLine(firstPoint, beginPoint);

                    if (CurrentSkillData.Result[nHotID].Low_Range > 0)
                    {
                        m_Theta = 0.01f;

                        beginPoint = Vector3.zero;
                        firstPoint = Vector3.zero;

                        for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                        {
                            float x = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Cos(theta);
                            float z = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Sin(theta);
                            Vector3 endPoint = new Vector3(x, 0, z);
                            if (theta == 0)
                            {
                                firstPoint = endPoint;
                            }
                            else
                            {
                                Gizmos.DrawLine(beginPoint, endPoint);
                            }
                            beginPoint = endPoint;
                        }

                        Gizmos.DrawLine(firstPoint, beginPoint);
                    }
                }
                else
                {
                    Vector3 fr = new Vector3(CurrentSkillData.Result[nHotID].Scope / 2.0f, 0, CurrentSkillData.Result[nHotID].Range / 2.0f);
                    Vector3 fl = new Vector3(CurrentSkillData.Result[nHotID].Scope / 2.0f, 0, CurrentSkillData.Result[nHotID].Rect_HalfEffect ? 0 : (-CurrentSkillData.Result[nHotID].Range / 2.0f));
                    Vector3 br = new Vector3(-CurrentSkillData.Result[nHotID].Scope / 2.0f, 0, CurrentSkillData.Result[nHotID].Range / 2.0f);
                    Vector3 bl = new Vector3(-CurrentSkillData.Result[nHotID].Scope / 2.0f, 0, CurrentSkillData.Result[nHotID].Rect_HalfEffect ? 0 : (-CurrentSkillData.Result[nHotID].Range / 2.0f));

                    Gizmos.DrawLine(fr, fl);
                    Gizmos.DrawLine(fl, bl);
                    Gizmos.DrawLine(bl, br);
                    Gizmos.DrawLine(br, fr);
                }
            }

            Gizmos.matrix = defaultMatrix;
            Gizmos.color = defaultColor;
        }

        void DrawManipulationFileds()
        {
            if (_state == DummyState.Fire)
            {
                if (_manipulate != null)
                {
                    foreach (XManipulationData data in _manipulate.Set.Values)
                    {
                        Vector3 offset = transform.rotation * new Vector3(data.OffsetX, 0, data.OffsetZ);

                        Color defaultColor = Gizmos.color;
                        Gizmos.color = Color.red;

                        Matrix4x4 defaultMatrix = Gizmos.matrix;
                        transform.position += offset;
                        Gizmos.matrix = transform.localToWorldMatrix;
                        transform.position -= offset;

                        float m_Theta = 0.01f;

                        Vector3 beginPoint = Vector3.zero;
                        Vector3 firstPoint = Vector3.zero;

                        for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                        {
                            float x = data.Radius / transform.localScale.y * Mathf.Cos(theta);
                            float z = data.Radius / transform.localScale.y * Mathf.Sin(theta);
                            Vector3 endPoint = new Vector3(x, 0, z);
                            if (theta == 0)
                            {
                                firstPoint = endPoint;
                            }
                            else
                            {
                                if (Vector3.Angle(endPoint, transform.forward) < data.Degree * 0.5f)
                                    Gizmos.DrawLine(beginPoint, endPoint);
                            }
                            beginPoint = endPoint;
                        }

                        if (data.Degree == 360)
                            Gizmos.DrawLine(firstPoint, beginPoint);
                        else
                        {
                            Gizmos.DrawLine(Vector3.zero, XCommon.singleton.HorizontalRotateVetor3(transform.forward, data.Degree * 0.5f, true) * (data.Radius / transform.localScale.y));
                            Gizmos.DrawLine(Vector3.zero, XCommon.singleton.HorizontalRotateVetor3(transform.forward, -data.Degree * 0.5f, true) * (data.Radius / transform.localScale.y));
                        }

                        Gizmos.matrix = defaultMatrix;
                        Gizmos.color = defaultColor;
                    }
                }
            }
            else
            {
                if (_xData.Manipulation != null)
                {
                    foreach (XManipulationData data in _xData.Manipulation)
                    {
                        if (data.Radius <= 0 || !_xDataExtra.ManipulationEx[data.Index].Present) continue;

                        Vector3 offset = transform.rotation * new Vector3(data.OffsetX, 0, data.OffsetZ);

                        Color defaultColor = Gizmos.color;
                        Gizmos.color = Color.red;

                        Matrix4x4 defaultMatrix = Gizmos.matrix;
                        transform.position += offset;
                        Gizmos.matrix = transform.localToWorldMatrix;
                        transform.position -= offset;

                        float m_Theta = 0.01f;

                        Vector3 beginPoint = Vector3.zero;
                        Vector3 firstPoint = Vector3.zero;

                        for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                        {
                            float x = data.Radius / transform.localScale.y * Mathf.Cos(theta);
                            float z = data.Radius / transform.localScale.y * Mathf.Sin(theta);
                            Vector3 endPoint = new Vector3(x, 0, z);
                            if (theta == 0)
                            {
                                firstPoint = endPoint;
                            }
                            else
                            {
                                if (Vector3.Angle(endPoint, transform.forward) < data.Degree * 0.5f)
                                    Gizmos.DrawLine(beginPoint, endPoint);
                            }
                            beginPoint = endPoint;
                        }

                        if (data.Degree == 360)
                            Gizmos.DrawLine(firstPoint, beginPoint);
                        else
                        {
                            Gizmos.DrawLine(Vector3.zero, XCommon.singleton.HorizontalRotateVetor3(transform.forward, data.Degree * 0.5f, true) * (data.Radius / transform.localScale.y));
                            Gizmos.DrawLine(Vector3.zero, XCommon.singleton.HorizontalRotateVetor3(transform.forward, -data.Degree * 0.5f, true) * (data.Radius / transform.localScale.y));
                        }

                        Gizmos.matrix = defaultMatrix;
                        Gizmos.color = defaultColor;
                    }
                }
            }
        }

        private float _action_framecount = 0;
        private Rect _rect = new Rect(10, 10, 150, 20);

        void OnGUI()
        {
            GUI.Label(_rect, "Action Frame: " + _action_framecount);
        }

        private int _comboskill_index = 0;

        void Update()
        {
            XTimerMgr.singleton.Update(Time.deltaTime);
            XGesture.singleton.Update();
            XBulletMgr.singleton.Update(Time.deltaTime);

            if (_update != null)
            {
                if (_update.Update(Time.deltaTime)) _update = null;
            }

            if (_manipulate != null)
            {
                _manipulate.Update(Time.deltaTime);
            }

            ControlDir = Vector3.zero;

            int nh = 0; int nv = 0;

            if (Input.GetKey(KeyCode.W)) nv++;
            if (Input.GetKey(KeyCode.S)) nv--;
            if (Input.GetKey(KeyCode.A)) nh--;
            if (Input.GetKey(KeyCode.D)) nh++;

            Vector3 h = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 v = SceneView.lastActiveSceneView != null ? SceneView.lastActiveSceneView.rotation * Vector3.forward : Vector3.forward; v.y = 0;
			if (Vector3.Angle (Vector3.forward, v) > 90)
				nh = -nh;

            Vector3.OrthoNormalize(ref v, ref up, ref h);

            if (_state != DummyState.Fire)
            {
                _action_framecount = 0;
                _comboskill_index = 0;

                //fire skill
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (_xData.TypeToken == 1 && ComboSkills.Count > 0) oVerrideController["Art"] = Resources.Load(_xData.ClipName) as AnimationClip;
                    _xOuterData = _xData;
                    _combinedlist.Clear();
                    _combined_id = 0;
                    foreach (XCombinedDataExtraEx data in _xDataExtra.CombinedEx)
                        _combinedlist.Add(data.Skill);

                    Fire();
                }
                else 
                {
                    if (nh != 0 || nv != 0)
                    {
                        ControlDir = h * nh + v * nv;
                        Move(ControlDir);

                        if (_state != DummyState.Move) _trigger = "ToMove";
                        _state = DummyState.Move;
                    }
                    else
                    {
                        if (_state == DummyState.Move) 
                            _trigger = "ToStand";
                        _state = DummyState.Idle;
                    }
                }
            }
            else
            {
                if (_execute || _xOuterData.TypeToken == 3)
                {
                    if (!_freezed) _delta += Time.deltaTime;
                    _action_framecount = _delta / XCommon.singleton.FrameStep;

                    if (_delta > (_xOuterData.TypeToken == 3 ? _xOuterData.Time : _current.Time))
                    {
                        StopFire();
                    }
                    else
                    {
                        if (nh != 0 || nv != 0)
                        {
                            ControlDir = h * nh + v * nv;
                            if (CanAct(ControlDir))
                            {
                                Move(ControlDir);
                            }
                        }
                        else if (_skill_when_move)
                        {
                            _trigger = "ToStand";
                            _skill_when_move = false;
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (_comboskill_index < ComboSkills.Count )
                        {
                            XSkillData data = ComboSkills[_comboskill_index];

                            if (CanReplacedBy(data))
                            {
                                _comboskill_index++;
                                StopFire();

                                _xOuterData = data;
                                _current = data;

                                if (data.TypeToken != 3)
                                {
                                    oVerrideController["Art"] = XResourceLoaderMgr.singleton.GetSharedResource<AnimationClip>(data.ClipName, ".anim");
                                    _trigger = "ToArtSkill";
                                }
                                else
                                {
                                    _combinedlist.Clear();
                                    for (int i = 0; i < data.Combined.Count; i++)
                                    {
                                        XSkillData x = XResourceLoaderMgr.singleton.GetData<XSkillData>("SkillPackage/" + XAnimationLibrary.AssociatedAnimations((uint)_xConfigData.Player).SkillLocation + data.Combined[i].Name, ".txt");
                                        AnimationClip c = Resources.Load(x.ClipName) as AnimationClip;
                                        oVerrideController[XSkillData.CombinedOverrideMap[i]] = c;
                                        _combinedlist.Add(x);
                                    }
                                    _trigger = XSkillData.Combined_Command[0];
                                    Combined(0);
                                }

                                _state = DummyState.Fire;
                                _fire_time = Time.time;
                                _delta = 0;
                                if (_ator != null)
                                    _ator.speed = 0;
                            }
                        }
                    }
                }

                if (_anim_init)
                    Execute();

                _anim_init = false;
            }
        }

        void OnApplicationQuit()
        {
            Quit = true;

            sData.Set(_xData);
            sEditorData.Set(_xEditorData);
            sConfigData.Set(_xConfigData);

            ir = 0;
            or = 0;
        }

        private float _move_follow_speed = 0;
        private void CameraSkillRotate()
        {
            if (_state == DummyState.Fire)
            {
                float move_follow_speed_basic = _xOuterData.CameraTurnBack * Time.deltaTime;

                Vector3 playerLookat = GetRotateTo();
                Vector3 viewForward = XCommon.singleton.Horizontal(_camera.UnityCamera.GetComponent<Camera>().transform.forward);

                float sin = Mathf.Sin(Mathf.Deg2Rad * Vector3.Angle(playerLookat, viewForward) * 0.5f);
                float move_follow_speed_target = move_follow_speed_basic * sin;

                if (XCommon.singleton.Clockwise(playerLookat, viewForward))
                    move_follow_speed_target *= -1;

                _move_follow_speed += (move_follow_speed_target - _move_follow_speed) * Mathf.Min(1.0f, Time.deltaTime * 6);
                _camera.YRotate(_move_follow_speed);
            }
        }

        void LateUpdate()
        {
            _camera.UnityCamera.fieldOfView = defaultFov;
            CameraSkillRotate();
            _camera.PostUpdate(Time.deltaTime);

            //face to
            UpdateRotation();

            if (_xcamera_effect != null) _xcamera_effect.Update(Time.deltaTime);

            if (null != _trigger && _ator != null && !_ator.IsInTransition(0))
            {
                if ("ToStand" != _trigger && "ToMove" != _trigger && "EndSkill" != _trigger && "ToUltraShow" != _trigger)
                    _anim_init = true;

                _ator.speed = 1;

                if (SkillData.TypeToken == 3)
                {
                    int i = 0;
                    for (; i < XSkillData.Combined_Command.Length; i++)
                    {
                        if (_trigger == XSkillData.Combined_Command[i]) break;
                    }

                    if (i < XSkillData.Combined_Command.Length)
                        _ator.Play(XSkillData.CombinedOverrideMap[i], 1, _time_offset);
                    else
                        _ator.SetTrigger(_trigger);
                }
                else
                {
                    _ator.SetTrigger(_trigger);
                }

                _trigger = null;
            }
        }

        public void FetchDataBack()
        {
            _xData = sData.Get();
            _xEditorData = sEditorData.Get();
            _xConfigData = sConfigData.Get();

            XDataBuilder.singleton.HotBuild(this, _xConfigData);
            XDataBuilder.singleton.HotBuildEx(this, _xConfigData);
        }

        public XSkillData SkillData
        { 
            get 
            {
                if (_xData == null) _xData = new XSkillData();
                return _xData;
            }
            set
            {
                //for load data from file.
                _xData = value;
            }
        }

        public XSkillData CurrentSkillData
        {
            get 
            {
                return _state == DummyState.Fire ? _current : SkillData; 
            }
        }

        public XConfigData ConfigData
        {
            get
            {
                if (_xConfigData == null) _xConfigData = new XConfigData();
                return _xConfigData;
            }
            set
            {
                //for load data from file.
                _xConfigData = value;
            }
        }

        public XEditorData EditorData
        {
            get
            {
                if (_xEditorData == null) _xEditorData = new XEditorData();
                return _xEditorData;
            }
        }

        public XSkillDataExtra SkillDataExtra
        {
            get
            {
                if (_xDataExtra == null)_xDataExtra = new XSkillDataExtra();
                return _xDataExtra;
            }
        }

        private void Move(Vector3 dir)
        {
            PrepareRotation(dir, _xConfigData.RotateSpeed);
            transform.Translate(dir * Time.deltaTime * ConfigData.Speed, Space.World);
        }

        public void PrepareRotation(Vector3 targetDir, float speed)
        {
            Vector3 from = transform.forward;

            _from = YRotation(from);
            float angle = Vector3.Angle(from, targetDir);

            if (XCommon.singleton.Clockwise(from, targetDir))
            {
                _to = _from + angle;
            }
            else
            {
                _to = _from - angle;
            }

            rotate_speed = speed;
        }

        public Vector3 GetRotateTo()
        {
            return XCommon.singleton.FloatToAngle(_to);
        }

        private float rotate_speed = 0;
        private void UpdateRotation()
        {
            if (_from != _to)
            {
                _from += (_to - _from) * Mathf.Min(1.0f, Time.deltaTime * rotate_speed);
                transform.rotation = Quaternion.Euler(0, _from, 0);
            }
        }

        private float YRotation(Vector3 dir)
        {
            float r = Vector3.Angle(Vector3.forward, dir);

            if (XCommon.singleton.Clockwise(Vector3.forward, dir))
            {
                return r;
            }
            else
            {
                return 360.0f - r;
            }
        }

        private List<HashSet<XSkillHit>> _hurt_target = new List<HashSet<XSkillHit>>();
        private void AddHurtTarget(XSkillData data, XSkillHit id, int triggerTime)
        {
            if (!data.Result[triggerTime].Loop && /*for multiple trigger end*/!data.Result[triggerTime].LongAttackEffect)
                _hurt_target[triggerTime].Add(id);
        }

        private bool IsHurtEntity(XSkillHit id, int triggerTime)
        {
            /*
             * this section not as same as client shows
             * but in editor mode just using it for simple.
             */
            return triggerTime < _hurt_target.Count ? _hurt_target[triggerTime].Contains(id) : false;
        }

        private void MainCoreExecute()
        {
            if (_xOuterData.Fx != null)
            {
                foreach (XFxData data in _xOuterData.Fx)
                {
                    AddedCombinedToken(XTimerMgr.singleton.SetTimer(data.At, Fx, data));
                }
            }

            if (_xOuterData.Audio != null)
            {
                foreach (XAudioData data in _xOuterData.Audio)
                {
                    AddedCombinedToken(XTimerMgr.singleton.SetTimer(data.At, Audio, data));
                }
            }

            if (_xOuterData.CameraEffect != null)
            {
                foreach (XCameraEffectData data in _xOuterData.CameraEffect)
                {
                    AddedCombinedToken(XTimerMgr.singleton.SetTimer(data.At, Shake, data));
                }
            }

            if (!string.IsNullOrEmpty(_xOuterData.CameraMotion.Motion3D))
            {
                AddedCombinedToken(XTimerMgr.singleton.SetTimer(_xOuterData.CameraMotion.At, CameraMotion, _xOuterData.CameraMotion));
            }

            if (!string.IsNullOrEmpty(_xOuterData.CameraPostEffect.Effect))
            {
                AddedCombinedToken(XTimerMgr.singleton.SetTimer(_xOuterData.CameraPostEffect.At, CameraPostEffect, _xOuterData.CameraPostEffect));
            }
        }

        private void Execute()
        {
            if(EditorData.XFrameByFrame) Debug.Break();

            //_effectual = false;
            _freezed = false;
            _execute = true;

            _jaCount = 0;

            int count = 0;
            nHotID = 0;

            _fire_time = Time.time;

            if(_xEditorData.XAutoSelected)
                Selection.activeObject = gameObject;

            _hurt_target.Clear();

            float play_offset = _xOuterData.TypeToken == 3 ? _xOuterData.Combined[_combined_id].At : 0;

            if (_xOuterData.TypeToken == 3 && _combined_id < _xOuterData.Combined.Count)
            {
                //specially
                AddedTimerToken(XTimerMgr.singleton.SetTimer(_xOuterData.Combined[_combined_id].End - _xOuterData.Combined[_combined_id].At, Combined, _combined_id + 1), true);
            }

            if (_current.Result != null)
            {
                foreach (XResultData data in _current.Result)
                {
                    _hurt_target.Add(new HashSet<XSkillHit>());

                    data.Token = count++;
                    AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At - play_offset, Result, data), true);
                }
            }

            if (_current.Charge != null)
            {
                XChargeSetting[] setting = new XChargeSetting[_current.Charge.Count];
                int i = 0;

                foreach (XChargeData data in _current.Charge)
                {
                    float delay = data.Using_Curve ? 0 : data.At;
                    setting[i] = new XChargeSetting();
                    setting[i].data = data;

                    if(delay >= play_offset)
                    {
                        setting[i].offset = 0;
                        AddedTimerToken(XTimerMgr.singleton.SetTimer(delay - play_offset, Charge, setting[i]), true);
                    }
                    else
                    {
                        setting[i].offset = play_offset - delay;
                        AddedTimerToken(XTimerMgr.singleton.SetTimer(0, Charge, setting[i]), true);
                    }

                    i++;
                }
            }

            if (_xOuterData.TypeToken != 3)
            {
                if (_current == _xOuterData && _current.Ja != null)
                {
                    int i = 0;
                    foreach (XJAData data in _current.Ja)
                    {
                        if (data.Point >= play_offset) AddedTimerToken(XTimerMgr.singleton.SetTimer(data.Point - play_offset, Ja, i++), true);
                    }
                }
            }

            if (_current.Manipulation != null)
            {
                foreach (XManipulationData data in _current.Manipulation)
                {
                    if (data.At >= play_offset) AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At - play_offset, Manipulate, data), true);
                }
            }

            if (_current.Fx != null && (_xOuterData.TypeToken != 3 || _xOuterData.Combined[_combined_id].Override_Presentation))
            {
                foreach (XFxData data in _current.Fx)
                {
                    if (data.At >= play_offset) AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At - play_offset, Fx, data), false);
                }
            }

            if (_current.Audio != null && (_xOuterData.TypeToken != 3 || _xOuterData.Combined[_combined_id].Override_Presentation))
            {
                foreach (XAudioData data in _current.Audio)
                {
                    if (data.At >= play_offset) AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At - play_offset, Audio, data), false);
                }
            }

            if (_current.Warning != null)
            {
                if (_current.Warning.Count > 0) WarningPosAt = new List<Vector3>[_current.Warning.Count];
                int i = 0;
                foreach (XWarningData data in _current.Warning)
                {
                    WarningPosAt[i] = new List<Vector3>(); i++;
                    if (data.At >= play_offset) AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At - play_offset, Warning, data), false);
                }
            }

            if (_current == null) return;

            if (_current.CameraEffect != null && (_xOuterData.TypeToken != 3 || _xOuterData.Combined[_combined_id].Override_Presentation))
            {
                foreach (XCameraEffectData data in _current.CameraEffect)
                {
                    if (data.At >= play_offset) AddedTimerToken(XTimerMgr.singleton.SetTimer(data.At - play_offset, Shake, data), false);
                }
            }

            if (_current.CameraMotion != null && !string.IsNullOrEmpty(_current.CameraMotion.Motion3D) && (_xOuterData.TypeToken != 3 || _xOuterData.Combined[_combined_id].Override_Presentation))
            {
                if (_current.CameraMotion.At >= play_offset) AddedTimerToken(XTimerMgr.singleton.SetTimer(_current.CameraMotion.At - play_offset, CameraMotion, _current.CameraMotion), false);
            }

            if (_current.CameraPostEffect != null && !string.IsNullOrEmpty(_current.CameraPostEffect.Effect) && (_xOuterData.TypeToken != 3 || _xOuterData.Combined[_combined_id].Override_Presentation))
            {
                if (_current.CameraPostEffect.At >= play_offset) AddedTimerToken(XTimerMgr.singleton.SetTimer(_current.CameraPostEffect.At - play_offset, CameraPostEffect, _current.CameraPostEffect), false);
            }

            if (_current.Mob != null)
            {
                for (int i = 0; i < _current.Mob.Count; i++)
                    if (_current.Mob[i].At >= play_offset) AddedTimerToken(XTimerMgr.singleton.SetTimer(_current.Mob[i].At - play_offset, Mob, _current.Mob[i]), true);
            }
        }

        private void Fire()
        {
            _current = _xOuterData;
            _fx.Clear();

            _skill_when_move = (_state == DummyState.Move);
            _state = DummyState.Fire;

            if (_xOuterData.TypeToken == 0)
                _trigger = _xOuterData.SkillPosition > 0 ? XSkillData.JA_Command[_xOuterData.SkillPosition] : "ToSkill";
            else if (_xOuterData.TypeToken == 1)
                _trigger = "ToArtSkill";
            else if (_xOuterData.TypeToken == 3)
                Combined(0);
            else
                _trigger = "ToUltraShow";

            FocusTarget();

            _anim_init = false;
            _delta = 0;
        }

        private void StopFire(bool cleanup = true)
        {
            if (_state != DummyState.Fire) return;

            _state = DummyState.Idle;
            _trigger = "EndSkill";

            _execute = false;

            for (int i = 0; i < _fx.Count; i++)
                XFxMgr.singleton.DestroyFx(_fx[i], false);
            _fx.Clear();

            if (_current.Audio != null)
            {
                foreach (XAudioData data in _current.Audio)
                {
                    AudioSource source = GetAudioSourceByChannel(data.Channel);
                    source.Stop();
                }
            }

            if (_manipulate != null) _manipulate.Remove(0);

            if (_current.CameraPostEffect != null && _current.CameraPostEffect.Effect != null && _current.CameraPostEffect.Effect.Length > 0)
            {
                Behaviour o = _camera.UnityCamera.GetComponent(_current.CameraPostEffect.Effect) as Behaviour;
                if(o!=null)
                o.enabled = false;
            }

            if (_mob_unit.Count > 0)
            {
                for (int i = 0; i < _mob_unit.Count; i++)
                {
                    if (_mob_unit[i].CompareTag("Finish")) GameObject.DestroyImmediate(_mob_unit[i]);
                }
            }

            if (cleanup)
            {
                _action_framecount = 0;

                for (int i = 0; i < _outer_fx.Count; i++)
                    XFxMgr.singleton.DestroyFx(_outer_fx[i], false);
                _outer_fx.Clear();

                if (_xOuterData.Audio != null)
                {
                    foreach (XAudioData data in _xOuterData.Audio)
                    {
                        AudioSource source = GetAudioSourceByChannel(data.Channel);
                        if (source != null)
                            source.Stop();
                    }
                }

                _camera.EndEffect(null);
                _xcamera_effect = null;

                if (_xOuterData.CameraPostEffect != null && _xOuterData.CameraPostEffect.Effect != null && _xOuterData.CameraPostEffect.Effect.Length > 0)
                {
                    Behaviour o = _camera.UnityCamera.GetComponent(_xOuterData.CameraPostEffect.Effect) as Behaviour;
                    if(o!=null)
                    o.enabled = false;
                }

                foreach (uint token in _combinedToken)
                {
                    XTimerMgr.singleton.KillTimer(token);
                }
                _combinedToken.Clear();
            }

            foreach (uint token in _presentToken)
            {
                XTimerMgr.singleton.KillTimer(token);
            }

            _presentToken.Clear();

            foreach (uint token in _logicalToken)
            {
                XTimerMgr.singleton.KillTimer(token);
            }

            _logicalToken.Clear();

            _update = null;
            _manipulate = null;

            nResultForward = Vector3.zero;
            Time.timeScale = 1;
            if (_ator != null)
                _ator.speed = 1;

            _mob_unit.Clear();
            _current = null;
        }

        public void Result(object param)
        {
            if (_state != DummyState.Fire) return;

            XResultData data = param as XResultData;

            if (data.Loop)
            {
                int i = (data.Index << 16) | 0;
                LoopResults(i);
            }
            else
            {
                if (data.Group)
                {
                    int i = (data.Index << 16) | 0;
                    GroupResults(i);
                }
                else
                {
                    if (data.LongAttackEffect)
                    {
                        Project(data);
                    }
                    else
                    {
                        innerResult(data.Index, transform.forward, transform.position, _current);
                    }
                }
            }
        }

        public void LoopResults(object param)
        {
            int i = (int)param;
            int count = i >> 16;
            int execute_cout = i & 0xFFFF;

            if (!_current.Result[count].Loop || _current.Result[count].Loop_Count <= execute_cout || _current.Result[count].Cycle <= 0)
                return;

            if (_current.Result[count].Group)
                GroupResults((count << 16) | (execute_cout << 8) | 0);
            else if (_current.Result[count].LongAttackEffect)
                Project(_current.Result[count]);
            else
            {
                innerResult(count, transform.forward, transform.position, _current);
            }

            ++execute_cout;

            if(_current.Result[count].Loop_Count > execute_cout)
                AddedTimerToken(XTimerMgr.singleton.SetTimer(_current.Result[count].Cycle, LoopResults, ((count << 16) | execute_cout)), true);
        }

        private void GroupResults(object param)
        {
            if (_state != DummyState.Fire) return;

            int i = (int)param;
            int count = i >> 16;

            int group_cout = i & 0x00FF;
            int loop_cout = (i & 0xFF00) >> 8;

            if (!_current.Result[count].Group || group_cout >= _current.Result[count].Group_Count)
                return;

            Vector3 face = transform.forward;

            int angle = _current.Result[count].Deviation_Angle + _current.Result[count].Angle_Step * group_cout;
            angle = _current.Result[count].Clockwise ? angle : -angle;

            if (_current.Result[count].LongAttackEffect)
                Project(_current.Result[count], angle);
            else
                innerResult(count, XCommon.singleton.HorizontalRotateVetor3(face, angle), transform.position, _current);

            group_cout++;
            if (group_cout < _current.Result[count].Group_Count)
            {
                i = (count << 16) | (loop_cout << 8) | group_cout;
                AddedTimerToken(XTimerMgr.singleton.SetTimer(_current.Result[count].Time_Step, GroupResults, i), true);
            }
        }

        public void innerResult(int triggerTime, Vector3 forward, Vector3 pos, XSkillData data, XSkillHit hitted = null)
        {
            nHotID = triggerTime;

            if (hitted == null)
            {
                pos += XCommon.singleton.VectorToQuaternion(transform.forward) * new Vector3(data.Result[triggerTime].Offset_X, 0, data.Result[triggerTime].Offset_Z);
                nResultForward = forward;

                XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();

                foreach (XSkillHit hit in hits)
                {
                    if (IsHurtEntity(hit, triggerTime)) continue;

                    Vector3 dir = hit.RadiusCenter - pos; dir.y = 0;
                    float distance = dir.magnitude;

                    if (distance > hit.Radius) distance -= hit.Radius;

                    if (dir.sqrMagnitude == 0) dir = forward;
                    dir.Normalize();

                    if (IsInField(data, triggerTime, pos, forward, hit.RadiusCenter, Vector3.Angle(forward, dir), distance))
                    {
                        Vector3 vHitDir = data.Result[triggerTime].Affect_Direction == XResultAffectDirection.AttackDir ?
                            (hit.RadiusCenter - pos).normalized :
                            GetRotateTo();

                        //_effectual = true;

                        AddHurtTarget(data, hit, triggerTime);

                        hit.Begin(this, data.Hit[triggerTime], vHitDir, data.Logical.AttackOnHitDown);
                    }
                }
            }
            else
            {
                Vector3 vHitDir = data.Result[triggerTime].Affect_Direction == XResultAffectDirection.AttackDir ?
                            (hitted.RadiusCenter - pos) :
                            GetRotateTo();

                vHitDir.y = 0; vHitDir.Normalize();
                hitted.Begin(this, data.Hit[triggerTime], vHitDir, data.Logical.AttackOnHitDown);
            }
        }

        private void Charge(object param)
        {
            XChargeSetting setting = param as XChargeSetting;
            XChargeData data = setting.data;

            XSkillCharge charge = null;
            if (data.Using_Curve)
            {
                if (data.Curve_Forward != null && data.Curve_Forward.Length > 0)
                {
                    GameObject forward = Resources.Load(data.Curve_Forward) as GameObject;
                    GameObject side = Resources.Load(data.Curve_Side) as GameObject;
                    GameObject up = Resources.Load(data.Curve_Up) as GameObject;

                    charge = new XSkillCharge(
                                    this,
                                    forward != null ? forward.GetComponent<XCurve>() : null,
                                    side != null ? side.GetComponent<XCurve>() : null,
                                    up != null ? up.GetComponent<XCurve>() : null,
                                    data.Using_Up,
                                    setting.offset,
                                    data.AimTarget,
                                    data.StandOnAtEnd,
                                    data.Control_Towards);
                }
            }
            else
            {
                charge = new XSkillCharge(
                                this,
                                data.End - data.At,
                                data.Offset,
                                data.Height,
                                setting.offset,
                                data.Rotation_Speed,
                                data.AimTarget,
                                data.StandOnAtEnd,
                                data.Control_Towards);
            }

            _update = charge;
            _update.Update(Time.deltaTime);
        }

        protected List<XFx> _fx = new List<XFx>();
        protected List<XFx> _outer_fx = new List<XFx>();

        public List<Vector3>[] WarningPosAt = null;

        private bool IsPickedInRange(int n, int d)
        {
	        if(n >= d) return true;

	        int i = XCommon.singleton.RandomInt(0, d);
	        return i < n;
        }

        private void Warning(object param)
        {
            XWarningData data = param as XWarningData;
            WarningPosAt[data.Index].Clear();

            if (data.RandomWarningPos || data.Type == XWarningType.Warning_Multiple)
            {
                if (data.RandomWarningPos)
                {
                    List<GameObject> item = new List<GameObject>();
                    switch (data.Type)
                    {
                        case XWarningType.Warning_All:
                        case XWarningType.Warning_Multiple:
                            {
                                XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();
                                int n = (data.Type == XWarningType.Warning_All) ? hits.Length : data.MaxRandomTarget;

                                for (int i = 0; i < hits.Length; i++)
                                {
                                    bool counted = (data.Type == XWarningType.Warning_All) ? true : IsPickedInRange(n, hits.Length - i);

                                    if (counted)
                                    {
                                        n--;
                                        item.Add(hits[i].gameObject);
                                    }
                                }
                            }break;
                        case XWarningType.Warning_Target:
                            {
                                if (_target != null) item.Add(_target);
                            }break;
                    }

                    for (int i = 0; i < item.Count; i++)
                    {
                        for (int n = 0; n < data.PosRandomCount; n++)
                        {
                            int d = XCommon.singleton.RandomInt(0, 360);
                            float r = XCommon.singleton.RandomFloat(0, data.PosRandomRange);

                            Vector3 v = r * XCommon.singleton.HorizontalRotateVetor3(Vector3.forward, d);

                            if (!string.IsNullOrEmpty(data.Fx))
                            {
                                XFxMgr.singleton.CreateAndPlay(
                                        data.Fx,
                                        item[i].transform,
                                        new Vector3(v.x, 0.05f - item[i].transform.position.y, v.z),
                                        data.Scale * Vector3.one,
                                        1,
                                        false,
                                        data.FxDuration);
                            }

                            WarningPosAt[data.Index].Add(item[i].transform.position + v);
                        }
                    }
                }
                else if(data.Type == XWarningType.Warning_Multiple)
                {
                    XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();
                    int n = data.MaxRandomTarget;

                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (IsPickedInRange(n, hits.Length - i))
                        {
                            n--;

                            if (!string.IsNullOrEmpty(data.Fx))
                            {
                                XFxMgr.singleton.CreateAndPlay(
                                        data.Fx,
                                        hits[i].transform,
                                        new Vector3(0, 0.05f - hits[i].transform.position.y, 0),
                                        data.Scale * Vector3.one,
                                        1,
                                        false,
                                        data.FxDuration);
                            }

                            WarningPosAt[data.Index].Add(hits[i].transform.position);
                        }
                    }
                }
            }
            else
            {
                switch (data.Type)
                {
                    case XWarningType.Warning_None:
                        {
                            Vector3 offset = transform.rotation * new Vector3(data.OffsetX, data.OffsetY, data.OffsetZ);

                            XFxMgr.singleton.CreateAndPlay(
                                    data.Fx,
                                    transform,
                                    offset,
                                    data.Scale * Vector3.one,
                                    1,
                                    false,
                                    data.FxDuration);

                            WarningPosAt[data.Index].Add(transform.position + offset);
                        }break;
                    case XWarningType.Warning_Target:
                        {
                            if (_target != null)
                            {
                                if (!string.IsNullOrEmpty(data.Fx))
                                {
                                    XFxMgr.singleton.CreateAndPlay(
                                            data.Fx,
                                            _target.transform,
                                            new Vector3(0, 0.05f - _target.transform.position.y, 0),
                                            data.Scale * Vector3.one,
                                            1,
                                            false,
                                            data.FxDuration);
                                }

                                WarningPosAt[data.Index].Add(_target.transform.position);
                            }
                            else
                            {
                                Vector3 offset = transform.rotation * new Vector3(data.OffsetX, data.OffsetY, data.OffsetZ);

                                XFxMgr.singleton.CreateAndPlay(
                                        data.Fx,
                                        transform,
                                        offset,
                                        data.Scale * Vector3.one,
                                        1,
                                        false,
                                        data.FxDuration);

                                WarningPosAt[data.Index].Add(transform.position + offset);
                            }
                        }break;
                    case XWarningType.Warning_All:
                        {
                            XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();

                            for (int i = 0; i < hits.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(data.Fx))
                                {
                                    XFxMgr.singleton.CreateAndPlay(
                                            data.Fx,
                                            hits[i].transform,
                                             new Vector3(0, 0.05f - hits[i].transform.position.y, 0),
                                            data.Scale * Vector3.one,
                                            1,
                                            false,
                                            data.FxDuration);
                                }

                                WarningPosAt[data.Index].Add(hits[i].transform.position);
                            }
                        }break;
                }
            }
        }

        private void Fx(object param)
        {
            XFxData data = param as XFxData;

            if (data.Shield) return;

            Transform trans = transform;
            Vector3 offset = new Vector3(data.OffsetX, data.OffsetY, data.OffsetZ);

            XFx fx = XFxMgr.singleton.CreateFx(data.Fx);
            fx.DelayDestroy = data.Destroy_Delay;

            if (data.StickToGround)
            {
                switch (data.Type)
                {
                    case SkillFxType.FirerBased:
                        {

                        }break;
                    case SkillFxType.TargetBased:
                        {
                            if (_current.NeedTarget && _target != null)
                            {
                                trans = _target.transform;
                                offset = new Vector3(data.Target_OffsetX, data.Target_OffsetY, data.Target_OffsetZ);
                            }
                        }break;
                }

                Vector3 pos = trans.position + trans.rotation * offset;
                pos.y = 0;

                fx.Play(pos, Quaternion.identity, new Vector3(data.ScaleX, data.ScaleY, data.ScaleZ));
            }
            else
            {
                switch (data.Type)
                {
                    case SkillFxType.FirerBased:
                        {
                            if (data.Bone != null && data.Bone.Length > 0)
                            {
                                Transform attachPoint = trans.Find(data.Bone);
                                if (attachPoint != null)
                                {
                                    trans = attachPoint;
                                }
                                else
                                {
                                    int index = data.Bone.LastIndexOf("/");
                                    if (index >= 0)
                                    {
                                        string bone = data.Bone.Substring(index + 1);
                                        attachPoint = trans.Find(bone);
                                        if (attachPoint != null)
                                        {
                                            trans = attachPoint;
                                        }
                                    }
                                }
                            }
                        }break;
                    case SkillFxType.TargetBased:
                        {
                            if (_current.NeedTarget && _target != null)
                            {
                                trans = _target.transform;
                                offset = new Vector3(data.Target_OffsetX, data.Target_OffsetY, data.Target_OffsetZ);
                            }
                        }break;
                }

                fx.Play(trans, offset, new Vector3(data.ScaleX, data.ScaleY, data.ScaleZ), 1, data.Follow);
            }

            if (data.Combined)
            {
                if (data.End > 0)
                    AddedCombinedToken(XTimerMgr.singleton.SetTimer(data.End - data.At, KillFx, fx));
                _outer_fx.Add(fx);
            }
            else
            {
                if (data.End > 0)
                    AddedTimerToken(XTimerMgr.singleton.SetTimer(data.End - data.At, KillFx, fx), false);
                _fx.Add(fx);
            }
        }

        private void AddedTimerToken(uint token, bool logical)
        {
            if(logical)
                _logicalToken.Add(token);
            else
                _presentToken.Add(token);
        }

        private void AddedCombinedToken(uint token)
        {
            _combinedToken.Add(token);
        }

        private void AddChild(Transform parent, GameObject child)
        {
            child.transform.parent = parent;
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
        }

        private void Audio(object param)
        {
            XAudioData data = param as XAudioData;

            //FMOD_StudioSystem.instance.PlayOneShot("event:/" + data.Clip, transform.position);

            if (_emitter == null)
                _emitter = gameObject.AddComponent<XFmod>();

            _emitter.StartEvent("event:/" + data.Clip, data.Channel);
        }

        private AudioSource GetAudioSourceByChannel(AudioChannel channel)
        {
            switch (channel)
            {
                case AudioChannel.Action:
                    {
                        if (_audio_action == null)
                            _audio_action = gameObject.AddComponent<AudioSource>();

                        return _audio_action;
                    }
                case AudioChannel.Motion:
                    {
                        if (_audio_motion == null)
                            _audio_motion = gameObject.AddComponent<AudioSource>();

                        return _audio_motion;
                    }
                case AudioChannel.Skill:
                    {
                        if (_audio_skill == null)
                            _audio_skill = gameObject.AddComponent<AudioSource>();

                        return _audio_skill;
                    }
                case AudioChannel.Behit:
                    {
                        if (_audio_behit == null)
                            _audio_behit = gameObject.AddComponent<AudioSource>();

                        return _audio_behit;
                    }
            }

            return _audio_action;
        }

        private void Manipulate(object param)
        {
            XManipulationData data = param as XManipulationData;

            if (_manipulate == null) _manipulate = new XSkillManipulate(this);

            long token = XCommon.singleton.UniqueToken;
            _manipulate.Add(token, data);
            AddedTimerToken(XTimerMgr.singleton.SetTimer(data.End - data.At, KillManipulate, token), true);
        }

        private void KillManipulate(object param)
        {
            _manipulate.Remove((long)param);
        }

        private void Shake(object param)
        {
            XCameraEffectData data = param as XCameraEffectData;

            _xcamera_effect = new XCameraShake(gameObject, _camera.UnityCamera);

            _xcamera_effect.OnShake(
                data.Time,
                data.FovAmp,
                data.AmplitudeX,
                data.AmplitudeY,
                data.AmplitudeZ,
                data.Frequency,
                data.Coordinate,
                data.ShakeX,
                data.ShakeY,
                data.ShakeZ,
                data.Random);
        }

        private void Mob(object param)
        {
            XMobUnitData mob = param as XMobUnitData;

            uint id = XStatisticsLibrary.AssociatedData((uint)mob.TemplateID).PresentID;
            XEntityPresentation.RowData data = XAnimationLibrary.AssociatedAnimations(id);

            GameObject mob_unit = GameObject.Instantiate(Resources.Load("Prefabs/" + data.Prefab)) as GameObject;

            Vector3 offset = transform.rotation * new Vector3(mob.Offset_At_X, mob.Offset_At_Y, mob.Offset_At_Z);
            Vector3 pos = transform.position + offset;

            mob_unit.transform.position = pos;
            mob_unit.transform.forward = transform.forward;

            if (mob.LifewithinSkill) mob_unit.tag = "Finish";

            _mob_unit.Add(mob_unit);
        }

        private void CameraMotion(object param)
        {
            XCameraMotionData data = param as XCameraMotionData;

            _camera.Effect(data, _current.TypeToken != 2);
        }

        private void CameraPostEffect(object param)
        {
            XCameraPostEffectData data = param as XCameraPostEffectData;

            Behaviour o = _camera.UnityCamera.GetComponent(data.Effect) as Behaviour;
            if (o != null) o.enabled = true;

            AddedTimerToken(XTimerMgr.singleton.SetTimer(_current.CameraPostEffect.End - _current.CameraPostEffect.At, CameraPostEffectEnd, o), false);
        }

        private void CameraPostEffectEnd(object param)
        {
            Behaviour o = param as Behaviour;
            if (o != null) o.enabled = false;
        }

        private bool CanAct(Vector3 dir)
        {
            bool can = false;
            float now = Time.time - _fire_time;

            XLogicalData logic = (SkillData.TypeToken == 3) ? SkillData.Logical : _current.Logical;

            can = true;

            if (XCommon.singleton.IsLess(now, logic.Not_Move_End) &&
                XCommon.singleton.IsGreater(now, logic.Not_Move_At))
            {
                can = false;
            }

            if (can) 
                StopFire();
            else
            {
                if (XCommon.singleton.IsLess(now, logic.Rotate_End) &&
                    XCommon.singleton.IsGreater(now, logic.Rotate_At))
                {
                    //perform rotate
                    PrepareRotation(XCommon.singleton.Horizontal(dir), logic.Rotate_Speed > 0 ? logic.Rotate_Speed : _xConfigData.RotateSpeed);
                }
            }

            return can;
        }

        //can replace by other skill
        private bool CanReplacedBy(XSkillData skill)
        {
            /*
             * Main skill can always be act
             */
            bool cancel = (_xOuterData.Logical.CanReplacedby & (1 << skill.TypeToken)) != 0;

            if (!cancel)
            {
                cancel = XCommon.singleton.IsGreater(_delta, _xOuterData.Logical.CanCancelAt);
            }

            return cancel;
        }

        private void Combined(object param)
        {
            int i = (int)param;

            if (i < _xOuterData.Combined.Count && _xOuterData.Combined[i].Name != null && _xOuterData.Combined[i].Name.Length > 0)
            {
                _combined_id = i;

                if (_combined_id > 0)
                    StopFire(false);
                else
                    MainCoreExecute();

                _trigger = XSkillData.Combined_Command[_combined_id];

                _current = _combinedlist[_combined_id];

                _state = DummyState.Fire;
                _fire_time = Time.time;
                if (_ator != null)
                    _ator.speed = 0;

                _time_offset = _xOuterData.Combined[_combined_id].At / _combinedlist[i].Time;
            }
            else
            {
                StopFire();
            }
        }

        private void Ja(object param)
        {
            if (!EditorData.XAutoJA) return;

            int i = (int)param;

            float swype = XGesture.singleton.LastSwypeAt;
            float trigger_at = swype - _fire_time - Time.deltaTime;

            XJAData jd = _current.Ja[_jaCount];

            if (!XCommon.singleton.IsEqual(swype, _last_swype_time) &&
                XCommon.singleton.IsLess(trigger_at, jd.End) &&
                XCommon.singleton.IsGreater(trigger_at, jd.At))
            {
                if (_xOuterData.Ja[i].Name != null && _xOuterData.Ja[i].Name.Length > 0)
                {
                    StopFire();
                    _trigger = XSkillData.JA_Command[_xDataExtra.JaEx[i].Ja.SkillPosition];

                    _current = _xDataExtra.JaEx[i].Ja;

                    _state = DummyState.Fire;
                    _fire_time = Time.time;
                    _delta = 0;
                    if (_ator != null)
                        _ator.speed = 0;
                }
            }
            else if (_xOuterData.Ja[i].Next_Name != null && _xOuterData.Ja[i].Next_Name.Length > 0)
            {
                StopFire();
                _trigger = XSkillData.JA_Command[_xDataExtra.JaEx[i].Next.SkillPosition];

                _current = _xDataExtra.JaEx[i].Next;

                _state = DummyState.Fire;
                _fire_time = Time.time;
                _delta = 0;
                if (_ator != null)
                    _ator.speed = 0;
            }

            _last_swype_time = swype;
            _jaCount++;
        }

        private void Project(XResultData param, int additionalAngle = 0)
        {
            if (param.Attack_All)
            {
                XSkillHit[] hits = GameObject.FindObjectsOfType<XSkillHit>();

                for (int i = 0; i < hits.Length; i++)
                {
                    XBulletMgr.singleton.ShootBullet(GenerateBullet(param, hits[i].gameObject, additionalAngle));
                }
            }
            else if (param.Warning)
            {
                for (int i = 0; i < WarningPosAt[param.Warning_Idx].Count; i++)
                {
                    XBulletMgr.singleton.ShootBullet(GenerateBullet(param, null, additionalAngle, i));
                }
            }
            else
                XBulletMgr.singleton.ShootBullet(GenerateBullet(param, _target, additionalAngle));
        }

        private XBullet GenerateBullet(XResultData data, GameObject target, int additionalAngle, int wid = -1)
        {
            return new XBullet(new XBulletData(
                this, 
                _current,
                target,
                data.Index,
                data.LongAttackData.FireAngle + additionalAngle,
                wid));
        }

        private void FocusTarget()
        {
            XSkillHit hit = GameObject.FindObjectOfType<XSkillHit>();
            _target = (_xOuterData.NeedTarget && hit != null) ? hit.gameObject : null;

            if (_target != null && IsInAttckField(_xOuterData, transform.position, transform.forward, _target))
            {
                PrepareRotation(XCommon.singleton.Horizontal(_target.transform.position - transform.position), _xConfigData.RotateSpeed);
            }
        }

        private void KillFx(object o)
        {
            XFx fx = o as XFx;

            _fx.Remove(fx);
            _outer_fx.Remove(fx);

            XFxMgr.singleton.DestroyFx(fx, false);
        }

        private bool IsInField(XSkillData data, int triggerTime, Vector3 pos, Vector3 forward, Vector3 target, float angle, float distance)
        {
            bool log = true;
            if (data.Warning != null && data.Warning.Count > 0)
            {
                for (int i = 0; i < data.Warning.Count; i++)
                {
                    if (data.Warning[i].RandomWarningPos || data.Warning[i].Type == XWarningType.Warning_Multiple)
                    {
                        log = false; break;
                    }
                }
            }

            if (data.Result[triggerTime].Sector_Type)
            {
                if (!(XCommon.singleton.IsEqualGreater(distance, data.Result[triggerTime].Low_Range) &&
                        XCommon.singleton.IsLess(distance, data.Result[triggerTime].Range) &&
                        angle <= data.Result[triggerTime].Scope * 0.5f))
                {
                    if (log)
                    {
                        Debug.Log("-----------------------------------");
                        Debug.Log("At " + triggerTime + " Hit missing: distance is " + distance.ToString("F3") + " ( >= " + data.Result[triggerTime].Low_Range.ToString("F3") + ")");
                        Debug.Log("At " + triggerTime + " Hit missing: distance is " + distance.ToString("F3") + " ( < " + data.Result[triggerTime].Range.ToString("F3") + ")");
                        Debug.Log("At " + triggerTime + " Hit missing: dir is " + angle.ToString("F3") + " ( < " + (data.Result[triggerTime].Scope * 0.5f).ToString("F3") + ")");
                    }

                    return false;
                }
            }
            else
            {
                if (!IsInAttackRect(target, pos, forward, data.Result[triggerTime].Range, data.Result[triggerTime].Scope, data.Result[triggerTime].Rect_HalfEffect, data.Result[triggerTime].None_Sector_Angle_Shift))
                {
                    float d = data.Result[triggerTime].Range;
                    float w = data.Result[triggerTime].Scope;

                    Vector3[] vecs = new Vector3[4];
                    vecs[0] = new Vector3(-w / 2.0f, 0, data.Result[triggerTime].Rect_HalfEffect ? 0 : (-d / 2.0f));
                    vecs[1] = new Vector3(-w / 2.0f, 0, d / 2.0f);
                    vecs[2] = new Vector3(w / 2.0f, 0, d / 2.0f);
                    vecs[3] = new Vector3(w / 2.0f, 0, data.Result[triggerTime].Rect_HalfEffect ? 0 : (-d / 2.0f));

                    if (log)
                    {
                        Debug.Log("-----------------------------------");
                        Debug.Log("Not in rect " + vecs[0] + " " + vecs[1] + " " + vecs[2] + " " + vecs[3]);
                    }

                    return false;
                }
            }

            return true;
        }

        private bool IsInAttckField(XSkillData data, Vector3 pos, Vector3 forward, GameObject target)
        {
            forward = XCommon.singleton.HorizontalRotateVetor3(forward, data.Cast_Scope_Shift);
            Vector3 targetPos = target.transform.position;

            if (data.Cast_Range_Rect)
            {
                pos.x += data.Cast_Offset_X;
                pos.z += data.Cast_Offset_Z;

                return IsInAttackRect(targetPos, pos, forward, data.Cast_Range_Upper, data.Cast_Scope, false, 0);
            }
            else
            {
                Vector3 dir = targetPos - pos; dir.y = 0;
                float distance = dir.magnitude;

                //normalize
                dir.Normalize();

                float angle = (distance == 0) ? 0 : Vector3.Angle(forward, dir);

                if (XCommon.singleton.IsEqualLess(distance, data.Cast_Range_Upper) &&
                    XCommon.singleton.IsEqualGreater(distance, data.Cast_Range_Lower) &&
                    angle <= data.Cast_Scope * 0.5f)
                {
                    return true;
                }
                
                return false;
            }
        }

        private bool IsInAttackRect(Vector3 target, Vector3 anchor, Vector3 forward, float d, float w, bool half, float shift)
        {
            Quaternion rotation = XCommon.singleton.VectorToQuaternion(XCommon.singleton.HorizontalRotateVetor3(forward, shift));

            Rect rect = new Rect();

            rect.xMin = -w / 2.0f;
            rect.xMax = w / 2.0f;
            rect.yMin = half ? 0 : (-d / 2.0f);
            rect.yMax = d / 2.0f;

            return XCommon.singleton.IsInRect(target - anchor, rect, Vector3.zero, rotation);
        }
	}
}
#endif