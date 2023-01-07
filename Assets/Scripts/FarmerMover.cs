using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class FarmerMover : MonoBehaviour
{
    private Equipment RightHand = null;

    private Equipment LeftHand = null;

    public void MoveFarmer(InputAction.CallbackContext context)
    {
        if (context.performed)
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
            if (LevelBounds.IsWithinLevelBounds(newPosition))
            {
                transform.position = newPosition;
            }
        }
    }

    public void Grab(InputAction.CallbackContext context)
    {
        if (context.performed)
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
            }
            
        }
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3Int position = Vector3Int.FloorToInt(transform.position);
            if (RightHand != null)
            {
                if (EquipmentManager.DropEquipmentAtPosition(position, RightHand))
                {
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
                    LeftHand = null;
                }
            }
        }
    }

    public void UseTool(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (RightHand != null)
            {
                if (RightHand.useHands == 2 && (LeftHand != RightHand && LeftHand != null))
                {
                    Debug.Log("Cannot perform action. Need additional hand.");
                    return;
                }
                Vector3Int position = Vector3Int.FloorToInt(transform.position);
                TileManager.UseToolOnTile(position, RightHand.EquipmentId);
            }
        }
    }
}
