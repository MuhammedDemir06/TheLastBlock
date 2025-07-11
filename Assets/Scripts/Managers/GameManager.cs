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
    [UnityEditor.MenuItem("Tools/Game/How To Create Level And The Last Block Gameplay ?")]
    public static void OperTutorialURL()
    {
        Application.OpenURL("https://www.youtube.com/watch?v=IIXfBZR0km8&t=103s");
    }
#endif
}
