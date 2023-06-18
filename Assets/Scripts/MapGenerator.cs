using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//���������� �������� ������� Unity, �� ��� ��� ��������� ������, �� ����� �� ����
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //������ ������
    public DrawNoiseTextureData[] drawNoiseTexture;
    public DrawTextureData[] drawTextureData;
    public int[] scale;
    //���������� ��� ������ �� ����� ���������� ��� ��� ����� ��������, ��� ����� �����������
    public enum DrawMode { NoiseMap, ColourMap, Mesh};
    public DrawMode drawMode;

    //������ ������ ������ �����(������ ������ � ������ ���������� ��������).
    //������������ ����� ������ ��� ������� ��������� 255^2,
    //241 ������ ���, ��� 240 ������ ������ ������,
    //��� ��������� ����� ������� ����� �������������(��� �����������)
    public const int mapChunkSize = 241;
    //������� �����������
    [Range(0, 6)]
    public int levelOfDetail;
    //public float noiseScale;

    //[Range(1, 10)]
    //public int octaves;
    //���������� � ��������
    //[Range(0f, 1f)]
    //public float persistance;
    //public float lacunarity;

    //public int seed;
    //public Vector2 offset;

    //������ ������ ���������� ����� �������������
    public float meshHeightMultiplier;
    //������ ������ ����� (����� �������� ���������� ����������� ������ �����)
    public AnimationCurve meshHeightCurve;

    //�������� �� ���������
    public bool autoUpdate;


    //����� ���������
    public CustomGradient regionsGradient;

    //������ ������������� ����� ���������
    //public TerrainType[] regions;

    NewNoise.noise2DMapData noiseData;
    NewNoise.Noise2D classNoiseMap;

    //���������� 2� ����� �� ������ Noise
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


        //����������� ������ � �� ����� �����
        //������������� ����� �����
        float[,] map = new float[mapChunkSize, mapChunkSize];
        //������� ���������� ������ ������� (��� ������������)
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


        //������ ������ ��� ������ ����� (�� �������, ������ �� ����������)
        //�������� ����� ��� ��� ����

        ///
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
            for (int x = 0; x < mapChunkSize; x++)
            {
                //����������� �������� � ����� ������� ����
                float currentHeight = map[x, y];
                //���������� �� ���� ��������, ����� ����������
                //������ ������� ������������� ������ � ������ �����
                colourMap[y * mapChunkSize + x] = regionsGradient.Evaluate(currentHeight);
                //for (int i = 0; i < regions.Length; i++)
                //{
                //    //���� �������� ������ <= ������ �������
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
        //        //����������� �������� � ����� ������� ����
        //        float currentHeight = noiseMap[x, y];
        //        //���������� �� ���� ��������, ����� ����������
        //        //������ ������� ������������� ������ � ������ �����
        //        for (int i = 0; i < regions.Length; i++)
        //        {
        //            //���� �������� ������ <= ������ �������
        //            if(currentHeight <= regions[i].height)
        //            {
        //                colourMap[y * mapChunkSize + x] = regions[i].color;
        //                break;
        //            }
        //        }
        //    }

        //����� MapDisplay � ������� ����� ����� ����
        //���� ������ ������� ����, ��� �������� ������ ��������,
        //� ���� ����� �������� ��������� ������� ����� ������,
        //+ �� ������������, ��� ��� ���� �� ���� �����
        MapDisplay display = FindObjectOfType<MapDisplay>();
        //���������� ����� ����� � ������ MapDisplay
        //� ������� ���������� ����� ��������� �����
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

    //�������� ������ � ��������� �����(����������), ��������� ����� ��������� ������ �����������,
    //��� ���������� ������ ����
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

//������������� ��������� ��� ��������� ����� ���������
//��� ���� ��� �� ���� ���������, ��������� � �����������
[System.Serializable]
public struct BiomType
{
    //���� ��� ��������� (�����,����� � �.�.)
    //public string name;
    //public DrawNoiseTextureData drawNoiseTexture;
    //public DrawTextureData drawTextureData;
    //public CustomGradient regionsGradient;
}
