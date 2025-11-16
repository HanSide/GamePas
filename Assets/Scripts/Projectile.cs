using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public int damage = 1;
    public float lifetime = 3f;
    public LayerMask hitLayers; 

    public GameObject hitEffectPrefab;
    public TrailRenderer trailRenderer;

    private Rigidbody2D rb;
    private Vector2 direction;
    private float speed;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;

        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if (other.CompareTag("Enemy"))
        {
            hasHit = true;

            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"[Projectile] Hit enemy: {other.name}");
            }
            SpawnHitEffect(other.transform.position);
            Destroy(gameObject);
        }

        else if (other.CompareTag("Wall") || other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            hasHit = true;

            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }

    void SpawnHitEffect(Vector3 position)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 0.8f);
        }
    }
}