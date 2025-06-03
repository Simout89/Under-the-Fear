#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

public class TagSelectorTool : EditorWindow
{
    private string selectedTag = "Untagged";
    private Vector2 scrollPosition;
    private List<GameObject> foundObjects = new List<GameObject>();
    private bool searchInScene = true;
    private bool searchInProject = false;
    private bool includeInactive = true;
    private bool autoRefresh = true;
    
    // Новые поля для поиска по компонентам
    private enum SearchMode { Tag, Component, Both }
    private SearchMode searchMode = SearchMode.Tag;
    private string componentTypeName = "";
    private Type selectedComponentType;
    private List<Type> availableComponentTypes = new List<Type>();
    private Vector2 componentScrollPosition;
    private bool showComponentList = false;
    private string componentSearchFilter = "";
    
    // Стили для GUI
    private GUIStyle headerStyle;
    private GUIStyle buttonStyle;
    private GUIStyle countStyle;
    
    [MenuItem("Tools/Tag & Component Selector Tool")]
    public static void ShowWindow()
    {
        TagSelectorTool window = GetWindow<TagSelectorTool>("Tag & Component Selector");
        window.minSize = new Vector2(400, 500);
        window.Show();
    }
    
    private void OnEnable()
    {
        // Автоматически обновлять список при изменениях в сцене
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        Selection.selectionChanged += OnSelectionChanged;
        
        LoadAvailableComponentTypes();
        RefreshObjectList();
    }
    
    private void OnDisable()
    {
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        Selection.selectionChanged -= OnSelectionChanged;
    }
    
    private void LoadAvailableComponentTypes()
    {
        availableComponentTypes.Clear();
        
        // Получаем все типы компонентов
        var componentTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(Component).IsAssignableFrom(type) && !type.IsAbstract)
            .OrderBy(type => type.Name)
            .ToList();
            
        availableComponentTypes = componentTypes;
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
        EditorGUILayout.LabelField("🏷️ Tag & Component Selector Tool", headerStyle);
        
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
        
        // Режим поиска
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Режим поиска:", EditorStyles.boldLabel);
        
        SearchMode newSearchMode = (SearchMode)EditorGUILayout.EnumPopup("Искать по:", searchMode);
        if (newSearchMode != searchMode)
        {
            searchMode = newSearchMode;
            RefreshObjectList();
        }
        
        // Выбор тега (если нужен)
        if (searchMode == SearchMode.Tag || searchMode == SearchMode.Both)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Выбор тега:", EditorStyles.boldLabel);
            
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            int currentIndex = System.Array.IndexOf(tags, selectedTag);
            if (currentIndex == -1) currentIndex = 0;
            
