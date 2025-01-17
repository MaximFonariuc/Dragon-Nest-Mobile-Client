using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System;

public class TurnScriptU4ToU5 : MonoBehaviour
{
    [MenuItem(@"XEditor/Tools/TurnScriptU4ToU5")]
    static void Execute()
    {
        EditorWindow.GetWindowWithRect<TurnScriptU4ToU5Editor>(new Rect(300, 300, 350, 350), true, @"TurnScriptU4ToU5");
    }
}

public class ValueWrapper
{
    public virtual bool Equal(ValueWrapper valueWrapper)
    {
        return true;
    }
    public virtual void Parse(FieldInfo f, MonoBehaviour mb)
    {

    }
    public virtual void Write(BinaryWriter bw)
    {

    }
    public virtual void Read(BinaryReader br)
    {

    }
    public static ValueWrapper Parse(FieldInfo f, MonoBehaviour mb, string typeName)
    {
        ValueWrapper vw = null;
        if (typeName == "System.String")
        {
            vw = new StringValueWrapper();
        }
        else if (typeName == "XSpawnWall+etrigger_type")
        {
            vw = new SpawnWalletriggerWrapper();
        }
        else if (typeName == "XCylinderTrigger+etrigger_type")
        {
            vw = new CylinderTriggeretriggerWrapper();
        }
        else if (typeName == "XCameraWall")
        {
            vw = new SceneObjPathWrapper();
        }
        else if (typeName == "XCurve")
        {
            vw = new ResObjPathWrapper();
        }
        else if (typeName == "System.Boolean")
        {
            vw = new BoolValueWrapper();
        }
        else if (typeName == "System.Single")
        {
            vw = new FloatValueWrapper();
        }
        else if (typeName == "XTransferWall+transfer_type")
        {
            vw = new TransferWalltransferWrapper();
        }
        else if (typeName == "System.Int32")
        {
            vw = new IntValueWrapper();
        }
        else if (typeName == "UnityEngine.GameObject")
        {
            vw = new SceneObjPathWrapper();
        }
        else if (typeName == "UnityEngine.Renderer[]")
        {
            vw = new SceneObjListPathWrapper();
        }
        else if (typeName == "System.Collections.Generic.List`1[UnityEngine.GameObject]")
        {
            vw = new SceneObjListPathWrapper();
        }
        if (vw != null)
        {
            vw.Parse(f, mb);
        }
        return vw;
    }
    public static ValueWrapper ReadValue(BinaryReader br, string typeName)
    {
        ValueWrapper vw = null;
        if (typeName == "System.String")
        {
            vw = new StringValueWrapper();
        }
        else if (typeName == "XSpawnWall+etrigger_type")
        {
            vw = new SpawnWalletriggerWrapper();
        }
        else if (typeName == "XCylinderTrigger+etrigger_type")
        {
            vw = new CylinderTriggeretriggerWrapper();
        }
        else if (typeName == "XCameraWall")
        {
            vw = new SceneObjPathWrapper();
        }
        else if (typeName == "XCurve")
        {
            vw = new ResObjPathWrapper();
        }
        else if (typeName == "System.Boolean")
        {
            vw = new BoolValueWrapper();
        }
        else if (typeName == "System.Single")
        {
            vw = new FloatValueWrapper();
        }
        else if (typeName == "XTransferWall+transfer_type")
        {
            vw = new TransferWalltransferWrapper();
        }
        else if (typeName == "System.Int32")
        {
            vw = new IntValueWrapper();
        }
        else if (typeName == "UnityEngine.GameObject")
        {
            vw = new SceneObjPathWrapper();
        }
        else if (typeName == "UnityEngine.Renderer[]")
        {
            vw = new SceneObjListPathWrapper();
        }
        else if (typeName == "System.Collections.Generic.List`1[UnityEngine.GameObject]")
        {
            vw = new SceneObjListPathWrapper();
        }
        if (vw != null)
        {
            vw.Read(br);
        }
        return vw;
    }
}
public class StringValueWrapper : ValueWrapper
{
    public string value = "";
    public override bool Equal(ValueWrapper valueWrapper)
    {
        StringValueWrapper w = valueWrapper as StringValueWrapper;
        if (w != null)
        {
            return w.value == value;
        }
        return true;
    }
    public override void Parse(FieldInfo f, MonoBehaviour mb)
    {
        value = f.GetValue(mb) as string;
    }
    public override void Write(BinaryWriter bw)
    {
        bw.Write(value);
    }
    public override void Read(BinaryReader br)
    {
        value = br.ReadString();
    }
}
public class SpawnWalletriggerWrapper : ValueWrapper
{
    public XSpawnWall.etrigger_type value = XSpawnWall.etrigger_type.once;
    public override bool Equal(ValueWrapper valueWrapper)
    {
        SpawnWalletriggerWrapper w = valueWrapper as SpawnWalletriggerWrapper;
        if (w != null)
        {
            return w.value == value;
        }
        return false;
    }
    public override void Parse(FieldInfo f, MonoBehaviour mb)
    {
        value = (XSpawnWall.etrigger_type)f.GetValue(mb);
    }
    public override void Write(BinaryWriter bw)
    {
        bw.Write((int)value);
    }
    public override void Read(BinaryReader br)
    {
        int v = br.ReadInt32();
        value = (XSpawnWall.etrigger_type)v;
    }
}

