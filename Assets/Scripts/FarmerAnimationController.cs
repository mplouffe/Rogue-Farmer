using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerAnimationController : MonoBehaviour
{
    [SerializeField] Animator m_farmerAnimator;
    [SerializeField] Animator m_itemAnimator;

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

    public void EquipAxe()
    {
        m_itemAnimator.SetBool("AxeEquipped", true);
        m_itemAnimator.SetTrigger("EquipUpdated");
        m_farmerAnimator.SetBool("HandsEmpty", false);
        if (m_currentDirection == FarmerDirection.Up)
        {
            m_farmerAnimator.Play("FarmerIdle_Up", 0, 0.0f);
        }
        else if (m_currentDirection == FarmerDirection.Down)
        {
            m_farmerAnimator.Play("FarmerIdle_Down", 0, 0.0f);
        }
        else
        {
            m_farmerAnimator.SetTrigger("Idle");
        }
    }

    public void EquipPickAxe()
    {
        m_itemAnimator.SetBool("PickAxeEquipped", true);
        m_itemAnimator.SetTrigger("EquipUpdated");
        m_farmerAnimator.SetBool("HandsEmpty", false);
        if (m_currentDirection == FarmerDirection.Up)
        {
            m_farmerAnimator.Play("FarmerIdle_Up", 0, 0.0f);
        }
        else if (m_currentDirection == FarmerDirection.Down)
        {
            m_farmerAnimator.Play("FarmerIdle_Down", 0, 0.0f);
        }
        else
        {
            m_farmerAnimator.SetTrigger("Idle");
        }
    }

    public void UnEquipAxe()
    {
        m_itemAnimator.SetBool("AxeEquipped", false);
        m_itemAnimator.SetTrigger("EquipUpdated");
        m_farmerAnimator.SetBool("HandsEmpty", true);
        m_farmerAnimator.SetTrigger("Idle");
    }

    public void UnEquipPickAxe()
    {
        m_itemAnimator.SetBool("PickAxeEquipped", false);
        m_itemAnimator.SetTrigger("EquipUpdated");
        m_farmerAnimator.SetBool("HandsEmpty", true);
        m_farmerAnimator.SetTrigger("Idle");
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
        m_farmerAnimator.SetInteger("FarmerDirection", (int)direction);
        m_farmerAnimator.SetTrigger("Idle");

        m_itemAnimator.SetInteger("FarmerDirection", (int)direction);
        m_itemAnimator.SetTrigger("DirectionUpdated");
        
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
