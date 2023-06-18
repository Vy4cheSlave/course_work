using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class MeshGenerator
{
    public static MeshData GenerateTerraiMesh(float[,] heightMap, float heighMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        //нормализуем координаты и убираем необходимые крааевые вершины
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        //шаг, с которым будем брать каждую вершину в массиве вершин
        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1: levelOfDetail * 2;
        //количество вершин на линию, для дальнейшей прогонки
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                //именно такие координаты x,z для того чтобы центрировать к левому верхнему углу из массива вершин
                //+z(x) для смещнния от этого угла
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heighMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    //добавляем треугольники так, чтобы они представляли квадрат между 4 соседними вершинами
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }
        //передаются именно данные сетки, а не сетка, для будущей многопоточности
        //т.к. в Unity нельзя создавать новые сетки(Mesh) внутри потока
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    //uv карта для добавления текстуры в сетку(не знаю как это работает)
    public Vector2[] uvs;

    int triangleIndex;

    //инициализация сетки
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    //метод добавления треугольников по индексово, считается от верхнего левого угла
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    //удобное создание сетки для массива треугольников
    public Mesh CreateMesh()
    {
        //
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        //пересчитывает нормали чтобы все освещение работало нормально
        mesh.RecalculateNormals();
        return mesh;
    }
}
