using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Profiling;
using UnityEngine.Scripting;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
class CustomProfilerWindow : EditorWindow
{
    internal class Styles
    {
        public static GUIStyle background = "OL Box";

        public static GUIStyle header = "OL title";

        public static GUIStyle entryEven = "OL EntryBackEven";

        public static GUIStyle entryOdd = "OL EntryBackOdd";

        public static GUIStyle numberLabel = "OL Label";

        public static GUIStyle foldout = "IN foldout";
    }
    
    //internal class ObjectInfo
    //{
    //    public int instanceId;

    //    public int memorySize;

    //    public int reason;

    //    public List<ObjectInfo> referencedBy;

    //    public string name;

    //    public string className;
    //}
    internal enum EStatus
    {
        None,
        Removed,
        Persistent,
        Added,
    }
    internal interface IMemoryElement
    {
        string Name
        {
            get;
        }
        bool IsExpanded
        {
            get;
            set;
        }
        EStatus Status
        {
            get;
        }

        bool IsFolder
        {
            get;
        }
        int GetSize0(EStatus state);
        int GetSize1(EStatus state);
        int GetRecursiveCount(EStatus state);
        int GetChildCount();
        IMemoryElement Get(int index, EStatus state);
    }

    [Serializable]
    internal class MemoryElement : IMemoryElement
    {
        public List<MemoryElement> children = new List<MemoryElement>();

        //public MemoryElement parent = null;

        //public ObjectInfo memoryInfo;
        public bool isAsset = false;

        public int totalMemory = 0;

        public string name = "None";

        public bool expanded = false;

