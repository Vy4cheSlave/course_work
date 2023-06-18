using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DrawNoiseTextureData))]
public class NoiseTextureDrawingCustomEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Event guiEvent = Event.current;
        //DrawNoiseTextureData drawNoiseTextureData = new DrawNoiseTextureData();//(DrawNoiseTextureData)fieldInfo.GetValue(property.serializedObject.targetObject);
        //var drawNoiseTextureData = fieldInfo.GetValue(property.serializedObject.targetObject) as DrawNoiseTextureData[];
        
        if (GUI.Button(position, label))
        {
            var drawNoiseTextureData = fieldInfo.GetValue(property.serializedObject.targetObject) as DrawNoiseTextureData[];
            NoiseTextureDrawingEditorWindow window = EditorWindow.GetWindow<NoiseTextureDrawingEditorWindow>();
            string[] separatedString = label.ToString().Split(' ');
            int elementNumber = int.Parse(separatedString[1]);
            window.SetMap(drawNoiseTextureData[elementNumber]);
        }

    }
}
