using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    private List<PlayerInput> m_playerInputs;

    [SerializeField]
    private Vector3Int m_playerStartingPosition;

    private FarmerController m_player;

    [SerializeField] private int m_maxPlayerHealth;
    private int m_currentPlayerHealth;

    [SerializeField] private HealthUIManager m_healthUIManager;

    [SerializeField] private CinemachineVirtualCamera m_virtualCamera;

    public void MoveFarmer(InputAction.CallbackContext context) => m_player.MoveFarmer(context);

    public void Grab(InputAction.CallbackContext context) => m_player.Grab(context);

    public void Drop(InputAction.CallbackContext context) => m_player.Drop(context);

    public void UseTool(InputAction.CallbackContext context) => m_player.UseTool(context);

    public void Inventory(InputAction.CallbackContext context) => m_player.Inventory(context);

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
        m_healthUIManager.UpdateCurrentHealth(m_currentPlayerHealth);
    }

    private float m_durationSinceGameOver = 0;
    private float m_gameOverMessageDuration = 6;
    private bool m_gameOver = false;

    private void Update()
    {
        if (m_gameOver)
        {
            m_durationSinceGameOver += Time.deltaTime;
            if (m_durationSinceGameOver > m_gameOverMessageDuration)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public static Vector2Int GetSize()
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to get size from null instance.");
            return default;
        }

        return new Vector2Int(m_instance.m_width, m_instance.m_height);
    }

    public static Vector3Int GetPlayerPosition()
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to retrieve player position from null instance.");
            return default;
        }

        if (m_instance.m_player == null)
        {
            return GetRandomPosition();
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
        if (m_instance.m_player != null)
        {
            m_instance.m_player.TakeDamage();
        }

        if (m_instance.m_currentPlayerHealth <= 0)
        {
            // Dead player
            ChangeInputMap(InputMap.Dead);
            Toaster.PopToast("You have been killed... Game Over...", 5);
            m_instance.m_currentPlayerHealth = 0;
            Destroy(m_instance.m_player.gameObject);
            m_instance.m_player = null;
            m_instance.m_gameOver = true;
            m_instance.m_durationSinceGameOver = 0;
        }

        m_instance.m_healthUIManager.UpdateCurrentHealth(m_instance.m_currentPlayerHealth);
    }

    public static void HealPlayer(int healValue)
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to retrieve player position from null instance.");
            return;
        }

        m_instance.m_currentPlayerHealth = Mathf.Min(m_instance.m_maxPlayerHealth, m_instance.m_currentPlayerHealth + healValue);
        m_instance.m_healthUIManager.UpdateCurrentHealth(m_instance.m_currentPlayerHealth);
    }

    public static void ChangeInputMap(InputMap newInputMap)
    {
        if (m_instance == null)
        {
            Debug.LogError("Trying to retrieve player position from null instance.");
            return;
        }
        switch (newInputMap)
        {
            case InputMap.Player:
                foreach(var playerInput in m_instance.m_playerInputs)
                {
                    playerInput.SwitchCurrentActionMap("Player");
                    Debug.Log(playerInput.currentControlScheme);
                }
                break;
            case InputMap.Inventory:
                foreach (var playerInput in m_instance.m_playerInputs)
                {
                    playerInput.SwitchCurrentActionMap("Inventory");
                    Debug.Log(playerInput.currentControlScheme);
                }
                Debug.Log(m_instance.m_playerInputs[0].currentControlScheme);
                break;
            case InputMap.Dead:
                foreach (var playerInput in m_instance.m_playerInputs)
                {
                    playerInput.SwitchCurrentActionMap("Dead");
                }
                break;
        }  
    }

    public static Vector3Int GetRandomPosition()
    {
        int randomX = Random.Range(0, m_instance.m_width);
        int randomY = Random.Range(0, m_instance.m_height);
        return new Vector3Int(randomX, randomY, 0);
    }

    public static bool PlayerNeedsHealing()
    {
        return m_instance.m_currentPlayerHealth < m_instance.m_maxPlayerHealth;
    }
}

public enum InputMap
{
    Player,
    Inventory,
    Dead
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
