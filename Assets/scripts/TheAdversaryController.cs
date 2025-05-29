using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

// Estados del jefe final para manejo secuencial de comportamientos
public enum TheAdversaryState { Init, Idle, Attack, Follow, Dead, GenerateChild };

/// <summary>
/// Controlador del jefe final con manejo secuencial de eventos y comportamientos complejos
/// Implementa múltiples sistemas secuenciales: máquina de estados, cola de eventos, 
/// secuencias de animación, y manejo temporal de cooldowns
/// </summary>
public class TheAdversaryController : MonoBehaviour
{
    public static TheAdversaryController instance;
    void Awake() {
            instance = this;
    } 
    private int life;
    private Animator animator;
    private GameObject player;
    TheAdversaryState curreState = TheAdversaryState.Idle;
    public float speed;
    public bool notInRoom = false;
    private bool isInitFinish = false;
    private Rigidbody2D rb;


    public GameObject childPrefab; // Asigna el prefab del "hijo" desde el Inspector
    public Transform spawnPoint; // Punto donde se genera el hijo    // === SISTEMA DE COLA SECUENCIAL ===
    // Cola FIFO para manejo ordenado de eventos de generación
    private Queue<System.Action> generationQueue = new Queue<System.Action>();
    private bool hasStartedChildGeneration = false;

    // === CONTROL DE PROGENIE ===
    // Limita y rastrea la generación secuencial de enemigos hijos
    private int currentChildren = 0;
    public int maxChildren = 10;
    public bool isGenerateChild = false;

    // === SISTEMA DE COOLDOWN SECUENCIAL ===
    // Maneja intervalos temporales para prevenir spam de daño
    private bool canDealDamage = true;
    private float damageCooldown = 1f;

