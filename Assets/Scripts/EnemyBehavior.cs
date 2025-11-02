using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr; // Tambahin ini
    public int damage = 10;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); // Ambil SpriteRenderer
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            // Flip sprite tergantung arah gerak
            if (direction.x > 0)
                sr.flipX = false; // Ngadep kanan
            else if (direction.x < 0)
                sr.flipX = true;  // Ngadep kiri
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
