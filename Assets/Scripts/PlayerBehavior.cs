using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastMove;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        // Update animator parameters
        if (moveInput.magnitude > 0)
        {
            // Pakai absolute value untuk MoveX biar cuma perlu animasi kiri
            animator.SetFloat("MoveX", Mathf.Abs(moveInput.x));
            animator.SetFloat("MoveY", moveInput.y);

            // Simpan arah terakhir
            lastMove = moveInput;

            // Handle flip untuk kiri/kanan
            if (moveInput.x > 0)
            {
                spriteRenderer.flipX = true; // Kanan (mirror)
            }
            else if (moveInput.x < 0)
            {
                spriteRenderer.flipX = false; // Kiri (normal)
            }
        }
        else
        {
            // Saat idle, set semua ke 0
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }
}