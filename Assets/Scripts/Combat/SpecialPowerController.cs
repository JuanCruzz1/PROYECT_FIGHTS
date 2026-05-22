using UnityEngine;

public struct SpecialPowerContext
{
    public readonly Transform Owner;
    public readonly Transform SpawnPoint;
    public readonly PlayerController PlayerController;
    public readonly FighterTeam OwnerTeam;
    public readonly int Damage;

    public SpecialPowerContext(
        Transform owner,
        Transform spawnPoint,
        PlayerController playerController,
        FighterTeam ownerTeam,
        int damage
    )
    {
        Owner = owner;
        SpawnPoint = spawnPoint != null ? spawnPoint : owner;
        PlayerController = playerController;
        OwnerTeam = ownerTeam;
        Damage = damage;
    }

    public int FacingDirection => PlayerController != null ? PlayerController.FacingDirection : Owner != null && Owner.localScale.x < 0f ? -1 : 1;
}

public abstract class SpecialPowerBase : MonoBehaviour
{
    public abstract bool TryActivate(SpecialPowerContext context);
}

public class SpecialPowerController : MonoBehaviour
{
    [Header("Owner")]
    [SerializeField] private FighterCharacter fighterCharacter = FighterCharacter.FerchoPoker;
    [SerializeField] private FighterTeam ownerTeam = FighterTeam.Player;
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private Transform spawnPoint = null;

    [Header("Stats")]
    [SerializeField] private int specialDamage = 20;
    [SerializeField] private float cooldown = 5f;

    [Header("Powers")]
    [SerializeField] private SpecialPowerBase cheersBoomPower = null;
    [SerializeField] private SpecialPowerBase phantomRushPower = null;
    [SerializeField] private SpecialPowerBase chainWhipPower = null;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private float nextAvailableTime;
    private SpecialPowerBase activePower;

    public FighterCharacter FighterCharacter => fighterCharacter;
    public bool IsReady => Time.time >= nextAvailableTime;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }

        FindPowerReferencesIfNeeded();
        SelectPower();
    }

    public void Configure(FighterTeam team, CharacterData characterData)
    {
        ownerTeam = team;

        if (characterData != null)
        {
            fighterCharacter = characterData.fighterCharacter;
            specialDamage = characterData.specialDamage;
            cooldown = characterData.specialCooldown;
        }

        SelectPower();
    }

    public void SetFighterCharacter(FighterCharacter newFighterCharacter)
    {
        fighterCharacter = newFighterCharacter;
        SelectPower();
    }

    public bool TryUseSpecial()
    {
        if (activePower == null)
        {
            Debug.LogWarning($"{nameof(SpecialPowerController)} on {name}: No active special power for {fighterCharacter}.");
            return false;
        }

        if (!IsReady)
        {
            if (debugLogs)
            {
                Debug.Log($"{nameof(SpecialPowerController)} on {name}: Special cooldown {nextAvailableTime - Time.time:0.00}s.");
            }

            return false;
        }

        SpecialPowerContext context = new SpecialPowerContext(
            transform,
            spawnPoint,
            playerController,
            ownerTeam,
            specialDamage
        );

        if (!activePower.TryActivate(context))
        {
            return false;
        }

        nextAvailableTime = Time.time + cooldown;

        if (debugLogs)
        {
            Debug.Log($"{nameof(SpecialPowerController)} on {name}: Used {fighterCharacter} special.");
        }

        return true;
    }

    private void FindPowerReferencesIfNeeded()
    {
        SpecialPowerBase[] powers = GetComponents<SpecialPowerBase>();

        for (int i = 0; i < powers.Length; i++)
        {
            string powerTypeName = powers[i].GetType().Name;

            if (powerTypeName == "CheersBoomPower" && cheersBoomPower == null)
            {
                cheersBoomPower = powers[i];
            }
            else if (powerTypeName == "PhantomRushPower" && phantomRushPower == null)
            {
                phantomRushPower = powers[i];
            }
            else if (powerTypeName == "ChainWhipPower" && chainWhipPower == null)
            {
                chainWhipPower = powers[i];
            }
        }
    }

    private void SelectPower()
    {
        switch (fighterCharacter)
        {
            case FighterCharacter.FerchoPoker:
                activePower = cheersBoomPower;
                break;
            case FighterCharacter.CruzRusher:
                activePower = phantomRushPower;
                break;
            case FighterCharacter.KingBling:
                activePower = chainWhipPower;
                break;
        }
    }
}
