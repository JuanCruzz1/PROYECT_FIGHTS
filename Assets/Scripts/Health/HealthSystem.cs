using UnityEngine;


public class HealthSystem : MonoBehaviour
{
    /*
    public int maxHealth;
    public int currentHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    public void Initialize(int health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
    */
}