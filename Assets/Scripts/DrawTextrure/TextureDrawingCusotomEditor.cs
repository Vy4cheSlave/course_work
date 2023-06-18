using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawTextureData))]
//[CustomEditor(typeof(DrawTextureData))]
public class TextureDrawingCusotomEditor : PropertyDrawer /*Editor*/
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Event guiEvent = Event.current;
        //var drawTextureData = new DrawTextureData();//(DrawTextureData)fieldInfo.GetValue(property.serializedObject.targetObject);
        //var drawTextureData = fieldInfo.GetValue(property.serializedObject.targetObject) as DrawTextureData[];
        var drawTextureData = fieldInfo.GetValue(property.serializedObject.targetObject) as DrawTextureData[];
        if (GUI.Button(position, label))
        {
            
            TextureDrawingEditorWindow window = EditorWindow.GetWindow<TextureDrawingEditorWindow>();
            string[] separatedString = label.ToString().Split(' ');
            int elementNumber = int.Parse(separatedString[1]);
            window.SetMap(drawTextureData[elementNumber]);
        }

    }
}
