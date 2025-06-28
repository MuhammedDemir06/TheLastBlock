using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileInfo
{
    public Vector3Int Position;       
    public string TileName;
    public Color TileColor;
}
[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/Create New Level")]
public class LevelData : ScriptableObject
{
    public List<TileInfo> tiles = new List<TileInfo>(); 
}
