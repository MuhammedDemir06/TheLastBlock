using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    [Header("Player Health")]
    [Range(0, 3)]
    [SerializeField] private int playerHealthAmount = 3;
    public void Damage(int damage)
    {
        playerHealthAmount -= damage;

        PlayerUIManager.Instance.DamageControl(playerHealthAmount,damage, true);
    }
}
