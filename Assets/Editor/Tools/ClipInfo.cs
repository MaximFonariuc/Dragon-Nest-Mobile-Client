using UnityEditor;
using UnityEngine;

// Editor window for listing all object reference curves in an animation clip
public class ClipInfo : EditorWindow
{
    private AnimationClip m_clip;

    private EditorCurveBinding[] curveBinding = null;
    private EditorCurveBinding[] objReferencecurveBinding = null;
    private Vector2 m_ScrollPos0 = Vector2.zero;
    private Vector2 m_ScrollPos1 = Vector2.zero;
    [MenuItem(@"Assets/Tool/Animation/ClipInfo", false, 0)]
    static void Init()
    {
        ClipInfo ci = EditorWindow.GetWindow<ClipInfo>("动画信息", true);
        ci.position = new Rect(100, 100, 800, 800);
        ci.Init(Selection.activeObject as AnimationClip);
    }
    public void Init(AnimationClip clip)
    {
        m_clip = clip;
        if (m_clip != null)
        {
            curveBinding = AnimationUtility.GetCurveBindings(m_clip);
            objReferencecurveBinding = AnimationUtility.GetObjectReferenceCurveBindings(m_clip);
        }
    }

    public void OnGUI()
    {
        AnimationClip newClip = EditorGUILayout.ObjectField("Clip", m_clip, typeof(AnimationClip), false) as AnimationClip;
        if (newClip != m_clip)
        {
            Init(newClip);
        }

        if (m_clip != null && curveBinding != null)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Curves:");
            GUILayout.EndHorizontal();
            m_ScrollPos0 = GUILayout.BeginScrollView(m_ScrollPos0, false, false);
            for (int i = 0; i < curveBinding.Length; ++i)
            {
                var binding = curveBinding[i];
                AnimationCurve curve = AnimationUtility.GetEditorCurve(m_clip, binding);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Format("{0}/{1},Keys:{2}", binding.path, binding.propertyName, curve.keys.Length));
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        if (m_clip != null && objReferencecurveBinding != null)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Object reference curves:");
            GUILayout.EndHorizontal();
            m_ScrollPos1 = GUILayout.BeginScrollView(m_ScrollPos1, false, false);
            for (int i = 0; i < objReferencecurveBinding.Length; ++i)
            {
                var binding = objReferencecurveBinding[i];
                ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(m_clip, binding);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Format("{0}/{1},Keys:{2}", binding.path, binding.propertyName, keyframes.Length));
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}