public class CylinderTriggeretriggerWrapper : ValueWrapper
{
    public XCylinderTrigger.etrigger_type value = XCylinderTrigger.etrigger_type.once;
    public override bool Equal(ValueWrapper valueWrapper)
    {
        CylinderTriggeretriggerWrapper w = valueWrapper as CylinderTriggeretriggerWrapper;
        if (w != null)
        {
            return w.value == value;
        }
        return false;
    }
    public override void Parse(FieldInfo f, MonoBehaviour mb)
    {
        value = (XCylinderTrigger.etrigger_type)f.GetValue(mb);
    }
    public override void Write(BinaryWriter bw)
    {
        bw.Write((int)value);
    }
    public override void Read(BinaryReader br)
    {
        int v = br.ReadInt32();
        value = (XCylinderTrigger.etrigger_type)v;
    }
}

public class SceneObjPathWrapper : ValueWrapper
{
    public string value = "";
    public override bool Equal(ValueWrapper valueWrapper)
    {
        SceneObjPathWrapper w = valueWrapper as SceneObjPathWrapper;
        if (w != null)
        {
            return w.value == value;
        }
        return false;
    }
    public override void Parse(FieldInfo f, MonoBehaviour mb)
    {
        UnityEngine.Object obj = f.GetValue(mb) as UnityEngine.Object;
        if (obj != null)
        {
            GameObject go = null;
            if (obj is Component)
            {
                Component c = obj as Component;
                go = c.gameObject;

            }
            else if (obj is GameObject)
            {
                go = obj as GameObject;
            }
            value = GetGOPath(go);
        }
    }
    public override void Write(BinaryWriter bw)
    {
        bw.Write(value);
    }
    public override void Read(BinaryReader br)
    {
        value = br.ReadString();
    }

    public static string GetGOPath(GameObject go)
    {
        string path = "";
        if (go != null)
        {
            path = go.name;
            Transform t = go.transform;
            while (t.parent != null)
            {
                path = t.parent.name + "/" + path;
                t = t.parent;
            }
        }
        return path;
    }
}
public class SceneObjListPathWrapper : ValueWrapper
{
    public List<string> value = new List<string>();
    public override bool Equal(ValueWrapper valueWrapper)
    {
        SceneObjListPathWrapper w = valueWrapper as SceneObjListPathWrapper;
        if (w != null)
        {
            if (value.Count != w.value.Count)
                return false;
            for (int i = 0; i < value.Count; ++i)
            {
                if (w.value[i] != value[i])
                    return false;
            }
            return true;
        }
        return false;
    }
    public override void Parse(FieldInfo f, MonoBehaviour mb)
    {
        System.Object obj = f.GetValue(mb);
        if (obj is System.Collections.IList)
        {
            System.Collections.IList list = obj as System.Collections.IList;
            foreach (System.Object o in list)
            {
                GameObject go = o as GameObject;
                string path = SceneObjPathWrapper.GetGOPath(go);
                value.Add(path);
            }
        }
        else if (obj is Array)
        {
            Array arr = obj as Array;
            foreach (System.Object o in arr)
            {
                Renderer r = o as Renderer;
                string path = SceneObjPathWrapper.GetGOPath(r != null ? r.gameObject : null);
                value.Add(path);
            }
        }
    }
    public override void Write(BinaryWriter bw)
    {
        bw.Write(value.Count);
        for (int i = 0; i < value.Count; ++i)
        {
            bw.Write(value[i]);
        }

    }
    public override void Read(BinaryReader br)
    {
        int count = br.ReadInt32();
        for (int i = 0; i < count; ++i)
        {
            value.Add(br.ReadString());
        }
    }
}
public class ResObjPathWrapper : ValueWrapper
{
    public string value = "";
    public override bool Equal(ValueWrapper valueWrapper)
    {
        ResObjPathWrapper w = valueWrapper as ResObjPathWrapper;
        if (w != null)
        {
            return w.value == value;
        }
        return false;
    }
    public override void Parse(FieldInfo f, MonoBehaviour mb)
    {
        UnityEngine.Object obj = f.GetValue(mb) as UnityEngine.Object;
        if (obj != null)
        {
            value = AssetDatabase.GetAssetPath(obj);
        }
    }
    public override void Write(BinaryWriter bw)
    {
        bw.Write(value);
    }
    public override void Read(BinaryReader br)
    {
        value = br.ReadString();
    }
}

