using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XUtliPoolLib;
using System.Collections.Generic;
using System.Reflection;

namespace XEditor
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExposePropertyAttribute : Attribute
    {

    }

    public static class ExposeProperties
    {
        public static void Expose(PropertyField[] properties)
        {
            GUILayoutOption[] emptyOptions = new GUILayoutOption[0];

            EditorGUILayout.BeginVertical(emptyOptions);

            foreach (PropertyField field in properties)
            {
                EditorGUILayout.BeginHorizontal(emptyOptions);

                switch (field.Type)
                {
                    case SerializedPropertyType.Integer:
                        field.SetValue(EditorGUILayout.IntField(field.Name, (int)field.GetValue(), emptyOptions));
                        break;

                    case SerializedPropertyType.Float:
                        field.SetValue(EditorGUILayout.FloatField(field.Name, (float)field.GetValue(), emptyOptions));
                        break;

                    case SerializedPropertyType.Boolean:
                        field.SetValue(EditorGUILayout.Toggle(field.Name, (bool)field.GetValue(), emptyOptions));
                        break;

                    case SerializedPropertyType.String:
                        field.SetValue(EditorGUILayout.TextField(field.Name, (String)field.GetValue(), emptyOptions));
                        break;

                    case SerializedPropertyType.Vector2:
                        field.SetValue(EditorGUILayout.Vector2Field(field.Name, (Vector2)field.GetValue(), emptyOptions));
                        break;

                    case SerializedPropertyType.Vector3:
                        field.SetValue(EditorGUILayout.Vector3Field(field.Name, (Vector3)field.GetValue(), emptyOptions));
                        break;

                    case SerializedPropertyType.Enum:
                        field.SetValue(EditorGUILayout.EnumPopup(field.Name, (Enum)field.GetValue(), emptyOptions));
                        break;

                    default:
                        break;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        public static PropertyField[] GetProperties(System.Object obj)
        {
            List<PropertyField> fields = new List<PropertyField>();

            PropertyInfo[] infos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in infos)
            {

                if (!(info.CanRead && info.CanWrite))
                    continue;

                object[] attributes = info.GetCustomAttributes(true);

                bool isExposed = false;

                foreach (object o in attributes)
                {
                    if (o.GetType() == typeof(ExposePropertyAttribute))
                    {
                        isExposed = true;
                        break;
                    }
                }

                if (!isExposed)
                    continue;

                SerializedPropertyType type = SerializedPropertyType.Integer;

                if (PropertyField.GetPropertyType(info, out type))
                {
                    PropertyField field = new PropertyField(obj, info, type);
                    fields.Add(field);
                }

            }

            return fields.ToArray();
        }
    }

    public class PropertyField
    {
        System.Object m_Instance;
        PropertyInfo m_Info;
        SerializedPropertyType m_Type;

        MethodInfo m_Getter;
        MethodInfo m_Setter;

        public SerializedPropertyType Type
        {
            get
            {
                return m_Type;
            }
        }

        public String Name
        {
            get
            {
                return ObjectNames.NicifyVariableName(m_Info.Name);
            }
        }

        public PropertyField(System.Object instance, PropertyInfo info, SerializedPropertyType type)
        {

            m_Instance = instance;
            m_Info = info;
            m_Type = type;

            m_Getter = m_Info.GetGetMethod();
            m_Setter = m_Info.GetSetMethod();
        }

        public System.Object GetValue()
        {
            return m_Getter.Invoke(m_Instance, null);
        }

        public void SetValue(System.Object value)
        {
            m_Setter.Invoke(m_Instance, new System.Object[] { value });
        }

        public static bool GetPropertyType(PropertyInfo info, out SerializedPropertyType propertyType)
        {
            propertyType = SerializedPropertyType.Generic;

            Type type = info.PropertyType;

            if (type == typeof(int))
            {
                propertyType = SerializedPropertyType.Integer;
                return true;
            }

            if (type == typeof(float))
            {
                propertyType = SerializedPropertyType.Float;
                return true;
            }

            if (type == typeof(bool))
            {
                propertyType = SerializedPropertyType.Boolean;
                return true;
            }

            if (type == typeof(string))
            {
                propertyType = SerializedPropertyType.String;
                return true;
            }

            if (type == typeof(Vector2))
            {
                propertyType = SerializedPropertyType.Vector2;
                return true;
            }

            if (type == typeof(Vector3))
            {
                propertyType = SerializedPropertyType.Vector3;
                return true;
            }

            if (type.IsEnum)
            {
                propertyType = SerializedPropertyType.Enum;
                return true;
            }

            return false;
        }
    }

	public abstract class XPanel
	{
        protected GUIContent _content_add = new GUIContent("+");
        protected GUIContent _content_remove = new GUIContent("-", "Remove Item.");

        private GUIStyle _style = null;
        private GUILayoutOption[] _line = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) };

        public XSkillHoster Hoster { get; set; }

        public virtual void Init()
        {

        }

        public void OnGUI()
        {
            if(_style == null) _style = new GUIStyle(GUI.skin.GetStyle("Label"));
            _style.alignment = TextAnchor.UpperRight;

            EditorGUILayout.BeginHorizontal();
            FoldOut = EditorGUILayout.Foldout(FoldOut, PanelName);
            GUILayout.FlexibleSpace();
            if (Count > 0) EditorGUILayout.LabelField("Total " + Count.ToString(), _style);
            EditorGUILayout.EndHorizontal();

            if (FoldOut)
            {
                GUILayout.Box("", _line);
                OnInnerGUI();
            }
            else
            {
                OnInnerUpdate();
            }
        }

        public void Add<T>() 
            where T : XBaseData, new()
        {
            T data = new T();
            Type t = typeof(T);

            if (t == typeof(XResultData)) { if (Hoster.SkillData.Result == null) Hoster.SkillData.Result = new List<XResultData>(); Hoster.SkillData.Result.Add(data as XResultData); }
            else if (t == typeof(XChargeData)) { if (Hoster.SkillData.Charge == null) Hoster.SkillData.Charge = new List<XChargeData>(); Hoster.SkillData.Charge.Add(data as XChargeData); }
            else if (t == typeof(XJAData)) { if (Hoster.SkillData.Ja == null) Hoster.SkillData.Ja = new List<XJAData>(); Hoster.SkillData.Ja.Add(data as XJAData); }
            else if (t == typeof(XHitData)) { if (Hoster.SkillData.Hit == null) Hoster.SkillData.Hit = new List<XHitData>(); Hoster.SkillData.Hit.Add(data as XHitData); }
            else if (t == typeof(XFxData)) {if( Hoster.SkillData.Fx == null) Hoster.SkillData.Fx = new List<XFxData>();Hoster.SkillData.Fx.Add(data as XFxData);}
            else if (t == typeof(XAudioData)) { if (Hoster.SkillData.Audio == null) Hoster.SkillData.Audio = new List<XAudioData>(); Hoster.SkillData.Audio.Add(data as XAudioData); }
            else if (t == typeof(XCameraEffectData)) { if (Hoster.SkillData.CameraEffect == null) Hoster.SkillData.CameraEffect = new List<XCameraEffectData>(); Hoster.SkillData.CameraEffect.Add(data as XCameraEffectData); }
            else if (t == typeof(XWarningData)) { if (Hoster.SkillData.Warning == null) Hoster.SkillData.Warning = new List<XWarningData>(); Hoster.SkillData.Warning.Add(data as XWarningData); }
            else if (t == typeof(XMobUnitData)) { if (Hoster.SkillData.Mob == null) Hoster.SkillData.Mob = new List<XMobUnitData>(); Hoster.SkillData.Mob.Add(data as XMobUnitData); }
            else if (t == typeof(XCombinedData)) { if (Hoster.SkillData.Combined == null) Hoster.SkillData.Combined = new List<XCombinedData>(); Hoster.SkillData.Combined.Add(data as XCombinedData); }
            else if (t == typeof(XManipulationData)) { if (Hoster.SkillData.Manipulation == null) Hoster.SkillData.Manipulation = new List<XManipulationData>(); Hoster.SkillData.Manipulation.Add(data as XManipulationData); }

			Hoster.EditorData.ToggleFold<T> (true);

            if (t == typeof(XResultData))
            {
                AddExtra<XResultDataExtra>();
                AddExtraEx<XResultDataExtraEx>();
            }
            else if (t == typeof(XChargeData))
            {
                AddExtra<XChargeDataExtra>();
                AddExtraEx<XChargeDataExtraEx>();
            }
            else if (t == typeof(XJAData))
            {
                AddExtra<XJADataExtra>();
                AddExtraEx<XJADataExtraEx>();
            }
            else if (t == typeof(XManipulationData)) AddExtraEx<XManipulationDataExtra>();
            else if (t == typeof(XFxData)) AddExtraEx<XFxDataExtra>();
            else if (t == typeof(XWarningData)) AddExtraEx<XWarningDataExtra>();
            else if (t == typeof(XMobUnitData)) AddExtraEx<XMobUnitDataExtra>();
            else if (t == typeof(XAudioData)) AddExtraEx<XAudioDataExtra>();
            else if (t == typeof(XHitData))
            {
                AddExtraEx<XHitDataExtraEx>();
            }
            else if (t == typeof(XCameraEffectData)) AddExtra<XCameraEffectDataExtra>();
            else if (t == typeof(XCombinedData))
            {
                AddExtra<XCombinedDataExtra>();
                AddExtraEx<XCombinedDataExtraEx>();
            }
        }

        private void AddExtra<T>()
            where T : XBaseDataExtra, new()
        {
            T data = new T();
            Hoster.ConfigData.Add<T>(data);
        }

        private void AddExtraEx<T>()
            where T : XBaseDataExtra, new()
        {
            T data = new T();
            Hoster.SkillDataExtra.Add<T>(data);
        }

        protected abstract void OnInnerGUI();
        protected virtual void OnInnerUpdate() { }

        protected abstract bool FoldOut { get; set; }
        protected abstract string PanelName { get; }
        protected abstract int Count { get; }
	}
}