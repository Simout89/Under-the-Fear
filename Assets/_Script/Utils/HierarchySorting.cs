using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Расширение редактора Unity для сортировки объектов в иерархии по имени
/// </summary>
public class HierarchySorting : EditorWindow
{
    private bool sortChildrenOnly = true;
    private bool sortRecursively = false;
    private GameObject selectedParent;
    private SortDirection sortDirection = SortDirection.Ascending;

    private enum SortDirection
    {
        Ascending,
        Descending
    }

    [MenuItem("Tools/Hierarchy Sorter")]
    public static void ShowWindow()
    {
        GetWindow<HierarchySorting>("Hierarchy Sorter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Сортировка объектов в иерархии", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        sortDirection = (SortDirection)EditorGUILayout.EnumPopup("Направление сортировки:", sortDirection);
        sortChildrenOnly = EditorGUILayout.Toggle("Только выбранный объект", sortChildrenOnly);
        
        if (sortChildrenOnly)
        {
            sortRecursively = EditorGUILayout.Toggle("Сортировать рекурсивно", sortRecursively);
            
            selectedParent = EditorGUILayout.ObjectField(
                "Родительский объект:", 
                selectedParent, 
                typeof(GameObject), 
                true
            ) as GameObject;
            
            if (selectedParent == null)
            {
                EditorGUILayout.HelpBox("Выберите родительский объект для сортировки его дочерних объектов.", MessageType.Info);
            }
        }

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(sortChildrenOnly && selectedParent == null))
        {
            if (GUILayout.Button("Сортировать"))
            {
                if (sortChildrenOnly)
                {
                    if (selectedParent != null)
                    {
                        Undo.RegisterFullObjectHierarchyUndo(selectedParent, "Sort Hierarchy");
                        SortChildrenByName(selectedParent.transform, sortDirection == SortDirection.Ascending, sortRecursively);
                    }
                }
                else
                {
                    Undo.RegisterFullObjectHierarchyUndo(Selection.activeGameObject, "Sort All Hierarchy");
                    SortAllRootObjects(sortDirection == SortDirection.Ascending);
                }
            }
        }
        
        if (GUILayout.Button("Сортировать выбранные объекты"))
        {
            GameObject[] selection = Selection.gameObjects;
            if (selection != null && selection.Length > 0)
            {
                // Получаем общего родителя для всех выбранных объектов
                Transform commonParent = selection[0].transform.parent;
                bool sameParent = true;
                
                // Проверяем, что все объекты имеют одного родителя
                foreach (GameObject obj in selection)
                {
                    if (obj.transform.parent != commonParent)
                    {
                        sameParent = false;
                        break;
                    }
                }
                
                if (sameParent && commonParent != null)
                {
                    Undo.RegisterFullObjectHierarchyUndo(commonParent.gameObject, "Sort Selected Objects");
                    SortSelectedObjects(selection, sortDirection == SortDirection.Ascending);
                    EditorUtility.DisplayDialog("Готово", "Выбранные объекты отсортированы.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Ошибка", "Выбранные объекты должны иметь общего родителя.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Ошибка", "Не выбрано ни одного объекта для сортировки.", "OK");
            }
        }
    }
    /// <summary>
    /// Сортирует только выбранные объекты у общего родителя
    /// </summary>
    private void SortSelectedObjects(GameObject[] selection, bool ascending)
    {
        if (selection.Length == 0) return;
        
        // Получаем общего родителя
        Transform parent = selection[0].transform.parent;
        
        // Сортируем выбранные объекты
        List<Transform> selectedTransforms = selection.Select(obj => obj.transform).ToList();
        
        // Сортируем по имени
        selectedTransforms.Sort((a, b) => ascending 
            ? string.Compare(a.name, b.name) 
            : string.Compare(b.name, a.name));
        
        // Перемещаем объекты в отсортированном порядке
        for (int i = 0; i < selectedTransforms.Count; i++)
        {
            selectedTransforms[i].SetSiblingIndex(parent.childCount - 1);
        }
        
        // Снова сортируем, чтобы они оказались в правильном порядке
        for (int i = 0; i < selectedTransforms.Count; i++)
        {
            int targetIndex = parent.childCount - selectedTransforms.Count + i;
            selectedTransforms[i].SetSiblingIndex(targetIndex);
        }
    }

    /// <summary>
    /// Сортирует все дочерние объекты указанного Transform по имени
    /// </summary>
    private void SortChildrenByName(Transform parent, bool ascending, bool recursive)
    {
        if (parent == null) return;

        // Получаем список всех дочерних объектов
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            children.Add(parent.GetChild(i));
        }

        // Сортируем по имени
        children.Sort((a, b) => ascending 
            ? string.Compare(a.name, b.name) 
            : string.Compare(b.name, a.name));

        // Перемещаем объекты в отсортированном порядке
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(parent.childCount - 1);
        }
        
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
            
            // Если включена рекурсивная сортировка, сортируем и дочерние объекты
            if (recursive && children[i].childCount > 0)
            {
                SortChildrenByName(children[i], ascending, true);
            }
        }
    }

    /// <summary>
    /// Сортирует все корневые объекты в сцене
    /// </summary>
    private void SortAllRootObjects(bool ascending)
    {
        // Получаем все корневые объекты в сцене
        List<GameObject> rootObjects = new List<GameObject>();
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.GetActiveScene().rootCount; i++)
        {
            rootObjects.Add(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[i]);
        }

        // Сортируем корневые объекты по имени
        rootObjects.Sort((a, b) => ascending 
            ? string.Compare(a.name, b.name) 
            : string.Compare(b.name, a.name));

        // Применяем сортировку, перемещая каждый объект в нужную позицию
        for (int i = 0; i < rootObjects.Count; i++)
        {
            rootObjects[i].transform.SetSiblingIndex(i);
        }
    }
}