using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    //��������� ������� �� ���������� �� 0 �� 1
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        // ����������� �������� ����� 0 � 1
        static float InverseLerp(float minNoiseHeight, float maxNoiseHeight, float inputValue)
        {
            return (inputValue - minNoiseHeight) / (maxNoiseHeight - minNoiseHeight);
        }

        //������� ���������� ������
        float[,] noiseMap = new float[mapWidth, mapHeight];

        //��������� ������ ��������� �����
        System.Random prng = new System.Random(seed);
        //�������� ������
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            //������ ������ ������� ������� ����������, ������� ������������
            //offset ����� ��� ������� �������� �� ���� �������
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            //�������� ���� �����, �� ��� ���� � ���� ����������, ��� � ����� (����� ��������)
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        //�������� �� �� ��� �� ������� �� �������, ������ ����
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        //
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //��������� ��� ������ �� ������, � �� �������� ������� ����
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        //�������� �� ����� ����
        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                //�������
                float frequency = 1;
                //�������� ������ ����
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    //��� ���� ������� ��� ������ ���� �� ����� �������� �������
                    float sampleX = (x - halfWidth) /*/ scale*/ * frequency + octaveOffsets[i].x * frequency;
                    float sampleY = (y - halfHeight) /*/ scale*/ * frequency - octaveOffsets[i].y * frequency;
                    //float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x * frequency;
                    //float sampleY = (y - halfHeight) / scale * frequency - octaveOffsets[i].y * frequency;

                    //������������� ��������� ���� ������� ����������� � �����,
                    //� �������, �������� ���� �� ���������� ���������� ������
                    //� �����" * 2 - 1" ������� ��� ����,
                    //����� �������� ������� ����� ����������� �������������,
                    //����� ������ ���� ����������� (-2, 2) - ����� ����� ������������� �� [0,1]

                    //float perlinValue = PerlinNoise2D.Noise(sampleX, sampleY, scale, seed);
                    float perlinValue = (1 - Math.Abs(PerlinNoise2D.Noise(sampleX, sampleY, scale, seed)));
                    //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    //���������� ������ ���� �� �������� ������� ������ ������
                    noiseHeight += perlinValue * amplitude;

                    //� ����� ������ ������ ��������� ���������� �� �������� ����������� [0,1]
                    //� ������ ������� ������� �� ��� ������, �.�. ��������� �����������
                    amplitude *= persistance;
                    //������� ������������� ������ ������ � �������� ������ ���� ������ 1
                    //���������������� �����, ��� ���������� ������ ������� (����� ����->����� ������)
                    //� ������� ���� ������� �������� ����������� � ����������
                    frequency *= lacunarity;
                }

                //����������� ������������ ��������
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                //���������� ������ ���� � ����� ����
                noiseMap[x, y] = noiseHeight;
            }

        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                //������������, �� ���� ���������� �������� �� 0 �� 1, ������(-2,2)
                //inverselerp -  ����� ���������, ���� � �������� ���������� ���������(0, 1) �������� ��������(2)
                noiseMap[x, y] = InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        
        return noiseMap;
    }

}

public static class PerlinNoise2D
{
    const int RANDMAX = int.MaxValue;
    //������� ������� �� ����������� �������
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

    //����� C ������� �������
    static int rng(int seed, int x, int y)
    {
        int value = (int)((seed / 65536) % 32768 ^ (x * 1836311903) ^ (y * 2971215073)); // �������������� 3 ��������� ����
        return value;
    }

    //

    private static float[] GetPseudoRandomGradientVector(int x, int y, int seed)
    {
        int SEED = (int)((seed / 65536) % 32768 ^ (x * 1836311903) ^ (y * 2971215073));
        var rand = new System.Random(SEED);
        return new float[] { (float)rand.NextDouble() * 2 - 1, (float)rand.NextDouble() * 2 - 1 };
    }

    // �������� ������������,
    // ���������� �������� ������������ ����� ��������� a � b
    // �� ������ �������� t, ��� t=[0;1]
    // ����� ������� ������������ � ��������� �� ����� ����������
    static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    //��������������� ��������� t,
    //��� ����� ����������� �������� � ������� ���������
    static float QunticCurve(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    // ��� ��������� �������� �� ������ ����


    // ��������� ������������ ������� ��������
    static float Dot(float[] a, float[] b)
    {
        return a[0] * b[0] + a[1] * b[1];
    }

    public static float Noise(float fx, float fy, float scale, int seed)
    {
        fx /= scale;
        fy /= scale;
        // ������� ���������� ����� ������� ������� ��������
        int left = (int)System.Math.Floor(fx);
        int top = (int)System.Math.Floor(fy);

        // ������� ��������� ���������� ����� ������ ��������
        float pointInQuadX = fx - left;
        float pointInQuadY = fy - top;

        // ������� ����������� ������� ��� ���� ������ ��������
        float[] topLeftGradient = GetPseudoRandomGradientVector(left, top, seed);
        float[] topRightGradient = GetPseudoRandomGradientVector(left + 1, top, seed);
        float[] bottomLeftGradient = GetPseudoRandomGradientVector(left, top + 1, seed);
        float[] bottomRightGradient = GetPseudoRandomGradientVector(left + 1, top + 1, seed);

        // ������� ������� �� ������ �������� �� ����� ������ ��������
        float[] distanceToTopLeft = new float[] { pointInQuadX, pointInQuadY };
        float[] distanceToTopRight = new float[] { pointInQuadX - 1, pointInQuadY };
        float[] distanceToBottomLeft = new float[] { pointInQuadX, pointInQuadY - 1 };
        float[] distanceToBottomRight = new float[] { pointInQuadX - 1, pointInQuadY - 1 };

        // ������� ��������� ������������ ����� �������� ����� ���������������
        /*
            tx1--tx2
            |    |
            bx1--bx2
        */

        float tx1 = Dot(distanceToTopLeft, topLeftGradient);
        float tx2 = Dot(distanceToTopRight, topRightGradient);
        float bx1 = Dot(distanceToBottomLeft, bottomLeftGradient);
        float bx2 = Dot(distanceToBottomRight, bottomRightGradient);

        // ������� ��������� ������������, ����� ��� �� ���� ��������:
        pointInQuadX = QunticCurve(pointInQuadX);
        pointInQuadY = QunticCurve(pointInQuadY);

        // ����������, ������������:
        float tx = Lerp(tx1, tx2, pointInQuadX);
        float bx = Lerp(bx1, bx2, pointInQuadX);
        float tb = Lerp(tx, bx, pointInQuadY);

        // ���������� ���������:
        return tb;
    }
}