using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    [SerializeField] private Vector3Int m_target;
    [SerializeField] private float m_intervalBetweenActions;
    [SerializeField] private float m_intervalBeforePlantAttack;
    [SerializeField] private float m_chasingPlayerInterval;
    [SerializeField] private float m_attackingPlayerInterval;
    [SerializeField] private SpriteRenderer m_actingBar;
    [SerializeField] private float m_playerDetectionRadius;
    [SerializeField] private int m_monsterStrength;
    [SerializeField] private DamageFeedback m_monsterDamangeFeedback;
    [SerializeField] private SpriteRenderer m_monsterSprite;

    private Action m_actionCallback;

    private bool m_reachedDestination;
    private bool m_canAct;
    private float m_durationSinceLastAction;
    private const float k_actingBarMax = 0.7f;
    private float m_actionCooldown = 0f;
    private bool m_cooldownBarActive = false;

    public int MonsterHealth;

    public bool UpdateMonster()
    {
        if (m_canAct)
        {
            if (CheckAreaForPlayer(out Vector3Int playerPosition))
            {
                m_target = playerPosition;
                List<Vector3Int> adjacentPositions = GetSurroundingTilePositions();
                if (adjacentPositions.Contains(playerPosition))
                {
                    LevelManager.AttackPlayer(m_monsterStrength);
                    TakeSilentAction(m_attackingPlayerInterval, null);
                    return false;
                }
                else
                {
                    m_reachedDestination = false;
                    var stepTaken = TakeStepTowardTarget();
                    TakeSilentAction(m_chasingPlayerInterval, null);
                    return stepTaken;
                }
            }
            else if (CheckAreaForPlant(out Vector3 plantPosition))
            {
                m_durationSinceLastAction = 0;
                transform.position = plantPosition;
                m_reachedDestination = false;
                TakeAction(m_intervalBeforePlantAttack, AttackPlant);
                return true;
            }
            else if (!m_reachedDestination)
            {
                var stepTaken =  TakeStepTowardTarget();
                TakeSilentAction(m_intervalBetweenActions, null);
                return stepTaken;
            }
            return false;
        }
        else
        {
            m_durationSinceLastAction += Time.deltaTime;
            if (m_cooldownBarActive)
            {
                var inversePercentComplete = 1 - (m_durationSinceLastAction / m_actionCooldown);
                var barScaleValue = inversePercentComplete * k_actingBarMax;
                m_actingBar.transform.localScale = new Vector3(barScaleValue, 0.1f, 1);
            }
            
            if (m_durationSinceLastAction > m_actionCooldown)
            {
                m_durationSinceLastAction = 0;
                m_canAct = true;
                if (m_actionCallback != null)
                {
                    m_actionCallback.Invoke();
                }

                if (m_cooldownBarActive)
                {
                    m_actingBar.color = new Color(1, 1, 1, 0);
                    m_cooldownBarActive = false;
                }
            }
            return false;
        }   
    }

    public bool IsVisible() {

        return m_monsterSprite.isVisible;     
    }

    public void SetTarget(Vector3Int newTarget)
    {
        m_target = newTarget;
        m_reachedDestination = false;
    }

    public void TakeDamage()
    {
        m_monsterDamangeFeedback.TakeDamage();
    }

    private bool TakeStepTowardTarget()
    {
        var directionVector = m_target - transform.position;
        if (directionVector.magnitude < 1)
        {
            m_reachedDestination = true;
        }
        else
        {
            var direction = Vector3.Normalize(directionVector);
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && direction.x != 0)
            {
                MoveHorizontal(directionVector);
                return true;
            }
            else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y) && direction.y != 0)
            {
                MoveVertical(directionVector);
                return true;
            }
            else if (direction.x != 0)
            {
                int randomDirection = Random.Range(0, 2);
                if (randomDirection == 0)
                {
                    MoveHorizontal(directionVector);
                    return true;
                }
                else
                {
                    MoveVertical(directionVector);
                    return true;
                }
            }
        }
        return false;
    }

    private void TakeAction(float actionCooldown, Action callback)
    {
        m_canAct = false;
        m_durationSinceLastAction = 0;
        m_actionCooldown = actionCooldown;
        m_actionCallback = callback;
        m_actingBar.color = new Color(1, 1, 1, 1);
        m_actingBar.transform.localScale = new Vector3(k_actingBarMax, 0.1f, 1);
        m_cooldownBarActive = true;
    }

    private void TakeSilentAction(float actionCooldown, Action callback)
    {
        m_canAct = false;
        m_durationSinceLastAction = 0;
        m_actionCooldown = actionCooldown;
        m_actionCallback = callback;
        m_cooldownBarActive = false;
    }

    private void MoveHorizontal(Vector3 moveDirection)
    {
        var multiplier = moveDirection.x / Mathf.Abs(moveDirection.x);
        var currentPosition = transform.position;
        var targetPosition = new Vector3(currentPosition.x + (1 * multiplier), currentPosition.y, currentPosition.z);
        if (!MonsterManager.IsMonsterInPosition(targetPosition))
        {
            transform.position = targetPosition;
        }
    }

    private void AttackPlant()
    {
        PlantManager.AttackPlant(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z));
    }

    private void MoveVertical(Vector3 moveDirection)
    {
        var multiplier = moveDirection.y / Mathf.Abs(moveDirection.y);
        var currentPosition = transform.position;
        var targetPosition = new Vector3(currentPosition.x, currentPosition.y + (1 * multiplier), currentPosition.z);
        if (!MonsterManager.IsMonsterInPosition(targetPosition))
        {
            transform.position = targetPosition;
        }
    }

    private bool CheckAreaForPlant(out Vector3 plantPosition)
    {
        List<Vector3Int> surroundingTiles = GetSurroundingTilePositions();
        foreach(var tilePosition in surroundingTiles)
        {
            if (PlantManager.TryToFindPlant(tilePosition))
            {
                plantPosition = tilePosition;
                return true;
            }
        }
        plantPosition = default;
        return false;
    }

    private bool CheckAreaForPlayer(out Vector3Int detectedPlayer)
    {
        var playerPosition = LevelManager.GetPlayerPosition();
        if (Vector3.Distance(playerPosition, transform.position) < m_playerDetectionRadius)
        {
            detectedPlayer = playerPosition;
            return true;
        }
        detectedPlayer = default;
        return false;
    }

    private List<Vector3Int> GetSurroundingTilePositions()
    {
        List<Vector3Int> surroundingTiles = new List<Vector3Int>(4);
        for (int i = -1; i < 2; i ++)
        {
            surroundingTiles.Add(new Vector3Int((int)transform.position.x + i, (int)transform.position.y, 0));
            surroundingTiles.Add(new Vector3Int((int)transform.position.x, (int)transform.position.y + i, 0));
        }
        return surroundingTiles;
    }
}
