using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFeedback : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_characterSpriteRenderer;

    [SerializeField] private float m_totalBlinkDuration;

    [SerializeField] private float m_blinkInterval;

    [SerializeField] private Color m_blinkColor;

    private Color m_defaultColor = Color.white;

    private float m_durationSinceBlinkStarted;
    private float m_durationThisBlink;
    private bool m_blinking = false;
    private bool m_blinkOn = false;

    private void FixedUpdate()
    {
        if (m_blinking)
        {
            m_durationSinceBlinkStarted += Time.fixedDeltaTime;
            if (m_durationSinceBlinkStarted > m_totalBlinkDuration)
            {
                m_blinking = false;
                m_characterSpriteRenderer.color = m_defaultColor;
            }

            m_durationThisBlink += Time.fixedDeltaTime;
            if (m_durationThisBlink > m_blinkInterval)
            {
                if (m_blinkOn)
                {
                    m_blinkOn = false;
                    m_characterSpriteRenderer.color = m_defaultColor;
                }
                else
                {
                    m_blinkOn = true;
                    m_characterSpriteRenderer.color = m_blinkColor;
                }
                m_durationThisBlink = 0;
            }
        }
        
    }

    public void TakeDamage()
    {
        m_blinking = true;
        m_durationSinceBlinkStarted = 0;
        m_durationThisBlink = 0;
        m_blinkOn = true;
        m_characterSpriteRenderer.color = m_blinkColor;
    }
}
