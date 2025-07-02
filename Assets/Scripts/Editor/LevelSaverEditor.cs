using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
public class LevelSaverEditor : EditorWindow
{
    private Tilemap tilemap;
    private LevelData levelToEdit;
    private bool isEditMode = false;
    private Dictionary<string, TileBase> tileLookup = new();

    private bool showTraps;
    private GameObject[] trapPrefabs;
    private List<GameObject> spawnedTraps = new List<GameObject>();

    private bool showEnemies;
    private GameObject[] enemyPrefabs;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    [MenuItem("Tools/Level Saver")]
    public static void ShowWindow()
    {
        GetWindow<LevelSaverEditor>("Level Saver");
    }
    private void ShowTitle()
    {
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 25;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;

        GUILayout.Space(10);
        GUILayout.Label("Level Saver", titleStyle);
        GUILayout.Space(10);
    }
    private void OnGUI()
    {
        ShowTitle();

        GUILayout.Label("Tilemap Saver Panel", EditorStyles.boldLabel);

        tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);

        //Edit Mode
        GUILayout.Space(10);
        EditMode();
        //Clear Button
        GUILayout.Space(10);
        ClearLevel();
        //Tools
        GUILayout.Space(20);
        Tools();
    }
    //-----Editor Tools
    private void Tools()
    {
        GUIStyle newStyle = new GUIStyle(GUI.skin.label);
        newStyle.alignment = TextAnchor.MiddleCenter;
        newStyle.fontSize = 20;
        newStyle.fontStyle = FontStyle.Bold;
        newStyle.normal.textColor = Color.white;

        GUILayout.Space(10);
        GUILayout.Label("-Tools-", newStyle);
        GUILayout.Space(10);

        GUILayout.Label("🧲 Drag traps into the scene", EditorStyles.boldLabel);

        //Trap Browser
        EditorGUILayout.Space(20);
        Traps();
        //Enemy Browser
        EditorGUILayout.Space(20);
        Enemies();
    }
    //-----Editor Enemies
    private void Enemies()
    {
        showEnemies = EditorGUILayout.Foldout(showEnemies, "Enemies");

        if (showEnemies)
        {
            EditorGUILayout.Space(30);

            enemyPrefabs = Resources.LoadAll<GameObject>("Enemies");

            foreach (var enemy in enemyPrefabs)
            {
                GUILayout.BeginHorizontal();

                Texture2D preview = AssetPreview.GetAssetPreview(enemy);
                if (preview == null) preview = AssetPreview.GetMiniThumbnail(enemy);

                GUILayout.Label(preview, GUILayout.Width(40), GUILayout.Height(40));
                GUILayout.Label(enemy.name);

                DrawDraggablePrefab(enemy);

                GUILayout.EndHorizontal();
            }
        }
    }
    //-----Editor Trap
    private void Traps()
    {
        showTraps = EditorGUILayout.Foldout(showTraps, "Traps");

        if (showTraps)
        {
            EditorGUILayout.Space(30);

            trapPrefabs = Resources.LoadAll<GameObject>("Traps");

            foreach (var trap in trapPrefabs)
            {
                GUILayout.BeginHorizontal();

                Texture2D preview = AssetPreview.GetAssetPreview(trap);
                if (preview == null) preview = AssetPreview.GetMiniThumbnail(trap);

                GUILayout.Label(preview, GUILayout.Width(40), GUILayout.Height(40));
                GUILayout.Label(trap.name);

                DrawDraggablePrefab(trap);

                GUILayout.EndHorizontal();
            }
        }
    }
    private void DrawDraggablePrefab(GameObject prefab)
    {
        Rect rect = GUILayoutUtility.GetRect(100, 40, GUILayout.ExpandWidth(true));
        GUI.Box(rect, prefab.name);

        // Drag handling
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = new Object[] { prefab };
            DragAndDrop.StartDrag("Dragging Trap");
            Event.current.Use();
        }
    }
    //---- Editor Load Level
    private void EditMode()
    {
        isEditMode = EditorGUILayout.Toggle("Edit Existing Level", isEditMode);

        if (isEditMode)
        {
            levelToEdit = (LevelData)EditorGUILayout.ObjectField("Level to Edit", levelToEdit, typeof(LevelData), false);

            if (levelToEdit != null)
            {
                if (GUILayout.Button("Load Level"))
                {
                    LoadLevel(levelToEdit);
                }

                if (GUILayout.Button("Update Level"))
                {
                    UpdateLevel(levelToEdit);
                }
            }
        }
        else
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Save as New Level"))
            {
                SaveNewLevel();
            }
        }
    }
    private void UpdateLevel(LevelData level)
    {
        level.Tiles.Clear();

        level.Traps.Clear();

        level.Enemies.Clear();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                TileBase tile = tilemap.GetTile(pos);
                level.Tiles.Add(new TileInfo { Position = pos, TileName = tile.name });
            }
        }

        GameObject[] allTraps = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (var trap in allTraps)
        {
            if (trap.tag == "Trap")
            {
                Vector2 trapPos = trap.transform.position;
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(trap);

                if (prefab != null)
                {
                    level.Traps.Add(new TrapInfo
                    {
                        TrapPosition = trapPos,
                        TrapPrefab = prefab
                    });
                }
                else
                {
                    Debug.LogWarning("Trap prefab source not Found: " + trap.name);
                }
            }
        }

        GameObject[] allEnemies = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (var enemy in allEnemies)
        {
            if (enemy.tag == "Enemy")
            {
                Vector2 trapPos = enemy.transform.position;
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(enemy);

                if (prefab != null)
                {
                    level.Enemies.Add(new EnemyInfo
                    {
                        EnemyPos = trapPos,
                        EnemyPrefab = prefab,
                    });
                }
                else
                {
                    Debug.LogWarning("Trap prefab source not Found: " + enemy.name);
                }
            }
        }

        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();

        Debug.Log("Level updated.");
    }
    private void LoadLevel(LevelData data)
    {
        tileLookup.Clear();

        TileBase[] allTiles = Resources.LoadAll<TileBase>("");
        foreach (TileBase tile in allTiles)
        {
            if (!tileLookup.ContainsKey(tile.name))
                tileLookup[tile.name] = tile;
        }

        tilemap.ClearAllTiles();

        foreach (var tileInfo in data.Tiles)
        {
            if (tileLookup.TryGetValue(tileInfo.TileName, out TileBase tile))
            {
                tilemap.SetTile(tileInfo.Position, tile);
            }
            else
            {
                Debug.LogWarning($"Tile Not Found: {tileInfo.TileName}");
            }
        }
        //Traps
        foreach (var trap in spawnedTraps)
        {
            if (trap != null)
                Destroy(trap);
        }

        spawnedTraps.Clear();

        foreach (var trapInfo in data.Traps)
        {
            if (trapInfo.TrapPrefab != null)
            {
                GameObject newTrap = Instantiate(trapInfo.TrapPrefab, (Vector3)trapInfo.TrapPosition, Quaternion.identity);
                spawnedTraps.Add(newTrap);
            }
            else
            {
                Debug.LogWarning("Trap prefab is null in level data.");
            }
        }
        //Enemy
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        spawnedEnemies.Clear();

        foreach (var enemyInfo in data.Enemies)
        {
            if (enemyInfo.EnemyPrefab != null)
            {
                GameObject newTrap = Instantiate(enemyInfo.EnemyPrefab, (Vector3)enemyInfo.EnemyPos, Quaternion.identity);
                spawnedTraps.Add(newTrap);
            }
            else
            {
                Debug.LogWarning("Trap prefab is null in level data.");
            }
        }
        Debug.Log("Level loaded.");
    }
    private void ClearLevel()
    {
        if(GUILayout.Button("Clear Tilemap"))
        {
            if (EditorUtility.DisplayDialog("Are you sure?", "This will clear the entire Tilemap.", "Yes", "Cancel"))
            {
                tilemap.ClearAllTiles();

                GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

                foreach (var obj in allObjects)
                {
                    if (obj!=null && obj.CompareTag("Trap"))
                    {
                        DestroyImmediate(obj);
                    }
                    if(obj != null && obj.CompareTag("Enemy"))
                    {
                        DestroyImmediate(obj);
                    }
                }
                Debug.Log("Cleared.");
            }
        }
    }
    //---- Editor Save Level
    private void SaveNewLevel()
    {
        if (tilemap == null || tilemap.size.x < 3 || tilemap.size.y < 3)
        {
            Debug.LogError("Tilemap field is empty or too small!");
            return;
        }

        LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                TileBase tile = tilemap.GetTile(pos);
                newLevel.Tiles.Add(new TileInfo
                {
                    Position = pos,
                    TileName = tile.name,
                    TileColor = tilemap.GetColor(pos) // isteğe bağlı
                });
            }
        }

        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj.CompareTag("Trap"))
            {
                Vector2 trapPos = obj.transform.position;
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);

                if (prefab != null)
                {
                    newLevel.Traps.Add(new TrapInfo
                    {
                        TrapPosition = trapPos,
                        TrapPrefab = prefab
                    });
                }
                else
                    Debug.LogWarning("Trap prefab source not Found: " + obj.name);
            }
            else if(obj.CompareTag("Enemy"))
            {
                Vector2 trapPos = obj.transform.position;
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);

                if (prefab != null)
                {
                    newLevel.Enemies.Add(new EnemyInfo
                    {
                        EnemyPos = trapPos,
                        EnemyPrefab = prefab
                    });
                }
                else
                    Debug.LogWarning("Trap prefab source not Found: " + obj.name);
            }
        }

        string folderPath = "Assets/Resources/Levels";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Levels");
        }

        string path = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/Level_" + System.DateTime.Now.Ticks + ".asset");
        AssetDatabase.CreateAsset(newLevel, path);
        AssetDatabase.SaveAssets();
    }
}