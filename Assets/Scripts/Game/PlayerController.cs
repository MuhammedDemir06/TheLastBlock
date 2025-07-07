using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour,SetablePlayerPos
{
    [Header("Movement Settings")]
    [Range(.5f,10)][SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [Header("Player Size")]
    [SerializeField] private float playerSize = .5f;
    private Rigidbody2D rb;

    //Last Pos
    [HideInInspector] public Vector2 StartPos;
    private Vector2 lastPos;
    private void OnEnable()
    {
        GameInputManager.PlayerInputX += Move;
        GameInputManager.PlayerJump += Jump;
        PlayerHealth.DamageControl += KillEffect;
    }
    private void OnDisable()
    {
        GameInputManager.PlayerInputX -= Move;
        GameInputManager.PlayerJump -= Jump;
        PlayerHealth.DamageControl -= KillEffect;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Move(float input)
    {
        rb.linearVelocity = new Vector2(input * moveSpeed, rb.linearVelocity.y);

        SetDirection(input);
    }
    private void SetDirection(float inputX)
    {
        var newDir = transform.localScale;

        if (inputX > 0)
            newDir.x = playerSize;
        else if (inputX < 0)
            newDir.x = -playerSize;

        transform.localScale = newDir;
    }
    private void Jump()
    {
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    //public void KillEffect(int healthAmount,int damage,bool isDamage)
    //{
    //    if(healthAmount==0)
    //    {
    //        gameObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
    //        SpriteRenderer sr = GetComponent<SpriteRenderer>();
    //        if (sr != null)
    //            sr.DOFade(0f, 0.5f);
    //    }
    //    else
    //    {
    //        playerEffect.emitting = false;
    //        rb.simulated = false;

    //        Invoke(nameof(PlayerEffect), 1f);

    //        if (lastPos.x != 0 && lastPos.y != 0)
    //        {
    //            transform.position = lastPos;
    //        }
    //        else
    //        {
    //            transform.localPosition = StartPos;
    //        }
    //    }
    //}

    public void KillEffect(int healthAmount, int damage, bool isDamage)
    {
        if (healthAmount == 0)
        {
            gameObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.DOFade(0f, 0.5f);
        }
        else
        {
           // playerEffect.emitting = false;
            rb.simulated = false;

           // Invoke(nameof(PlayerEffect), 1f);

            Vector2 targetPos = (lastPos.x != 0 && lastPos.y != 0) ? lastPos : StartPos;

            // Rigidbody physics bozulmasın diye önce simülasyonu kapat
            transform.DOMove(targetPos, 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    // Animasyon tamamlanınca Rigidbody tekrar aktif
                    rb.simulated = true;
                });
        }
    }
    public void PlayerSetPos(float x, float y)
    {
        lastPos = new Vector2(x, y);
    }
}
