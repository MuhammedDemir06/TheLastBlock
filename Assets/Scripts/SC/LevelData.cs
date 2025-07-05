using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileInfo
{
    public Vector3Int Position;       
    public string TileName;
    public Color TileColor;
}
[System.Serializable]
public class ToolInfo
{
    public GameObject ToolPrefab;
    public Vector2 ToolPosition;
}
[System.Serializable]
public class BackgroundInfo
{
    public Sprite BackgroundSprite;

    public Color PlayerBackgroundColor;
}
public class LevelData : ScriptableObject
{
    public List<TileInfo> Tiles = new List<TileInfo>();

    public List<ToolInfo> Tools = new List<ToolInfo>();

    public BackgroundInfo Background = new BackgroundInfo();
}
