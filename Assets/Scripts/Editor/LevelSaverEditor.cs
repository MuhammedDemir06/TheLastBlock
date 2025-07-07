using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
public class LevelSaverEditor : EditorWindow
{
    private Tilemap tilemap;
    private LevelData levelToEdit;
    private bool isEditMode = false;
    private Dictionary<string, TileBase> tileLookup = new();

    private Vector2 scrollPositionTools;

    private string chapterName;

    private bool showTraps;
    private GameObject[] toolPrefabs;
    private List<GameObject> spawnedTools = new List<GameObject>();

    private bool showEnemies;

    private bool showPlatforms;

    private int lastLevel;

    //Background
    private bool changeBackground;
    private Color playerBackgroundColor = new Color(173, 143, 102, 148);
    private Sprite backgroundSprite;

    private int startPosValue;
    private int nextLevelValue;

    private bool newChapter;

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
    private void OnEnable()
    {
        lastLevel = EditorPrefs.GetInt("MyGame_LastLevel", 0);
        chapterName = EditorPrefs.GetString("MyGame_LastChapterName");
    }
    private void OnGUI()
    {
        ShowTitle();

        scrollPositionTools = EditorGUILayout.BeginScrollView(scrollPositionTools, GUILayout.Height(800));

        GUILayout.Label("Tilemap Saver Panel", EditorStyles.boldLabel);

        tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);

        //Edit Mode
        GUILayout.Space(10);
        EditMode();
        //Clear Button
        GUILayout.Space(10);
        ClearLevel();
        //Reset Data
        GUILayout.Space(10);
        ResetData();
        //Tools
        Tools();

        EditorGUILayout.EndScrollView();
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

        GameBackground();

        GUILayout.Space(10);
        GUILayout.Label("🧲 Drag traps into the scene", EditorStyles.boldLabel);

