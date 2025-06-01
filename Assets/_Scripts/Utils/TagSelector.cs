using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class TagSelectorTool : EditorWindow
{
    private string selectedTag = "Untagged";
    private Vector2 scrollPosition;
    private List<GameObject> foundObjects = new List<GameObject>();
    private bool searchInScene = true;
    private bool searchInProject = false;
    private bool includeInactive = true;
    private bool autoRefresh = true;
    
    // Стили для GUI
    private GUIStyle headerStyle;
    private GUIStyle buttonStyle;
    private GUIStyle countStyle;
    
    [MenuItem("Tools/Tag Selector Tool")]
    public static void ShowWindow()
    {
        TagSelectorTool window = GetWindow<TagSelectorTool>("Tag Selector");
        window.minSize = new Vector2(350, 400);
        window.Show();
    }
    
    private void OnEnable()
    {
        // Автоматически обновлять список при изменениях в сцене
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        Selection.selectionChanged += OnSelectionChanged;
        
        RefreshObjectList();
    }
    
    private void OnDisable()
    {
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        Selection.selectionChanged -= OnSelectionChanged;
    }
    
    private void OnHierarchyChanged()
    {
        if (autoRefresh)
        {
            RefreshObjectList();
        }
    }
    
    private void OnSelectionChanged()
    {
        Repaint();
    }
    
    private void InitializeStyles()
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };
        }
        
        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold
            };
        }
        
        if (countStyle == null)
        {
            countStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
    
    private void OnGUI()
    {
        InitializeStyles();
        
        GUILayout.Space(10);
        
        // Заголовок
        EditorGUILayout.LabelField("🏷️ Tag Selector Tool", headerStyle);
        
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("box");
        
        // Настройки поиска
        EditorGUILayout.LabelField("Настройки поиска:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        searchInScene = EditorGUILayout.Toggle("В сцене", searchInScene);
        searchInProject = EditorGUILayout.Toggle("В проекте", searchInProject);
        EditorGUILayout.EndHorizontal();
        
        includeInactive = EditorGUILayout.Toggle("Включить неактивные", includeInactive);
        autoRefresh = EditorGUILayout.Toggle("Автообновление", autoRefresh);
        
        EditorGUILayout.EndVertical();
        
        GUILayout.Space(5);
        
        // Выбор тега
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Выбор тега:", EditorStyles.boldLabel);
        
        // Дропдаун с тегами
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
        int currentIndex = System.Array.IndexOf(tags, selectedTag);
        if (currentIndex == -1) currentIndex = 0;
        
        int newIndex = EditorGUILayout.Popup("Тег:", currentIndex, tags);
        if (newIndex != currentIndex || selectedTag != tags[newIndex])
        {
            selectedTag = tags[newIndex];
            RefreshObjectList();
        }
        
        EditorGUILayout.EndVertical();
        
        GUILayout.Space(5);
        
        // Кнопки действий
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("🔍 Найти объекты", buttonStyle))
        {
            RefreshObjectList();
        }
        
        if (GUILayout.Button("✅ Выделить все", buttonStyle))
        {
            SelectAllFoundObjects();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("👁️ Показать все"))
        {
            SetObjectsActive(true);
        }
        
        if (GUILayout.Button("🙈 Скрыть все"))
        {
            SetObjectsActive(false);
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        
        GUILayout.Space(5);
        
        // Информация о найденных объектах
        string countText = $"Найдено объектов: {foundObjects.Count}";
        EditorGUILayout.LabelField(countText, countStyle);
        
        if (foundObjects.Count > 0)
        {
            GUILayout.Space(5);
            
            // Список найденных объектов
            EditorGUILayout.LabelField("Найденные объекты:", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, "box");
            
            foreach (GameObject obj in foundObjects)
            {
                if (obj == null) continue;
                
                EditorGUILayout.BeginHorizontal();
                
                // Иконка и название объекта
                bool isSelected = Selection.gameObjects.Contains(obj);
                Color originalColor = GUI.backgroundColor;
                
                if (isSelected)
                {
                    GUI.backgroundColor = Color.cyan;
                }
                
                // Кнопка для выделения объекта
                if (GUILayout.Button($"🎯 {obj.name}", GUILayout.Height(20)))
                {
                    Selection.activeGameObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                
                GUI.backgroundColor = originalColor;
                
                // Кнопка включения/выключения
                bool isActive = obj.activeInHierarchy;
                Color buttonColor = isActive ? Color.green : Color.red;
                string buttonText = isActive ? "👁️" : "🙈";
                
                GUI.backgroundColor = buttonColor;
                if (GUILayout.Button(buttonText, GUILayout.Width(30), GUILayout.Height(20)))
                {
                    Undo.RecordObject(obj, $"Toggle {obj.name}");
                    obj.SetActive(!obj.activeSelf);
                }
                GUI.backgroundColor = originalColor;
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        GUILayout.Space(10);
        
        // Дополнительные действия
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Дополнительные действия:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📝 Логировать список"))
        {
            LogFoundObjects();
        }
        
        if (GUILayout.Button("🗑️ Удалить все"))
        {
            DeleteAllFoundObjects();
        }
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("📋 Копировать имена в буфер"))
        {
            CopyNamesToClipboard();
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void RefreshObjectList()
    {
        foundObjects.Clear();
        
        if (searchInScene)
        {
            // Поиск в сцене
            GameObject[] allObjects = includeInactive ? 
                Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.scene.isLoaded).ToArray() :
                FindObjectsOfType<GameObject>();
            
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag(selectedTag))
                {
                    foundObjects.Add(obj);
                }
            }
        }
        
        if (searchInProject)
        {
            // Поиск в проекте (префабы)
            string[] guids = AssetDatabase.FindAssets("t:GameObject");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null && prefab.CompareTag(selectedTag))
                {
                    foundObjects.Add(prefab);
                }
            }
        }
        
        // Сортировка по имени
        foundObjects.Sort((a, b) => a.name.CompareTo(b.name));
        
        Repaint();
    }
    
    private void SelectAllFoundObjects()
    {
        if (foundObjects.Count > 0)
        {
            Selection.objects = foundObjects.ToArray();
            Debug.Log($"Выделено {foundObjects.Count} объектов с тегом '{selectedTag}'");
        }
        else
        {
            Debug.Log($"Объекты с тегом '{selectedTag}' не найдены");
        }
    }
    
    private void SetObjectsActive(bool active)
    {
        if (foundObjects.Count == 0) return;
        
        Undo.RecordObjects(foundObjects.ToArray(), active ? "Show objects" : "Hide objects");
        
        foreach (GameObject obj in foundObjects)
        {
            if (obj != null && obj.scene.isLoaded) // Только объекты в сцене
            {
                obj.SetActive(active);
            }
        }
        
        string action = active ? "показаны" : "скрыты";
        Debug.Log($"{foundObjects.Count} объектов {action}");
    }
    
    private void LogFoundObjects()
    {
        Debug.Log($"=== Объекты с тегом '{selectedTag}' ({foundObjects.Count}) ===");
        
        for (int i = 0; i < foundObjects.Count; i++)
        {
            GameObject obj = foundObjects[i];
            if (obj != null)
            {
                string path = GetGameObjectPath(obj);
                Debug.Log($"{i + 1}. {obj.name} - {path}");
            }
        }
    }
    
    private void DeleteAllFoundObjects()
    {
        if (foundObjects.Count == 0) return;
        
        bool confirm = EditorUtility.DisplayDialog(
            "Подтверждение удаления",
            $"Вы действительно хотите удалить {foundObjects.Count} объектов с тегом '{selectedTag}'?",
            "Удалить",
            "Отмена"
        );
        
        if (confirm)
        {
            Undo.RecordObjects(foundObjects.ToArray(), "Delete tagged objects");
            
            foreach (GameObject obj in foundObjects)
            {
                if (obj != null)
                {
                    Undo.DestroyObjectImmediate(obj);
                }
            }
            
            Debug.Log($"Удалено {foundObjects.Count} объектов с тегом '{selectedTag}'");
            RefreshObjectList();
        }
    }
    
    private void CopyNamesToClipboard()
    {
        if (foundObjects.Count == 0) return;
        
        string names = string.Join("\n", foundObjects.Where(obj => obj != null).Select(obj => obj.name));
        EditorGUIUtility.systemCopyBuffer = names;
        
        Debug.Log($"Имена {foundObjects.Count} объектов скопированы в буфер обмена");
    }
    
    private string GetGameObjectPath(GameObject obj)
    {
        if (obj == null) return "";
        
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}