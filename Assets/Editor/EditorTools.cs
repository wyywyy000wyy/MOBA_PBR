using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorTools : MonoBehaviour
{
    public static bool IsAsset(Object o)
    {
        return AssetDatabase.Contains(o);
    }

    public static bool IsHirachyGameObject(Object o)
    {
        return !IsAsset(o) && o.GetType() == typeof(GameObject);
    }

    // Start is called before the first frame update
    [MenuItem("GameObject/Toogle active _`")]
    static void ChangeActive()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject o in objs)
        {
            if (!IsHirachyGameObject(o)) continue;
            o.SetActive(!o.activeSelf);
            AssetDatabase.Refresh();
        }
    }


}
