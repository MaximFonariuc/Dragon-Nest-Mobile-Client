using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenFadeIn))]

public class TweenFadeInEditor : Editor
{

    TweenFadeIn fi;
    public override void OnInspectorGUI()
    {

        NGUIEditorTools.SetLabelWidth(130f);
        GUILayout.Space(6f);
        fi = target as TweenFadeIn;

        GUI.changed = false;

        if (fi._editor_need_init)
        {
            fi._editor_need_init = false;
            _Init();
        }

        GUILayout.BeginHorizontal();
        float dur = EditorGUILayout.FloatField("Duration", fi.Duration, GUILayout.Width(170f));
        GUILayout.Label("seconds");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        float del = EditorGUILayout.FloatField("Start Delay", fi.StartDelay, GUILayout.Width(170f));
        GUILayout.Label("seconds");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        float interval = EditorGUILayout.FloatField("Delay Interval", fi.DelayInterval, GUILayout.Width(170f));
        GUILayout.Label("seconds");
        GUILayout.EndHorizontal();

        int group = EditorGUILayout.IntField("Tween Group", fi.Group, GUILayout.Width(170f));

        int itemNum = EditorGUILayout.IntField("Item Num", fi.TweenPlayItemNum, GUILayout.Width(170f));

        Vector3 delta = EditorGUILayout.Vector3Field("Move", fi.MoveDeltaPos);

 

        if (GUI.changed)
        {
            fi.Duration = dur;
            fi.PosT.duration = dur;
            fi.AlpT.duration = dur;

            fi.StartDelay = del;
            fi.PosT.delay = del;
            fi.AlpT.delay = del;

            fi.DelayInterval = interval;

            fi.Group = group;
            fi.PosT.tweenGroup = group;
            fi.AlpT.tweenGroup = group;
            fi.PlayT.tweenGroup = group;

            fi.MoveDeltaPos = delta;

            fi.TweenPlayItemNum = itemNum;
        }
    }

    void _Init()
    {

        GUI.changed = true;

        fi.PlayT.trigger = AnimationOrTween.Trigger.Customer;
        fi.PlayT.ifDisabledOnPlay = AnimationOrTween.EnableCondition.EnableThenPlay;
        fi.PlayT.resetOnPlay = true;
        fi.PlayT.resetIfDisabled = false;

    }
}