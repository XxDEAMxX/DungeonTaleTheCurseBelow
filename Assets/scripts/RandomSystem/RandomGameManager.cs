using UnityEngine;

public class RandomGameManager : MonoBehaviour
{
    [Header("Sistema de Números Aleatorios")]
    [SerializeField] private bool initializeCustomRandom = true;
    [SerializeField] private bool initializeRandomMatrix = true;
    [SerializeField] private bool initializeOptimizedRandom = true;

    [Header("Configuración Específica (si aplica)")]
    // Ejemplo: podrías querer configurar la seed base para OptimizedRandom desde aquí
    [SerializeField] private int optimizedRandomBaseSeed = 12345;
    [SerializeField] private int randomMatrixSeed = 12345;
    [SerializeField] private int randomMatrixSize = 50000;

    [Header("Validación (Opcional)")]
    [SerializeField] private bool validateOnStart = false; 
    [SerializeField] private GameObject randomQualityMonitorPrefab; // Asigna tu prefab aquí

    private static RandomGameManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        SetupRandomSystem();
    }

    private void SetupRandomSystem()
    {
        Debug.Log("RandomGameManager: Iniciando configuración del sistema de números aleatorios...");

        if (initializeCustomRandom)
        {
            if (FindObjectOfType<CustomRandom>() == null)
            {
                GameObject crObj = new GameObject("CustomRandom_ManagedInstance");
                crObj.AddComponent<CustomRandom>(); 
                // CustomRandom maneja su propio DontDestroyOnLoad y se inicializa en su Awake.
                Debug.Log("CustomRandom instanciado por GameManager.");
            }
        }

        if (initializeRandomMatrix)
        {
            if (FindObjectOfType<RandomMatrix>() == null)
            {
                GameObject rmObj = new GameObject("RandomMatrix_ManagedInstance");
                RandomMatrix matrixInstance = rmObj.AddComponent<RandomMatrix>();
                // Configurar antes de que su Awake sea llamado (si es necesario y posible)
                // matrixInstance.seed = randomMatrixSeed; // Necesitaría que 'seed' sea public y se lea antes de Awake
                // matrixInstance.matrixSize = randomMatrixSize;
                Debug.Log("RandomMatrix instanciado por GameManager.");
            }
        }

        if (initializeOptimizedRandom)
        {
            if (FindObjectOfType<OptimizedRandom>() == null)
            {
                GameObject orObj = new GameObject("OptimizedRandom_ManagedInstance");
                OptimizedRandom optimizedInstance = orObj.AddComponent<OptimizedRandom>();
                // optimizedInstance.baseSeed = optimizedRandomBaseSeed; // Necesitaría que 'baseSeed' sea public y se lea antes de Awake
                Debug.Log("OptimizedRandom instanciado por GameManager.");
            }
        }

        if (validateOnStart && randomQualityMonitorPrefab != null)
        {
            if (FindObjectOfType<RandomQualityMonitor>() == null)
            {
                Instantiate(randomQualityMonitorPrefab);
                Debug.Log("RandomQualityMonitor instanciado para validación.");
            }
        }
        else if (validateOnStart && randomQualityMonitorPrefab == null)
        {
            Debug.LogWarning("validateOnStart está activado pero randomQualityMonitorPrefab no está asignado en RandomGameManager.");
        }

        Debug.Log("RandomGameManager: Configuración del sistema de números aleatorios completada.");
    }
}
