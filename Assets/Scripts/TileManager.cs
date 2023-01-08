using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private List<TileData> m_tileAssets;

    [SerializeField]
    private Tilemap m_levelDayTilemap;

    [SerializeField]
    private Tilemap m_levelNightTilemap;

    public static TileManager Instance;

    private TileData[,] m_tileData;
    private bool m_initialized = false;

    private Dictionary<TileType, (Tile DayTile, Tile NightTile)> m_tileAssetDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of Tile Manager present. Destroying...");
            Destroy(Instance);
        }

        Instance = this;

        m_tileAssetDictionary = new Dictionary<TileType, (Tile, Tile)>(m_tileAssets.Count);
        foreach(var tileAsset in m_tileAssets)
        {
            m_tileAssetDictionary.Add(tileAsset.TileType, (tileAsset.DayTile, tileAsset.NightTile));
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
        var halfWidth = mapSize.x / 2;
        var halfHeight = mapSize.y / 2;


        for (int i = 0, c = mapSize.x; i < c; i++)
        {
            for (int j = 0, d = mapSize.y; j < d; j++)
            {
                // carve out an area for  the starting position
                if ((i > halfWidth - 4 && i < halfWidth + 4) && (j > halfHeight - 5 && j < halfHeight + 2))
                {
                    Instance.m_tileData[i, j] = new TileData()
                    {
                        TileType = TileType.Dirt,
                        DayTile = Instance.m_tileAssets[2].DayTile,
                        NightTile = Instance.m_tileAssets[2].NightTile
                    };
                }
                else
                {
                    Instance.m_tileData[i, j] = GetRandomTileData();
                }

                Instance.m_levelDayTilemap.SetTile(new Vector3Int(i, j, 0), Instance.m_tileData[i, j].DayTile);
                Instance.m_levelNightTilemap.SetTile(new Vector3Int(i, j, 0), Instance.m_tileData[i, j].NightTile);
            }
        }
        Instance.m_initialized = true;
        return true;
    }

    public static TileData GetRandomTileData()
    {
        var min = 2;
        var max = Instance.m_tileAssets.Count;

        int randomTileIndex = Random.Range(min, max);
        var randomTile = Instance.m_tileAssets[randomTileIndex];
        return new TileData()
        {
            TileType = (TileType)randomTileIndex,
            DayTile = randomTile.DayTile,
            NightTile = randomTile.NightTile,
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

        TileData tileData = Instance.m_tileData[tilePosition.x, tilePosition.y];
        bool tileUpdated = false;
        switch (tileData.TileType)
        {
            case TileType.Soil:
                if (tool == EquipmentId.BagOfSeeds)
                {
                    tileData.TileType = TileType.Crop;
                    tileData.DayTile = Instance.m_tileAssetDictionary[TileType.Crop].DayTile;
                    tileData.NightTile = Instance.m_tileAssetDictionary[TileType.Crop].NightTile;
                    PlantManager.PlantSeed(tilePosition);
                    tileUpdated = true;
                }
                else if (tool == EquipmentId.DungeonSeed)
                {
                    Debug.Log("Planting dungeon seed");
                    tileData.TileType = TileType.Crop;
                    tileData.DayTile = Instance.m_tileAssetDictionary[TileType.Crop].DayTile;
                    tileData.NightTile = Instance.m_tileAssetDictionary[TileType.Crop].NightTile;
                    PlantManager.PlantDungeon(tilePosition);
                    tileUpdated = true;
                }
                break;
            case TileType.Dirt:
                if (tool == EquipmentId.Hoe)
                {
                    tileData.TileType = TileType.Soil;
                    tileData.DayTile = Instance.m_tileAssetDictionary[TileType.Soil].DayTile;
                    tileData.NightTile = Instance.m_tileAssetDictionary[TileType.Soil].NightTile;
                    tileUpdated = true;
                }
                break;
            case TileType.Scrub:
                if (tool == EquipmentId.Shovel)
                {
                    tileData.TileType = TileType.Dirt;
                    tileData.DayTile = Instance.m_tileAssetDictionary[TileType.Dirt].DayTile;
                    tileData.NightTile = Instance.m_tileAssetDictionary[TileType.Dirt].NightTile;
                    tileUpdated = true;
                }
                break;
            case TileType.Rock:
                if (tool == EquipmentId.PickAxe)
                {
                    tileData.TileType = TileType.Dirt;
                    tileData.DayTile = Instance.m_tileAssetDictionary[TileType.Dirt].DayTile;
                    tileData.NightTile = Instance.m_tileAssetDictionary[TileType.Dirt].NightTile;
                    tileUpdated = true;
                }
                break;
            case TileType.Tree:
                if (tool == EquipmentId.Axe)
                {
                    tileData.TileType = TileType.Scrub;
                    tileData.DayTile = Instance.m_tileAssetDictionary[TileType.Scrub].DayTile;
                    tileData.NightTile = Instance.m_tileAssetDictionary[TileType.Scrub].NightTile;
                    tileUpdated = true;
                }
                break;
            case TileType.Crop:
                if (tool == EquipmentId.Scythe)
                {
                    if (PlantManager.TryHarvestCrop(tilePosition, out int plantYield))
                    {
                        tileData.TileType = TileType.Dirt;
                        tileData.DayTile = Instance.m_tileAssetDictionary[TileType.Dirt].DayTile;
                        tileData.NightTile = Instance.m_tileAssetDictionary[TileType.Dirt].NightTile;
                        tileUpdated = true;
                        InventoryManager.AddFruit(plantYield);
                    }
                    else
                    {
                        Toaster.PopToast("It's too early to harvest.");
                    }
                }
                break;
        }

        if (tileUpdated)
        {
            Instance.m_levelDayTilemap.SetTile(tilePosition, tileData.DayTile);
            Instance.m_levelNightTilemap.SetTile(tilePosition, tileData.NightTile);
        }
    }

    private static Color k_invisible = new Color(1, 1, 1, 0);
    private static Color k_visibile = new Color(1, 1, 1, 1);

    public static void SwapTimeOfDay(bool isNight)
    {
        if (Instance == null)
        {
            Debug.LogError("Trying to swap time of day on an null instance.");
            return;
        }

        Instance.m_levelDayTilemap.color = isNight ? k_invisible : k_visibile;
        Instance.m_levelNightTilemap.color = isNight ? k_visibile : k_invisible;

        if (isNight)
        {
            Toaster.PopToast("Oh what a horrible night to have a curse...");
        }
        else
        {
            Toaster.PopToast("The morning sun has vanquished the horrible night...");
        }
    }

    public static void PlantDiedOnTile(Vector3Int tilePosition)
    {
        if (!ManagerIsValid())
        {
            return;
        }

        TileData tileData = Instance.m_tileData[tilePosition.x, tilePosition.y];
        bool tileUpdated = false;
        if (tileData.TileType == TileType.Crop)
        {
            tileUpdated = true;
            tileData.TileType = TileType.Scrub;
            tileData.DayTile = Instance.m_tileAssetDictionary[TileType.Scrub].DayTile;
            tileData.NightTile = Instance.m_tileAssetDictionary[TileType.Scrub].NightTile;
        }

        if (tileUpdated)
        {
            Instance.m_levelDayTilemap.SetTile(tilePosition, tileData.DayTile);
            Instance.m_levelNightTilemap.SetTile(tilePosition, tileData.NightTile);
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
    Crop,
    Dirt,
    Rock,
    Scrub,
    Tree
}

[System.Serializable]
public class TileData
{
    public TileType TileType;
    public Tile DayTile;
    public Tile NightTile;
}