        public EStatus processedState = EStatus.None;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return expanded;
            }
            set
            {
                expanded = value;
            }
        }
     
        public EStatus Status
        {
            get
            {
                return processedState;
            }
        }
        public bool IsFolder
        {
            get
            {
                return !isAsset;
            }
        }
        public int GetSize0(EStatus state)
        {
            return totalMemory;
        }
        public int GetSize1(EStatus state)
        {
            return 0;
        }
        public int GetRecursiveCount(EStatus state)
        {
            if (isAsset)
            {
                return 1;
            }
            return children.Count;
        }
        public int GetChildCount()
        {
            return GetRecursiveCount(EStatus.None);
        }
        public IMemoryElement Get(int index, EStatus state)
        {
            return children[index];
        }
        public void Clear()
        {
            if (children != null)
                children.Clear();
            //parent = null;
            //memoryInfo = null;
            totalMemory = 0;
            name = null;
            expanded = false;
        }

        public MemoryElement FindChild(MemoryElement node)
        {
            if (children != null)
            {
                for (int i = 0; i < children.Count; ++i)
                {
                    MemoryElement me = children[i];
                    if (me.name == node.name)
                    {
                        me.processedState = EStatus.Persistent;
                        return me;
                    }
                }
            }
            return null;
        }

        public void Save(BinaryWriter bw)
        {
            bw.Write(name);
            bw.Write(totalMemory);
            bw.Write(isAsset);
            int count = children.Count;
            bw.Write(count);
            for(int i=0;i<count;++i)
            {
                MemoryElement me = children[i];
                me.Save(bw);
            }
        }
        public void Load(BinaryReader br)
        {
            name = br.ReadString();
            totalMemory = br.ReadInt32();
            isAsset = br.ReadBoolean();
            int count = br.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                MemoryElement me = new MemoryElement();
                me.Load(br);
                children.Add(me);
            }
        }

    }
    internal class DiffMemoryElement : IMemoryElement
    {
        public List<DiffMemoryElement> children = new List<DiffMemoryElement>();

        public int persistentSize0 = 0;
        public int persistentSize1 = 0;
        public int addSize = 0;
        public int removeSize = 0;

        public int persistentCount = 0;
        public int addCount = 0;
        public int removeCount = 0;        

        public string name;
        public bool expanded;
        public bool isAsset = false;
        public EStatus processedState = EStatus.None;
        public string Name
        {
            get
            {
                return name;
            }
        }
        public bool IsExpanded
        {
            get
            {
                return expanded;
            }
            set
            {
                expanded = value;
            }
        }
        public EStatus Status
        {
            get
            {
                return processedState;
            }
        }
        public bool IsFolder
        {
            get
            {
                 return !isAsset;
            }
        }
        public int GetSize0(EStatus state)
        {
            switch (state)
            {
                case EStatus.None:
                    {
                        switch (processedState)
                        {
                            case EStatus.None:
                                return persistentSize0;
                            case EStatus.Removed:
                                return removeSize;
                            case EStatus.Persistent:
                                return persistentSize0;
                            case EStatus.Added:
                                return 0;

                        }
                    }                   
                    return persistentSize0;
                case EStatus.Removed:
                    return removeSize;
                case EStatus.Persistent:
                    return persistentSize0;
                case EStatus.Added:
                    return addSize;

            }
            return 0;
        }
        public int GetSize1(EStatus state)
        {
            switch (state)
            {
                case EStatus.None:
                    {
                        switch (processedState)
                        {
                            case EStatus.None:
                                return persistentSize0;
                            case EStatus.Removed:
                                return 0;
                            case EStatus.Persistent:
                                return persistentSize0;
                            case EStatus.Added:
                                return addSize;
                        }
                    }
                    return persistentSize0;
                case EStatus.Removed:
                    return removeSize;
                case EStatus.Persistent:
                    return persistentSize1;
                case EStatus.Added:
                    return 0;

            }
            return 0;
        }
        public int GetRecursiveCount(EStatus state)
        {
            if (isAsset)
            {
                return 1;
            }                
            else
            {
                switch (state)
                {
                    case EStatus.None:
                        return persistentCount + addCount + removeCount;
                    case EStatus.Removed:
                        return removeCount;
                    case EStatus.Persistent:
                        return persistentCount;
                    case EStatus.Added:
                        return addCount;

                }
            }
            
            return 0;
        }

        public int GetChildCount()
        {
            if (isAsset)
            {
                return 1;
            }
            else
            {
                return children.Count;
            }
        }
        public IMemoryElement Get(int index, EStatus state)
        {
            IMemoryElement me = children[index];
            if (state == EStatus.None || me.IsFolder && me.GetRecursiveCount(state) > 0)
            {
                return me;
            }
            return me.Status == state ? me : null;
        }
        public void CalcSize()
        {
            int sizeP = 0;
            int sizeA = 0;
            int sizeR = 0;
            int size1 = 0;
            int countP = 0;
            int countA = 0;
            int countR = 0;
            for (int i = 0; i < children.Count; ++i)
            {
                DiffMemoryElement me = children[i];
                me.CalcSize();
                sizeP += me.persistentSize0;
                size1 += me.persistentSize1;
                sizeA += me.addSize;
                sizeR += me.removeSize;
                if (me.IsFolder)
                {
                    countP += me.persistentCount;
                    countA += me.addCount;
                    countR += me.removeCount;
                }
                else
                {
                    countP += me.processedState == EStatus.Persistent ? 1 : 0;
                    countA += me.processedState == EStatus.Added ? 1 : 0;
                    countR += me.processedState == EStatus.Removed ? 1 : 0;
                }
            }
            persistentSize0 += sizeP;
            persistentSize1 += size1;
            persistentCount += countP;
            addSize += sizeA;            
            addCount += countA;
            removeSize += sizeR;
            removeCount += countR;
        }
        public void Clear()
        {
            children.Clear();
            persistentSize0 = 0;
            persistentSize1 = 0;
            addSize = 0;
            removeSize = 0;
            persistentCount = 0;
            addCount = 0;
            removeCount = 0;
            name = null;
            expanded = false;
            processedState = EStatus.Removed;
        }
    }
    private FieldInfo memoryElementField = null;
    private FieldInfo showDetailedMemoryPaneField = null;
    private MethodInfo refreshMemoryDataMethod = null;

    private object profileWindow = null;
    private object memoryListView = null;
    private object rootElement = null;

    private List<MemoryElement> profileHistory = new List<MemoryElement>();
    
    private MemoryElement currentRoot = new MemoryElement();
    private DiffMemoryElement diffRoot = new DiffMemoryElement();
    private MemoryElement currentDisplayRoot = null;    
    private EStatus diffState = EStatus.None;
    private IMemoryElement currentSelected = null;
    private IMemoryElement diffSelected = null;
    private bool isDiff = false;
    private bool sizeChange = false;

    private List<string> snapShotIndex = new List<string>();
    private string[] snapShotIndexArr = null;
    private int diffIndex0 = 0;
    private int diffIndex1 = 0;

    private FieldInfo childrenField = null;
    private FieldInfo totalMemoryField = null;
    private FieldInfo totalChildCountField = null;
    private FieldInfo nameField = null;
    private FieldInfo descriptionField = null;

    //private Type objectInfoType = null;
    private FieldInfo instanceIdField = null;

    private FieldInfo memorySizeField = null;

    private FieldInfo reasonField = null;
    private FieldInfo referencedByField = null;
    private FieldInfo objNameField = null;
    private FieldInfo classNameField = null;

    private Type GUIClipType = null;
    private static PropertyInfo visibleRectField = null;

    private float selectionOffset = 0.0f;
    private float visibleHeight = 0.0f;
    private int controlID;

    private object vertSplit = null;
    private MethodInfo beginSplitMethod = null;
    private MethodInfo endHorizontalSplitMethod = null;
    private object[] splitParam = null;
    private Vector2 currentMemoryInfoScrollView = Vector2.zero;
    private Vector2 diffMemoryInfoScrollView = Vector2.zero;
    private static Assembly editorAssembly = Assembly.GetAssembly(typeof(Editor));
    private static Assembly engineAssembly = Assembly.GetAssembly(typeof(UnityEngine.Object));
    [MenuItem(@"Assets/CustomProfiler", false, 0)]
    private static void CustomProfiler()
    {
        CustomProfilerWindow window = EditorWindow.GetWindow<CustomProfilerWindow>("CustomProfiler", true);
        window.position = new Rect(100, 100, 1200, 800);
        window.Init();
    }
   
    public void Init()
    {
        Type[] types = editorAssembly.GetTypes();


        Type memoryTreeListType = types.Where(t => t.Name == "MemoryTreeListClickable").FirstOrDefault();
        if (memoryTreeListType != null)
        {
            memoryElementField = memoryTreeListType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
        }            

        Type memoryElementType = types.Where(t => t.Name == "MemoryElement").FirstOrDefault();
        if (memoryElementType!=null)
        {
            childrenField = memoryElementType.GetField("children", BindingFlags.Public | BindingFlags.Instance);
            totalMemoryField = memoryElementType.GetField("totalMemory", BindingFlags.Public | BindingFlags.Instance);
            totalChildCountField = memoryElementType.GetField("totalChildCount", BindingFlags.Public | BindingFlags.Instance);
            nameField = memoryElementType.GetField("name", BindingFlags.Public | BindingFlags.Instance);
            descriptionField = memoryElementType.GetField("description", BindingFlags.Public | BindingFlags.Instance);
        }

        Type objectInfoType = types.Where(t => t.Name == "ObjectInfo").FirstOrDefault();
        if (objectInfoType!=null)
        {
            instanceIdField = objectInfoType.GetField("instanceId", BindingFlags.Public | BindingFlags.Instance);
            memorySizeField = objectInfoType.GetField("memorySize", BindingFlags.Public | BindingFlags.Instance);
            reasonField = objectInfoType.GetField("reason", BindingFlags.Public | BindingFlags.Instance);
            referencedByField = objectInfoType.GetField("referencedBy", BindingFlags.Public | BindingFlags.Instance);
            objNameField = objectInfoType.GetField("name", BindingFlags.Public | BindingFlags.Instance);
            classNameField = objectInfoType.GetField("className", BindingFlags.Public | BindingFlags.Instance);
        }

        Type profileWindowType = types.Where(t => t.Name == "ProfilerWindow").FirstOrDefault();
        if (profileWindowType != null)
        {
            showDetailedMemoryPaneField = profileWindowType.GetField("m_ShowDetailedMemoryPane", BindingFlags.NonPublic | BindingFlags.Instance);
            refreshMemoryDataMethod = profileWindowType.GetMethod("RefreshMemoryData", BindingFlags.NonPublic | BindingFlags.Instance);


            FieldInfo profilerWindowsField = profileWindowType.GetField("m_ProfilerWindows", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo memoryListViewField = profileWindowType.GetField("m_MemoryListView", BindingFlags.NonPublic | BindingFlags.Instance);
            if (profilerWindowsField != null && memoryListViewField != null)
            {
                IList memoryInfoList = profilerWindowsField.GetValue(null) as IList;
                if (memoryInfoList.Count > 0)
                {
                    profileWindow = memoryInfoList[0];
                    memoryListView = memoryListViewField.GetValue(profileWindow);
                    
                }
            }

            FieldInfo vertSplitField = profileWindowType.GetField("m_VertSplit", BindingFlags.NonPublic | BindingFlags.Instance);
            if (vertSplitField != null)
            {
                vertSplit = vertSplitField.GetValue(profileWindow);
            }

        }
        splitParam = new object[] { vertSplit, GUIStyle.none, true, null };
        Type splitterGUILayoutType = types.Where(t => t.Name == "SplitterGUILayout").FirstOrDefault();
        if(splitterGUILayoutType!=null)
        {
            beginSplitMethod = splitterGUILayoutType.GetMethod("BeginSplit", BindingFlags.Public | BindingFlags.Static);
            endHorizontalSplitMethod = splitterGUILayoutType.GetMethod("EndHorizontalSplit", BindingFlags.Public | BindingFlags.Static);
        }
        types = engineAssembly.GetTypes();
        GUIClipType = types.Where(t => t.Name == "GUIClip").FirstOrDefault();
        if (GUIClipType != null)
        {
            visibleRectField = GUIClipType.GetProperty("visibleRect", BindingFlags.Public | BindingFlags.Static);
        }
        controlID = 100;
        currentRoot.name = "None";
        currentDisplayRoot = currentRoot;

    }
    private void ParseMemoryInfo(object srcNode, MemoryElement customNode)
    {
        IList children = childrenField.GetValue(srcNode) as IList;
        customNode.totalMemory = (int)totalMemoryField.GetValue(srcNode);
        customNode.name = nameField.GetValue(srcNode) as string;   
        
        for (int i = 0; i < children.Count; ++i)
        {
            object child = children[i];
            MemoryElement me = new MemoryElement();
            customNode.children.Add(me);
            ParseMemoryInfo(child, me);
        }
        if (children.Count == 0)
        {
            customNode.isAsset = true;
        }
    }
    private void TakeSnapShot()
    {
        if (profileWindow != null)
        {
            if (showDetailedMemoryPaneField != null)
            {
                showDetailedMemoryPaneField.SetValue(profileWindow, ProfilerMemoryView.Detailed);
            }
            if (refreshMemoryDataMethod != null)
            {
                refreshMemoryDataMethod.Invoke(profileWindow, null);
            }
        }        
    }

    private void GetData()
    {
        if (memoryElementField != null && memoryListView != null)
        {
            rootElement = memoryElementField.GetValue(memoryListView);
            if (rootElement != null)
            {
                if (currentRoot.totalMemory != 0)
                {
                    profileHistory.Add(currentRoot);
                    currentRoot = new MemoryElement();
                }

                ParseMemoryInfo(rootElement, currentRoot);
                currentRoot.name = string.Format("{0}:{1}", profileHistory.Count, SceneManager.GetActiveScene().name);
                currentDisplayRoot = currentRoot;
                currentSelected = null;
                snapShotIndex.Add(currentRoot.name);
                snapShotIndexArr = snapShotIndex.ToArray();
            }
        }
        else
        {
            Debug.LogError("reflect MemoryElement m_Root fail!");
        }
    }
    private void Diff(MemoryElement me0, MemoryElement me1, DiffMemoryElement diff)
    {
        if (me1 != null)
        {
            for (int i = 0; i < me1.children.Count; ++i)
            {
                MemoryElement meSrc = me1.children[i];
                meSrc.processedState = EStatus.None;
            }
        }

        if (me0 != null)
        {
            for (int i = 0; i < me0.children.Count; ++i)
            {
                MemoryElement meSrc = me0.children[i];
                MemoryElement find = me1 != null ? me1.FindChild(meSrc) : null;
                DiffMemoryElement me = new DiffMemoryElement();
                me.name = meSrc.name;
                diff.children.Add(me);
                if (find != null)
                {
                    me.isAsset = find.isAsset;                    
                    me.processedState = EStatus.Persistent;
                    

                    if (meSrc.IsFolder)
                    {
                        me.persistentSize0 = 0;
                        me.persistentSize1 = 0;
                    }
                    else
                    {
                        me.persistentSize0 = meSrc.totalMemory;
                        me.persistentSize1 = find.totalMemory;
                    }
                }
                else
                {
                    me.isAsset = meSrc.isAsset;
                    me.processedState = EStatus.Added;
                    if (meSrc.IsFolder)
                    {
                        me.addSize = 0;
                    }
                    else
                    {
                        me.addSize = meSrc.totalMemory;
                    }
                }
                Diff(meSrc, find, me);
            }
        }

        if (me1 != null)
        {
            for (int i = 0; i < me1.children.Count; ++i)
            {
                MemoryElement meSrc = me1.children[i];
                if (meSrc.processedState == EStatus.None)
                {
                    DiffMemoryElement me = new DiffMemoryElement();
                    me.isAsset = meSrc.isAsset;
                    me.removeSize = meSrc.totalMemory;
                    me.name = meSrc.name;
                    me.processedState = EStatus.Removed;
                    diff.children.Add(me);
                    Diff(null, meSrc, me);
                }
            }
        }
            
    }
    private void Export()
    {
        string name = string.Format("{0}{1}{2}_{3}{4}{5}",
                DateTime.Now.Year.ToString().PadLeft(2, '0'), DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'),
                DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'), DateTime.Now.Second.ToString().PadLeft(2, '0'));
        string path = EditorUtility.SaveFilePanel("Export", "Assets/../", name, "data");
        try
        {
            if(!string.IsNullOrEmpty(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    BinaryWriter bw = new BinaryWriter(fs);
                    int count = profileHistory.Count;
                    bw.Write(count);
                    for (int i = 0; i < count; ++i)
                    {
                        MemoryElement me = profileHistory[i];
                        me.Save(bw);
                    }
                    currentRoot.Save(bw);
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

           
    }
    private void Import()
    {
        string path = EditorUtility.OpenFilePanel("Import", "Assets/../","data");
        try
        {
           
            if (!string.IsNullOrEmpty(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    snapShotIndex.Clear();
                    profileHistory.Clear();
                    BinaryReader br = new BinaryReader(fs);
                    int count = br.ReadInt32();
                    for (int i = 0; i < count; ++i)
                    {
                        MemoryElement me = new MemoryElement();
                        me.Load(br);
                        profileHistory.Add(me);
                        snapShotIndex.Add(me.name);
                    }
                    currentRoot.Load(br);
                }
                currentDisplayRoot = currentRoot;
                currentSelected = null;
                diffSelected = null;
                diffIndex0 = 0;
                diffIndex1 = 0;
                snapShotIndexArr = snapShotIndex.ToArray();
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }

    }

    private MemoryElement GetSnapShot(int index)
    {
        if (index >= 0 && index < profileHistory.Count)
            return profileHistory[index];
        return currentRoot;
    }
    private string FormatMemorySize(int size)
    {
        return string.Format("Size:{0}B-{1}KB-{2}MB", size, size / 1024, size / 1024 / 1024);
    }

    private static Rect GenerateRect(int row)
    {
        Rect rect = new Rect();
        if (visibleRectField != null)
        {
            rect = (Rect)visibleRectField.GetValue(null, null);
        }
        Rect result = new Rect(1f, 16f * (float)row, rect.width, 16f);
        return result;
    }

    private static void DrawBackground(int row, bool selected)
    {
        Rect position = GenerateRect(row);
        GUIStyle gUIStyle = (row % 2 != 0) ? Styles.entryOdd : Styles.entryEven;
        if (Event.current.type == EventType.Repaint)
        {
            gUIStyle.Draw(position, GUIContent.none, false, false, selected, false);
        }
    }

    private void DrawData(Rect rect, IMemoryElement memoryElement, int indent, int row, bool selected, EStatus status)
    {
        if (Event.current.type == EventType.Repaint)
        {
            string text = memoryElement.Name;
            int childCount = memoryElement.GetRecursiveCount(status);
            if (memoryElement.IsFolder)
            {
                text = text + " (" + childCount.ToString() + ")";
            }
            
            rect.xMax = 500f;

            Styles.numberLabel.Draw(rect, text, false, false, false, selected);
            rect.x = rect.xMax;
            rect.width = 100 - 4f;
            Styles.numberLabel.Draw(rect, memoryElement.Status.ToString(), false, false, false, selected);
            rect.x = rect.xMax;
            rect.width = 100 - 4f;
            int size = memoryElement.GetSize0(status);
            Styles.numberLabel.Draw(rect, EditorUtility.FormatBytes(size), false, false, false, selected);
            if (isDiff && (status == EStatus.None || status == EStatus.Persistent))
            {               
                int size1 = memoryElement.GetSize1(status);
                if(sizeChange)
                {
                    rect.x = rect.xMax;
                    rect.width = 100 - 4f;
                    Styles.numberLabel.Draw(rect, EditorUtility.FormatBytes(size1), false, false, false, selected);

                    rect.x = rect.xMax;
                    rect.width = 100 - 4f;

                    if (size != size1)
                    {
                        
                        int delta = size - size1;
                        string deltaStr = "";
                        if (delta > 0)
                        {
                            deltaStr = EditorUtility.FormatBytes(delta);
                        }
                        else
                        {
                            deltaStr = "-" + EditorUtility.FormatBytes(-delta);
                        }
                        Styles.numberLabel.Draw(rect, deltaStr, false, false, false, selected);
                    }
                    else
                    {
                        Styles.numberLabel.Draw(rect, "-", false, false, false, selected);
                    }
                }
                else
                {
                    rect.x = rect.xMax;
                    rect.width = 100 - 4f;
                    Styles.numberLabel.Draw(rect, EditorUtility.FormatBytes(size1), false, false, false, selected);
                }

            }
        }
    }
    private void DrawRecursiveData(IMemoryElement element, ref int row, int indent, EStatus status)
    {
        int childCount = element.GetChildCount();
        for (int i = 0; i < childCount; ++i)
        {
            IMemoryElement me = element.Get(i, status);
            if (me != null)
            {
                DrawItem(me, ref row, indent, status);
            }
        }
    }
    private void RowClicked(Event evt, IMemoryElement memoryElement)
    {
        if (isDiff)
        {
            diffSelected = memoryElement;
        }
        else
        {
            currentSelected = memoryElement;
        }
        //GUIUtility.keyboardControl = controlID;
        //if (evt.clickCount == 2 && memoryElement.memoryInfo != null && memoryElement.memoryInfo.instanceId != 0)
        //{
        //    Selection.instanceIDs = new int[0];
        //    Selection.activeInstanceID = memoryElement.memoryInfo.instanceId;
        //}
        //evt.Use();
        //if (memoryElement.memoryInfo != null)
        //{
        //    EditorGUIUtility.PingObject(memoryElement.memoryInfo.instanceId);
        //}
        this.Repaint();
    }
    private bool DrawItem(IMemoryElement memoryElement, ref int row, int indent, EStatus status)
    {
        bool flag = false;
        if (isDiff)
        {
            flag = diffSelected == memoryElement;
        }
        else
        {
            flag = currentSelected == memoryElement;
        }
        if (memoryElement.GetRecursiveCount(status) > 0)
        {
            row++;
            DrawBackground(row, flag);
            Rect rect = GenerateRect(row);
            rect.x = 4f + (float)indent * 16f - 14f;
            Rect position = rect;
            position.width = 14f;
            if (memoryElement.IsFolder)
            {
                memoryElement.IsExpanded = GUI.Toggle(position, memoryElement.IsExpanded, GUIContent.none, Styles.foldout);                
            }
            rect.x += 14f;

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                RowClicked(Event.current, memoryElement);
            }
            DrawData(rect, memoryElement, indent, row, flag, status);
            if (memoryElement.IsFolder && memoryElement.IsExpanded)
            {

                DrawRecursiveData(memoryElement, ref row, indent + 1, status);
            }
            return true;
        }
        return false;
    }

    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Dump", GUILayout.MaxWidth(150)))
        {
            TakeSnapShot();
        }
        if (GUILayout.Button("GetData", GUILayout.MaxWidth(150)))
        {
            GetData();
        }
        if (GUILayout.Button("Clear", GUILayout.MaxWidth(150)))
        {
            currentRoot.Clear();
            profileHistory.Clear();
            currentRoot.name = "None";
            currentDisplayRoot = currentRoot;
            currentSelected = null;
            diffSelected = null;
            snapShotIndexArr = null;
            snapShotIndex.Clear();
            diffIndex0 = 0;
            diffIndex1 = 0;

        }
        if (GUILayout.Button("Export", GUILayout.MaxWidth(150)))
        {
            Export();
        }
        if (GUILayout.Button("Import", GUILayout.MaxWidth(150)))
        {
            Import();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(currentRoot.name);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();       
        
        GUILayout.BeginVertical(GUILayout.MaxWidth(150f));
        if (currentRoot.name != null)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(currentRoot.name, GUILayout.MaxWidth(150f)))
            {
                currentDisplayRoot = currentRoot;
            }
            GUILayout.EndHorizontal();
        }
   
        GUILayout.EndVertical();

        for (int i = profileHistory.Count - 1; i >= 0; --i)
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(150f));
            MemoryElement me = profileHistory[i];
            if (me.name != null)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(me.name, GUILayout.MaxWidth(150f)))
                {
                    currentDisplayRoot = me;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(FormatMemorySize(currentDisplayRoot.totalMemory));
        GUILayout.EndHorizontal();

        if (beginSplitMethod != null && splitParam != null)
            beginSplitMethod.Invoke(null, splitParam);

        //current memory info
        GUILayout.BeginHorizontal();        
        GUILayout.BeginVertical();
        //head
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", Styles.header);
        GUILayout.Label("Memory", Styles.header);
        GUILayout.EndHorizontal();        
        //detial
        currentMemoryInfoScrollView = EditorGUILayout.BeginScrollView(currentMemoryInfoScrollView, Styles.background);

        int row = -1;
        for (int i = 0; i < currentDisplayRoot.children.Count; ++i)
        {
            DrawItem(currentDisplayRoot.children[i], ref row, 1, EStatus.None);
        }

        GUILayoutUtility.GetRect(0f, (float)row * 16f, GUILayout.ExpandWidth(true));

        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        //diff
        //head
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (snapShotIndexArr != null&& snapShotIndexArr.Length>0)
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(150f));
            diffIndex0 = EditorGUILayout.Popup(diffIndex0, snapShotIndexArr, GUILayout.MaxWidth(150f));
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.MaxWidth(10f));
            GUILayout.Label("-");
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.MaxWidth(150f));
            diffIndex1 = EditorGUILayout.Popup(diffIndex1, snapShotIndexArr, GUILayout.MaxWidth(150f));
            GUILayout.EndVertical();
        }
        GUILayout.BeginVertical(GUILayout.MinWidth(100f));
        if (GUILayout.Button("Diff", GUILayout.MaxWidth(100)))
        {
            if (diffIndex0 != diffIndex1)
            {
                MemoryElement diff0 = GetSnapShot(diffIndex0);
                MemoryElement diff1 = GetSnapShot(diffIndex1);
                if (diff0 != diff1)
                {
                    diffRoot.Clear();
                    Diff(diff0, diff1, diffRoot);
                    diffRoot.CalcSize();
                }
            }
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical(GUILayout.MinWidth(100f));
        diffState = (EStatus)EditorGUILayout.EnumPopup("Diff类型", diffState, GUILayout.MaxWidth(200));
        GUILayout.EndVertical();
        GUILayout.BeginVertical(GUILayout.MinWidth(100f));
        if (diffState == EStatus.Persistent || diffState == EStatus.None)
        {
           
            sizeChange = GUILayout.Toggle(sizeChange, "SizeChange", GUILayout.MaxWidth(150));
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        //detial        
        GUILayout.BeginHorizontal();
        diffMemoryInfoScrollView = EditorGUILayout.BeginScrollView(diffMemoryInfoScrollView, Styles.background);
        if (diffRoot != null)
        {
            isDiff = true;
            row = -1;
            for (int i = 0; i < diffRoot.children.Count; ++i)
            {
                DrawItem(diffRoot.children[i], ref row, 1, diffState);


            }
            isDiff = false;
            GUILayoutUtility.GetRect(0f, (float)row * 16f, GUILayout.ExpandWidth(true));
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        if (endHorizontalSplitMethod != null)
            endHorizontalSplitMethod.Invoke(null, null);
    }
}
