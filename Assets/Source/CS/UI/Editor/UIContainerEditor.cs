using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UIElements;

[CustomEditor(typeof(UIContainer), true)]
public class UIContainerEditor : Editor
{
     ReorderableList reorderableList_monos;

    void OnEnable()
    {
        reorderableList_monos = new ReorderableList(serializedObject, serializedObject.FindProperty("uiList"), true, true, true, true);
        reorderableList_monos.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "UI List");
        };
        reorderableList_monos.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = reorderableList_monos.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            float width = rect.width;
            float width3 = width / 3;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, width3, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + width3, rect.y, width3, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("comp"), GUIContent.none);
            //EditorGUI.PropertyField(new Rect(rect.x + 120, rect.y, rect.width - 120, EditorGUIUtility.singleLineHeight), )

            Rect position = new Rect(rect.x + width3 * 2, rect.y, rect.width - width3 * 2, EditorGUIUtility.singleLineHeight);

            var comp1 = element.FindPropertyRelative("comp");
            var obj = comp1?.objectReferenceValue;
            if (obj != null && obj is UnityEngine.Object)
            {
                Component[] comps;
                if (obj is GameObject)
                {
                    comps = ((GameObject)obj).GetComponents<Component>();
                }
                else if (obj is Component)
                {
                    comps = ((Component)obj).GetComponents<Component>();
                }
                else
                {
                    Debug.LogFormat("PopUpComponentsDrawer.value={0}", obj);
                    return;
                }

                List<string> m_AllComponents = new List<string>();

                int i = 0;
                int selectIndex = 0;
                foreach (var comp in comps)
                {
                    m_AllComponents.Add(comp.GetType().Name);
                    if (comp == obj) selectIndex = i;
                    i++;
                }
                int newselectIndex = EditorGUI.Popup(position, selectIndex, m_AllComponents.ToArray());
                if(newselectIndex != selectIndex)
                {
                    var t = comps[newselectIndex];
                    comp1.objectReferenceValue = t;
                }
            }

        };
        reorderableList_monos.onAddCallback = (ReorderableList list) =>
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("name").stringValue = "";
            element.FindPropertyRelative("comp").objectReferenceValue = null;
        };

    }

    static HashSet<Transform> added_transforms;
    internal static void AutoAddToLuaMonosHierarchyChildren(UIContainer container)
    {
        added_transforms = new HashSet<Transform>();
        for (int i = 0; i < container.uiList.Count; i++)
        {
            var ui = container.uiList[i];
            if (ui.comp is Component)
            {
                added_transforms.Add(((Component)ui.comp).transform);
            }
            else if (ui.comp is Transform)
            {
                added_transforms.Add((Transform)ui.comp);
            }
        }

        AutoAddToLuaMonosHierarchyChildren(container, container.transform);
    }


    static UnityEngine.EventSystems.UIBehaviour GetGraphic(Transform t)
    {
        UnityEngine.EventSystems.UIBehaviour[] graphics = t.GetComponents<UnityEngine.EventSystems.UIBehaviour>();
        if (graphics.Length == 0)
        {
            return null;
        }
        foreach (var g in graphics)
        {
            if (! (g is UnityEngine.UI.Image))
            {
                return g;
            }
        }
        return graphics[0];
    }
    static void AutoAddToLuaMonosHierarchyChildren(UIContainer container, Transform trans)
    {
        List<Transform> transforms = GetChilds(trans);

        foreach (var t in transforms)
        {
            if(added_transforms.Contains(t))
            {
                continue;
            }

            {
                UIContainer c = t.GetComponent<UIContainer>();
                if (c != null)
                {
                    container.uiList.Add(new UIContainer.UIInfo() { name = t.name, comp = c });
                    added_transforms.Add(t);
                    continue;
                }
            }

            if (!t.name.StartsWith("_"))
            {
                AutoAddToLuaMonosHierarchyChildren(container, t);
                continue;
            }

            UnityEngine.EventSystems.UIBehaviour g = GetGraphic(t); //t.GetComponent<UnityEngine.UI.Graphic>();
            if (g == null)
            {
                container.uiList.Add(new UIContainer.UIInfo() { name = t.name, comp = t });
            }
            else
            {
                container.uiList.Add(new UIContainer.UIInfo() { name = t.name, comp = g });
            }
            added_transforms.Add(t);

            AutoAddToLuaMonosHierarchyChildren(container, t);
        }
    }

    static List<Transform> GetChilds(Transform parent)
    {
        List<Transform> childs = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            childs.Add(parent.GetChild(i));
        }
        return childs;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (GUILayout.Button("auto add hierarchy  children to lua monos"))
        {
            AutoAddToLuaMonosHierarchyChildren((UIContainer)target);
            EditorUtility.SetDirty(target);
        }
        if (GUILayout.Button("clear"))
        {
            ((UIContainer)target).uiList.Clear();
            EditorUtility.SetDirty(target);
        }
        reorderableList_monos.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

}
