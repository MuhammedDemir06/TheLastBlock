using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    [Header("Player Health")]
    [Range(1, 3)]
    [SerializeField] private int playerHealthAmount = 3;
    public void Damage(int damage)
    {
        playerHealthAmount -= damage;
        if (playerHealthAmount <= 0)
        {
            Debug.Log("Player Died!");
        }
        else
            Debug.Log("Player Damaged");
    }
}
