using UnityEditor;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Сохраняем предыдущее состояние GUI
        var previousGUIState = GUI.enabled;
        
        // Делаем поле неактивным
        GUI.enabled = false;
        
        // Рисуем поле стандартным способом
        EditorGUI.PropertyField(position, property, label);
        
        // Восстанавливаем предыдущее состояние GUI
        GUI.enabled = previousGUIState;
    }
}
#endif