public class BoolValueWrapper : ValueWrapper
{
    public bool value = false;
    public override bool Equal(ValueWrapper valueWrapper)
    {
        BoolValueWrapper w = valueWrapper as BoolValueWrapper;
        if (w != null)
        {
            return w.value == value;
        }
        return false;
    }
    public override void Parse(FieldInfo f, MonoBehaviour mb)
    {
        value = (bool)f.GetValue(mb);
    }
    public override void Write(BinaryWriter bw)
    {
        bw.Write(value);
    }
    public override void Read(BinaryReader br)
    {
        value = br.ReadBoolean();
    }
}

public class FloatValueWrapper : ValueWrapper
{
    public float value = 0.0f;
    public override bool Equal(ValueWrapper valueWrapper)
    {
        FloatValueWrapper w = valueWrapper as FloatValueWrapper;
        if (w != null)
        {
            float d = Mathf.Abs(w.value - value);
            return d <= 0.01f;
        }
        return false;
    }
    public override void Parse(FieldInfo f, MonoBehaviour mb)
    {
        value = (float)f.GetValue(mb);
    }
    public override void Write(BinaryWriter bw)
    {
        bw.Write(value);
    }
    public override void Read(BinaryReader br)
    {
        value = br.ReadSingle();
    }
}

public class IntValueWrapper : ValueWrapper
{
    public int value = 0;
    public override bool Equal(ValueWrapper valueWrapper)
    {
        IntValueWrapper w = valueWrapper as IntValueWrapper;
        if (w != null)
        {
            return w.value == value;
        }
        return false;
    }
    public override void Parse(FieldInfo f, MonoBehaviour mb)
    {
        value = (int)f.GetValue(mb);
    }
    public override void Write(BinaryWriter bw)
    {
        bw.Write(value);
    }
    public override void Read(BinaryReader br)
    {
        value = br.ReadInt32();
    }
}
public class TransferWalltransferWrapper : ValueWrapper
{
    public XTransferWall.transfer_type value = XTransferWall.transfer_type.current_scene;
    public override bool Equal(ValueWrapper valueWrapper)
    {
        TransferWalltransferWrapper w = valueWrapper as TransferWalltransferWrapper;
        if (w != null)
        {
            return w.value == value;
        }
        return false;
    }
    public override void Parse(FieldInfo f, MonoBehaviour mb)
    {
        value = (XTransferWall.transfer_type)f.GetValue(mb);
    }
    public override void Write(BinaryWriter bw)
    {
        bw.Write((int)value);
    }
    public override void Read(BinaryReader br)
    {
        int v = br.ReadInt32();
        value = (XTransferWall.transfer_type)v;
    }
}
public class ComponentValueInfo
{
    public string fieldName = "";
    public string typeName = "";
    public ValueWrapper value = null;
    public void SyncData(MonoBehaviour mb, Type t, bool sync, ref string errorStr)
    {
        FieldInfo field = t.GetField(fieldName);
        if (field != null)
        {
            ValueWrapper vw = ValueWrapper.Parse(field, mb, field.FieldType.ToString());
            if (vw != null && !vw.Equal(value))
            {
                errorStr += "Field:" + fieldName + " Value not same \r\n";
            }
        }
        else
        {
            errorStr += "Field:" + fieldName + " not found \r\n";
        }
    }
}


