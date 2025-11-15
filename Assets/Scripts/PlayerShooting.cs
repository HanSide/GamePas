using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint; 
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f;
    private InputAction shootAction;
    private float nextFireTime = 0f;
    private Vector2 lastMoveDirection = Vector2.down;
    private Rigidbody2D rb;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        shootAction = new InputAction("Shoot", binding: "<Keyboard>/space");
        shootAction.Enable();
    }

    void Update()
    {
        UpdateFacingDirection();
        if (shootAction.triggered && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void UpdateFacingDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            lastMoveDirection = new Vector2(horizontal, vertical).normalized;
        }
        else if (animator != null)
        {
            float moveX = animator.GetFloat("MoveX");
            float moveY = animator.GetFloat("MoveY");

            if (moveX != 0 || moveY != 0)
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    lastMoveDirection = new Vector2(sr.flipX ? 1 : -1, moveY).normalized;
                }
                else
                {
                    lastMoveDirection = new Vector2(moveX, moveY).normalized;
                }
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        Vector2 shootDirection = -lastMoveDirection; 
                                                     

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.Initialize(shootDirection, projectileSpeed);
        }
        else
        {
            Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                projRb.linearVelocity = shootDirection * projectileSpeed;
            }
        }
    }

    void OnDisable()
    {
        shootAction.Disable();
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Vector3 start = transform.position;
            Vector3 end = start + (Vector3)(lastMoveDirection * 2f);
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(end, 0.1f);
        }
    }
}