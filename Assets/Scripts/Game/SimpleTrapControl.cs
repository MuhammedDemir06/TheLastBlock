using UnityEngine;

public class SimpleTrapControl : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision!=null)
        {
            var newDamage = collision.GetComponent<IDamageable>();
            if (newDamage != null)
                newDamage.Damage();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision!=null)
        {
            var newDamage = collision.gameObject.GetComponent<IDamageable>();
            if (newDamage != null)
                newDamage.Damage();
        }
    }
}
