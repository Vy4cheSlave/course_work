using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    //метод возвращающий 2д текстуру
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        //отвечает за размытость
        //режим фильтрации текстур
        texture.filterMode = FilterMode.Point;
        //режим обертывыния
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    //метод получения 2д текстуры на основе карты высот
    public static Texture2D TextureFromHeighMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //вычисляем индексы для цветовой карты, являющиеся одномерным массивом
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }
}
