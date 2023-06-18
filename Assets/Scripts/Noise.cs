using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    //генерация массива со значениями от 0 до 1
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        // Нормализует значения между 0 и 1
        static float InverseLerp(float minNoiseHeight, float maxNoiseHeight, float inputValue)
        {
            return (inputValue - minNoiseHeight) / (maxNoiseHeight - minNoiseHeight);
        }

        //создаем двухмерный массив
        float[,] noiseMap = new float[mapWidth, mapHeight];

        //генератор псевдо случайных чисел
        System.Random prng = new System.Random(seed);
        //смещения октавы
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            //нельзя давать слижком высокие координаты, поэтому ограничиваем
            //offset нужен для ручного смещения по шуму перлина
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            //смещение всех октав, но для всех в одни координаты, как я понял (можно улучшить)
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        //проверка на то что не делится на масштаб, равный нулю
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        //
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //параметры для скейла от центра, а не верхнего правого края
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        //прогонка по карте шума
        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                //частота
                float frequency = 1;
                //значение высоты шума
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    //чем выше частота тем дальше друг от друга значения выборки
                    float sampleX = (x - halfWidth) /*/ scale*/ * frequency + octaveOffsets[i].x * frequency;
                    float sampleY = (y - halfHeight) /*/ scale*/ * frequency - octaveOffsets[i].y * frequency;
                    //float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x * frequency;
                    //float sampleY = (y - halfHeight) / scale * frequency - octaveOffsets[i].y * frequency;

                    //использование алгоритма шума перлина встроенного в юнити,
                    //в будущем, написать один из улучшенных алгоритмов самому
                    //в конце" * 2 - 1" сделано для того,
                    //чтобы значение перлина могло становиться отрицательным,
                    //чтобы высота шума уменьшалась (-2, 2) - нужно будет нормализовать до [0,1]

                    //float perlinValue = PerlinNoise2D.Noise(sampleX, sampleY, scale, seed);
                    float perlinValue = (1 - Math.Abs(PerlinNoise2D.Noise(sampleX, sampleY, scale, seed)));
                    //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    //увеличение высоты шума на значение перлина каждой октавы
                    noiseHeight += perlinValue * amplitude;

                    //в конце каждой актавы амплитуда умножается на значение постоянства [0,1]
                    //с каждой актавой влияние на шум меньше, т.к. амплитуда уменьшается
                    amplitude *= persistance;
                    //частота увеличивается каждую актаву и значение должно быть больше 1
                    //интерпретировать можно, как увеличение мелких деталий (форма горы->форма камней)
                    //а влияние этих деталей наоборот уменьшается с амплитудой
                    frequency *= lacunarity;
                }

                //оперделение максимальных значений
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                //перменение высоты шума к карте шума
                noiseMap[x, y] = noiseHeight;
            }

        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                //нормализация, то есть возвращает значения от 0 до 1, вместо(-2,2)
                //inverselerp -  метод вычисляет, куда в пределах указанного диапазона(0, 1) попадает параметр(2)
                noiseMap[x, y] = InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        
        return noiseMap;
    }

}

public static class PerlinNoise2D
{
    const int RANDMAX = int.MaxValue;
    //функцмя рандома не испоьзующая деление
    static int XorShift128(int seed, int x, int y)
    {
        int X = x;
        int Y = y;
        int z = seed;
        int w = X ^ Y;
        int t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
        return w;
    }

    //около C функция рандома
    static int rng(int seed, int x, int y)
    {
        int value = (int)((seed / 65536) % 32768 ^ (x * 1836311903) ^ (y * 2971215073)); // дополнительные 3 случайных бита
        return value;
    }

    //

    private static float[] GetPseudoRandomGradientVector(int x, int y, int seed)
    {
        int SEED = (int)((seed / 65536) % 32768 ^ (x * 1836311903) ^ (y * 2971215073));
        var rand = new System.Random(SEED);
        return new float[] { (float)rand.NextDouble() * 2 - 1, (float)rand.NextDouble() * 2 - 1 };
    }

    // Линейная интерполяция,
    // возвращает линейную интерполяцию между значениям a и b
    // на основе значения t, где t=[0;1]
    // самая простая интерполяция и результат не очень интересный
    static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    //модифицирование параметра t,
    //для более сглаженного значения к узловым значениям
    static float QunticCurve(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    // для получения векторов на каждый узел


    // скалярное произведение узловых векторов
    static float Dot(float[] a, float[] b)
    {
        return a[0] * b[0] + a[1] * b[1];
    }

    public static float Noise(float fx, float fy, float scale, int seed)
    {
        fx /= scale;
        fy /= scale;
        // находим координаты левой верхней вершины квадрата
        int left = (int)System.Math.Floor(fx);
        int top = (int)System.Math.Floor(fy);

        // находим локальные координаты точки внутри квадрата
        float pointInQuadX = fx - left;
        float pointInQuadY = fy - top;

        // находим градиентные векторы для всех вершин квадрата
        float[] topLeftGradient = GetPseudoRandomGradientVector(left, top, seed);
        float[] topRightGradient = GetPseudoRandomGradientVector(left + 1, top, seed);
        float[] bottomLeftGradient = GetPseudoRandomGradientVector(left, top + 1, seed);
        float[] bottomRightGradient = GetPseudoRandomGradientVector(left + 1, top + 1, seed);

        // находим вектора от вершин квадрата до точки внутри квадрата
        float[] distanceToTopLeft = new float[] { pointInQuadX, pointInQuadY };
        float[] distanceToTopRight = new float[] { pointInQuadX - 1, pointInQuadY };
        float[] distanceToBottomLeft = new float[] { pointInQuadX, pointInQuadY - 1 };
        float[] distanceToBottomRight = new float[] { pointInQuadX - 1, pointInQuadY - 1 };

        // считаем скалярные произведения между которыми будем интерполировать
        /*
            tx1--tx2
            |    |
            bx1--bx2
        */

        float tx1 = Dot(distanceToTopLeft, topLeftGradient);
        float tx2 = Dot(distanceToTopRight, topRightGradient);
        float bx1 = Dot(distanceToBottomLeft, bottomLeftGradient);
        float bx2 = Dot(distanceToBottomRight, bottomRightGradient);

        // готовим параметры интерполяции, чтобы она не была линейной:
        pointInQuadX = QunticCurve(pointInQuadX);
        pointInQuadY = QunticCurve(pointInQuadY);

        // собственно, интерполяция:
        float tx = Lerp(tx1, tx2, pointInQuadX);
        float bx = Lerp(bx1, bx2, pointInQuadX);
        float tb = Lerp(tx, bx, pointInQuadY);

        // возвращаем результат:
        return tb;
    }
}