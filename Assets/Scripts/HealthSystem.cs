using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthBar();
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        int previousHealth = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); 

        int actualHealed = currentHealth - previousHealth;

        UpdateHealthBar();
        Debug.Log(gameObject.name + " healed for " + actualHealed + " HP. Current health: " + currentHealth + "/" + maxHealth);
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
                float t = (healthPercent - 0.5f) * 2;
                fillImage.color = Color.Lerp(midHealthColor, maxHealthColor, t);
            }
            else
            {
                float t = healthPercent * 2;
                fillImage.color = Color.Lerp(lowHealthColor, midHealthColor, t);
            }

            fillImage.enabled = healthPercent > 0;
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has been defeated.");
        Destroy(gameObject);
        SceneManager.LoadScene(2);

    }
    public bool IsFullHealth()
    {
        return currentHealth >= maxHealth;
    }
}