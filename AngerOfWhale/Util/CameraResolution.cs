using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraResolution : MonoBehaviour
{
    private void Awake()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleHight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
        float scaleWidth = 1f / scaleHight;

        if (scaleHight < 1)
        {
            rect.height = scaleHight;
            rect.y = (1f - scaleHight) / 2f;
        }
        else
        {
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) / 2f;
        }
        camera.rect = rect;
    }
}

