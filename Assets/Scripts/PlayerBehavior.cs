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

        Debug.Log("Player Start - Tag: " + gameObject.tag);
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        if (moveInput.magnitude > 0)
        {

            animator.SetFloat("MoveX", Mathf.Abs(moveInput.x));
            animator.SetFloat("MoveY", moveInput.y);

  
            lastMove = moveInput;

 
            if (moveInput.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (moveInput.x < 0)
            {
                spriteRenderer.flipX = false; 
            }
        }
        else
        {

            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player Trigger dengan: " + collision.name + " | Tag: " + collision.tag);
    }
}