#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XEditor;
using XUtliPoolLib;
using System.Collections.Generic;

public enum EEquipType
{
    Archer,
    Cleric,
    Sorcer,
    Warrior,
    Academic,
    Spirit,
    Tail,
    Wing,
    Char,
    SelectChar,
    Creator
}
public class XCharacterMaterial : MonoBehaviour 
{
    //private string characterType = "Archer";
    public Material mat = null;
    private SkinnedMeshRenderer skin = null;
    public void OnAttached()
    {
        skin = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

    }

    void Update()
    {
        if(mat!=null&&mat!= skin.sharedMaterial)
        {

            mat.CopyPropertiesFromMaterial(skin.sharedMaterial);
            skin.sharedMaterial = mat;

        }
    }

}
#endif