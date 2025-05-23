using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public AudioClip pickupSound; // Campo para el sonido de recolección
    private AudioSource audioSource; // Componente AudioSource

    void Start()
    {
        // Intentar obtener un AudioSource existente o añadir uno nuevo si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    void Update()
    {
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickupSound != null)
            {
                Debug.Log($"Intentando reproducir sonido de moneda: {pickupSound.name} en {transform.position}");
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            else
            {
                Debug.LogWarning("pickupSound (AudioClip) no está asignado en el Inspector para la moneda.", gameObject);
            }
            GameManager.instance.AddPoint(value);
            Destroy(gameObject);
        }
    }
}
