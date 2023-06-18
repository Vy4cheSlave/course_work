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

    //для дизайна
    const int borderSizeLeft = 15;
    //для работы с эллементами
    //private Texture2D _texture;
    private FilterMode _filterMode = FilterMode.Point;
    private TextureWrapMode _textureWrapMode = TextureWrapMode.Clamp;
    private Vector2 scrollPos;
    //List<Color> color = new List<Color>(mapWidth * mapWidth);

    //данные шума
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

    //извлечение 2д карты из класса Noise
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

        //массив цветов для каждой точки (не понимаю, почему он одномерный)
        //работает также как для шума

        Texture2D texture2D = new Texture2D(mapWidth, mapWidth);

        ///
        Color[] colourMap = new Color[mapWidth * mapWidth];
        for (int y = 0; y < mapWidth; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                //присваиваем значение в точке значене шума
                float currentHeight = noiseMap[x, y];
                //проходимся по всем регионам, чтобы определить
                //какому региону соответствует высота в данной точке
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
        //если происходят изменения в текстуре
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
        //для дизайна правого края
        int borderSizeRight = (int)(position.width - borderSizeLeft * 2);
        
        //for (int y = 0; y < _brushSize; y++)
        //{
        //    for (int x = 0; x < _brushSize; x++)
        //    {

        //        _texture.SetPixel(x, y, _brushColor);
        //    }
        //}

        drawNoiseTextureData.texture.Apply();

        //отображение эллементов на window editor
        Rect previewRect = new Rect(borderSizeLeft, borderSizeLeft, borderSizeRight, borderSizeRight);
        EditorGUI.DrawPreviewTexture(previewRect, drawNoiseTextureData.texture);

        GUILayout.BeginArea(new Rect(borderSizeLeft, position.width, borderSizeRight, position.height));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal(); // начало горизонтальной группы элементов
        GUILayout.Label("Seed:"); // метка для поля ввода
        drawNoiseTextureData.seed = EditorGUILayout.IntField(drawNoiseTextureData.seed); // создание поля для ввода целочисленного значения 
        EditorGUILayout.EndHorizontal(); // конец горизонтальной группы элементов

        EditorGUILayout.BeginHorizontal(); // начало горизонтальной группы элементов
        GUILayout.Label("Scale:"); // метка для поля ввода
        drawNoiseTextureData.scale = EditorGUILayout.FloatField(drawNoiseTextureData.scale); // создание поля для ввода целочисленного значения 
        EditorGUILayout.EndHorizontal(); // конец горизонтальной группы элементов

        EditorGUILayout.BeginHorizontal(); // начало горизонтальной группы элементов
        GUILayout.Label("octaves [1;10]: (" + drawNoiseTextureData.octaves.ToString() + ")"); // метка для поля ввода
        drawNoiseTextureData.octaves = Mathf.RoundToInt(GUILayout.HorizontalSlider(drawNoiseTextureData.octaves, 1, 10)); // создание поля для ввода целочисленного значения 
        EditorGUILayout.EndHorizontal(); // конец горизонтальной группы элементов

        EditorGUILayout.BeginHorizontal(); // начало горизонтальной группы элементов
        GUILayout.Label("persistance [0;1]:"); // метка для поля ввода
        drawNoiseTextureData.persistance = GUILayout.HorizontalSlider(drawNoiseTextureData.persistance, 0, 1); // создание поля для ввода целочисленного значения 
        EditorGUILayout.EndHorizontal(); // конец горизонтальной группы элементов
        GUILayout.Label(drawNoiseTextureData.persistance.ToString());

        EditorGUILayout.BeginHorizontal(); // начало горизонтальной группы элементов
        GUILayout.Label("lacunarity:"); // метка для поля ввода
        drawNoiseTextureData.lacunarity = EditorGUILayout.FloatField(drawNoiseTextureData.lacunarity); // создание поля для ввода целочисленного значения 
        EditorGUILayout.EndHorizontal(); // конец горизонтальной группы элементов

        EditorGUILayout.BeginHorizontal(); // начало горизонтальной группы элементов
        GUILayout.Label("x:"); // метка для поля ввода
        drawNoiseTextureData.offset.x = EditorGUILayout.FloatField(drawNoiseTextureData.offset.x); // создание поля для ввода целочисленного значения 
        GUILayout.Label("y:"); // метка для поля ввода
        drawNoiseTextureData.offset.y = EditorGUILayout.FloatField(drawNoiseTextureData.offset.y); // создание поля для ввода целочисленного значения 
        EditorGUILayout.EndHorizontal(); // конец горизонтальной группы элементов

        GUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}
