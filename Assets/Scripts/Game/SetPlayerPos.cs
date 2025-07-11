using UnityEngine;

public class SetPlayerPos : MonoBehaviour
{
    [SerializeField] private GameObject activeEffect;
    private bool isActive;

    private void Start()
    {
        activeEffect.SetActive(false);
        isActive = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var newPos = collision.GetComponent<SetablePlayerPos>();

        if (collision.gameObject.tag == "Player" && newPos != null && !isActive)
        {
            newPos.PlayerSetPos(transform.position.x, transform.position.y);

            activeEffect.SetActive(true);
            isActive = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var newPos = collision.gameObject.GetComponent<SetablePlayerPos>();

        if (collision.gameObject.tag == "Player" && newPos != null && !isActive)
        {
            newPos.PlayerSetPos(transform.position.x, transform.position.y);

            activeEffect.SetActive(true);
            isActive = true;
        }
    }
}
