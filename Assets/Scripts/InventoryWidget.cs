using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryWidget : MonoBehaviour
{
    [SerializeField] private PlayerInput m_playerInput;

    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Inventory called from Inventory widget");
            m_playerInput.SwitchCurrentActionMap("Player");
            gameObject.SetActive(false);
        }
    }
}
