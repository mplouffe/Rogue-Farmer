using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager m_instance;

    private Dictionary<Vector3, Monster> m_monsterDictionary;

    [SerializeField] private Monster m_monsterPrefab;
    [SerializeField] private Vector3Int m_monsterStartingPosition;
    [SerializeField] private int m_monsterBaseRate;

    private

    void Start()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogError("Multiple instances of Monster Manager present. Destroying...");
            Destroy(m_instance);
        }
        m_instance = this;
        m_monsterDictionary = new Dictionary<Vector3, Monster>(20);

        var firstMonster = Instantiate(m_monsterPrefab, m_monsterStartingPosition, Quaternion.identity);
        m_monsterDictionary.Add(m_monsterStartingPosition, firstMonster);
    }

    private void Update()
    {
        if (Clock.IsNight())
        {
            var totalAllowedMonsters = m_monsterBaseRate * Clock.NightNumber();
            if (m_monsterDictionary.Keys.Count < totalAllowedMonsters)
            {
                var levelSize = LevelManager.GetSize();
                int entranceDirection = Random.Range(0, 4);
                int randomX = Random.Range(0, levelSize.x);
                int randomY = Random.Range(0, levelSize.y);
                Vector3 entrancePosition = Vector3.zero;
                switch (entranceDirection)
                {
                    case 0:
                        entrancePosition.x = randomX;
                        break;
                    case 1:

                        entrancePosition.y = randomY;
                        entrancePosition.x = levelSize.x;
                        break;
                    case 2:
                        entrancePosition.x = randomX;
                        entrancePosition.y = levelSize.y;
                        break;
                    case 3:
                        entrancePosition.y = randomY;
                        break;
                }
                if (!m_monsterDictionary.ContainsKey(entrancePosition))
                {
                    var newMonster = Instantiate(m_monsterPrefab, entrancePosition, Quaternion.identity);
                    int targetRandomizer = Random.Range(0, 10);
                    switch (targetRandomizer)
                    {
                        case 1:
                        case 2:
                        case 3:
                            newMonster.SetTarget(LevelManager.GetPlayerPosition());
                            break;
                        case 4:
                        case 5:
                        case 6:
                            newMonster.SetTarget(PlantManager.GetRandomPlantPosition());
                            break;
                        case 9:
                            newMonster.SetTarget(EquipmentManager.GetRandomEquipmentPosition());
                            break;
                    }

                    m_monsterDictionary.Add(newMonster.transform.position, newMonster);
                }
            }
        }
        else
        {
            RemoveOffCameraMonsters();
        }

        UpdateMonsters();
    }

    private void RemoveOffCameraMonsters()
    {
        List<Vector3> keysToRemove = new List<Vector3>(m_monsterDictionary.Keys.Count);

        foreach (var monsterKey in m_monsterDictionary.Keys)
        {
            var monster = m_monsterDictionary[monsterKey];
            if (!monster.IsVisible())
            {
                keysToRemove.Add(monsterKey);
                Destroy(monster.gameObject);
            }
        }

        if (keysToRemove.Count > 0)
        {
            foreach (var key in keysToRemove)
            {
                m_monsterDictionary.Remove(key);
            }
        }
    }

    private void UpdateMonsters()
    {
        List<Vector3> keysToRemove = new List<Vector3>(m_monsterDictionary.Keys.Count);
        List<Monster> updatedMonsters = new List<Monster>(m_monsterDictionary.Keys.Count);

        foreach (var monsterKey in m_monsterDictionary.Keys)
        {
            var monster = m_monsterDictionary[monsterKey];
            if (monster.UpdateMonster())
            {
                keysToRemove.Add(monsterKey);
                updatedMonsters.Add(monster);
            }
        }

        if (keysToRemove.Count > 0)
        {
            foreach (var key in keysToRemove)
            {
                m_monsterDictionary.Remove(key);
            }
        }

        if (updatedMonsters.Count > 0)
        {
            for(int i = updatedMonsters.Count - 1; i >= 0; i--)
            {
                var monster = updatedMonsters[i];
                if (m_monsterDictionary.ContainsKey(monster.transform.position))
                {
                    Destroy(monster);
                }
                else
                {
                    m_monsterDictionary.Add(monster.transform.position, monster);
                }
            }
        }
    }

    public static bool IsMonsterInPosition(Vector3 position)
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to check monster position with a null instance.");
            return false;
        }

        return m_instance.m_monsterDictionary.ContainsKey(position);
    }

    public static void AttackMonsterInPosition(Vector3 position, int strength)
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to check monster position with a null instance.");
            return;
        }

        if (m_instance.m_monsterDictionary.TryGetValue(position, out Monster monster))
        {
            monster.MonsterHealth -= strength;
            monster.TakeDamage();
            if (monster.MonsterHealth < 0)
            {
                m_instance.m_monsterDictionary.Remove(position);
                Destroy(monster.gameObject);
            }
        }
    }
}
