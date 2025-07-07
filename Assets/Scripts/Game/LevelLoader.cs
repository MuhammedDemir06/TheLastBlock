using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform gameParent;
    [SerializeField] private LevelData levelData;
    [Header("Background")]
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private SpriteRenderer playerBackgroundColor;
    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject playerLit;
    private void Start()
    {
        playerBackgroundColor.gameObject.SetActive(true);
        backgroundSprite.gameObject.SetActive(true);
        Init();
    }
    private void Init()
    {
        Dictionary<string, TileBase> tileDict = new Dictionary<string, TileBase>();

        foreach (var t in Resources.LoadAll<TileBase>("Tileset Palette/TP Ground"))
        {
            tileDict[t.name] = t;
        }

        LoadLevel(levelData, tilemap, tileDict);
    }
    public void LoadLevel(LevelData levelData, Tilemap targetTilemap, Dictionary<string, TileBase> tileLookup)
    {
        targetTilemap.ClearAllTiles();

        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (var tileInfo in levelData.Tiles)
        {
            if (tileLookup.TryGetValue(tileInfo.TileName, out TileBase tile))
            {
                targetTilemap.SetTile(tileInfo.Position, tile);

                minX = Mathf.Min(minX, tileInfo.Position.x);
                maxX = Mathf.Max(maxX, tileInfo.Position.x);
                minY = Mathf.Min(minY, tileInfo.Position.y);
                maxY = Mathf.Max(maxY, tileInfo.Position.y);
            }
            else
            {
                Debug.LogWarning($"Tile Not Found: {tileInfo.TileName}");
            }
        }

        bool hasPlayerStartPos = false;

        foreach (var tool in levelData.Tools)
        {
            var newTool = Instantiate(tool.ToolPrefab, tool.ToolPosition, Quaternion.identity);
            newTool.transform.SetParent(gameParent);

            if (newTool.CompareTag("PlayerStartPos"))
            {
                player.position = newTool.transform.position;
                hasPlayerStartPos = true;
                player.gameObject.SetActive(true);
                playerLit.SetActive(true);
                player.GetComponent<PlayerController>().StartPos = newTool.transform.position;
            }
        }

        if (!hasPlayerStartPos)
        {
            playerLit.SetActive(false);
            Debug.LogError("Player Start Position Not Found!");
        }

        int tileWidth = maxX - minX + 1;
        int tileHeight = maxY - minY + 1;

        Vector3 center = new Vector3(minX + tileWidth / 2f, minY + tileHeight / 2f, 10f);

        playerBackgroundColor.transform.position = center;
        playerBackgroundColor.transform.localScale = new Vector3(tileWidth, tileHeight, 1f);

        int marginX = 10;
        int marginY = 4;
        minX -= marginX;
        maxX += marginX;
        minY -= marginY;
        maxY += marginY;

        tileWidth = maxX - minX + 1;
        tileHeight = maxY - minY + 1;

        Vector3 centerPos = new Vector3(minX + tileWidth / 2f, minY + tileHeight / 2f, 10f);

        backgroundSprite.transform.position = centerPos;
        backgroundSprite.transform.localScale = new Vector3(tileWidth, tileHeight, 1f);

        backgroundSprite.sprite = levelData.Background.BackgroundSprite;
        playerBackgroundColor.color = levelData.Background.PlayerBackgroundColor;


    }
}