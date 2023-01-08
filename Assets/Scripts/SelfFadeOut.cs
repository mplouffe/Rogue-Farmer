using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfFadeOut : MonoBehaviour
{
    [SerializeField]
    private float m_fadeOutDuration;

    [SerializeField]
    private float m_onScreenDuration;

    [SerializeField]
    private CanvasGroup m_widgetCanvasGroup;

    private float m_currentPhaseDuration;

    private SelfFadeOutState m_currentState;

    private enum SelfFadeOutState
    {
        OnScreen,
        FadingOut,
        Finished
    }

    // Update is called once per frame
    void Update()
    {
        m_currentPhaseDuration += Time.deltaTime;
        switch(m_currentState)
        {
            case SelfFadeOutState.OnScreen:
                if (m_currentPhaseDuration > m_onScreenDuration)
                {
                    m_currentState = SelfFadeOutState.FadingOut;
                    m_currentPhaseDuration = 0;
                }
                break;
            case SelfFadeOutState.FadingOut:
                var t = m_currentPhaseDuration / m_fadeOutDuration;
                var currentAlpa = Mathf.Lerp(1, 0, t);
                m_widgetCanvasGroup.alpha = currentAlpa;
                if (t > 1)
                {
                    m_currentState = SelfFadeOutState.Finished;
                }
                break;
            case SelfFadeOutState.Finished:
                Destroy(gameObject);
                break;
        }
    }
}
