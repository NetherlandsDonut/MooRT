using UnityEngine;
using System.Collections.Generic;

public class AspectRatioEnforce : MonoBehaviour
{
    private int ScreenSizeX = 960;
    private int ScreenSizeY = 540;

    private void RescaleCamera()
    {
        if (Screen.width == ScreenSizeX && Screen.height == ScreenSizeY) return;
        float targetaspect = 16.0f / 9.0f;
        float windowaspect = (float)Screen.width / Screen.height;
        float scaleheight = windowaspect / targetaspect;
        Camera camera = GetComponent<Camera>();
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }

        ScreenSizeX = Screen.width;
        ScreenSizeY = Screen.height;
    }

    void OnPreCull()
    {
        if (Application.isEditor) return;
        Camera camera = GetComponent<Camera>();
        Rect wp = camera.rect;
        Rect nr = new(0, 0, 1, 1);
        camera.rect = nr;
        GL.Clear(true, true, Color.black);
        camera.rect = wp;
    }

    void Update()
    {
        RescaleCamera();
    }
}