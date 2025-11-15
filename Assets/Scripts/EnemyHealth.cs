using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isKnockedBack = false;

    public GameObject deathEffectPrefab;

    void Awake()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"[EnemyHealth] {gameObject.name} took {damage} damage. HP: {currentHealth}/{maxHealth}");
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 0.1f);
    }

    System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = originalColor;
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (rb == null || isKnockedBack) return;

        StartCoroutine(KnockbackCoroutine(direction));
    }

    System.Collections.IEnumerator KnockbackCoroutine(Vector2 direction)
    {
        isKnockedBack = true;
        var enemyBehavior = GetComponent<EnemyBehaviour>();
        if (enemyBehavior != null)
        {
            enemyBehavior.enabled = false;
        }
        rb.linearVelocity = direction.normalized * knockbackForce;
        yield return new WaitForSeconds(knockbackDuration);
        if (enemyBehavior != null)
        {
            enemyBehavior.enabled = true;
        }

        isKnockedBack = false;
    }
}