using UnityEngine;

public class IsaacController : MonoBehaviour
{
  public float velocidad = 5f;
  private Rigidbody2D rb;
  private Animator animator;

  void Awake()
  {
    Debug.Log("IsaacController Awake llamado");
  }

  void Start()
  {
    Debug.Log("IsaacController Start llamado");
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();

    if (rb == null)
    {
      Debug.LogError("No se encontró Rigidbody2D en " + gameObject.name);
    }
    if (animator == null)
    {
      Debug.LogError("No se encontró Animator en " + gameObject.name);
    }
  }

  void Update()
  {
    Mover();
  }

  void Mover()
  {
    float inputX = 0f;
    float inputY = 0f;

    // Usar exactamente el mismo método de entrada que en Movement
    if (Input.GetKey(KeyCode.W)) inputY = 1f;
    if (Input.GetKey(KeyCode.S)) inputY = -1f;
    if (Input.GetKey(KeyCode.D)) inputX = 1f;
    if (Input.GetKey(KeyCode.A)) inputX = -1f;

    // Orientar el personaje
    if (inputX > 0)
    {
      Debug.Log("IsaacController: Movimiento a la derecha");
      transform.localScale = new Vector3(1, 1, 1);
    }
    else if (inputX < 0)
    {
      transform.localScale = new Vector3(-1, 1, 1);
      Debug.Log("IsaacController: Movimiento a la izquierda");
    }

    // Manejo de animación
    if (animator != null)
    {
      if (inputX != 0 || inputY != 0)
      {
        animator.SetBool("walk", true);
      }
      else
      {
        animator.SetBool("walk", false);
      }
    }

    // Normalizar y aplicar movimiento (usando el mismo método que Movement)
    Vector2 input = new Vector2(inputX, inputY).normalized;

    if (rb != null)
    {
      rb.linearVelocity = input * velocidad;
    }
  }
}
