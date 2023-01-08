using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private TextMeshProUGUI m_cropInventoryCounterLabel;
    [SerializeField] private InventoryWidget m_inventoryWidget;
    [SerializeField] private int m_fruitHealingAmount;

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

    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LevelManager.ChangeInputMap(InputMap.Player);
            m_inventoryWidget.gameObject.SetActive(false);
        }
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Instance.m_fruits > 0 && LevelManager.PlayerNeedsHealing())
            {
                Instance.m_fruits--;
                LevelManager.HealPlayer(m_fruitHealingAmount);
                Instance.m_cropInventoryCounterLabel.text = Instance.m_fruits.ToString();
            }
        }
    }

    private int m_fruits;
}
