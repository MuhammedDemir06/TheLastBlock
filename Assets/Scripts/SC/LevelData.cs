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
public class TrapInfo
{
    public GameObject TrapPrefab;
    public Vector2 TrapPosition;
}
[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/Create New Level")]
public class LevelData : ScriptableObject
{
    public List<TileInfo> Tiles = new List<TileInfo>();

    public List<TrapInfo> Traps = new List<TrapInfo>();
}
