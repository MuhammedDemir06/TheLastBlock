using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [Space(5)]
    [Header("Movement Settings")]
    [Range(1,10)]
    [SerializeField] protected float speed = 2f;
    [Header("Enemy Direction")]
    [Range(.5f,10)]
    [SerializeField] protected float enemyDirSize = 5;

    protected virtual void Start()
    {
        //Start
    }
    protected virtual void Update()
    {
        //Update
    }
}