public class ComponentFieldInfo
{
    public bool enable = true;
    public string typeName = "";
    public List<ComponentValueInfo> valueList = new List<ComponentValueInfo>();
    public void Write(StreamWriter sw)
    {
        sw.WriteLine(typeName);
        for (int i = 0; i < valueList.Count; ++i)
        {
            ComponentValueInfo cvi = valueList[i];
            string str = cvi.fieldName + ": ";

            if (cvi.value != null)
            {
                str += cvi.value.ToString();
            }
            else
            {
                str += "null";
            }
            sw.WriteLine(str);
        }
    }
    public void Write(BinaryWriter bw)
    {
        bw.Write(typeName);
        bw.Write(valueList.Count);
        for (int i = 0; i < valueList.Count; ++i)
        {
            ComponentValueInfo cvi = valueList[i];
            bw.Write(cvi.fieldName);
            bw.Write(cvi.typeName);
            if (cvi.value != null)
            {
                cvi.value.Write(bw);
            }
            else
            {
                bw.Write(0);
            }
        }
    }

    public void Read(BinaryReader br)
    {
        typeName = br.ReadString();
        int count = br.ReadInt32();
        for (int i = 0; i < count; ++i)
        {
            ComponentValueInfo cvi = new ComponentValueInfo();
            valueList.Add(cvi);
            cvi.fieldName = br.ReadString();
            cvi.typeName = br.ReadString();
            cvi.value = ValueWrapper.ReadValue(br, cvi.typeName);
        }
    }

    public void SyncData(GameObject go, bool sync, ref string errorStr)
    {
        MonoBehaviour mb = go.GetComponent(typeName) as MonoBehaviour;
        if (mb != null)
        {
            Type t = mb.GetType();
            for (int i = 0; i < valueList.Count; ++i)
            {
                ComponentValueInfo cvi = valueList[i];
                cvi.SyncData(mb, t, sync, ref errorStr);
            }
        }
        else
        {
            errorStr += "Script:" + typeName + " not found \r\n";
        }
    }
}
public class ComponentInfo
{
    public string name = "";
    public string path = "";
    public string pathStr = "";
    public bool active = true;
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;
    public bool hasBoxCollider = false;
    public bool isTrigger = false;
    public Vector3 center;
    public Vector3 size;
    public List<ComponentFieldInfo> fieldList = new List<ComponentFieldInfo>();

    public void Write(StreamWriter sw)
    {
        sw.WriteLine(path);
        for (int i = 0; i < fieldList.Count; ++i)
        {
            ComponentFieldInfo cfi = fieldList[i];
            cfi.Write(sw);
        }
    }
    public void Write(BinaryWriter bw)
    {
        bw.Write(name);
        bw.Write(path);
        bw.Write(pathStr);
        bw.Write(active);
        bw.Write(pos.x);
        bw.Write(pos.y);
        bw.Write(pos.z);
        bw.Write(rot.x);
        bw.Write(rot.y);
        bw.Write(rot.z);
        bw.Write(rot.w);
        bw.Write(scale.x);
        bw.Write(scale.y);
        bw.Write(scale.z);
        bw.Write(hasBoxCollider);
        if (hasBoxCollider)
        {
            bw.Write(isTrigger);
            bw.Write(center.x);
            bw.Write(center.y);
            bw.Write(center.z);
            bw.Write(size.x);
            bw.Write(size.y);
            bw.Write(size.z);
        }
        bw.Write(fieldList.Count);
        for (int i = 0; i < fieldList.Count; ++i)
        {
            ComponentFieldInfo cfi = fieldList[i];
            cfi.Write(bw);
        }
    }
    public void Read(BinaryReader br)
    {
        name = br.ReadString();
        path = br.ReadString();
        pathStr = br.ReadString();
        active = br.ReadBoolean();
        pos.x = br.ReadSingle();
        pos.y = br.ReadSingle();
        pos.z = br.ReadSingle();
        rot.x = br.ReadSingle();
        rot.y = br.ReadSingle();
        rot.z = br.ReadSingle();
        rot.w = br.ReadSingle();
        scale.x = br.ReadSingle();
        scale.y = br.ReadSingle();
        scale.z = br.ReadSingle();
        hasBoxCollider = br.ReadBoolean();
        if (hasBoxCollider)
        {
            isTrigger = br.ReadBoolean();
            center.x = br.ReadSingle();
            center.y = br.ReadSingle();
            center.z = br.ReadSingle();
            size.x = br.ReadSingle();
            size.y = br.ReadSingle();
            size.z = br.ReadSingle();
        }
        int count = br.ReadInt32();
        for (int i = 0; i < count; ++i)
        {
            ComponentFieldInfo cfi = new ComponentFieldInfo();
            cfi.Read(br);
            fieldList.Add(cfi);
        }
    }

