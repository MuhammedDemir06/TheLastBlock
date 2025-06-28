using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private LevelData levelData;

    private void Start()
    {
        Dictionary<string, TileBase> tileDict = new Dictionary<string, TileBase>();

        foreach (var t in Resources.LoadAll<TileBase>("Tileset Palette/TP Ground"))
        {
            tileDict[t.name] = t;
        }

        LoadLevel(levelData, tilemap , tileDict);
    }

    public void LoadLevel(LevelData levelData, Tilemap targetTilemap, Dictionary<string, TileBase> tileLookup)
    {
        targetTilemap.ClearAllTiles();

        foreach (var tileInfo in levelData.tiles)
        {
            if (tileLookup.TryGetValue(tileInfo.TileName, out TileBase tile))
            {
                targetTilemap.SetTile(tileInfo.Position, tile);
            }
            else
            {
                Debug.LogWarning($"Tile Not Found: {tileInfo.TileName}");
            }
        }
    }
}