using System;
using UnityEngine;


public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool debugLogs = true;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        if (currentHealth <= 0)
        {
            Initialize(maxHealth);
        }
    }

    public void Initialize(int health)
    {
        maxHealth = Mathf.Max(1, health);
        currentHealth = maxHealth;
        IsDead = false;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (debugLogs)
        {
            Debug.Log($"{nameof(HealthSystem)} on {name}: took {damage}. Health {currentHealth}/{maxHealth}.");
        }

        if (currentHealth <= 0)
        {
            IsDead = true;

            if (debugLogs)
            {
                Debug.Log($"{nameof(HealthSystem)} on {name}: death.");
            }

            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
