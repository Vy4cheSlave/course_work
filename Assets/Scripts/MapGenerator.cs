using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//Подключает основные функции Unity, но что это выражение значит, до конца не знаю
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //данные биомов
    public DrawNoiseTextureData[] drawNoiseTexture;
    public DrawTextureData[] drawTextureData;
    public int[] scale;
    //определяет что именно мы хотим нарисовать шум или карту регионов, или сетку треугольную
    public enum DrawMode { NoiseMap, ColourMap, Mesh};
    public DrawMode drawMode;

    //размер длинны одного чанка(раньше широта и высота задавались отдельно).
    //максимальное число вершин при разовой генерации 255^2,
    //241 удобно так, как 240 кратно многим числам,
    //для отрисовки более дешевой сетки треугольников(для оптимизации)
    public const int mapChunkSize = 241;
    //уровень детализации
    [Range(0, 6)]
    public int levelOfDetail;
    //public float noiseScale;

    //[Range(1, 10)]
    //public int octaves;
    //превращает в ползунок
    //[Range(0f, 1f)]
    //public float persistance;
    //public float lacunarity;

    //public int seed;
    //public Vector2 offset;

    //скаляр высоты разрешимой сетки треугольников
    public float meshHeightMultiplier;
    //кривая высоты сетки (можно задовать нелинейное отображение разных высот)
    public AnimationCurve meshHeightCurve;

    //проверка на изменения
    public bool autoUpdate;


    //карта градиента
    public CustomGradient regionsGradient;

    //массив общедоступных типов местности
    //public TerrainType[] regions;

    NewNoise.noise2DMapData noiseData;
    NewNoise.Noise2D classNoiseMap;

    //извлечение 2д карты из класса Noise
    public void GenerateMap()
    {
        //noiseData.mapWidth = mapChunkSize;
        //noiseData.mapHeight = mapChunkSize;
        //noiseData.seed = seed;
        //noiseData.scale = noiseScale;
        //noiseData.octaves = octaves;
        //noiseData.persistance = persistance;
        //noiseData.lacunarity = lacunarity;
        //noiseData.offset = offset;
        //classNoiseMap = new NewNoise.Noise2D(noiseData);
        //float[,] noiseMap = classNoiseMap.generateNoiseMap();
        //float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);


        //объединение биомов в на одной карте
        //генерирование общей карты
        float[,] map = new float[mapChunkSize, mapChunkSize];
        //находим наименьшую длинну массива (для безопасности)
        int minValue = Mathf.Min(drawNoiseTexture.Length, drawTextureData.Length);
        minValue = Mathf.Min(scale.Length, minValue);
        float minval = float.MaxValue;
        float maxval = float.MinValue;
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                map[x, y] = 0;
                for (int i = 0; i < minValue; i++)
                {
                    map[x, y] += (float)(drawNoiseTexture[i].texture.GetPixel(x, y).r / 255) * drawTextureData[i].map[x, y] * scale[i];
                    if(minval > map[x, y])
                    {
                        minval = map[x, y];
                    }
                    if (maxval < map[x, y])
                    {
                        maxval = map[x, y];
                    }
                }
            }
        }
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                map[x, y] = Mathf.InverseLerp(minval, maxval, map[x, y]);
            }
        }


        //массив цветов для каждой точки (не понимаю, почему он одномерный)
        //работает также как для шума

        ///
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
            for (int x = 0; x < mapChunkSize; x++)
            {
                //присваиваем значение в точке значене шума
                float currentHeight = map[x, y];
                //проходимся по всем регионам, чтобы определить
                //какому региону соответствует высота в данной точке
                colourMap[y * mapChunkSize + x] = regionsGradient.Evaluate(currentHeight);
                //for (int i = 0; i < regions.Length; i++)
                //{
                //    //если текущщая высота <= высоте региона
                //    if (currentHeight <= regions[i].height)
                //    {
                //        colourMap[y * mapChunkSize + x] = regions[i].color;
                //        break;
                //    }
                //}
            }
        ///
        //Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        //for (int y = 0; y < mapChunkSize; y++)
        //    for (int x = 0; x < mapChunkSize; x++)
        //    {
        //        //присваиваем значение в точке значене шума
        //        float currentHeight = noiseMap[x, y];
        //        //проходимся по всем регионам, чтобы определить
        //        //какому региону соответствует высота в данной точке
        //        for (int i = 0; i < regions.Length; i++)
        //        {
        //            //если текущщая высота <= высоте региона
        //            if(currentHeight <= regions[i].height)
        //            {
        //                colourMap[y * mapChunkSize + x] = regions[i].color;
        //                break;
        //            }
        //        }
        //    }

        //вызов MapDisplay с помощью нашей карты шума
        //ищет объект данного типо, что являетвя плохим решением,
        //и если таких объектов несколько берется самый первый,
        //+ он медлительный, так как ищет по всей сцене
        MapDisplay display = FindObjectOfType<MapDisplay>();
        //передается карта шумов в скрипт MapDisplay
        //в условии выбирается режим рисования карты
        if(drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeighMap(map));
        }
        else if(drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
        else if(drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerraiMesh(map, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
    }

    //работает только в редакторе юнити(инспекторе), конкретно здесь прописаны нужные ограничения,
    //для нормальной работы шума
    void OnValidate()
    {
        //if (lacunarity < 1)
        //{
        //    lacunarity = 1;
        //}
        //if(octaves < 0)
        //{
        //    octaves = 0;
        //}
        //else if(octaves > 10)
        //{
        //    octaves = 10;
        //}
    }
}

//общедоступная структура для различных типов местности
//для того что бы была структура, доступная в инструкторе
[System.Serializable]
public struct BiomType
{
    //поле для местности (травы,камня и т.д.)
    //public string name;
    //public DrawNoiseTextureData drawNoiseTexture;
    //public DrawTextureData drawTextureData;
    //public CustomGradient regionsGradient;
}
