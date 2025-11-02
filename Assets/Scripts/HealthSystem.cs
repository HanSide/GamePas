using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color maxHealthColor = Color.green;
    [SerializeField] private Color midHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;

            if (fillImage == null)
            {
                fillImage = healthBar.fillRect.GetComponent<Image>();

            }

            UpdateHealthBarColor();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        UpdateHealthBar();
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
            UpdateHealthBarColor();
        }
    }

    private void UpdateHealthBarColor()
    {
        if (fillImage != null)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            if (healthPercent > 0.5f)
            {
                // Between mid and max health
                float t = (healthPercent - 0.5f) * 2; // Normalize to 0-1
                fillImage.color = Color.Lerp(midHealthColor, maxHealthColor, t);
            }
            else
            {
                // Between low and mid health
                float t = healthPercent * 2; // Normalize to 0-1
                fillImage.color = Color.Lerp(lowHealthColor, midHealthColor, t);
            }

            fillImage.enabled = healthPercent > 0; // Hide fill if health is zero
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has been defeated.");
        Destroy(gameObject); // The simplest way to handle 
    }
}