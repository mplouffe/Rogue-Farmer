using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    private static LevelManager m_instance;

    [SerializeField]
    private Tilemap m_levelTilemap;

    [SerializeField]
    private int m_width;

    [SerializeField]
    private int m_height;

    [SerializeField]
    private FarmerController m_playerPrefab;

    [SerializeField]
    private Vector3Int m_playerStartingPosition;

    private FarmerController m_player;

    [SerializeField] private int m_maxPlayerHealth;
    private int m_currentPlayerHealth;

    [SerializeField] private HealthUIManager m_healthUIManager;

    [SerializeField] private CinemachineVirtualCamera m_virtualCamera;

    private void Start()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogError("Multiple instances of Level Manager present. Destroying...");
            Destroy(m_instance);
        }
        m_instance = this;

        m_levelTilemap.ClearAllTiles();
        m_levelTilemap.size = new Vector3Int(m_width, m_height, 1);
        if (TileManager.InitializeTileData(m_levelTilemap.size))
        {
            LevelBounds.SetLevelBounds(m_levelTilemap.size);
        }
        
        m_player = Instantiate(m_playerPrefab, m_playerStartingPosition, Quaternion.identity);
        m_virtualCamera.m_LookAt = m_player.transform;
        m_virtualCamera.m_Follow = m_player.transform;

        m_currentPlayerHealth = m_maxPlayerHealth;
        m_healthUIManager.UpdateMaxHealth(m_maxPlayerHealth);
        m_healthUIManager.UdpateCurrentHealth(m_currentPlayerHealth);
    }

    public static Vector3Int GetPlayerPosition()
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to retrieve player position from null instance.");
            return default;
        }

        return Vector3Int.FloorToInt(m_instance.m_player.transform.position);
    }

    public static void AttackPlayer(int strength)
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to retrieve player position from null instance.");
            return;
        }

        m_instance.m_currentPlayerHealth -= strength;
        if (m_instance.m_currentPlayerHealth < 0)
        {
            // Dead player
            m_instance.m_currentPlayerHealth = 0;
        }

        m_instance.m_healthUIManager.UdpateCurrentHealth(m_instance.m_currentPlayerHealth);
    }
}

public enum TileTypes
{
    Empty,
    Soil,
    Dirt,
    Rock,
    Tree,
    Shrub
}
