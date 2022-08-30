using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private Animator anim;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;

    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        Animate();

        // Wall Jump Logic 
        if (wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(
                horizontalInput * speed, // Horizontal Speed
                body.velocity.y          // Vertical Speed
                );

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = 2.5f;
            }

            if (Input.GetKeyDown(KeyCode.Space))
                Jump();
        } 
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(
                body.velocity.x, // Horizontal Speed
                jumpPower        // Vertical Speed
                );
            anim.SetTrigger("Jumping");
        }
        else if (onWall() && !isGrounded())
        {
            if (horizontalInput > -0.05f && horizontalInput < 0.05f)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 6, 6);
            }
            wallJumpCooldown = 0;
        }
    }

    private void Animate()
    {
        // Flip Player based on Horizontal Movement
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Set Animator Parameters
        anim.SetBool("Running", horizontalInput != 0);
        anim.SetBool("Grounded", isGrounded());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            Vector2.down,
            0.1f,
            groundLayer
            );
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            new Vector2(transform.localScale.x, 0),
            0.1f,
            wallLayer
            );
        return raycastHit.collider != null;
    }
}
