using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;

    public float currentHealth;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        Debug.Log("Player Health: " + currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log("Player получил урон: " + damage +
                  " | HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("PLAYER DEFEATED!");
    }
}