    private GameObject FindGameObject(Transform t)
    {
        if(string.IsNullOrEmpty(path))
        {
            return null;
        }
        string[] indexs = path.Split(',');
        for (int i = 0; i < indexs.Length; ++i)
        {
            int index = int.Parse(indexs[i]);
            if (index >= 0 && index < t.childCount)
            {
                t = t.GetChild(index);
            }
            else
            {
                return null;
            }
        }
        if(t.gameObject.name==name)
        {
            return t.gameObject;
        }
        return null;
    }
    public void SyncData(bool sync,GameObject dynamicScene,ref string errorStr,ref string transformErrorStr)
    {
        string newPath = string.Format("{0}({1})", pathStr, path);
        GameObject go = FindGameObject(dynamicScene.transform);
        if (go != null)
        {
            if (active != go.activeSelf)
            {
                errorStr += "active not same\r\n";
            }
            float delta = (pos - go.transform.localPosition).magnitude;
            if (delta > 0.1f)
            {
                transformErrorStr += "pos not same\r\n";
            }
            Quaternion q = go.transform.localRotation;

            delta = (rot.x - q.x) * (rot.x - q.x) + (rot.y - q.y) * (rot.y - q.y) + (rot.z - q.z) * (rot.z - q.z) + (rot.w - q.w) * (rot.w - q.w);
            if (delta > 0.1f )
            {
                transformErrorStr += "rot not same\r\n";
            }
            delta = (scale - go.transform.localScale).magnitude;
            if (delta > 0.1f)
            {
                transformErrorStr += "scale not same\r\n";
            }
            if (hasBoxCollider)
            {
                BoxCollider bc = go.GetComponent<BoxCollider>();
                if (bc == null)
                {
                    errorStr += "BoxCollider not same\r\n";
                }
                else
                {
                    if (bc.isTrigger != isTrigger)
                    {
                        errorStr += "BoxCollider isTrigger not same\r\n";
                    }
                    delta = (center - bc.center).magnitude;
                    if (delta > 0.1f)
                    {
                        errorStr += "BoxCollider center not same\r\n";
                    }
                    delta = (size - bc.size).magnitude;
                    if (delta > 0.1f)
                    {
                        errorStr += "BoxCollider size not same\r\n";
                    }
                }
            }
            for (int i = 0; i < fieldList.Count; ++i)
            {
                ComponentFieldInfo cfi = fieldList[i];
                cfi.SyncData(go, sync, ref errorStr);
            }
            if (!string.IsNullOrEmpty(errorStr))
            {
                
                errorStr = newPath + "\r\n" + errorStr + "\r\n";
                Debug.LogError(errorStr);
            }
            if (!string.IsNullOrEmpty(transformErrorStr))
            {
                transformErrorStr = newPath + "\r\n" + transformErrorStr + "\r\n";
            }
        }
        else
        {
            errorStr = newPath + "\r\n" + "GameObject not found\r\n";
            Debug.LogError(errorStr);
        }

    }
}

[ExecuteInEditMode]
internal class TurnScriptU4ToU5Editor : EditorWindow
{
    string CUSTOMPATH = "Assets/XScene/";
    string SceneName = "";
    bool haveError;

