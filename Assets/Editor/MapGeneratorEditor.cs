using System.Collections;
using System.Collections.Generic;
//��� �������� ����������������� ���������� ��� ��������� ����������������� �������
using UnityEditor;
using UnityEngine;

//��������� ��� ��� ���������������� �������� (��� ��������� ��������� ���������
//����� ������ ������� ��������� ������������ �������� ����������� � ������ ��������������,
//���� ���� ���������� �� ��������)
[CustomEditor(typeof(MapGenerator))]
//�������� Unity, ��� �� ����� ������� ��������� �������� � ������� ����� ���������
//� �������� �� ��� ������������.
[CanEditMultipleObjects]
public class MapGeneratorEditor : Editor
{   //�������������� ����� ���������� ��� ������� MapGenerator �� ����� ������ � ������
    public override void OnInspectorGUI()
    {   
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            //�������������� ����������
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        //�������� �� ��������� (���� ������ ������)
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }   
    }
}
