using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class LevelSaverEditor : EditorWindow
{
    private Tilemap tilemap;
    private bool showResetConfirm = false;

    [MenuItem("Tools/Level Saver")]
    public static void ShowWindow()
    {
        GetWindow<LevelSaverEditor>("Level Saver");
    }

    private void OnGUI()
    {
        GUILayout.Space(30);
        GUILayout.Label("Tilemap Saver  ||  Attention:Drag Tilemap In Scene Here", EditorStyles.boldLabel);
        tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);

        GUILayout.Space(30);

        GUILayout.Label("Save", EditorStyles.boldLabel);
        if (GUILayout.Button("Save Level"))
        {
            SaveLevel();
        }

        GUILayout.Space(35);
        GUILayout.Label("Reset", EditorStyles.boldLabel);

        if (!showResetConfirm)
        {
            if (GUILayout.Button("Reset Tiles"))
            {
                showResetConfirm = true;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Are you sure you want to reset all tiles? This action cannot be undone.", MessageType.Warning);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Yes"))
            {
                ResetTiles();
                showResetConfirm = false;
            }
            if (GUILayout.Button("No"))
            {
                showResetConfirm = false;
            }
            GUILayout.EndHorizontal();
        }
    }

    private void SaveLevel()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap field is empty!");
            return;
        }

        if (tilemap == null)
        {
            Debug.LogError("Tilemap area is empty!");
            return;
        }

        LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Tile tile = tilemap.GetTile(pos) as Tile;
                if (tile != null)
                {
                    newLevel.tiles.Add(new TileInfo
                    {
                        Position = pos,
                        TileName = tile.name,
                        TileColor = tile.color
                    });
                }
            }
        }

        string folderPath = "Assets/Resources/Levels/";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Levels");
        }

        string path = AssetDatabase.GenerateUniqueAssetPath(folderPath + "TestLevel.asset");
        AssetDatabase.CreateAsset(newLevel, path);
        AssetDatabase.SaveAssets();
    }

    private void ResetTiles()
    {
        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap field is empty, nothing to reset.");
            return;
        }

        tilemap.ClearAllTiles();

        Debug.Log("All tiles have been reset.");
    }
}
