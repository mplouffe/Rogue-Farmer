using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_levelBounds;

    [SerializeField]
    private Camera m_mainCamera;

    [SerializeField]
    private PixelPerfectCamera m_pixelPerfectCamera;

    private RectTransform m_cameraRectTransform;

    private void Start()
    {
        
    }
}