    List<ComponentInfo> componentsInfo = new List<ComponentInfo>();
    public string CurrSceneName;

#if UNITY_5_5
    UnityEngine.SceneManagement.Scene CurrScene;
#endif

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        CUSTOMPATH = EditorGUILayout.TextField("Dir", CUSTOMPATH);
        GUILayout.Space(20f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        bool writeall = GUILayout.Button("WriteAll");
        GUILayout.Space(20f);
        GUILayout.EndHorizontal();       

        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        bool writecurrent = GUILayout.Button("WriteCurrent");
        GUILayout.Space(20f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        bool checkAll = GUILayout.Button("CheckAll");
        GUILayout.Space(20f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        bool checkCurrent = GUILayout.Button("CheckCurrent");
        GUILayout.Space(20f);
        GUILayout.EndHorizontal();

        if (writeall)
        {
            valueType.Clear();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                string[] ss = scenes[i].path.Split('/', '.');
                CurrSceneName = ss[ss.Length - 2];
                EditorUtility.DisplayProgressBar(string.Format("DealWith - {0}/{1}", i + 1, scenes.Length), CurrSceneName, (i + 1) * 1f / scenes.Length);
                OpenScene(scenes[i].path);
                CheckAll();
                WriteFile(CUSTOMPATH, CurrSceneName);
            }
            var it = valueType.GetEnumerator();
            //WriteFile(CUSTOMPATH, saveStr);
            EditorUtility.ClearProgressBar();
        }

        if (writecurrent)
        {
            valueType.Clear();
#if UNITY_5_5
            CurrScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            CurrSceneName = CurrScene.name;
#else
            CurrSceneName = EditorApplication.currentScene;
            string[] ss = CurrSceneName.Split('/', '.');
            CurrSceneName = ss[ss.Length - 2];
#endif
            CheckAll();
            WriteFile(CUSTOMPATH, CurrSceneName);
        }

        if (checkAll)
        {
            sb0.Length = 0;
            sb1.Length = 0;
            DirectoryInfo di = new DirectoryInfo("Assets/XScene/Data/");
            FileInfo[] fileInfos = di.GetFiles("*.bytes", SearchOption.TopDirectoryOnly);
            for (int j=0;j< fileInfos.Length;++j)
            {
                FileInfo fileInfo = fileInfos[j];
                EditorUtility.DisplayProgressBar(string.Format("DealWith - {0}/{1}", j + 1, fileInfos.Length), CurrSceneName, (j + 1) * 1f / fileInfos.Length);
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                for (int i = 0; i < scenes.Length; i++)
                {
                    string[] ss = scenes[i].path.Split('/', '.');
                    CurrSceneName = ss[ss.Length - 2];
                    string name = CurrSceneName + ".bytes";
                    if (name.ToLower() == fileInfo.Name.ToLower())
                    {
                        
                        OpenScene(scenes[i].path);
                        SyncData(fileInfo.FullName, false);
                        break;
                    }
                }
            }
            File.WriteAllText("Assets/XScene/Diff.txt", sb0.ToString());
            File.WriteAllText("Assets/XScene/DiffPos.txt", sb1.ToString());
            EditorUtility.ClearProgressBar();
        }
        if (checkCurrent)
        {
#if UNITY_5_5
            CurrScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            CurrSceneName = CurrScene.name;
#else
            CurrSceneName = EditorApplication.currentScene;
            string[] ss = CurrSceneName.Split('/', '.');
            CurrSceneName = ss[ss.Length - 2];
#endif

            SyncData("Assets/XScene/Data/" + CurrSceneName + ".bytes", false);
        }
    }

    public void SaveScene(string path)
    {
#if UNITY_5_5
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(CurrScene, path);
#else
        //lose
#endif
    }

    public void OpenScene(string path)
    {
#if UNITY_5_5
        CurrScene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path);
#else
        EditorApplication.OpenScene(path);
#endif
    }

    public void CheckAll()
    {
        componentsInfo.Clear();
        GameObject parent = GameObject.Find("DynamicScene");
        if (parent != null)
            FindAndFind("", parent.transform, "", true);
    }
    private List<MonoBehaviour> tmp = new List<MonoBehaviour>();
    private HashSet<string> valueType = new HashSet<string>();