            int newIndex = EditorGUILayout.Popup("Тег:", currentIndex, tags);
            if (newIndex != currentIndex || selectedTag != tags[newIndex])
            {
                selectedTag = tags[newIndex];
                RefreshObjectList();
            }
        }
        
        // Выбор компонента (если нужен)
        if (searchMode == SearchMode.Component || searchMode == SearchMode.Both)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Выбор компонента:", EditorStyles.boldLabel);
            
            // Поле для ввода имени компонента
            EditorGUILayout.BeginHorizontal();
            string newComponentName = EditorGUILayout.TextField("Компонент:", componentTypeName);
            if (newComponentName != componentTypeName)
            {
                componentTypeName = newComponentName;
                UpdateSelectedComponentType();
                RefreshObjectList();
            }
            
            if (GUILayout.Button("📋", GUILayout.Width(30)))
            {
                showComponentList = !showComponentList;
            }
            EditorGUILayout.EndHorizontal();
            
            // Показать список доступных компонентов
            if (showComponentList)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Доступные компоненты:", EditorStyles.miniLabel);
                
                componentSearchFilter = EditorGUILayout.TextField("Фильтр:", componentSearchFilter);
                
                componentScrollPosition = EditorGUILayout.BeginScrollView(componentScrollPosition, GUILayout.Height(150));
                
                var filteredTypes = availableComponentTypes
                    .Where(type => string.IsNullOrEmpty(componentSearchFilter) || 
                                 type.Name.ToLower().Contains(componentSearchFilter.ToLower()))
                    .Take(50); // Ограничиваем количество для производительности
                
                foreach (Type type in filteredTypes)
                {
                    if (GUILayout.Button(type.Name, EditorStyles.miniButton))
                    {
                        componentTypeName = type.Name;
                        selectedComponentType = type;
                        showComponentList = false;
                        RefreshObjectList();
                    }
                }
                
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            
            if (selectedComponentType != null)
            {
                EditorGUILayout.HelpBox($"Выбран компонент: {selectedComponentType.FullName}", MessageType.Info);
            }
            else if (!string.IsNullOrEmpty(componentTypeName))
            {
                EditorGUILayout.HelpBox($"Компонент '{componentTypeName}' не найден", MessageType.Warning);
            }
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
        string searchInfo = GetSearchInfo();
        string countText = $"Найдено объектов: {foundObjects.Count}\n{searchInfo}";
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
                string objectInfo = GetObjectInfo(obj);
                if (GUILayout.Button($"🎯 {obj.name} {objectInfo}", GUILayout.Height(20)))
                {
                    Selection.activeGameObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                
                GUI.backgroundColor = originalColor;
                
                // Кнопка включения/выключения (только для объектов в сцене)
                if (obj.scene.isLoaded)
                {
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
                }
                
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
    
    private void UpdateSelectedComponentType()
    {
        selectedComponentType = null;
        
        if (string.IsNullOrEmpty(componentTypeName))
            return;
            
        // Попробуем найти точное совпадение
        selectedComponentType = availableComponentTypes
            .FirstOrDefault(type => type.Name.Equals(componentTypeName, StringComparison.OrdinalIgnoreCase));
            
        // Если не найдено, попробуем найти по частичному совпадению
        if (selectedComponentType == null)
        {
            selectedComponentType = availableComponentTypes
                .FirstOrDefault(type => type.Name.ToLower().Contains(componentTypeName.ToLower()));
        }
    }
    
    private string GetSearchInfo()
    {
        switch (searchMode)
        {
            case SearchMode.Tag:
                return $"Поиск по тегу: {selectedTag}";
            case SearchMode.Component:
                return $"Поиск по компоненту: {componentTypeName}";
            case SearchMode.Both:
                return $"Поиск по тегу: {selectedTag} И компоненту: {componentTypeName}";
            default:
                return "";
        }
    }
    
    private string GetObjectInfo(GameObject obj)
    {
        if (searchMode == SearchMode.Component || searchMode == SearchMode.Both)
        {
            if (selectedComponentType != null)
            {
                var component = obj.GetComponent(selectedComponentType);
                if (component != null)
                {
                    return $"[{selectedComponentType.Name}]";
                }
            }
        }
        return "";
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
                if (MatchesSearchCriteria(obj))
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
                
                if (prefab != null && MatchesSearchCriteria(prefab))
                {
                    foundObjects.Add(prefab);
                }
            }
        }
        
        // Сортировка по имени
        foundObjects.Sort((a, b) => a.name.CompareTo(b.name));
        
        Repaint();
    }
    
    private bool MatchesSearchCriteria(GameObject obj)
    {
        bool tagMatch = true;
        bool componentMatch = true;
        
        // Проверяем тег если нужно
        if (searchMode == SearchMode.Tag || searchMode == SearchMode.Both)
        {
            tagMatch = obj.CompareTag(selectedTag);
        }
        
        // Проверяем компонент если нужно
        if (searchMode == SearchMode.Component || searchMode == SearchMode.Both)
        {
            if (selectedComponentType != null)
            {
                componentMatch = obj.GetComponent(selectedComponentType) != null;
            }
            else
            {
                componentMatch = false; // Если компонент не выбран, считаем что не найдено
            }
        }
        
        // Возвращаем результат в зависимости от режима
        switch (searchMode)
        {
            case SearchMode.Tag:
                return tagMatch;
            case SearchMode.Component:
                return componentMatch;
            case SearchMode.Both:
                return tagMatch && componentMatch;
            default:
                return false;
        }
    }
    
    private void SelectAllFoundObjects()
    {
        if (foundObjects.Count > 0)
        {
            Selection.objects = foundObjects.ToArray();
            Debug.Log($"Выделено {foundObjects.Count} объектов {GetSearchInfo()}");
        }
        else
        {
            Debug.Log($"Объекты не найдены. {GetSearchInfo()}");
        }
    }
    
    private void SetObjectsActive(bool active)
    {
        if (foundObjects.Count == 0) return;
        
        var sceneObjects = foundObjects.Where(obj => obj != null && obj.scene.isLoaded).ToArray();
        if (sceneObjects.Length == 0) return;
        
        Undo.RecordObjects(sceneObjects, active ? "Show objects" : "Hide objects");
        
        foreach (GameObject obj in sceneObjects)
        {
            obj.SetActive(active);
        }
        
        string action = active ? "показаны" : "скрыты";
        Debug.Log($"{sceneObjects.Length} объектов {action}");
    }
    
    private void LogFoundObjects()
    {
        Debug.Log($"=== {GetSearchInfo()} ({foundObjects.Count}) ===");
        
        for (int i = 0; i < foundObjects.Count; i++)
        {
            GameObject obj = foundObjects[i];
            if (obj != null)
            {
                string path = GetGameObjectPath(obj);
                string componentInfo = "";
                
                if (selectedComponentType != null)
                {
                    var component = obj.GetComponent(selectedComponentType);
                    if (component != null)
                    {
                        componentInfo = $" [{selectedComponentType.Name}]";
                    }
                }
                
                Debug.Log($"{i + 1}. {obj.name}{componentInfo} - {path}");
            }
        }
    }
    
    private void DeleteAllFoundObjects()
    {
        if (foundObjects.Count == 0) return;
        
        bool confirm = EditorUtility.DisplayDialog(
            "Подтверждение удаления",
            $"Вы действительно хотите удалить {foundObjects.Count} объектов?\n{GetSearchInfo()}",
            "Удалить",
            "Отмена"
        );
        
        if (confirm)
        {
            Undo.RecordObjects(foundObjects.ToArray(), "Delete objects by criteria");
            
            foreach (GameObject obj in foundObjects)
            {
                if (obj != null)
                {
                    Undo.DestroyObjectImmediate(obj);
                }
            }
            
            Debug.Log($"Удалено {foundObjects.Count} объектов. {GetSearchInfo()}");
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
#endif