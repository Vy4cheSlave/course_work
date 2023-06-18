using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DrawNoiseTextureData
{
    public const int mapWidth = MapGenerator.mapChunkSize;
    public int seed;
    public float scale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public Vector2 offset;
    public Texture2D texture;


    public DrawNoiseTextureData()
    {
        this.seed = 0;
        this.scale = 30;
        this.octaves = 4;
        this.persistance = 0.5f;
        this.lacunarity = 2.5f;
        this.offset = new Vector2(0, 0);
    }

}
