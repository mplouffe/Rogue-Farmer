using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_levelBounds;

    [SerializeField]
    private Camera m_mainCamera;

    private RectTransform m_cameraRectTransform;

    private void Start()
    {
        Debug.Log("mainCameraPosition: " + m_mainCamera.transform);
        Debug.Log("mainCameraBounds: " + m_mainCamera.pixelRect);
    }
}
