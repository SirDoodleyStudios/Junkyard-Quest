using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUIScript : MonoBehaviour
{
    public delegate void D_ResizeCameraUI(float i);
    public event D_ResizeCameraUI d_ResizeCameraUI;

    RectTransform objectRect;
    public void Awake()
    {
        objectRect = gameObject.GetComponent<RectTransform>();

    }

    public void SetUISize(float orthoSize)
    {
        objectRect.sizeDelta = new Vector2(orthoSize/3.6f, 0);
        d_ResizeCameraUI(orthoSize / 3.8f);

    }
}
