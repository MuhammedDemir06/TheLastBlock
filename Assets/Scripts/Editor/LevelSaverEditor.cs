using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering.VirtualTexturing;
public enum GameMode
{
    PC,
    Mobile,
    Console
}
public class LevelSaverEditor : EditorWindow
{
    private Tilemap tilemap;
    private LevelData levelToEdit;
    private bool isEditMode = false;
    private Dictionary<string, TileBase> tileLookup = new();

    private bool showTraps;
    private GameObject[] trapPrefabs;
    private List<GameObject> spawnedTraps = new List<GameObject>();

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
        //Trap Browser
        EditorGUILayout.Space(20);
        Traps();
    }
    //-----Editor Trap
    private void Traps()
    {
        showTraps = EditorGUILayout.Foldout(showTraps, "Traps");

        if (showTraps)
        {
            GUILayout.Label("🧲 Drag traps into the scene", EditorStyles.boldLabel);
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

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                TileBase tile = tilemap.GetTile(pos);
                level.Tiles.Add(new TileInfo { Position = pos, TileName = tile.name });
            }
        }

        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (var trap in allObjects)
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

        foreach (var trap in spawnedTraps)
        {
            if (trap != null)
                Destroy(trap);
        }
        spawnedTraps.Clear();

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
                    if (obj.CompareTag("Trap"))
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
                {
                    Debug.LogWarning("Trap prefab source not Found: " + obj.name);
                }
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
