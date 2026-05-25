using UnityEngine;

public class EnemyTestAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 3f;

    private float horizontalInput;
    private bool isJumping = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        horizontalInput = 0f;
        isJumping = false;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsJumping", false);

        animator.ResetTrigger("Punch");
        animator.ResetTrigger("Special");
        animator.ResetTrigger("Hit");
        animator.ResetTrigger("KO");
        animator.ResetTrigger("Win");
    }

    private void Update()
    {
        ReadMovementInput();

        HandleWalkAnimation();
        HandleJump();
        HandlePunch();
        HandleSpecial();
        HandleHit();
        HandleKO();
        HandleWin();
        HandleFlip();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void ReadMovementInput()
    {
        horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A))
            horizontalInput = -1f;

        if (Input.GetKey(KeyCode.D))
            horizontalInput = 1f;
    }

    private void HandleMovement()
    {
        if (rb == null)
            return;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = horizontalInput * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void HandleWalkAnimation()
    {
        bool isWalking = horizontalInput != 0f && !isJumping;
        animator.SetBool("IsWalking", isWalking);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isJumping)
        {
            isJumping = true;

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsJumping", true);

            Invoke(nameof(EndJump), 0.6f);
        }
    }

    private void EndJump()
    {
        isJumping = false;
        animator.SetBool("IsJumping", false);
    }

    private void HandlePunch()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.ResetTrigger("Special");
            animator.SetTrigger("Punch");
        }
    }

    private void HandleSpecial()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            animator.ResetTrigger("Punch");
            animator.SetTrigger("Special");
        }
    }

    private void HandleHit()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.ResetTrigger("Punch");
            animator.ResetTrigger("Special");
            animator.SetTrigger("Hit");
        }
    }

    private void HandleKO()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            animator.ResetTrigger("Punch");
            animator.ResetTrigger("Special");
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Win");
            animator.SetTrigger("KO");
        }
    }

    private void HandleWin()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.ResetTrigger("Punch");
            animator.ResetTrigger("Special");
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("KO");
            animator.SetTrigger("Win");
        }
    }

    private void HandleFlip()
    {
        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}