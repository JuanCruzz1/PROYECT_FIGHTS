using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;

    private void Awake()
    {
        if (healthSystem == null)
        {
            healthSystem = GetComponentInParent<HealthSystem>();
        }
    }

    public void SetHealthSystem(HealthSystem targetHealthSystem)
    {
        healthSystem = targetHealthSystem;
    }

    public void ReceiveDamage(int damage)
    {
        if (healthSystem == null)
        {
            Debug.LogWarning($"{nameof(DamageReceiver)} on {name}: HealthSystem reference is missing.");
            return;
        }

        healthSystem.TakeDamage(damage);
    }
}
