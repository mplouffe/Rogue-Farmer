using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [SerializeField]
    private List<EquipmentPrefab> m_equipmentPrefabs;

    private Dictionary<EquipmentId, Equipment> m_equipmentDictionary;

    private Dictionary<EquipmentId, Vector3Int> m_equipmentLocations;

    private Dictionary<Vector3Int, EquipmentId> m_equipmentByLocation;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Found additional instances of Equipment Manager. Deleting...");
            Destroy(Instance);
        }
        Instance = this;

        m_equipmentDictionary = new Dictionary<EquipmentId, Equipment>(m_equipmentPrefabs.Count);
        m_equipmentLocations = new Dictionary<EquipmentId, Vector3Int>(m_equipmentPrefabs.Count);
        m_equipmentByLocation = new Dictionary<Vector3Int, EquipmentId>(m_equipmentPrefabs.Count);

        foreach (var equipmentPrefab in m_equipmentPrefabs)
        {
            var equipmentGameObject = Instantiate<Equipment>(equipmentPrefab.Prefab, equipmentPrefab.StartingLocation, Quaternion.identity);
            m_equipmentDictionary.Add(equipmentPrefab.EquipmentId, equipmentGameObject);
            m_equipmentLocations.Add(equipmentPrefab.EquipmentId, equipmentPrefab.StartingLocation);
            m_equipmentByLocation.Add(equipmentPrefab.StartingLocation, equipmentPrefab.EquipmentId);
        }
    }

    public static Equipment GrabEquipmentAtPosition(Vector3Int position)
    {
        if (!ManagerIsValid())
        {
            return null;
        }

        if(Instance.m_equipmentByLocation.TryGetValue(position, out EquipmentId equipmentId))
        {
            if(Instance.m_equipmentDictionary.TryGetValue(equipmentId, out Equipment equipment))
            {
                equipment.gameObject.SetActive(false);
                Instance.m_equipmentByLocation.Remove(position);
                Instance.m_equipmentLocations.Remove(equipmentId);
                return equipment;
            }
        }
        return null;
    }

    public static Vector3Int GetRandomEquipmentPosition()
    {
        var randomIndex = Random.Range(0, Instance.m_equipmentByLocation.Keys.Count);
        return Instance.m_equipmentByLocation.Keys.ToArray()[randomIndex];
    }

    public static Equipment CheckEquipmentAtPosition(Vector3Int position)
    {
        if (!ManagerIsValid())
        {
            return null;
        }

        if (Instance.m_equipmentByLocation.TryGetValue(position, out EquipmentId equipmentId))
        {
            if (Instance.m_equipmentDictionary.TryGetValue(equipmentId, out Equipment equipment))
            {
                return equipment;
            }
        }
        return null;
    }

    public static bool DropEquipmentAtPosition(Vector3Int position, Equipment equipment)
    {
        if (!ManagerIsValid())
        {
            return false;
        }

        if (!Instance.m_equipmentByLocation.ContainsKey(position))
        {
            equipment.transform.position = position;
            equipment.gameObject.SetActive(true);
            Instance.m_equipmentByLocation.Add(position, equipment.EquipmentId);
            Instance.m_equipmentLocations.Add(equipment.EquipmentId, position);
            return true;
        }

        return false;
    }

    private static bool ManagerIsValid()
    {
        if (Instance == null)
        {
            Debug.LogError("Trying to use an Invalid Equipment Manager.");
            return false;
        }
        return true;
    }
}

[System.Serializable]
public class EquipmentPrefab
{
    public EquipmentId EquipmentId;
    public Equipment Prefab;
    public Vector3Int StartingLocation;
}

public enum EquipmentId
{
    Hoe,
    Shovel,
    PickAxe,
    Axe,
    WateringCan,
    PruningSheers,
    DeSeeder,
    Scythe,
    BagOfSeeds
}
