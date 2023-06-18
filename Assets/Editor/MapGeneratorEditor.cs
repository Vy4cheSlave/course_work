using System.Collections;
using System.Collections.Generic;
//дл€ создани€ пользовательского инспектора или редактора пользовательского объекта
using UnityEditor;
using UnityEngine;

//указываем что это пользовательский редактор (ѕри написании сценариев редактора
//часто бывает полезно заставить определенные сценарии выполн€тьс€ в режиме редактировани€,
//пока ваше приложение не запущено)
[CustomEditor(typeof(MapGenerator))]
//сообщает Unity, что мы можем выбрать несколько объектов с помощью этого редактора
//и изменить их все одновременно.
[CanEditMultipleObjects]
public class MapGeneratorEditor : Editor
{   //переопредел€ем метод интерфейса дл€ скрипта MapGenerator во врем€ работы с сценой
    public override void OnInspectorGUI()
    {   
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            //автоматическое обновление
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        //проверка на изменени€ (если нажата кнопка)
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }   
    }
}
