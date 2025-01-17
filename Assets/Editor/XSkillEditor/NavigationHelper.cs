// NavigationHelper - Allows you to create Unity Nav Meshes using Box Colliders - 2014-01-09
// released under MIT License
// http://www.opensource.org/licenses/mit-license.php
//
//@author		Devin Reimer - Owlchemy Labs
//@website 		http://blog.almostlogical.com
/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

/*
 * Usage: To use set COLLIDER_LAYER const (below) to whatever layer you wish for the Box Colliders to be used from. This allows you to use some box colliders and
 *        not others. If you don't change it (-1) it will use all layers.
 *        You will also need to create a new Tag (Edit->Project Settings->Tags) name this tag: TempNavMeshItemDestroyable
 */

//Note: This class uses UnityEditorInternal which is an undocumented internal feature, it uses it to make this script less error prone
using UnityEngine;
using UnityEditor;

public class NavigationHelper : EditorWindow
{
    private const int COLLIDER_LAYER = -1;//terrain; //edit this valve to set which layer a box collider needs to be on to be read

    private const string TEMP_NAV_MESH_OBJECT_TAG = "TempNavMeshItemDestroyable"; //you can change to what ever tag you would like as long as it isn't used by anything else
    private bool isSetup = true;

    [MenuItem("Window/Navigation Helper")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(NavigationHelper));
    }

    //return true if exists
    private bool CheckIfTagExists()
    {
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
		
		Debug.Log("Tag num:" + tags.Length.ToString());
		
        for (int n = 0; n < tags.Length; n++)
        {
            if (tags[n] == TEMP_NAV_MESH_OBJECT_TAG)
            {
                return true;
            }
        }
        return false;
    }

    void OnGUI()
    {
        GUILayout.Label("Build Nav Mesh Including Custom Box Colliders", EditorStyles.boldLabel);

        if (!isSetup)
        {
            isSetup = CheckIfTagExists();
            if (!isSetup)
            {
                GUILayout.Label("Error - You first need to create a Tag called:");
                EditorGUILayout.TextArea(TEMP_NAV_MESH_OBJECT_TAG);
            }
        }

        if (isSetup)
        {
            if (GUILayout.Button("Build Nav Mesh!"))
            {
                if (CheckIfTagExists())
                {
                    BakeBoxColliders();
                    BakeCapsuleColliders();
                }
                else
                {
                    Debug.LogError("Custom tag was not created");
                    isSetup = false;
                }
            }
			
            if (GUILayout.Button("Start Bak"))
            {
				UnityEditor.AI.NavMeshBuilder.BuildNavMeshAsync();
            }			
			
            if (GUILayout.Button("Clear Nav Mesh!"))
            {
                CleanUpOldNavMeshItems();
            }			
        }
    }


    private void BakeBoxColliders()
    {
        CleanUpOldNavMeshItems();
        BoxCollider[] allBoxColliders = GameObject.FindObjectsOfType<BoxCollider>();
		Debug.Log("Colliders num:" + allBoxColliders.Length.ToString());		
        GameObject navMeshCubePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        DestroyImmediate(navMeshCubePrefab.GetComponent<Collider>());
        navMeshCubePrefab.name = TEMP_NAV_MESH_OBJECT_TAG;
        GameObjectUtility.SetStaticEditorFlags(navMeshCubePrefab, StaticEditorFlags.NavigationStatic);
        GameObjectUtility.SetNavMeshArea(navMeshCubePrefab, 1);
        navMeshCubePrefab.tag = TEMP_NAV_MESH_OBJECT_TAG;

        GameObject tempNavMeshCube;
        foreach (BoxCollider c in allBoxColliders)
        {
            //if (COLLIDER_LAYER < 0 || c.gameObject.layer == COLLIDER_LAYER)
            {
                tempNavMeshCube = Instantiate(navMeshCubePrefab) as GameObject;
                tempNavMeshCube.name = navMeshCubePrefab.name;
                tempNavMeshCube.transform.parent = c.transform;
                tempNavMeshCube.transform.localPosition = c.center;
                tempNavMeshCube.transform.localRotation = Quaternion.identity;
                tempNavMeshCube.transform.localScale = c.size;
                tempNavMeshCube.hideFlags = HideFlags.DontSave;
                //tempNavMeshCube.hideFlags = 
            }
        }
        DestroyImmediate(navMeshCubePrefab);

    }

    private void BakeCapsuleColliders()
    {
        //CleanUpOldNavMeshItems();
        CapsuleCollider[] allCapsuleColliders = GameObject.FindObjectsOfType<CapsuleCollider>();
        Debug.Log("allCapsuleColliders num:" + allCapsuleColliders.Length.ToString());
        GameObject navMeshCubePrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(navMeshCubePrefab.GetComponent<Collider>());
        navMeshCubePrefab.name = TEMP_NAV_MESH_OBJECT_TAG;
        GameObjectUtility.SetStaticEditorFlags(navMeshCubePrefab, StaticEditorFlags.NavigationStatic);
        GameObjectUtility.SetNavMeshArea(navMeshCubePrefab, 2);
        navMeshCubePrefab.tag = TEMP_NAV_MESH_OBJECT_TAG;

        GameObject tempNavMeshCube;
        foreach (CapsuleCollider c in allCapsuleColliders)
        {
            //if (COLLIDER_LAYER < 0 || c.gameObject.layer == COLLIDER_LAYER)
            {
                tempNavMeshCube = Instantiate(navMeshCubePrefab) as GameObject;
                tempNavMeshCube.name = navMeshCubePrefab.name;
                tempNavMeshCube.transform.parent = c.transform;
                tempNavMeshCube.transform.localPosition = c.center;
                tempNavMeshCube.transform.localRotation = Quaternion.identity;
                tempNavMeshCube.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                tempNavMeshCube.hideFlags = HideFlags.DontSave;
                GameObjectUtility.SetNavMeshArea(tempNavMeshCube, 1);

            }
        }
        DestroyImmediate(navMeshCubePrefab);

    }

    private void CleanUpOldNavMeshItems()
    {
        GameObject[] oldNavMeshItems = GameObject.FindGameObjectsWithTag(TEMP_NAV_MESH_OBJECT_TAG);
        foreach (GameObject go in oldNavMeshItems)
        {
            DestroyImmediate(go);
        }
    }
}
