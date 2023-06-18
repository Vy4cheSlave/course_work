using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.TerrainTools;

public class TextureDrawingEditorWindow : EditorWindow
{
    DrawTextureData drawTextureData;
    public void SetMap(DrawTextureData data)
    {
        this.drawTextureData = data;
    }

    //дл€ дизайна
    const int borderSizeLeft = 15;
    //дл€ работы с эллементами
    //private Texture2D _texture;
    const int mapWidth = 241;
    //private FilterMode _filterMode = FilterMode.Point;
    //private TextureWrapMode _textureWrapMode = TextureWrapMode.Clamp;
    private Vector2 scrollPos;
    //private int _brushSize = 8;
    //private float _brushColorIntValue = 0.5f;
    private Color _brushColor = Color.white;
    //List<Color> color = new List<Color>(mapWidth * mapWidth);


    [MenuItem("Tools/TextureDrawing")]
    public static void ShowWindow()
    {
        GetWindow<TextureDrawingEditorWindow>().Show();
    }

    private void OnEnable()
    {
        titleContent.text = "Texture Drawing";
        minSize = new Vector2(300, 400);
    }

    //private void OnValidate()
    //{
    //    if (_texture == null)
    //    {
    //        _texture = new Texture2D(mapWidth, mapWidth);
    //    }
    //    if(_texture.width != mapWidth)
    //    {
    //        _texture.Reinitialize(mapWidth, mapWidth);
    //    }
    //}

    private void DrawQuad(int xTransformed, int yTransformed)
    {
        for (int y = 0; y < drawTextureData.brushSize; y++)
        {
            for (int x = 0; x < drawTextureData.brushSize; x++)
            {
                //_texture.SetPixel(xTransformed + x - _brushSize / 2, yTransformed + y - _brushSize / 2, _brushColor);
                
            }
        }
    }
    private void DrawCircle(int xTransformed, int yTransformed)
    {
        for (int y = 0; y < drawTextureData.brushSize; y++)
        {
            float y2 = Mathf.Pow(y - drawTextureData.brushSize / 2, 2);
            for (int x = 0; x < drawTextureData.brushSize; x++)
            {
                float x2 = Mathf.Pow(x - drawTextureData.brushSize / 2, 2);
                float r2 = Mathf.Pow(drawTextureData.brushSize / 2 - 0.5f, 2);
                if (x2 + y2 < r2)
                {
                    //drawTextureData.texture.SetPixel(xTransformed + x - drawTextureData.brushSize / 2, yTransformed + y - drawTextureData.brushSize / 2, _brushColor);
                    if(xTransformed + x - drawTextureData.brushSize / 2 >= 0 && xTransformed + x - drawTextureData.brushSize / 2 < mapWidth && yTransformed + y - drawTextureData.brushSize / 2 >= 0 && yTransformed + y - drawTextureData.brushSize / 2 < mapWidth)
                    {
                        drawTextureData.map[xTransformed + x - drawTextureData.brushSize / 2, yTransformed + y - drawTextureData.brushSize / 2] = drawTextureData.brushColorIntValue;
                    }
                }
                
            }
        }
    }

    private Texture2D UpdateTexture()
    {
        Texture2D result = new Texture2D(mapWidth, mapWidth);
        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                result.SetPixel(x, y, new Color(drawTextureData.map[x, y], drawTextureData.map[x, y], drawTextureData.map[x, y]));
            }
        }
        return result;
    }

    private void OnGUI()
    {
        //если происход€т изменени€ в текстуре
        if (drawTextureData == null)
        {
            drawTextureData = new DrawTextureData();
        }
        if (drawTextureData.texture == null)
        {
            drawTextureData.texture = new Texture2D(mapWidth, mapWidth);
        }
        if (drawTextureData.texture.width != mapWidth)
        {
            drawTextureData.texture.Reinitialize(mapWidth, mapWidth);
        }
        if(new Color(drawTextureData.brushColorIntValue, drawTextureData.brushColorIntValue, drawTextureData.brushColorIntValue) != _brushColor)
        {
            _brushColor = new Color(drawTextureData.brushColorIntValue, drawTextureData.brushColorIntValue, drawTextureData.brushColorIntValue);
        }

        drawTextureData.texture.filterMode = drawTextureData.filterMode;
        drawTextureData.texture.wrapMode = drawTextureData.textureWrapMode;
        //дл€ дизайна правого кра€
        int borderSizeRight = (int)(position.width - borderSizeLeft * 2);
        int windowWidthForPaint = borderSizeRight - borderSizeLeft;

        //данные расположени€ мыши на window editor (дл€ рисовани€)
        // ѕолучаем текущее событие мыши
        Event currentEvent = Event.current;

        // ѕровер€ем, что событие €вл€етс€ событием перемещени€ мыши с зажатой левой кнопкой
        if (currentEvent.type == EventType.MouseDrag && currentEvent.button == 0)
        {
            // ѕолучаем позицию курсора мыши
            Vector2 mousePos = currentEvent.mousePosition;
            if (mousePos.x >= borderSizeLeft && mousePos.x <= borderSizeRight && mousePos.y >= borderSizeLeft && mousePos.y <= borderSizeRight)
            {
                int xTransformed = (int)((mousePos.x - borderSizeLeft) * mapWidth / windowWidthForPaint);
                int yTransformed = (int)((windowWidthForPaint - mousePos.y + borderSizeLeft) * mapWidth / windowWidthForPaint);
                // ¬ыводим позицию курсора мыши в консоль
                //DrawQuad(xTransformed, yTransformed);
                DrawCircle(xTransformed, yTransformed);
            }
        }

        drawTextureData.texture = UpdateTexture();
        drawTextureData.texture.Apply();

        //отображение эллементов на window editor
        Rect previewRect = new Rect(borderSizeLeft, borderSizeLeft, borderSizeRight, borderSizeRight);
        EditorGUI.DrawPreviewTexture(previewRect, drawTextureData.texture);

        GUILayout.BeginArea(new Rect(borderSizeLeft, position.width, borderSizeRight + borderSizeLeft, position.height));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginVertical();

        drawTextureData.filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode:", drawTextureData.filterMode);
        drawTextureData.textureWrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("TextureWrapMode:", drawTextureData.textureWrapMode);
        //brushsize
        GUILayout.Label("Brush Size");
        GUILayout.BeginArea(new Rect(80, 40, borderSizeRight - 80, 20));
        drawTextureData.brushSize = Mathf.RoundToInt(GUILayout.HorizontalSlider(drawTextureData.brushSize, 1, 72));
        GUILayout.EndArea();
        //brushcolor
        GUILayout.Label("Brush Color");
        GUILayout.BeginArea(new Rect(80, 60, borderSizeRight - 80, 20));
        drawTextureData.brushColorIntValue = GUILayout.HorizontalSlider(drawTextureData.brushColorIntValue, 0, 1);
        GUILayout.EndArea();
        //

        GUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}
