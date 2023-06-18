using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NoiseTextureDrawingEditorWindow : EditorWindow
{
    DrawNoiseTextureData drawNoiseTextureData;
    public void SetMap(DrawNoiseTextureData data)
    {
        this.drawNoiseTextureData = data;
    }

    //��� �������
    const int borderSizeLeft = 15;
    //��� ������ � �����������
    //private Texture2D _texture;
    private FilterMode _filterMode = FilterMode.Point;
    private TextureWrapMode _textureWrapMode = TextureWrapMode.Clamp;
    private Vector2 scrollPos;
    //List<Color> color = new List<Color>(mapWidth * mapWidth);

    //������ ����
    const int mapWidth = MapGenerator.mapChunkSize;
    //public int seed = 0;
    //public float scale = 30;
    //public int octaves = 4;
    //public float persistance = 0.5f;
    //public float lacunarity = 2.5f;
    //public Vector2 offset = new Vector2(0, 0);

    NewNoise.noise2DMapData noiseData;
    NewNoise.Noise2D classNoiseMap;

    private bool TextrureUpdate()
    {
        if(drawNoiseTextureData.seed != noiseData.seed || drawNoiseTextureData.scale != noiseData.scale || drawNoiseTextureData.octaves != noiseData.octaves || drawNoiseTextureData.persistance != noiseData.persistance || drawNoiseTextureData.lacunarity != noiseData.lacunarity || drawNoiseTextureData.offset.x != noiseData.offset.x || drawNoiseTextureData.offset.y != noiseData.offset.y)
        {
            return true;
        }
        return false;
    }

    //���������� 2� ����� �� ������ Noise
    private Texture2D GenerateMap()
    {
        noiseData.mapWidth = mapWidth;
        noiseData.mapHeight = mapWidth;
        noiseData.seed = drawNoiseTextureData.seed;
        noiseData.scale = drawNoiseTextureData.scale;
        noiseData.octaves = drawNoiseTextureData.octaves;
        noiseData.persistance = drawNoiseTextureData.persistance;
        noiseData.lacunarity = drawNoiseTextureData.lacunarity;
        noiseData.offset = drawNoiseTextureData.offset;
        classNoiseMap = new NewNoise.Noise2D(noiseData);
        float[,] noiseMap = classNoiseMap.generateNoiseMap();

        //������ ������ ��� ������ ����� (�� �������, ������ �� ����������)
        //�������� ����� ��� ��� ����

        Texture2D texture2D = new Texture2D(mapWidth, mapWidth);

        ///
        Color[] colourMap = new Color[mapWidth * mapWidth];
        for (int y = 0; y < mapWidth; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                //����������� �������� � ����� ������� ����
                float currentHeight = noiseMap[x, y];
                //���������� �� ���� ��������, ����� ����������
                //������ ������� ������������� ������ � ������ �����
                colourMap[y * mapWidth + x] = new Color(currentHeight, currentHeight, currentHeight);
                texture2D.SetPixel(x, y, colourMap[y * mapWidth + x]);
            }
        return texture2D;
    }


    [MenuItem("Tools/NoiseTextureDrawing")]
    public static void ShowWindow()
    {
        GetWindow<NoiseTextureDrawingEditorWindow>().Show();
    }

    private void OnEnable()
    {
        titleContent.text = "Noise Texture";
        minSize = new Vector2(300, 450);
    }

    private void OnGUI()
    {
        //���� ���������� ��������� � ��������
        if (drawNoiseTextureData.texture == null)
        {
            drawNoiseTextureData.texture = GenerateMap();
        }
        if (drawNoiseTextureData.texture.width != mapWidth)
        {
            drawNoiseTextureData.texture.Reinitialize(mapWidth, mapWidth);
        }
        if (TextrureUpdate())
        {
            drawNoiseTextureData.texture = GenerateMap();
        }

        drawNoiseTextureData.texture.filterMode = _filterMode;
        drawNoiseTextureData.texture.wrapMode = _textureWrapMode;
        //��� ������� ������� ����
        int borderSizeRight = (int)(position.width - borderSizeLeft * 2);
        
        //for (int y = 0; y < _brushSize; y++)
        //{
        //    for (int x = 0; x < _brushSize; x++)
        //    {

        //        _texture.SetPixel(x, y, _brushColor);
        //    }
        //}

        drawNoiseTextureData.texture.Apply();

        //����������� ���������� �� window editor
        Rect previewRect = new Rect(borderSizeLeft, borderSizeLeft, borderSizeRight, borderSizeRight);
        EditorGUI.DrawPreviewTexture(previewRect, drawNoiseTextureData.texture);

        GUILayout.BeginArea(new Rect(borderSizeLeft, position.width, borderSizeRight, position.height));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal(); // ������ �������������� ������ ���������
        GUILayout.Label("Seed:"); // ����� ��� ���� �����
        drawNoiseTextureData.seed = EditorGUILayout.IntField(drawNoiseTextureData.seed); // �������� ���� ��� ����� �������������� �������� 
        EditorGUILayout.EndHorizontal(); // ����� �������������� ������ ���������

        EditorGUILayout.BeginHorizontal(); // ������ �������������� ������ ���������
        GUILayout.Label("Scale:"); // ����� ��� ���� �����
        drawNoiseTextureData.scale = EditorGUILayout.FloatField(drawNoiseTextureData.scale); // �������� ���� ��� ����� �������������� �������� 
        EditorGUILayout.EndHorizontal(); // ����� �������������� ������ ���������

        EditorGUILayout.BeginHorizontal(); // ������ �������������� ������ ���������
        GUILayout.Label("octaves [1;10]: (" + drawNoiseTextureData.octaves.ToString() + ")"); // ����� ��� ���� �����
        drawNoiseTextureData.octaves = Mathf.RoundToInt(GUILayout.HorizontalSlider(drawNoiseTextureData.octaves, 1, 10)); // �������� ���� ��� ����� �������������� �������� 
        EditorGUILayout.EndHorizontal(); // ����� �������������� ������ ���������

        EditorGUILayout.BeginHorizontal(); // ������ �������������� ������ ���������
        GUILayout.Label("persistance [0;1]:"); // ����� ��� ���� �����
        drawNoiseTextureData.persistance = GUILayout.HorizontalSlider(drawNoiseTextureData.persistance, 0, 1); // �������� ���� ��� ����� �������������� �������� 
        EditorGUILayout.EndHorizontal(); // ����� �������������� ������ ���������
        GUILayout.Label(drawNoiseTextureData.persistance.ToString());

        EditorGUILayout.BeginHorizontal(); // ������ �������������� ������ ���������
        GUILayout.Label("lacunarity:"); // ����� ��� ���� �����
        drawNoiseTextureData.lacunarity = EditorGUILayout.FloatField(drawNoiseTextureData.lacunarity); // �������� ���� ��� ����� �������������� �������� 
        EditorGUILayout.EndHorizontal(); // ����� �������������� ������ ���������

        EditorGUILayout.BeginHorizontal(); // ������ �������������� ������ ���������
        GUILayout.Label("x:"); // ����� ��� ���� �����
        drawNoiseTextureData.offset.x = EditorGUILayout.FloatField(drawNoiseTextureData.offset.x); // �������� ���� ��� ����� �������������� �������� 
        GUILayout.Label("y:"); // ����� ��� ���� �����
        drawNoiseTextureData.offset.y = EditorGUILayout.FloatField(drawNoiseTextureData.offset.y); // �������� ���� ��� ����� �������������� �������� 
        EditorGUILayout.EndHorizontal(); // ����� �������������� ������ ���������

        GUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}
