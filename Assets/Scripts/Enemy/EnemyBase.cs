using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [Range(1, 10)]
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Move()
    {
        Debug.Log("Walking");
    }
    protected virtual void Update()
    {
        Move();
    }
}