using UnityEngine;

public class King_Animator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isJumping = false;
    private void Start()
    {
        isJumping = false;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsJumping", false);

        animator.ResetTrigger("Punch");
        animator.ResetTrigger("Kick");
        animator.ResetTrigger("Hit");
        animator.ResetTrigger("Win");
        animator.ResetTrigger("Special");
        animator.ResetTrigger("KO");
    }

    private void Update()
    {
        HandleWalk();
        HandlePunch();
        HandleKick();
        HandleJump();
        HandleFlip();
        HandleHit();
        HandleWin();
        HandleKO();
        HandleSpecial();
    }

    private void HandleWalk()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        bool isWalking = Mathf.Abs(horizontal) > 0.1f;

        if (!isJumping)
        {
            animator.SetBool("IsWalking", isWalking);
        }
    }

    private void HandlePunch()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.ResetTrigger("Kick");
            animator.SetTrigger("Punch");
        }
    }

    private void HandleKick()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.ResetTrigger("Punch");
            animator.SetTrigger("Kick");
        }
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

    private void HandleFlip()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontal < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void HandleHit()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.SetTrigger("Hit");
        }
    }

    private void HandleWin()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetTrigger("Win");
        }
    }
    private void HandleKO()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            animator.SetTrigger("KO");
        }
    }
    private void HandleSpecial()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            animator.ResetTrigger("Punch");
            animator.ResetTrigger("Kick");
            animator.SetTrigger("Special");
        }
    }
}