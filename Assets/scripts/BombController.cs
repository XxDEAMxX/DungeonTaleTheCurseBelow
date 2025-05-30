using UnityEngine;

public class BombController : MonoBehaviour
{
    public AudioClip explosionSound; // Asigna el clip de audio en el Inspector de Unity
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no encontrado en el objeto Bomb. Asegúrate de añadir un componente AudioSource.");
        }
    }

    void Update()
    {
        
    }

    public void PlayExplosionSound()
    {
        if (audioSource != null && explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position); // Ajusta el valor 2.0f según necesites
        }
    }

    void OnDestroy()
    {
        
        Destroy(gameObject);
    }
}
