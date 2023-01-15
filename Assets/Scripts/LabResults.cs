using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabResults : MonoBehaviour
{
    public static LabResults Instance;

    [SerializeField]
    private TextMeshProUGUI m_labResultsText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple copies of Lab Results found. Deleting!");
            Destroy(Instance);
        }
        Instance = this;
    }

    public static void DisplayLabResults(string labResults)
    {
        if (Instance == null)
        {
            Debug.LogError("Trying to display results with a null Instance.");
            return;
        }

        Instance.m_labResultsText.text = labResults;
    }
}