        //Trap Browser
        EditorGUILayout.Space(20);
        Traps();
        //Enemy Browser
        EditorGUILayout.Space(20);
        Enemies();
        //Platform Browser
        Platforms();
    }
    //-----Editor Platform
    private void Platforms()
    {
        showPlatforms = EditorGUILayout.Foldout(showPlatforms, "Platforms");

        if (showPlatforms)
        {
            EditorGUILayout.Space(30);

            toolPrefabs = Resources.LoadAll<GameObject>("Platforms");

            foreach (var platform in toolPrefabs)
            {
                GUILayout.BeginHorizontal();

                Texture2D preview = AssetPreview.GetAssetPreview(platform);
                if (preview == null) preview = AssetPreview.GetMiniThumbnail(platform);

                GUILayout.Label(preview, GUILayout.Width(40), GUILayout.Height(40));
                GUILayout.Label(platform.name);

                DrawDraggablePrefab(platform);

                GUILayout.EndHorizontal();
            }
        }
    }
    //-----Editor Background
    private void GameBackground()
    {
        changeBackground = EditorGUILayout.Toggle("Change Background", changeBackground);

        if(changeBackground)
        {
            GUILayout.Label("Background Sprite:", GUILayout.Width(150));
            backgroundSprite = (Sprite)EditorGUILayout.ObjectField(backgroundSprite, typeof(Sprite), false, GUILayout.Width(50), GUILayout.Height(50));
            EditorGUILayout.Space(20);
            GUILayout.Label("Player Background Color:", GUILayout.Width(150));
            playerBackgroundColor = EditorGUILayout.ColorField(playerBackgroundColor, GUILayout.Width(150), GUILayout.Height(30));
        }
    }
    //-----Editor Enemies
    private void Enemies()
    {
        showEnemies = EditorGUILayout.Foldout(showEnemies, "Enemies");

        if (showEnemies)
        {
            EditorGUILayout.Space(30);

            toolPrefabs = Resources.LoadAll<GameObject>("Enemies");

            foreach (var enemy in toolPrefabs)
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

            toolPrefabs = Resources.LoadAll<GameObject>("Traps");

            foreach (var trap in toolPrefabs)
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
                GUILayout.Space(10);
                if (GUILayout.Button("Load Level"))
                {
                    LoadLevel(levelToEdit);
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Update Level"))
                {
                    UpdateLevel(levelToEdit);
                }
                GUILayout.Space(10);
                if (GUILayout.Button("Delete Level"))
                {
                    string path = AssetDatabase.GetAssetPath(levelToEdit);
                    bool success = AssetDatabase.DeleteAsset(path);

                    if (success)
                    {
                        Debug.Log("Deleted Level ");
                        levelToEdit = null;
                        GUI.FocusControl(null);
                        ClearLevel();
                    }
                    else
                    {
                        Debug.LogError("Failed to delete level asset at: " + path);
                    }
                }
            }
        }
        else
        {
            GUILayout.Label("An Existing Chapther Or New Chapter Name", EditorStyles.boldLabel);
            chapterName = EditorGUILayout.TextField("Chapter Name:", chapterName);
            GUILayout.Space(10);

            if (GUILayout.Button("Create Or Open New Chapter"))
            {
                if (!string.IsNullOrEmpty(chapterName))
                {
                    string levelsRoot = "Assets/Resources/Levels";
                    string newChapterPath = Path.Combine(levelsRoot, chapterName);

                    if (!AssetDatabase.IsValidFolder(newChapterPath))
                    {
                        AssetDatabase.CreateFolder(levelsRoot, chapterName);

                        EditorPrefs.SetString("MyGame_LastChapterName", chapterName);

                        ResetLevelData();
                        lastLevel = 0;
                        Debug.Log("New chapter folder created: " + newChapterPath);
                    }
                    else
                    {
                        Debug.Log("Chapter folder already exists: " + newChapterPath);
                    }

                    newChapter = true;
                }
                else
                {
                    Debug.LogError("Please Fill Chapter Name Area");
                }
            }

            if (newChapter)
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Save as New Level"))
                {
                    SaveNewLevel();
                }
            }
        }
    }
    private void UpdateLevel(LevelData level)
    {
        GameObject[] allObj = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        startPosValue = 0;
        nextLevelValue = 0;

        foreach (var obj in allObj)
        {
            if (obj.gameObject.tag == "PlayerStartPos")
            {
                startPosValue += 1;
            }
            else if (obj.gameObject.tag == "NextLevel")
            {
                nextLevelValue += 1;
            }
        }
        if (startPosValue != 1 || nextLevelValue != 1)
        {
            Debug.LogError("Scene must contain exactly one Player Start Position Or Player Next Level Position! Current Player Start Pos count: " + startPosValue + " Current Player Next Level Count: " + nextLevelValue);
            return;
        }

        level.Tiles.Clear();

        level.Tools.Clear();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                TileBase tile = tilemap.GetTile(pos);
                level.Tiles.Add(new TileInfo { Position = pos, TileName = tile.name });
            }
        }

        foreach (var obj in allObj)
        {
            if (obj.CompareTag("Trap") || obj.CompareTag("Enemy") || obj.CompareTag("Platform") || obj.CompareTag("PlayerStartPos") || obj.CompareTag("NextLevel"))
            {
                Vector2 trapPos = obj.transform.position;
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);

                if (prefab != null)
                {
                    level.Tools.Add(new ToolInfo
                    {
                        ToolPosition = trapPos,
                        ToolPrefab = prefab
                    });
                }
                else
                {
                    Debug.LogWarning("Trap prefab source not Found: " + obj.name);
                }
            }
        }
        level.Background = new BackgroundInfo();

        if (changeBackground)
        {
            level.Background.BackgroundSprite = backgroundSprite;
            level.Background.PlayerBackgroundColor = playerBackgroundColor;
        }

        EditorUtility.SetDirty(level);
        AssetDatabase.SaveAssets();

        Debug.Log("Level updated.");
    }
    private void LoadLevel(LevelData data)
    {
        if (tilemap.size.x > 1 || tilemap.size.y > 1)
        {
            Debug.LogError("Tilemap field is Not Empty!");
            return;
        }

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
        foreach (var tool in spawnedTools)
        {
            if (tool != null)
                Destroy(tool);
        }

        spawnedTools.Clear();

        foreach (var toolInfo in data.Tools)
        {
            if (toolInfo.ToolPrefab != null)
            {
                GameObject newTrap = (GameObject)PrefabUtility.InstantiatePrefab(toolInfo.ToolPrefab, SceneManager.GetActiveScene());
                newTrap.transform.position = (Vector3)toolInfo.ToolPosition;
                spawnedTools.Add(newTrap);
            }
            else
            {
                Debug.LogWarning("Trap prefab is null in level data.");
            }
        }

        Debug.Log("Level loaded.");

        startPosValue = 0;
        nextLevelValue = 0;

        foreach (var obj in data.Tools)
        {
            string prefabTag = obj.ToolPrefab != null ? obj.ToolPrefab.tag : "Untagged";

            if (prefabTag == "PlayerStartPos")
            {
                startPosValue++;
            }
            else if (prefabTag == "NextLevel")
            {
                nextLevelValue++;
            }
        }

        if (startPosValue != 1 || nextLevelValue != 1)
            Debug.LogError("Scene must contain exactly one Player Start Position Or Player Next Level Position! Current Player Start Pos count: " + startPosValue + " Current Player Next Level Count: " + nextLevelValue);

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
                    if (obj != null && obj.CompareTag("Platform"))
                    {
                        DestroyImmediate(obj);
                    }
                    if (obj != null && obj.CompareTag("PlayerStartPos"))
                    {
                        DestroyImmediate(obj);
                    }
                    if (obj != null && obj.CompareTag("NextLevel"))
                    {
                        DestroyImmediate(obj);
                    }
                }
                Debug.Log("Cleared.");
            }
        }
    }
    //----Editor Reset
    private void ResetData()
    {
        GUILayout.Space(20);

        lastLevel = EditorGUILayout.IntField("Last Level", lastLevel);

        GUILayout.Label("Advanced", EditorStyles.boldLabel);

        if (GUILayout.Button("🔁 Reset Last Level (EditorPrefs)", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog(
                "Reset Last Level Data?",
                "This will reset the saved last level counter. Are you sure?",
                "Yes, reset it",
                "Cancel"))
            {
                ResetLevelData();
                ResetChapterData();
                Debug.Log("Last Level reset.");
            }
        }
    }
    private void ResetLevelData()
    {
        EditorPrefs.DeleteKey("MyGame_LastLevel");
        lastLevel = 0;
    }
    private void ResetChapterData()
    {
        EditorPrefs.DeleteKey("MyGame_LastChapterName");
    }
    //---- Editor Save Level
    private void SaveNewLevel()
    {
        if (tilemap == null || tilemap.size.x < 3 || tilemap.size.y < 3)
        {
            Debug.LogError("Tilemap field is empty or too small!");
            return;
        }

        LevelData newLevel = CreateInstance<LevelData>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                TileBase tile = tilemap.GetTile(pos);
                newLevel.Tiles.Add(new TileInfo
                {
                    Position = pos,
                    TileName = tile.name,
                    TileColor = tilemap.GetColor(pos)
                });
            }
        }

        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj.CompareTag("Trap") || obj.CompareTag("Enemy") || obj.CompareTag("Platform") || obj.CompareTag("PlayerStartPos") || obj.CompareTag("NextLevel"))
            {
                Vector2 trapPos = obj.transform.position;
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);

                if (prefab != null)
                {
                    newLevel.Tools.Add(new ToolInfo
                    {
                        ToolPosition = trapPos,
                        ToolPrefab = prefab
                    });
                }
                else
                    Debug.LogWarning("Trap prefab source not Found: " + obj.name);
            }
        }

        newLevel.Background = new BackgroundInfo();

        newLevel.Background.BackgroundSprite = backgroundSprite;
        newLevel.Background.PlayerBackgroundColor = playerBackgroundColor;

        startPosValue = 0;
        nextLevelValue = 0;

        foreach (var obj in allObjects)
        {
            if (obj.CompareTag("PlayerStartPos"))
            {
                startPosValue += 1;
            }
            else if (obj.CompareTag("NextLevel"))
            {
                nextLevelValue += 1;
            }
        }

        if (startPosValue != 1 || nextLevelValue != 1)
        {
            Debug.LogError("Scene must contain exactly one Player Start Position Or Player Next Level Position! Current Player Start Pos count: " + startPosValue + " Current Player Next Level Count: " + nextLevelValue);
            return;
        }

        lastLevel += 1;
        EditorPrefs.SetInt("MyGame_LastLevel", lastLevel);

        string folderPath = "Assets/Resources/Levels";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Levels");
        }

        string path = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/Level_" + lastLevel + ".asset");
        AssetDatabase.CreateAsset(newLevel, path);
        AssetDatabase.SaveAssets();
    }
}