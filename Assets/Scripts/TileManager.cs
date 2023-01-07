using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private List<TileData> m_tileAssets;

    [SerializeField]
    private Tilemap m_levelTilemap;

    public static TileManager Instance;

    private TileData[,] m_tileData;
    private bool m_initialized = false;

    private Dictionary<TileType, Tile> m_tileAssetDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of Tile Manager present. Destroying...");
            Destroy(Instance);
        }

        Instance = this;

        m_tileAssetDictionary = new Dictionary<TileType, Tile>(m_tileAssets.Count);
        foreach(var tileAsset in m_tileAssets)
        {
            m_tileAssetDictionary.Add(tileAsset.TileType, tileAsset.Tile);
        }
    }

    public static bool InitializeTileData(Vector3Int mapSize)
    {
        if (Instance == null)
        {
            Debug.LogError("Trying to Initialize a null instance.");
            return false;
        }

        if (Instance.m_initialized)
        {
            Debug.LogError("Trying to Initialize a TileManager that is already initialized.");
            return false;
        }

        Instance.m_tileData = new TileData[mapSize.x, mapSize.y];
        for (int i = 0, c = mapSize.x; i < c; i++)
        {
            for (int j = 0, d = mapSize.y; j < d; j++)
            {
                Instance.m_tileData[i, j] = GetRandomTileData();
            }
        }
        Instance.m_initialized = true;
        return true;
    }

    public static TileData GetRandomTileData()
    {
        var min = 1;
        var max = Instance.m_tileAssets.Count;

        int randomTileIndex = Random.Range(min, max);
        var randomTile = Instance.m_tileAssets[randomTileIndex];
        return new TileData()
        {
            TileType = (TileType)randomTileIndex,
            Tile = randomTile.Tile
        };
    }

    public static TileData GetTileData(Vector3Int tileLocation)
    {
        if (!ManagerIsValid())
        {
            return null;
        }

        if (tileLocation.x < 0 || tileLocation.x > Instance.m_tileData.GetLength(0))
        {
            Debug.LogError("Location has invalid x value.");
            return null;
        }

        if (tileLocation.y < 0 || tileLocation.y > Instance.m_tileData.GetLength(1))
        {
            Debug.LogError("Location has invalid y value.");
            return null;
        }

        return Instance.m_tileData[tileLocation.x, tileLocation.y];
    }

    public static void UseToolOnTile(Vector3Int tilePosition, EquipmentId tool)
    {
        if (!ManagerIsValid())
        {
            return;
        }

        var tileData = Instance.m_tileData[tilePosition.x, tilePosition.y];
        bool tileUpdated = false;
        switch (tileData.TileType)
        {
            case TileType.Dirt:
                if (tool == EquipmentId.Hoe)
                {
                    tileData.TileType = TileType.Soil;
                    tileData.Tile = Instance.m_tileAssetDictionary[TileType.Soil];
                    tileUpdated = true;
                }
                break;
            case TileType.Scrub:
                if (tool == EquipmentId.Shovel)
                {
                    tileData.TileType = TileType.Dirt;
                    tileData.Tile = Instance.m_tileAssetDictionary[TileType.Dirt];
                    tileUpdated = true;
                }
                break;
            case TileType.Rock:
                if (tool == EquipmentId.PickAxe)
                {
                    tileData.TileType = TileType.Dirt;
                    tileData.Tile = Instance.m_tileAssetDictionary[TileType.Dirt];
                    tileUpdated = true;
                }
                break;
            case TileType.Tree:
                if (tool == EquipmentId.Axe)
                {
                    tileData.TileType = TileType.Scrub;
                    tileData.Tile = Instance.m_tileAssetDictionary[TileType.Scrub];
                    tileUpdated = true;
                }
                break;
        }

        if (tileUpdated)
        {
            Instance.m_levelTilemap.SetTile(tilePosition, tileData.Tile);
        }
    }

    private static bool ManagerIsValid()
    {
        if (Instance == null || !Instance.m_initialized)
        {
            Debug.LogError("Trying to get tile data from uninitialized instance.");
            return false;
        }
        return true;
    }
}

public enum TileType
{
    Soil,
    Dirt,
    Rock,
    Scrub,
    Tree
}

[System.Serializable]
public class TileData
{
    public TileType TileType;
    public Tile Tile;
}
