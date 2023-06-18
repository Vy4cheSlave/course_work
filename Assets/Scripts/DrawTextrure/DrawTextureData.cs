using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DrawTextureData
{
    public Texture2D texture; //= new Texture2D(MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
    public int brushSize = 8;
    public float brushColorIntValue = 0.5f;
    public FilterMode filterMode = FilterMode.Point;
    public TextureWrapMode textureWrapMode = TextureWrapMode.Clamp;
    public float[,] map;

    public DrawTextureData()
    {
        map = new float[MapGenerator.mapChunkSize, MapGenerator.mapChunkSize];
        //texture = new Texture2D(MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
        //for (int y = 0; y < MapGenerator.mapChunkSize; y++)
        //{
        //    for (int x = 0; x < MapGenerator.mapChunkSize; x++)
        //    {
        //        texture.SetPixel(x, y, Color.black);
        //    }
        //}
        brushSize = 8;
        brushColorIntValue = 0.5f;
        filterMode = FilterMode.Point;
        textureWrapMode = TextureWrapMode.Clamp;
    }
}
