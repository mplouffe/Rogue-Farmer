using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FarmerMover : MonoBehaviour
{
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
}
