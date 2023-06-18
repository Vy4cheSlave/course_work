using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    //ссылка для визуализации текстуры
    public Renderer textureRenderer;
    //для взаимодействием с определенным Gameobject
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    //
    public void DrawTexture(Texture2D texture)
    {
        //возможность устанавливать генерацию текстуры без запуска приложения для 3д объекта
        //изменяет настройки текстуры на объекте
        textureRenderer.sharedMaterial.mainTexture = texture;
        //изменяет масштаб локальный объекта
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height); 
    }

    //создает треугольную сетку
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        //обязан быть общим, чтобы отрисовывать разные форматы визуализации
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
