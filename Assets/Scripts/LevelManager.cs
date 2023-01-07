using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap m_levelTilemap;

    [SerializeField]
    private List<Tile> m_tileAssets;

    [SerializeField]
    private int m_width;

    [SerializeField]
    private int m_height;

    private void Start()
    {
        int min = 0;
        int max = 5;
        m_levelTilemap.ClearAllTiles();
        m_levelTilemap.size = new Vector3Int(m_width, m_height, 1);
        foreach(var pos in m_levelTilemap.cellBounds.allPositionsWithin)
        {
            int randomTileIndex = Random.Range(min, max);
            var randomTile = m_tileAssets[randomTileIndex];
            m_levelTilemap.SetTile(pos, randomTile);
        }
        LevelBounds.SetLevelBounds(m_levelTilemap.size);
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

public enum SanityCheck
{
    sane,
    insane
}
