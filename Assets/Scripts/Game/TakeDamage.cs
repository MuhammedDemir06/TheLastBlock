using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    [Header("Damage")]
    [Range(1, 3)]
    [SerializeField] private int damage = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision!=null)
        {
            var newDamage = collision.GetComponent<IDamageable>();
            if (newDamage != null)
                newDamage.Damage(damage);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision!=null)
        {
            var newDamage = collision.gameObject.GetComponent<IDamageable>();
            if (newDamage != null)
                newDamage.Damage(damage);
        }
    }
}