    private bool isTouchingPlayer = false;
    private float damageInterval = 1f;
    private float damageTimer = 0f;
    private float lastDamageTime = -999f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<BoxCollider2D>().isTrigger = true;
        life = 2;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(ProcessQueue());
    }    void Update()
    {
        // SECUENCIA DE ESTADOS: Verificar primero si está muerto para evitar procesamiento innecesario
        if (curreState == TheAdversaryState.Dead)
            return;

        // TRANSICIÓN SECUENCIAL: Inicialización del jefe cuando entra en la sala
        if (!isInitFinish && notInRoom)
        {
            MusicManager.instance.SetBossMusic();
            curreState = TheAdversaryState.Init;  // Cambio secuencial automático de estado
            isInitFinish = true;
            GetComponent<BoxCollider2D>().isTrigger = false;
        }

        // MÁQUINA DE ESTADOS SECUENCIAL: Procesamiento ordenado de comportamientos
        switch (curreState)
        {
            case TheAdversaryState.Idle:
                Idle();           // Estado de espera - comportamiento pasivo
                break;
            case TheAdversaryState.Init:
                Init();           // Estado de inicialización - preparación del jefe
                break;
            case TheAdversaryState.Attack:
                break;            // Estado de ataque - comportamiento agresivo
            case TheAdversaryState.Follow:
                Follow();         // Estado de persecución - seguir al jugador
                break;            case TheAdversaryState.Dead:
                Death();          // Estado final - secuencia de muerte
                break;
            case TheAdversaryState.GenerateChild:
                stop();           // Estado especial - generación de enemigos hijo
                break;
        }
    }

    private void stop()
    {
    }    // SISTEMA DE COLA SECUENCIAL: Procesamiento ordenado de eventos de generación
    IEnumerator ProcessQueue()
    {
        while (true)
        {
            // Procesar eventos en orden FIFO (First In, First Out)
            if (generationQueue.Count > 0)
            {
                var action = generationQueue.Dequeue();  // Tomar siguiente evento en cola
                action.Invoke();                         // Ejecutar acción secuencialmente
            }
            yield return null;  // Esperar al siguiente frame para continuar
        }
    }    // SECUENCIA DE GENERACIÓN DE ENEMIGOS HIJO
    void generateSingleChild()
    {
        if (childPrefab != null && spawnPoint != null && currentChildren < maxChildren)
        {
            // PASO 1: Resetear rotación para animación consistente
            transform.rotation = Quaternion.Euler(0, 0, 0);

            // PASO 2: Detener movimiento del jefe durante generación
            rb.linearVelocity = Vector2.zero;
            
            // PASO 3: Cambiar estado secuencialmente
            curreState = TheAdversaryState.GenerateChild;
            isGenerateChild = true;
            
            // PASO 4: SECUENCIA DE ANIMACIONES - Orden específico para evitar conflictos
            animator.SetBool("isGenerateChild", true);   // Activar animación principal
            animator.SetBool("isIdle", false);           // Desactivar estados anteriores
            animator.SetBool("isAttack", false);
            animator.SetBool("isInit", false);
        }
    }

    public void InitialChild()
    {
        Vector2 spawnOffset = Random.insideUnitCircle * 0.5f;
        Vector3 spawnPosition = spawnPoint.position + (Vector3)spawnOffset;

        Instantiate(childPrefab, spawnPosition, Quaternion.identity);
        currentChildren++;        
    }
    // SECUENCIA DE GENERACIÓN TEMPORAL DE ENEMIGOS
    IEnumerator EnqueueChildGeneration()
    {
        while (curreState != TheAdversaryState.Dead)
        {
            // INTERVALO ALEATORIO: Crear variedad temporal en la generación
            float randomInterval = Random.Range(0f, 7f);
            yield return new WaitForSeconds(randomInterval);  // ESPERA SECUENCIAL

            // CONTROL DE LÍMITES: Verificar antes de encolar
            if (currentChildren < maxChildren)
            { 
                curreState = TheAdversaryState.GenerateChild;

                // ENCOLAR EVENTO: Agregar acción a la cola para procesamiento secuencial
                generationQueue.Enqueue(() => generateSingleChild());
            }
        }
    }


    private void Idle()
    {
        animator.SetBool("isIdle", true);
        animator.SetBool("isAttack", false);
        animator.SetBool("isGenerateChild", false);
        animator.SetBool("isInit", false);
    }    // TRANSICIÓN SECUENCIAL DE ESTADO IDLE A FOLLOW
    public void toFollow()
    {
        // PASO 1: Resetear flag de generación
        isGenerateChild = false; 
        
        // PASO 2-6: SECUENCIA ORDENADA DE CAMBIOS DE ANIMACIÓN
        animator.SetBool("isInit", false);              // Desactivar inicialización
        animator.SetBool("isIdle", false);              // Desactivar espera
        animator.SetBool("isAttack", true);             // Activar ataque
        animator.SetBool("isGenerateChild", false);     // Confirmar no-generación
        
        // PASO 7: Cambio final de estado
        curreState = TheAdversaryState.Follow;          // Transición secuencial completa
    }

    private void Init()
    {
        animator.SetBool("isInit", true);
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", false);
        animator.SetBool("isGenerateChild", false);

        if (!hasStartedChildGeneration)
        {
            StartCoroutine(EnqueueChildGeneration());
            hasStartedChildGeneration = true;
        }
    }

    private void Follow()
    {
        if (curreState != TheAdversaryState.Follow && !isGenerateChild) return;

        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", true);
        animator.SetBool("isGenerateChild", false);
        animator.SetBool("isInit", false);

        Vector2 direction = (player.transform.position - transform.position).normalized;

        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
    }
    // SECUENCIA DE MUERTE DEL JEFE FINAL
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword") || collision.CompareTag("Bullet"))
        {
            life--;
            if (life <= 0)
            {
                // SECUENCIA ORDENADA DE MUERTE DEL JEFE
                transform.rotation = Quaternion.Euler(0, 0, 0);     // 1. Resetear rotación
                rb.linearVelocity = Vector2.zero;                   // 2. Detener movimiento
                GetComponent<BoxCollider2D>().isTrigger = true;     // 3. Cambiar colisión
                curreState = TheAdversaryState.Dead;                // 4. Estado de muerte
                animator.SetBool("isDeath", true);                  // 5. Animación de muerte
                
                // SECUENCIA DE EVENTOS DE VICTORIA            }
        }
        if (collision.CompareTag("BombRange"))
        {
            curreState = TheAdversaryState.Dead;
            animator.SetBool("isDeath", true);
        }
    }

    public void setDead()
    {
        animator.SetBool("isDead", true);
        if (GameManager.instance != null)
        {
            Debug.Log("Llamando a WinGame() desde TheAdversaryController.");
            GameManager.instance.WinGame();                 // 6. Activar victoria
        }
        else
        {
            Debug.LogError("GameManager.instance no está asignado en TheAdversaryController. No se puede llamar a WinGame().");
        }
                
                // SECUENCIA DE DESACTIVACIÓN DEL JUGADOR
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerObject.SetActive(false);                  // 7. Desactivar jugador
        }
        else
        {
            Debug.LogWarning("No se encontró ningún GameObject con la etiqueta 'Player' para desactivar.");
        }
    }    // MANEJO SECUENCIAL DE COLISIÓN Y DAÑO AL JUGADOR
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && canDealDamage)
        {
            // SECUENCIA DE CÁLCULO DE DIRECCIÓN DE EMPUJE
            Vector2 rawDir = (other.transform.position - transform.position);
            Vector2 direction = Mathf.Abs(rawDir.x) > Mathf.Abs(rawDir.y)
                ? new Vector2(Mathf.Sign(rawDir.x), 0)     // Prioridad horizontal
                : new Vector2(0, Mathf.Sign(rawDir.y));    // Prioridad vertical

            GameManager.instance.DecreaseLife(direction);   // Aplicar daño
            StartCoroutine(DamageCooldown());               // Iniciar cooldown secuencial
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 rawDir = (other.transform.position - transform.position);
            Vector2 direction = Mathf.Abs(rawDir.x) > Mathf.Abs(rawDir.y)
                ? new Vector2(Mathf.Sign(rawDir.x), 0)
                : new Vector2(0, Mathf.Sign(rawDir.y));
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                GameManager.instance.DecreaseLife(direction);
                lastDamageTime = Time.time;
            }
        }
    }    // COOLDOWN SECUENCIAL TEMPORAL PARA PREVENIR SPAM DE DAÑO
    private IEnumerator DamageCooldown()
    {
        // PASO 1: Bloquear capacidad de hacer daño
        canDealDamage = false;
        
        // PASO 2: Esperar tiempo de cooldown secuencialmente
        yield return new WaitForSeconds(damageCooldown);
        
        // PASO 3: Restaurar capacidad de hacer daño
        canDealDamage = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            damageTimer = 0f;
        }
    }
      // SECUENCIA FINAL DE MUERTE Y LIMPIEZA
    void Death()
    {
        // PASO 1: Iniciar transición de sala
        RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
        
        // PASO 2: Destruir el objeto del jefe
        Destroy(gameObject);
        
        // PASO 3: Agregar puntos al jugador
        GameManager.instance.AddPoint(1);
    }
}
