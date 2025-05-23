using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IsaacController : MonoBehaviour
{
  public static IsaacController instance { get; private set; }

  public float speed;
  private Rigidbody2D rb;
  private Animator animator;
  private Animator animatorBody;
  private Animator animatorHead;
  private Animator animatorHair;
  public GameObject bulletPrefab;
  public GameObject bulletSidaPrefab;
  public GameObject bombPrefab;
  public GameObject body;
  public GameObject head;
  public GameObject hair;
  public GameObject death;
  private bool isDamage = false;
  public bool isDeath = false;
  public float bulletSpeed = 20f; // Velocidad de la bala
  public float fireDelay = 0.2f; // Delay entre disparos
  public float bombDelay = 5f; // Delay entre bombas
  private float lastBoom; // Velocidad de la bomba
  private float lastFire;
  private bool isShooting = false;
  private int numberOfBombs; // Número de bombas que tiene el jugador
  public Transform groundCheck;

  public AudioClip damageSound; // Campo para el sonido de daño
  private AudioSource audioSource; // Componente AudioSource

  public Text collectedText;
  public static int collectedAmount = 0;

  void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    animatorBody = GameObject.FindGameObjectWithTag("Body").GetComponent<Animator>();
    animatorHead = GameObject.FindGameObjectWithTag("Head").GetComponent<Animator>();
    animatorHair = GameObject.FindGameObjectWithTag("Hair").GetComponent<Animator>();
    head = GameObject.FindGameObjectWithTag("Head");
    body = GameObject.FindGameObjectWithTag("Body");
    hair = GameObject.FindGameObjectWithTag("Hair");
    death = GameObject.FindGameObjectWithTag("Death");
    death.SetActive(false);
    audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    if (rb == null)
    {
      Debug.LogError("No se encontró Rigidbody2D en " + gameObject.name);
    }
    if (animatorBody == null)
    {
      Debug.LogError("No se encontró Animator en " + gameObject.name);
    }
  }

  void Update(){
    fireDelay = GameManager.FireRate;
    bulletSpeed = GameManager.BulletSize;
    speed = GameManager.MoveSpeed;
    if (isDeath) return;
    Mover();
    preShoot();
    Bomb();
    // collectedText.text = "Bombs: " + collectedAmount;
  }

  public void SetBombs(int value)
  {
      numberOfBombs = value;
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

  public void Damage(Vector2 position)
  {
      if (isDamage) return;
      animatorHead.SetBool("isDamage", true);
      isDamage = true;
      Vector2 force = position * 5f;;
      rb.AddForce(force, ForceMode2D.Impulse);
      if (damageSound != null && audioSource != null) // Reproducir sonido si existe
      {
          audioSource.PlayOneShot(damageSound);
      }
  }

  public void IsDamageFalse()
  {
      isDamage = false;
      animatorHead.SetBool("isDamage", false);
  }

  public void Death()
  {
      head.SetActive(false);
      hair.SetActive(false);
      body.SetActive(false);
      death.SetActive(true);
      isDeath = true;
      rb.linearVelocity = Vector2.zero;
      rb.bodyType = RigidbodyType2D.Static;
  }

  IEnumerator ResetShootFlag()
  {
      yield return new WaitForSeconds(fireDelay);
      isShooting = false;
  }

  void shoot(float x, float y)
  {
      isShooting = true;
      int ran = Random.Range(0, 2);
      Debug.Log(ran);
      GameObject bull = ran == 0 ? bulletPrefab : bulletSidaPrefab;
      GameObject bullet = Instantiate(bull, transform.position, Quaternion.identity);
      Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
      rb.gravityScale = 0;

      Vector2 direction = new Vector2(x, y).normalized;

      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

      // Activar animaciones
      bool isHorizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
      bool isUpward = direction.y > 0 && Mathf.Abs(direction.y) > Mathf.Abs(direction.x);

      animatorHead.SetBool("isHor", isHorizontal);
      animatorHead.SetBool("isBack", isUpward);
      animatorHair.SetBool("isHor", isHorizontal);
      animatorHair.SetBool("isBack", isUpward);

      // Cambiar dirección de la cabeza si es horizontal
      if (isHorizontal)
      {
          Vector3 headScale = head.transform.localScale;
          headScale.x = direction.x > 0 ? Mathf.Abs(headScale.x) : -Mathf.Abs(headScale.x);
          head.transform.localScale = headScale;
          hair.transform.localScale = headScale;
      }

      rb.linearVelocity = direction * bulletSpeed;
      StartCoroutine(ResetShootFlag());
  }



  

 void Bomb() {
    if (Input.GetKey(KeyCode.E) && numberOfBombs > 0 && Time.time > lastBoom + bombDelay) {
        Vector3 spawnPos = new Vector3(groundCheck.position.x, groundCheck.position.y, 0);
        Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        lastBoom = Time.time;
        numberOfBombs--;
        GameManager.instance.DecreaseBombs();
    }
}

  void Mover(){
    float inputX = 0f;
    float inputY = 0f;
    // Usar exactamente el mismo método de entrada que en Movement
    if (Input.GetKey(KeyCode.W)) inputY = 1f;
    if (Input.GetKey(KeyCode.S)) inputY = -1f;
    if (Input.GetKey(KeyCode.D)) inputX = 1f;
    if (Input.GetKey(KeyCode.A)) inputX = -1f;
    if (!isShooting){
      headAnimtions(inputX, inputY);
    }
    bodyAnimtions(inputX, inputY);
    // Normalizar y aplicar movimiento (usando el mismo método que Movement)
    Vector2 input = new Vector2(inputX, inputY).normalized;

    if (rb != null)
    {
      rb.linearVelocity = input * speed;
    }
  }

  void headAnimtions(float x, float y){
    if (x > 0)
    {
      head.transform.localScale = new Vector3(1, 1, 1);
      hair.transform.localScale = new Vector3(1, 1, 1);
      animatorHead.SetBool("isHor", true);
      animatorHair.SetBool("isHor", true);
      animatorHead.SetBool("isBack", false);
      animatorHair.SetBool("isBack", false);
    }
    else if (x < 0)
    {
      head.transform.localScale = new Vector3(-1, 1, 1);
      hair.transform.localScale = new Vector3(-1, 1, 1);
      animatorHead.SetBool("isHor", true);
      animatorHair.SetBool("isHor", true);
      animatorHead.SetBool("isBack", false);
      animatorHair.SetBool("isBack", false);
    }
      else if (y > 0)
      {
        head.transform.localScale = new Vector3(1, 1, 1);
        hair.transform.localScale = new Vector3(1, 1, 1);
        animatorHead.SetBool("isHor", false);
        animatorHair.SetBool("isHor", false);
        animatorHead.SetBool("isBack", true);
        animatorHair.SetBool("isBack", true);
      }
      else if (y < 0)
      {
        head.transform.localScale = new Vector3(-1, 1, 1);
        hair.transform.localScale = new Vector3(-1, 1, 1);
        animatorHead.SetBool("isHor", false);
        animatorHair.SetBool("isHor", false);
        animatorHead.SetBool("isBack", false);
        animatorHair.SetBool("isBack", false);
    } else {
      animatorHead.SetBool("isBack", false);
      animatorHead.SetBool("isHor", false);
      animatorHair.SetBool("isHor", false);
      animatorHair.SetBool("isBack", false);
    }
  }
  void bodyAnimtions(float x, float y)
  {
    if (x != 0)
    {
      animatorBody.SetBool("isWalk", true);
    }
    else if (y != 0)
    {
      animatorBody.SetBool("isWalk", false);
      animatorBody.SetBool("isWalkHor", true);
    } else {
      animatorBody.SetBool("isWalk", false);
      animatorBody.SetBool("isWalkHor", false);
    }
    if (x > 0)
    {
      body.transform.localScale = new Vector3(1, 1, 1);
    }
    else if (x < 0)
    {
      body.transform.localScale = new Vector3(-1, 1, 1);
    }
    else if (y > 0)
    {
      body.transform.localScale = new Vector3(1, 1, 1);
    }
    else if (y < 0)
    {
      body.transform.localScale = new Vector3(-1, 1, 1);
    }
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
