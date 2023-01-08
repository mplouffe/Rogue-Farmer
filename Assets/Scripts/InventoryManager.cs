using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private TextMeshProUGUI m_cropInventoryCounterLabel;
    [SerializeField] private InventoryWidget m_inventoryWidget;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of Inventory Manager present. Destroying...");
            Destroy(Instance);
        }
        Instance = this;
    }

    public static void AddFruit(int addedFruits)
    {
        if (Instance == null)
        {
            Debug.LogError("Trying to add fruit to a null instance.");
            return;
        }

        Instance.m_fruits += addedFruits;
        Instance.m_cropInventoryCounterLabel.text = Instance.m_fruits.ToString();
    }

    public static bool ActiateInventoryScreen()
    {
        if (Instance == null)
        {
            Debug.LogError("Tying to activate a null instance");
            return false;
        }

        Instance.m_inventoryWidget.gameObject.SetActive(true);
        return true;
    }

    private int m_fruits;
}
