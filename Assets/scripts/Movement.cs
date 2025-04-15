using UnityEngine;

public class Movement : MonoBehaviour
{
    public static Movement instance { get; private set; }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public float velocidad;
    private Rigidbody2D rb;
    private Animator animator;
    private bool getDamage = false;
    private bool isDeath = false;
    private bool animationAttack = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDeath) return;
        Attack();
        Mover();
    }

    void Mover()
    {
        if (getDamage) return;
        float inputX = animationAttack ? 0 : Input.GetAxisRaw("Horizontal") ;
        float inputY = animationAttack ? 0 : Input.GetAxisRaw("Vertical");
        Orientation(inputX, inputY);
        if (inputX != 0 || inputY != 0)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }

        Vector2 input = new Vector2(inputX, inputY).normalized;

        rb.linearVelocity = input * velocidad;
    }

    void Attack()
    {
         if (Input.GetKeyDown(KeyCode.X) && !animationAttack)
        {
            animator.SetBool("BlAttack", true);
            animationAttack = true;
        }
    }

    void EndAttack()
    {
        animationAttack = false;
        animator.SetBool("BlAttack", false);
    }

    void Orientation(float inputMoveX, float inputMoveY)
    {
        if (inputMoveX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (inputMoveX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void DamageFalse(){
        getDamage = false;
        animator.SetBool("BlDamage", false);
    }

    public void GetDamage(Vector2 direction)
    {
        if (getDamage) return;
        animator.SetBool("BlDamage", true);
        getDamage = true;
        Vector2 force = new Vector2(direction.x, direction.y).normalized * 5f;
        rb.AddForce(force * 5, ForceMode2D.Impulse);
    }

    public void Death()
    {
        animator.SetBool("BlDeath", true);
        isDeath = true;
    }
}
