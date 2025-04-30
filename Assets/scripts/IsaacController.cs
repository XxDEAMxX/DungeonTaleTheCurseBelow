using System.Collections;
using UnityEngine;

public class IsaacController : MonoBehaviour
{
  public float velocidad = 5f;
  private Rigidbody2D rb;
  private Animator animatorBody;
  private Animator animatorHead;
  public GameObject bulletPrefab;
  public GameObject body;
  public GameObject head;
  public float bulletSpeed = 10f; // Velocidad de la bala
  public float fireDelay = 5f; // Delay entre disparos
  private float lastFire;
  private bool isShooting = false;
  void Start()
  {
    Debug.Log("IsaacController Start llamado");
    rb = GetComponent<Rigidbody2D>();
    animatorBody = GameObject.FindGameObjectWithTag("Body").GetComponent<Animator>();
    animatorHead = GameObject.FindGameObjectWithTag("Head").GetComponent<Animator>();
    head = GameObject.FindGameObjectWithTag("Head");
    body = GameObject.FindGameObjectWithTag("Body");

    if (rb == null)
    {
      Debug.LogError("No se encontró Rigidbody2D en " + gameObject.name);
    }
    if (animatorBody == null)
    {
      Debug.LogError("No se encontró Animator en " + gameObject.name);
    }
  }

  void Awake()
  {
    Debug.Log("IsaacController Awake llamado");
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

  IEnumerator ResetShootFlag()
  {
      yield return new WaitForSeconds(fireDelay);
      isShooting = false;
  }

  void shoot(float x, float y)
  {
      isShooting = true;
      GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
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

      // Cambiar dirección de la cabeza si es horizontal
      if (isHorizontal)
      {
          Vector3 headScale = head.transform.localScale;
          headScale.x = direction.x > 0 ? Mathf.Abs(headScale.x) : -Mathf.Abs(headScale.x);
          head.transform.localScale = headScale;
      }

      rb.linearVelocity = direction * bulletSpeed;
      StartCoroutine(ResetShootFlag());
  }



  void Update(){
    Mover();
    preShoot();
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
      rb.linearVelocity = input * velocidad;
    }
  }

  void headAnimtions(float x, float y){
    if (x > 0)
    {
      head.transform.localScale = new Vector3(1, 1, 1);
      animatorHead.SetBool("isHor", true);
      animatorHead.SetBool("isBack", false);
    }
    else if (x < 0)
    {
      head.transform.localScale = new Vector3(-1, 1, 1);
      animatorHead.SetBool("isHor", true);
      animatorHead.SetBool("isBack", false);
    }
      else if (y > 0)
      {
        head.transform.localScale = new Vector3(1, 1, 1);
        animatorHead.SetBool("isHor", false);
        animatorHead.SetBool("isBack", true);
      }
      else if (y < 0)
      {
        head.transform.localScale = new Vector3(-1, 1, 1);
        animatorHead.SetBool("isHor", false);
        animatorHead.SetBool("isBack", false);
    } else {
      animatorHead.SetBool("isBack", false);
      animatorHead.SetBool("isHor", false);
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
}
