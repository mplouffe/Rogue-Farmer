using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthUIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_currentPlayerHealthLabel;

    [SerializeField]
    private TextMeshProUGUI m_maxPlayerHealthLabel;

    public void UpdateCurrentHealth(int currentPlayerHealth)
    {
        m_currentPlayerHealthLabel.text = currentPlayerHealth.ToString();
    }

    public void UpdateMaxHealth(int maxPlayerHealth)
    {
        m_maxPlayerHealthLabel.text = maxPlayerHealth.ToString();
    }
}
