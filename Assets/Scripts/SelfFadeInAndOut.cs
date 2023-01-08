using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfFadeInAndOut : MonoBehaviour
{
    [SerializeField]
    private float m_fadeOutDuration;

    [SerializeField]
    private float m_fadeInDuration;

    [SerializeField]
    private float m_onScreenDuration;

    [SerializeField]
    private float m_waitingDuration;

    [SerializeField]
    private CanvasGroup m_widgetCanvasGroup;

    private float m_currentPhaseDuration;

    private SelfFadeInAndOutState m_currentState;

    private enum SelfFadeInAndOutState
    {
        Waiting,
        FadingIn,
        OnScreen,
        FadingOut,
        Finished
    }

    // Update is called once per frame
    void Update()
    {
        m_currentPhaseDuration += Time.deltaTime;
        switch (m_currentState)
        {
            case SelfFadeInAndOutState.Waiting:
                if (m_currentPhaseDuration > m_waitingDuration)
                {
                    m_currentState = SelfFadeInAndOutState.FadingIn;
                    m_currentPhaseDuration = 0;
                }
                break;
            case SelfFadeInAndOutState.FadingIn:
                var fadeInT = m_currentPhaseDuration / m_fadeInDuration;
                var fadeInAlpa = Mathf.Lerp(0, 1, fadeInT);
                m_widgetCanvasGroup.alpha = fadeInAlpa;
                if (fadeInT > 1)
                {
                    m_currentState = SelfFadeInAndOutState.OnScreen;
                    m_currentPhaseDuration = 0;
                }
                break;
            case SelfFadeInAndOutState.OnScreen:
                if (m_currentPhaseDuration > m_onScreenDuration)
                {
                    m_currentState = SelfFadeInAndOutState.FadingOut;
                    m_currentPhaseDuration = 0;
                }
                break;
            case SelfFadeInAndOutState.FadingOut:
                var fadeOutT = m_currentPhaseDuration / m_fadeOutDuration;
                var fadeOutAlpha = Mathf.Lerp(1, 0, fadeOutT);
                m_widgetCanvasGroup.alpha = fadeOutAlpha;
                if (fadeOutT > 1)
                {
                    m_currentState = SelfFadeInAndOutState.Finished;
                }
                break;
            case SelfFadeInAndOutState.Finished:
                Destroy(gameObject);
                break;
        }
    }
}
