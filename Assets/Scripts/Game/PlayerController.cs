using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour
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

    private void OnEnable()
    {
        GameInputManager.PlayerInputX += Move;
        GameInputManager.PlayerJump += Jump;
    }
    private void OnDisable()
    {
        GameInputManager.PlayerInputX -= Move;
        GameInputManager.PlayerJump -= Jump;
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
}
