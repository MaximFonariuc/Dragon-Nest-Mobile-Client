using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CallbackSettings))]
public class CallbackEditor : Editor 
{
    private CallbackSettings instance;

    public override void OnInspectorGUI()
    {
        instance = (CallbackSettings)target;

        ConfigGUI();
    }

    private void ConfigGUI()
    {
        EditorGUILayout.HelpBox("MSDK虚拟PC环境--部分回调结果配置项", MessageType.None);
        EditorGUILayout.Space();
        instance.LoginIndex = EditorGUILayout.Popup("登录事件", instance.LoginIndex, instance.loginStates);
        instance.AuthIndex = EditorGUILayout.Popup("实名认证事件", instance.AuthIndex, instance.authStates);
        instance.ShareIndex = EditorGUILayout.Popup("分享事件", instance.ShareIndex, instance.shareStates);
        instance.RelationIndex = EditorGUILayout.Popup("关系链事件", instance.RelationIndex, instance.relationStates);
        instance.WakeupIndex = EditorGUILayout.Popup("平台拉起事件", instance.WakeupIndex, instance.wakeupStates);
        instance.GroupIndex = EditorGUILayout.Popup("群相关事件", instance.GroupIndex, instance.groupStates);
        instance.CardIndex = EditorGUILayout.Popup("绑卡事件", instance.CardIndex, instance.cardStates);
        instance.LocationIndex = EditorGUILayout.Popup("定位事件", instance.LocationIndex, instance.lbsStates);
        instance.NearbyIndex = EditorGUILayout.Popup("附近的人事件", instance.NearbyIndex, instance.lbsStates);
    }
}
