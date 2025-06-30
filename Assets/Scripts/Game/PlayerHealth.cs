using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    public void Damage()
    {
        Debug.Log("Player Died!");
    }
}
