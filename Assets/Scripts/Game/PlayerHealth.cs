using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    public static System.Action<int,int,bool> DamageControl;

    [Header("Player Health")]
    [Range(0, 3)]
    [SerializeField] private int playerHealthAmount = 3;
    public void Damage(int damage)
    {
        if (PlayerUIManager.Instance.GamePaused)
            return;

        playerHealthAmount -= damage;

        if (playerHealthAmount <= 0)
            playerHealthAmount = 0;

        DamageControl?.Invoke(playerHealthAmount, damage, true);
     //   PlayerUIManager.Instance.DamageControl(playerHealthAmount, damage, true);
    }
}
