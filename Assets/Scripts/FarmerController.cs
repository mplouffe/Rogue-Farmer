using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class FarmerController : MonoBehaviour
{
    [SerializeField] private float m_grabCooldownDuration;
    [SerializeField] private float m_dropCooldownDuration;
    [SerializeField] private float m_attackCooldownDuration;

    [SerializeField] private SpriteRenderer m_actingBar;
    [SerializeField] private PlayerInput m_playerInput;

    private Equipment RightHand = null;
    private Equipment LeftHand = null;

    private bool m_freeToAct = true;
    private float m_timeSinceLastAction = 0f;
    private float m_actionCooldown = 0f;
    private Action m_actionCallback = null;

    [SerializeField] private int m_farmerStrength;
    [SerializeField] private DamageFeedback m_farmerDamageFeedback;

    private const float k_actingBarMax = 0.7f;

    private void Update()
    {
        if (!m_freeToAct)
        {
            m_timeSinceLastAction += Time.deltaTime;
            var inversePercentComplete = 1 - (m_timeSinceLastAction / m_actionCooldown);
            var barScaleValue = inversePercentComplete * k_actingBarMax;
            m_actingBar.transform.localScale = new Vector3(barScaleValue, 0.1f, 1);
            
            if (m_timeSinceLastAction > m_actionCooldown)
            {
                m_freeToAct = true;
                m_timeSinceLastAction = 0;
                m_actionCooldown = 0;
                m_actingBar.color = new Color(1, 1, 1, 0);
                if (m_actionCallback != null)
                {
                    m_actionCallback.Invoke();
                }
            }
        }
    }

    private void TakeAction(float actionCooldown, Action callback)
    {
        m_freeToAct = false;
        m_timeSinceLastAction = 0;
        m_actionCooldown = actionCooldown;
        m_actionCallback = callback;
        m_actingBar.color = new Color(1, 1, 1, 1);
        m_actingBar.transform.localScale = new Vector3(k_actingBarMax, 0.1f, 1);
    }

    public void TakeDamage()
    {
        m_farmerDamageFeedback.TakeDamage();
    }

    public void MoveFarmer(InputAction.CallbackContext context)
    {
        if (context.performed && m_freeToAct)
        {
            var moveValue = context.action.ReadValue<Vector2>();
            var moveInterval = new Vector3();
            if (moveValue.x < 0)
            {
                moveInterval.x = -1;
            }
            else if (moveValue.x > 0)
            {
                moveInterval.x = 1;
            }

            if (moveValue.y < 0)
            {
                moveInterval.y = -1;
            }
            else if (moveValue.y > 0)
            {
                moveInterval.y = 1;
            }

            var newPosition = transform.position + new Vector3(moveInterval.x, moveInterval.y, 0);

            if (MonsterManager.IsMonsterInPosition(newPosition))
            {
                MonsterManager.AttackMonsterInPosition(newPosition, m_farmerStrength);
                TakeAction(m_attackCooldownDuration, null);
            }
            else
            {
                if (LevelBounds.IsWithinLevelBounds(newPosition))
                {
                    transform.position = newPosition;
                }
            }
        }
    }

    public void Grab(InputAction.CallbackContext context)
    {
        if (context.performed && m_freeToAct)
        {
            Vector3Int position = Vector3Int.FloorToInt(transform.position);
            var equipment = EquipmentManager.CheckEquipmentAtPosition(position);
            if (equipment == null)
            {
                return;
            }

            int rightHandPoints = RightHand == null ? 1 : 0;
            int leftHandPoints = LeftHand == null ? 1 : 0;
            int totalHandPoints = rightHandPoints + leftHandPoints;

            if (totalHandPoints >= equipment.carryHands)
            {
                TakeAction(m_grabCooldownDuration, () =>
                {
                    switch (equipment.carryHands)
                    {
                        case 2:
                            RightHand = equipment;
                            LeftHand = equipment;
                            break;
                        case 1:
                            if (RightHand == null)
                            {
                                RightHand = equipment;
                            }
                            else
                            {
                                LeftHand = equipment;
                            }
                            break;
                    }
                    EquipmentManager.GrabEquipmentAtPosition(position);
                    Toaster.PopToast("You grabbed the " + equipment.EquipmentId);
                });

            }
            else
            {
                Toaster.PopToast("You don't have enough free hands to carry that. Drop what you're carrying to pick that up.");
            }
        }
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed && m_freeToAct)
        {
            Vector3Int position = Vector3Int.FloorToInt(transform.position);
            if (RightHand != null)
            {
                if (EquipmentManager.DropEquipmentAtPosition(position, RightHand))
                {
                    Toaster.PopToast("You dropped the " + RightHand.EquipmentId);
                    if (RightHand.carryHands == 2)
                    {
                        LeftHand = null;
                    }
                    RightHand = null;

                }
            }
            else if (LeftHand != null)
            {
                if (EquipmentManager.DropEquipmentAtPosition(position, LeftHand))
                {
                    Toaster.PopToast("You dropped the " + LeftHand.EquipmentId);
                    LeftHand = null;
                }
            }
        }
    }

    public void UseTool(InputAction.CallbackContext context)
    {
        if (context.performed && m_freeToAct)
        {
            if (RightHand != null)
            {
                if (RightHand.useHands == 2 && (LeftHand != RightHand && LeftHand != null))
                {
                    Toaster.PopToast("You need two hands for that.");
                    return;
                }
                TakeAction(RightHand.toolCooldown, () =>
                {
                    Vector3Int position = Vector3Int.FloorToInt(transform.position);
                    TileManager.UseToolOnTile(position, RightHand.EquipmentId);
                });
            }
            else if (LeftHand != null)
            {
                TakeAction(LeftHand.toolCooldown, () =>
                {
                    Vector3Int position = Vector3Int.FloorToInt(transform.position);
                    TileManager.UseToolOnTile(position, LeftHand.EquipmentId);
                });
            }
        }
    }

    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.performed && m_freeToAct)
        {
            if (InventoryManager.ActiateInventoryScreen())
            {
                LevelManager.ChangeInputMap(InputMap.Inventory);
            }
        }
    }
}
