using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject sceneTransitionPrefab;
    private void Awake()
    {
        if (transform.parent != null)
            transform.parent = null;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        if(SceneTransitionManager.Instance==null)
        {
            Instantiate(sceneTransitionPrefab);
        }
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Game/Delete Game Data")]
    public static void DeleteGameData()
    {
        PlayerDataManager.DeleteData();
    }
    [UnityEditor.MenuItem("Tools/Game/How To Create Level ?")]
    public static void OperTutorialURL()
    {
        Application.OpenURL("");
    }
    [UnityEditor.MenuItem("Tools/Game/The Last Block Gameplay ?")]
    public static void OperGameplayURL()
    {
        Application.OpenURL("");
    }
#endif
}
