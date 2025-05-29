using UnityEngine;

public class RandomSystemLoader : MonoBehaviour
{
    // Prefab del RandomGameManager que se encargará de inicializar todo el sistema.
    [SerializeField] private GameObject randomGameManagerPrefab;
    
    private void Awake()
    {
        // Verificar si ya existe una instancia de RandomGameManager.
        // Esto evita duplicados si el GameManager ya está en la escena o fue cargado por otro loader.
        if (FindFirstObjectByType<RandomGameManager>() == null)
        {
            if (randomGameManagerPrefab != null)
            {
                Instantiate(randomGameManagerPrefab);
                Debug.Log("RandomGameManager instanciado por RandomSystemLoader.");
            }
            else
            {
                Debug.LogError("randomGameManagerPrefab no está asignado en RandomSystemLoader. El sistema de aleatoriedad no se cargará.");
            }
        }
        else
        {
            Debug.Log("RandomGameManager ya existe en la escena. No se requiere acción de RandomSystemLoader.");
        }
    }
}
