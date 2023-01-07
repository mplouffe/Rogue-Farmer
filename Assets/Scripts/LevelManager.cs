using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap m_levelTilemap;

    [SerializeField]
    private int m_width;

    [SerializeField]
    private int m_height;

    private void Start()
    {
        m_levelTilemap.ClearAllTiles();
        m_levelTilemap.size = new Vector3Int(m_width, m_height, 1);
        if (TileManager.InitializeTileData(m_levelTilemap.size))
        {
            foreach (var pos in m_levelTilemap.cellBounds.allPositionsWithin)
            {
                m_levelTilemap.SetTile(pos, TileManager.GetTileData(pos).Tile);
            }
            LevelBounds.SetLevelBounds(m_levelTilemap.size);
        }
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
