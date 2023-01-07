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

    private void Start()
    {
        int min = 0;
        int max = 5;
        m_levelTilemap.ClearAllTiles();
        m_levelTilemap.size = new Vector3Int(8, 8, 1);

        foreach(var pos in m_levelTilemap.cellBounds.allPositionsWithin)
        {
            Debug.Log("pos: " + pos);
            int randomTileIndex = Random.Range(min, max);
            var randomTile = m_tileAssets[randomTileIndex];
            m_levelTilemap.SetTile(pos, randomTile);
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

public enum SanityCheck
{
    sane,
    insane
}
