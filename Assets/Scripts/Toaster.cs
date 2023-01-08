using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toaster : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_textField;

    [SerializeField]
    private float m_toasterInAnimationDuration;

    [SerializeField]
    private float m_defaultToasterDuration;

    private static Toaster Instance;

    private float m_toastDuration;
    private float m_onScreenDuration;
    private ToasterState m_currentToasterState = ToasterState.Waiting;

    private const float k_toasterOffscreenPosition = -135;
    private const float k_toasterOnscreenPosition = 30;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of Toaster present. Destroying...");
            Destroy(Instance);
        }
        Instance = this;
    }

    private void FixedUpdate()
    {
       if (m_currentToasterState != ToasterState.Waiting)
       {
            m_onScreenDuration += Time.fixedDeltaTime;
            switch(m_currentToasterState)
            {
                case ToasterState.AnimatingIn:
                    var tAnimIn = m_onScreenDuration / m_toasterInAnimationDuration;
                    var newToasterYIn = Mathf.Lerp(transform.position.y, k_toasterOnscreenPosition, tAnimIn);
                    transform.position = new Vector3(transform.position.x, newToasterYIn, transform.position.z);
                    if (tAnimIn > 1)
                    {
                        m_currentToasterState = ToasterState.Displaying;
                    }
                    break;
                case ToasterState.Displaying:
                    if (m_onScreenDuration > m_toastDuration)
                    {
                        m_onScreenDuration = 0;
                        m_currentToasterState = ToasterState.AnimatingOut;
                    }
                    break;
                case ToasterState.AnimatingOut:
                    var tAnimOut = m_onScreenDuration / m_toasterInAnimationDuration;
                    var newToasterYOut = Mathf.Lerp(transform.position.y, k_toasterOffscreenPosition, tAnimOut);
                    transform.position = new Vector3(transform.position.x, newToasterYOut, transform.position.z);
                    if (tAnimOut > 1)
                    {
                        m_currentToasterState = ToasterState.Waiting;
                    }
                    break;
            }
       }
    }

    public static void PopToast(string message)
    {
        if (Instance == null)
        {
            Debug.LogError("Trying to make toast with a null toaster.");
        }

        Instance.LoadToaster(message, Instance.m_defaultToasterDuration);

    }

    public static void PopToast(string message, float duration)
    {
        if (Instance == null)
        {
            Debug.LogError("Trying to make toast with a null toaster.");
        }

        Instance.LoadToaster(message, duration);

    }

    private void LoadToaster(string message, float duration)
    {
        m_textField.text = message;
        m_toastDuration = duration;

        switch (m_currentToasterState)
        {
            case ToasterState.Waiting:
                m_onScreenDuration = 0;
                m_currentToasterState = ToasterState.AnimatingIn;
                break;
            case ToasterState.Displaying:
                m_onScreenDuration = 0;
                break;
            case ToasterState.AnimatingOut:
                m_onScreenDuration = 0;
                m_currentToasterState = ToasterState.AnimatingIn;
                break;
        }
    }


}

    public enum ToasterState
    {
        AnimatingIn,
        Displaying,
        AnimatingOut,
        Waiting
    }
