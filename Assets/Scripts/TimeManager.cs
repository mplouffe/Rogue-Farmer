using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private Clock m_clockText;

    [SerializeField]
    private float m_durationBetweenTimeUpdates;

    private float m_cummulativeTimeSinceLastUpdate = 0;

    private int m_minuteChunk = 0;

    private int m_hourChunk = 0;

    private void Start()
    {
        m_hourChunk = 4;
        UpdateClockText();
    }

    // Update is called once per frame
    void Update()
    {
        m_cummulativeTimeSinceLastUpdate += Time.deltaTime;
        if (m_cummulativeTimeSinceLastUpdate > m_durationBetweenTimeUpdates)
        {
            m_minuteChunk++;
            if (m_minuteChunk > 3)
            {
                m_minuteChunk = 0;
                m_hourChunk++;

                if (m_hourChunk > 23)
                {
                    m_hourChunk = 0;
                }
            }

            UpdateClockText();
            m_cummulativeTimeSinceLastUpdate = 0;
        }
    }

    private void UpdateClockText()
    {
        m_clockText.UpdateTime(m_minuteChunk, m_hourChunk);
    }
}
