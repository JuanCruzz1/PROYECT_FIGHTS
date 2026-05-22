using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpVelocity = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer = 0;
    [SerializeField] private Vector2 groundCheckFallbackOffset = new Vector2(0f, -0.55f);

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private Rigidbody2D body;
    private float moveInput;
    private bool jumpQueued;
    private bool facingRight = true;
    private Vector2 pendingExternalMove;
    private Vector2 fallbackVelocity;

    public int FacingDirection => facingRight ? 1 : -1;
    public bool IsGrounded { get; private set; }
    public Vector2 Velocity => body != null ? body.linearVelocity : fallbackVelocity;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        if (body != null)
        {
            body.gravityScale = Mathf.Max(body.gravityScale, 1f);
        }
        else
        {
            Debug.LogWarning($"{nameof(PlayerController)} on {name}: Rigidbody2D is missing. Jump requires Rigidbody2D and will be ignored.");
        }

        if (groundCheck == null)
        {
            Debug.LogWarning($"{nameof(PlayerController)} on {name}: groundCheck is not assigned. Using fallback position under the player.");
        }

        if (groundLayer.value == 0)
        {
            Debug.LogWarning($"{nameof(PlayerController)} on {name}: groundLayer is not configured. Assign only the Ground layer to avoid detecting the player or opponents.");
        }
    }

    private void FixedUpdate()
    {
        ApplyPhysicsMovement(Time.fixedDeltaTime);
    }

    public void SetMoveInput(float input)
    {
        moveInput = Mathf.Clamp(input, -1f, 1f);

        if (moveInput > 0.01f)
        {
            facingRight = true;
        }
        else if (moveInput < -0.01f)
        {
            facingRight = false;
        }
    }

    public void RequestJump()
    {
        jumpQueued = true;
    }

    public void ConfigureMovement(float newMoveSpeed, float newJumpVelocity)
    {
        moveSpeed = Mathf.Max(0f, newMoveSpeed);
        jumpVelocity = Mathf.Max(0f, newJumpVelocity);
    }

    public void MoveBy(Vector2 delta)
    {
        pendingExternalMove += delta;
    }

    private void ApplyPhysicsMovement(float deltaTime)
    {
        IsGrounded = CheckGrounded();

        if (body != null)
        {
            Vector2 currentVelocity = body.linearVelocity;
            currentVelocity.x = moveInput * moveSpeed;
            body.linearVelocity = currentVelocity;

            if (pendingExternalMove.sqrMagnitude > 0f)
            {
                body.MovePosition(body.position + pendingExternalMove);
                pendingExternalMove = Vector2.zero;
            }

            if (jumpQueued && IsGrounded)
            {
                body.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);

                if (debugLogs)
                {
                    Debug.Log($"{nameof(PlayerController)} on {name}: Physics jump.");
                }
            }
            else if (jumpQueued && debugLogs)
            {
                Debug.Log($"{nameof(PlayerController)} on {name}: Jump requested but player is not grounded.");
            }

            jumpQueued = false;
            return;
        }

        if (jumpQueued && debugLogs)
        {
            Debug.LogWarning($"{nameof(PlayerController)} on {name}: Jump ignored because Rigidbody2D is missing.");
        }

        jumpQueued = false;
        fallbackVelocity = Vector2.right * moveInput * moveSpeed;
        transform.position = (Vector2)transform.position + (fallbackVelocity * deltaTime) + pendingExternalMove;
        pendingExternalMove = Vector2.zero;
    }

    private void OnDisable()
    {
        moveInput = 0f;
        jumpQueued = false;
        pendingExternalMove = Vector2.zero;

        if (body != null)
        {
            Vector2 currentVelocity = body.linearVelocity;
            currentVelocity.x = 0f;
            body.linearVelocity = currentVelocity;
        }
    }

    private bool CheckGrounded()
    {
        Vector2 origin = GetGroundCheckPosition();
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, groundCheckRadius, groundLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] != null && hits[i].transform.root != transform.root)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 origin = GetGroundCheckPosition();
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
    }

    private Vector2 GetGroundCheckPosition()
    {
        return groundCheck != null ? groundCheck.position : (Vector2)transform.position + groundCheckFallbackOffset;
    }
}
