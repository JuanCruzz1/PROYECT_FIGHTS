using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private AttackController attackController = null;
    [SerializeField] private SpecialPowerController specialPowerController = null;

    [Header("Keys")]
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode jumpKey = KeyCode.W;
    [SerializeField] private KeyCode punchKey = KeyCode.J;
    [SerializeField] private KeyCode kickKey = KeyCode.K;
    [SerializeField] private KeyCode specialKey = KeyCode.L;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        if (attackController == null)
        {
            attackController = GetComponent<AttackController>();
        }

        if (specialPowerController == null)
        {
            specialPowerController = GetComponent<SpecialPowerController>();
        }
    }

    private void Update()
    {
        float horizontal = 0f;

        if (Input.GetKey(leftKey))
        {
            horizontal -= 1f;
        }

        if (Input.GetKey(rightKey))
        {
            horizontal += 1f;
        }

        if (playerController != null)
        {
            playerController.SetMoveInput(horizontal);

            if (Input.GetKeyDown(jumpKey))
            {
                playerController.RequestJump();
            }
        }

        if (attackController != null)
        {
            if (Input.GetKeyDown(punchKey))
            {
                attackController.TryPunch();
            }

            if (Input.GetKeyDown(kickKey))
            {
                attackController.TryKick();
            }
        }

        if (specialPowerController != null && Input.GetKeyDown(specialKey))
        {
            specialPowerController.TryUseSpecial();
        }
    }
}
