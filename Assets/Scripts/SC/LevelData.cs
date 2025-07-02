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
[System.Serializable]
public class EnemyInfo
{
    public GameObject EnemyPrefab;
    public Vector2 EnemyPos;
}
[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/Create New Level")]
public class LevelData : ScriptableObject
{
    public List<TileInfo> Tiles = new List<TileInfo>();

    public List<TrapInfo> Traps = new List<TrapInfo>();

    public List<EnemyInfo> Enemies = new List<EnemyInfo>();
}
