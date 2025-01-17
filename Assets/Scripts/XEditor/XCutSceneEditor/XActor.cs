#if UNITY_EDITOR
using UnityEngine;
using XEditor;
using XUtliPoolLib;
using UnityEditor;

namespace XEditor
{
	internal class XActor
	{
        private GameObject _actor = null;
        private Transform _shadow = null;
        private Transform _bip = null;
        private Animator _ator = null;

        private static Terrain _terrain = null;

        private AudioSource _audio_motion = null;
        private AudioSource _audio_action = null;
        private AudioSource _audio_skill = null;

        static XActor()
        {
            _terrain = Terrain.activeTerrain;
            /*if (_terrain == null)
            {
                SceneTable.RowData sceneConf = XSceneMgr.singleton.GetSceneData(_scene_id);
                if (sceneConf.BlockFilePath.Length > 0)
                {
                    _grid = new XGrid();
                    if (!_grid.LoadFile(@"Assets\Resources\" + sceneConf.BlockFilePath))
                    {
                        Debug.Log(@"Load Grid file: Assets\Resources\" + sceneConf.BlockFilePath + " failed!");
                        _grid = null;
                    }
                }
            }*/
        }

        public GameObject Actor { get { return _actor; } }
        public Transform Bip { get { return _bip; } }

        public XActor(float x, float y, float z, string clip)
        {
            _actor = AssetDatabase.LoadAssetAtPath("Assets/Editor/EditorResources/Prefabs/ZJ_zhanshi.prefab", typeof(GameObject)) as GameObject;
            _actor = UnityEngine.Object.Instantiate(_actor) as GameObject;
            DisablePhysic();
            _actor.transform.position = new Vector3(x, y, z);
            _ator = _actor.GetComponent<Animator>();

            AnimatorOverrideController overrideController = new AnimatorOverrideController();

            overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
            _ator.runtimeAnimatorController = overrideController;

            overrideController["Idle"] = XResourceLoaderMgr.singleton.GetSharedResource<AnimationClip>(clip, ".anim");

            _shadow = _actor.transform.Find("Shadow");
            if (_shadow != null) _shadow.GetComponent<Renderer>().enabled = true;

            _ator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        public XActor(string prefab, float x, float y, float z, string clip)
        {
            _actor = XResourceLoaderMgr.singleton.CreateFromPrefab(prefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
            _ator = _actor.GetComponent<Animator>();
            DisablePhysic();
            if (_ator != null)
            {
                AnimatorOverrideController overrideController = new AnimatorOverrideController();

                overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
                _ator.runtimeAnimatorController = overrideController;

                overrideController["Idle"] = XResourceLoaderMgr.singleton.GetSharedResource<AnimationClip>(clip, ".anim");

                _shadow = _actor.transform.Find("Shadow");
                if (_shadow != null) _shadow.GetComponent<Renderer>().enabled = true;

                _ator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
        }

        public XActor(uint id, float x, float y, float z, string clip)
        {
            _actor = UnityEngine.Object.Instantiate(XStatisticsLibrary.GetDummy(id), new Vector3(x, y, z), Quaternion.identity) as GameObject;
            _ator = _actor.GetComponent<Animator>();
            DisablePhysic();
            AnimatorOverrideController overrideController = new AnimatorOverrideController();

            overrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
            _ator.runtimeAnimatorController = overrideController;

            overrideController["Idle"] = XResourceLoaderMgr.singleton.GetSharedResource<AnimationClip>(clip, ".anim");

            _shadow = _actor.transform.Find("Shadow");
            if (_shadow != null) _shadow.GetComponent<Renderer>().enabled = true;

            _ator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
        private void DisablePhysic()
        {
            if (_actor != null)
            {
                CharacterController cc = _actor.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
            }
        }
        public void Update(float fDelta)
        {
            StickShadow();
        }

        public AudioSource GetAudioSourceByChannel(AudioChannel channel)
        {
            switch (channel)
            {
                case AudioChannel.Action:
                    {
                        if (_audio_action == null)
                            _audio_action = _actor.AddComponent<AudioSource>();

                        return _audio_action;
                    }
                case AudioChannel.Motion:
                    {
                        if (_audio_motion == null)
                            _audio_motion = _actor.AddComponent<AudioSource>();

                        return _audio_motion;
                    }
                case AudioChannel.Skill:
                    {
                        if (_audio_skill == null)
                            _audio_skill = _actor.AddComponent<AudioSource>();

                        return _audio_skill;
                    }
            }

            return _audio_action;
        }

        protected void StickShadow()
        {
            if (_bip == null) return;

            Vector3 shadow_pos;

            shadow_pos.x = _bip.position.x;
            shadow_pos.z = _bip.position.z;
            shadow_pos.y = XActor.TerrainY(_bip.transform.position) + 0.02f;

            _shadow.position = shadow_pos;
        }

        public static float TerrainY(Vector3 pos)
        {
            if (_terrain != null)
            {
                return _terrain.SampleHeight(pos);
            }

            return 0;
        }
	}
}
#endif