using UnityEngine;

public class NextLevel : MonoBehaviour
{
    private void LevelFinish()
    {
        if (PlayerUIManager.Instance != null)
        {
            PlayerUIManager.Instance.NextLevel();
            Debug.Log("Level Finished");
        }
        else
            Debug.LogWarning("PlayerUIManager Instance Not Found!");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            LevelFinish();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            LevelFinish();
        }
    }
}
