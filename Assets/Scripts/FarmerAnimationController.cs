using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerAnimationController : MonoBehaviour
{
    [SerializeField] Animator m_animator;

    [SerializeField] SpriteRenderer m_renderer;

    private FarmerDirection m_currentDirection;

    public void UpdateDirection(Vector3 newDirection)
    {

        var direction = ParseNewDirection(newDirection);
        if (m_currentDirection != direction)
        {
            UpdateDirection(direction);
        }
    }

    private FarmerDirection ParseNewDirection(Vector3 newDirection)
    {
        if (newDirection == Vector3.zero)
        {
            return m_currentDirection;
        }
        else if (newDirection.y > 0)
        {
            return FarmerDirection.Up;
        }
        else if (newDirection.y < 0)
        {
            return FarmerDirection.Down;
        }
        else if (newDirection.x < 0)
        {
            return FarmerDirection.Left;
        }
        else if (newDirection.x > 0)
        {
            return FarmerDirection.Right;
        }
        else
        {
            return m_currentDirection;
        }
    }

    private void UpdateDirection(FarmerDirection direction)
    {
        m_animator.SetInteger("FarmerDirection", (int)direction);
        m_animator.SetTrigger("Idle");
        if (direction == FarmerDirection.Left)
        {
            m_renderer.flipX = true;
        }
        else
        {
            m_renderer.flipX = false;
        }

        m_currentDirection = direction;
    }
}

public enum FarmerDirection
{
    Up,
    Right,
    Down,
    Left
}
