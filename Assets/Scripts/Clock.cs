using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class Clock : MonoBehaviour
{
    private enum Period
    {
        AM,
        PM
    }

    public static Clock Instance;

    public static bool IsNight()
    {
        if (Instance == null)
        {
            Debug.LogError("Trying to do night check on uninitalized Clock instance.");
            return false;
        }

        return Instance.m_isCurrentlyNight;
    }

    [SerializeField]
    private TextMeshProUGUI m_timeLabel;

    [SerializeField]
    private TextMeshProUGUI m_hourTimeCounter;

    [SerializeField]
    private TextMeshProUGUI m_minuteTimeCounter;

    [SerializeField]
    private TextMeshProUGUI m_periodLabel;

    [SerializeField]
    private int SunriseHour;

    [SerializeField]
    private int SunriseMinute;

    [SerializeField]
    private int SunsetHour;

    [SerializeField]
    private int SunsetMinute;

    private (int Hour, int Minute) m_Sunset;
    private (int Hour, int Minute) m_Sunrise;

    private Period m_currentPeriod = Period.AM;

    private const string k_afternoonPeriodLabel = "PM";
    private const string k_moringPeriodLabel = "AM";

    protected bool m_isCurrentlyNight = false;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple Clock instances found. Deleting...");
            Destroy(Instance);
        }
        Instance = this;
        m_Sunrise = (SunriseHour, SunriseMinute);
        m_Sunset = (SunsetHour, SunsetMinute);
    }

    public void UpdateTime(int minuteChunk, int hourChunk)
    {
        UpdatePeriodLabel(hourChunk);
        UpdateMinuteChunk(minuteChunk);
        UpdateHourChunk(hourChunk);
        UpdateNightState(minuteChunk, hourChunk);
    }

    private void UpdatePeriodLabel(int hourChunk)
    {
        if (hourChunk < 12 && m_currentPeriod == Period.PM)
        {
            m_periodLabel.text = k_moringPeriodLabel;
            m_currentPeriod = Period.AM;
        }
        else if (hourChunk >= 12 && m_currentPeriod == Period.AM)
        {
            m_periodLabel.text = k_afternoonPeriodLabel;
            m_currentPeriod = Period.PM;
        }
    }

    private void UpdateMinuteChunk(int minuteChunk)
    {
        switch (minuteChunk)
        {
            case 0:
                m_minuteTimeCounter.text = "00";
                break;
            case 1:
                m_minuteTimeCounter.text = "15";
                break;
            case 2:
                m_minuteTimeCounter.text = "30";
                break;
            case 3:
                m_minuteTimeCounter.text = "45";
                break;
        }
    }

    private void UpdateHourChunk(int hourChunk)
    {
        if (hourChunk > 12)
        {
            m_hourTimeCounter.text = (hourChunk % 12).ToString();
        }
        else
        {
            if (hourChunk == 0)
            {
                m_hourTimeCounter.text = "12";
            }
            else
            {
                m_hourTimeCounter.text = hourChunk.ToString();
            }
        }
    }

    private void UpdateNightState(int minuteChunk, int hourChunk)
    {
        bool isCurrentlyNight;
        if (hourChunk > m_Sunrise.Hour && hourChunk < m_Sunset.Hour)
        {
            isCurrentlyNight = false;
        }
        else if (hourChunk == m_Sunrise.Hour && minuteChunk >= m_Sunrise.Minute)
        {
            isCurrentlyNight = false;
        }
        else if (hourChunk == m_Sunset.Hour && minuteChunk < m_Sunset.Minute)
        {
            isCurrentlyNight = false;
        }
        else
        {
            isCurrentlyNight = true;
        }

        if (isCurrentlyNight != m_isCurrentlyNight)
        {
            TileManager.SwapTimeOfDay(isCurrentlyNight);
            m_isCurrentlyNight = isCurrentlyNight;
        }
    }
}
