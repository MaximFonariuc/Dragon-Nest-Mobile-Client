using UnityEditor;
using UnityEngine;

public class ChangeFxTag : MonoBehaviour {

    const string targetFolder = "Effects";

    [MenuItem("Assets/ChangeFxTag")]
    private static void Execute()
    {
        Object[] fxs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        foreach (Object o in fxs)
        {
            if (!(o is GameObject)) continue;

            GameObject go = (GameObject)o;
            if (go == null) continue;

			ChangeGameobjectTag(go.transform);
        }
    }

    public static void ChangeGameobjectTag(Transform t)
    {
        t.gameObject.tag = "BindedRes";
        t.gameObject.layer = LayerMask.NameToLayer("Resources");

        for (int i = 0; i < t.childCount; i++)
        {
            ChangeGameobjectTag(t.GetChild(i));
        }
    }

}
