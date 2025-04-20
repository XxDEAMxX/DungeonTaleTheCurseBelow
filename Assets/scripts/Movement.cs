using UnityEngine;

public class Movement : MonoBehaviour
{
    public static Movement instance { get; private set; }
    public float velocidad;
    private Rigidbody2D rb;
    private Animator animator;
    private bool getDamage = false;
    private bool isDeath = false;
    private bool animationAttack = false;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float fireDelay;
    private float lastFire;
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
        preShoot();
    }

    void preShoot()
    {
        float shootHor = 0f;
        float shootVer = 0f;

        if (Input.GetKey(KeyCode.RightArrow)) shootHor = 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) shootHor = -1f;
        if (Input.GetKey(KeyCode.UpArrow)) shootVer = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) shootVer = -1f;

        if ((shootHor != 0 || shootVer != 0) && Time.time > lastFire + fireDelay)
        {
            shoot(shootHor, shootVer);
            lastFire = Time.time;
        }
    }

    void shoot(float x, float y)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        Vector2 direction = new Vector2(x, y).normalized;
        rb.linearVelocity = direction * bulletSpeed;
    }

    void Mover()
    {
        if (getDamage) return;

        float inputX = 0f;
        float inputY = 0f;

        if (!animationAttack)
        {
            if (Input.GetKey(KeyCode.W)) inputY = 1f;
            if (Input.GetKey(KeyCode.S)) inputY = -1f;
            if (Input.GetKey(KeyCode.D)) inputX = 1f;
            if (Input.GetKey(KeyCode.A)) inputX = -1f;
        }

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
        Vector2 force = direction * 5f;;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void Death()
    {
        animator.SetBool("BlDeath", true);
        isDeath = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
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
}