    public void FindAndFind(string path, Transform ts, string parentIndexStr, bool root = false)
    {
        string nextPath = root ? "" : path + '/';
        for (int i = 0; i < ts.childCount; i++)
        {
            Transform child = ts.GetChild(i);
            string currentPath = i.ToString();
            if (parentIndexStr != "")
            {
                currentPath = parentIndexStr + "," + i.ToString();
            }
            ComponentInfo ci = null;
            tmp.Clear();
            child.GetComponents<MonoBehaviour>(tmp);
            if (tmp.Count > 0)
            {
                ci = new ComponentInfo();
                ci.path = currentPath;
                ci.name = child.name;
                ci.pathStr = root ? child.name : path + '/' + child.name;
                for (int j = 0; j < tmp.Count; ++j)
                {
                    ComponentFieldInfo cfi = new ComponentFieldInfo();
                    ci.fieldList.Add(cfi);
                    MonoBehaviour mb = tmp[j];
                    Type t = mb.GetType();
                    cfi.typeName = t.Name;
                    FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (FieldInfo f in fields)
                    {
                        ComponentValueInfo cvi = new ComponentValueInfo();
                        cfi.valueList.Add(cvi);
                        cvi.fieldName = f.Name;
                        cvi.typeName = f.FieldType.ToString();
                        cvi.value = ValueWrapper.Parse(f, mb, cvi.typeName);
                        if (!valueType.Contains(cvi.typeName))
                        {
                            valueType.Add(cvi.typeName);
                        }
                    }

                }
                tmp.Clear();
                componentsInfo.Add(ci);
            }
            BoxCollider bc = child.GetComponent<BoxCollider>();
            if (bc != null)
            {
                if (ci == null)
                {
                    ci = new ComponentInfo();
                    ci.name = child.name;
                    ci.path = currentPath;
                    ci.pathStr = root ? child.name : path + '/' + child.name;
                    componentsInfo.Add(ci);
                }
                ci.hasBoxCollider = true;
                ci.isTrigger = bc.isTrigger;
                ci.center = bc.center;
                ci.size = bc.size;
            }
            if (ci != null)
            {
                ci.active = child.gameObject.activeSelf;
                ci.pos = child.localPosition;
                ci.rot = child.localRotation;
                ci.scale = child.localScale;
            }

            //XSpawnWall wall = child.GetComponent<XSpawnWall>();
            //if (wall != null)
            //    OutPut(wall, nextPath + child.name);

            if (child.childCount != 0)
                FindAndFind(nextPath + child.name, child, currentPath);
        }
    }

    public void WriteFile(string path, string sceneName)
    {
        //write str
        using (FileStream desStream = new FileStream(path + sceneName + ".txt", FileMode.Create))
        {
            StreamWriter sw = new StreamWriter(desStream);
            string line = string.Format("Component Count:{0}", componentsInfo.Count);
            sw.WriteLine(line);
            for (int i = 0; i < componentsInfo.Count; ++i)
            {
                ComponentInfo ci = componentsInfo[i];
                ci.Write(sw);
                sw.WriteLine();
            }
            sw.Flush();
        }

        //write bin
        using (FileStream desStream = new FileStream(path + sceneName + ".bytes", FileMode.Create))
        {
            BinaryWriter bw = new BinaryWriter(desStream);
            bw.Write(componentsInfo.Count);
            for (int i = 0; i < componentsInfo.Count; ++i)
            {
                ComponentInfo ci = componentsInfo[i];
                ci.Write(bw);
            }
            bw.Flush();
        }

    }
    System.Text.StringBuilder sb0 = new System.Text.StringBuilder();
    System.Text.StringBuilder sb1 = new System.Text.StringBuilder();
    public void SyncData(string path, bool sync)
    {
        using (FileStream desStream = new FileStream(path, FileMode.Open))
        {
            if (desStream.Length > 0)
            {
                sb0.AppendLine("\r\nProcess Scene:\r\n" + path);
                sb1.AppendLine("\r\nProcess Scene:r\n" + path);
                GameObject go = GameObject.Find("DynamicScene");
                BinaryReader br = new BinaryReader(desStream);
                int count = br.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    ComponentInfo ci = new ComponentInfo();
                    ci.Read(br);
                    string errorStr = "";
                    string transformErrorStr = "";
                    ci.SyncData(sync, go, ref errorStr, ref transformErrorStr);
                    if (errorStr != "")
                    {
                        sb0.Append(errorStr);
                    }
                    if (transformErrorStr != "")
                    {
                        sb1.Append(transformErrorStr);
                    }
                }
            }
        }
    }
}