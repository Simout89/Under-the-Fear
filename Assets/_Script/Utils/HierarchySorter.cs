using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Расширение редактора Unity для сортировки объектов в иерархии по имени
/// </summary>
public class HierarchySorter : EditorWindow
{
    private bool sortChildrenOnly = true;
    private bool sortRecursively = false;
    private GameObject selectedParent;
    private SortDirection sortDirection = SortDirection.Ascending;
    private bool useNaturalSort = true; // По умолчанию используем натуральную сортировку для чисел

    private enum SortDirection
    {
        Ascending,
        Descending
    }
    
    /// <summary>
    /// Выполняет натуральную сортировку строк, корректно обрабатывая числовые значения
    /// </summary>
    private int CompareNaturalOrder(string x, string y)
    {
        if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y)) return 0;
        if (string.IsNullOrEmpty(x)) return -1;
        if (string.IsNullOrEmpty(y)) return 1;

        int lenX = x.Length;
        int lenY = y.Length;
        int indexX = 0;
        int indexY = 0;
        
        while (indexX < lenX && indexY < lenY)
        {
            char xChar = x[indexX];
            char yChar = y[indexY];
            
            // Подготовим буферы для чисел или строк
            string xSubString = "";
            string ySubString = "";
            
            // Если оба символа - цифры, извлекаем полные числа для сравнения
            if (char.IsDigit(xChar) && char.IsDigit(yChar))
            {
                // Извлекаем число из первой строки
                while (indexX < lenX && char.IsDigit(x[indexX]))
                {
                    xSubString += x[indexX];
                    indexX++;
                }
                
                // Извлекаем число из второй строки
                while (indexY < lenY && char.IsDigit(y[indexY]))
                {
                    ySubString += y[indexY];
                    indexY++;
                }
                
                // Преобразуем в числа и сравниваем
                if (int.TryParse(xSubString, out int numX) && int.TryParse(ySubString, out int numY))
                {
                    if (numX != numY)
                    {
                        return numX.CompareTo(numY);
                    }
                }
                else
                {
                    // Если не смогли преобразовать в числа, сравниваем лексикографически
                    int result = string.Compare(xSubString, ySubString);
                    if (result != 0) return result;
                }
            }
            else
            {
                // Извлекаем последовательность не-цифр из первой строки
                while (indexX < lenX && !char.IsDigit(x[indexX]))
                {
                    xSubString += x[indexX];
                    indexX++;
                }
                
                // Извлекаем последовательность не-цифр из второй строки
                while (indexY < lenY && !char.IsDigit(y[indexY]))
                {
                    ySubString += y[indexY];
                    indexY++;
                }
                
                // Сравниваем лексикографически
                int result = string.Compare(xSubString, ySubString);
                if (result != 0) return result;
            }
        }
        
        // Если одна строка закончилась, а другая нет
        if (indexX < lenX) return 1;
        if (indexY < lenY) return -1;
        
        // Строки равны
        return 0;
    }

    [MenuItem("GameObject/Sort Children by Name", false, 49)]
    static void SortChildrenContextMenu()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            Undo.RegisterFullObjectHierarchyUndo(selectedObject, "Sort Children by Name");
            
            HierarchySorter sorter = CreateInstance<HierarchySorter>();
            sorter.SortChildrenByName(selectedObject.transform, true, false);
            
            EditorUtility.DisplayDialog("Успех", "Дочерние объекты отсортированы по имени.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Ошибка", "Не выбран ни один объект для сортировки дочерних элементов.", "OK");
        }
    }
    
    [MenuItem("GameObject/Sort Children Recursively", false, 50)]
    static void SortChildrenRecursivelyContextMenu()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            Undo.RegisterFullObjectHierarchyUndo(selectedObject, "Sort Children Recursively");
            
            HierarchySorter sorter = CreateInstance<HierarchySorter>();
            sorter.SortChildrenByName(selectedObject.transform, true, true);
            
            EditorUtility.DisplayDialog("Успех", "Дочерние объекты рекурсивно отсортированы по имени.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Ошибка", "Не выбран ни один объект для рекурсивной сортировки.", "OK");
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Сортировка объектов в иерархии", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        sortDirection = (SortDirection)EditorGUILayout.EnumPopup("Направление сортировки:", sortDirection);
        useNaturalSort = EditorGUILayout.Toggle("Числовая сортировка", useNaturalSort);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Выберите способ сортировки:", EditorStyles.boldLabel);
        
        sortChildrenOnly = EditorGUILayout.Toggle("Только дочерние объекты", sortChildrenOnly);
        
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
        
        if (GUILayout.Button("Сортировать выбранные объекты в иерархии"))
        {
            SortAllChildren();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Сортировать выбранные объекты по имени"))
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
        
        // Сортируем по имени с учетом числовых суффиксов
        selectedTransforms.Sort((a, b) => ascending 
            ? (useNaturalSort ? CompareNaturalOrder(a.name, b.name) : string.Compare(a.name, b.name))
            : (useNaturalSort ? CompareNaturalOrder(b.name, a.name) : string.Compare(b.name, a.name)));
        
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

    public void SortAllChildren()
    {
        // Получаем все выбранные объекты в иерархии
        GameObject[] selection = Selection.gameObjects;
        
        if (selection.Length > 0)
        {
            foreach (GameObject obj in selection)
            {
                Undo.RegisterFullObjectHierarchyUndo(obj, "Sort All Children");
                SortChildrenByName(obj.transform, sortDirection == SortDirection.Ascending, sortRecursively);
            }
            
            EditorUtility.DisplayDialog("Готово", "Все дочерние объекты выбранных объектов отсортированы.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Ошибка", "Не выбрано ни одного объекта для сортировки.", "OK");
        }
    }[MenuItem("Tools/Hierarchy Sorter")]
    public static void ShowWindow()
    {
        GetWindow<HierarchySorter>("Hierarchy Sorter");
    }
    
    /// <summary>
    /// Сортирует все дочерние объекты указанного Transform по имени с учетом числовых суффиксов
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

        // Сортируем по имени с учетом числовых суффиксов
        children.Sort((a, b) => 
        {
            // Используем натуральную сортировку для имен с числами или стандартную
            return ascending 
                ? (useNaturalSort ? CompareNaturalOrder(a.name, b.name) : string.Compare(a.name, b.name))
                : (useNaturalSort ? CompareNaturalOrder(b.name, a.name) : string.Compare(b.name, a.name));
        });

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

        // Сортируем корневые объекты по имени с учетом числовых суффиксов
        rootObjects.Sort((a, b) => ascending 
            ? (useNaturalSort ? CompareNaturalOrder(a.name, b.name) : string.Compare(a.name, b.name))
            : (useNaturalSort ? CompareNaturalOrder(b.name, a.name) : string.Compare(b.name, a.name)));

        // Применяем сортировку, перемещая каждый объект в нужную позицию
        for (int i = 0; i < rootObjects.Count; i++)
        {
            rootObjects[i].transform.SetSiblingIndex(i);
        }
    }
}