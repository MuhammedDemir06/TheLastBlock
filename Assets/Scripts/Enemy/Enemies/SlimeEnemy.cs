using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [Range(-5,5)]
    [SerializeField] private float checkDistanceWall = 2f;
    [Range(1, 10)]
    [SerializeField] private float checkDistanceGround = 2f;
    private bool movingRight = true;

    protected override void Update()
    {
        base.Update();

        transform.Translate(Vector2.right * speed * Time.deltaTime * (movingRight ? 1 : -1));

        Vector2 groundDir = new Vector2(movingRight ? 1 : -1, -1).normalized;
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, groundDir, checkDistanceGround, groundLayer);

        Vector2 wallDir = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, wallDir, checkDistanceWall, wallLayer);

        if (!groundHit.collider || wallHit.collider)
        {
            Flip();
        }
    }
    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -enemyDirSize;
        transform.localScale = scale;
    }
}