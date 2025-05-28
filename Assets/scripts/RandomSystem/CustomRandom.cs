using System.Collections.Generic;
using UnityEngine;
using RandomModels.Core;

public class CustomRandom : MonoBehaviour
{
    private static ValidatedRandom _rng;
    private static CustomRandom _instance;
    
    [SerializeField] private int seed = 0;
    [SerializeField] private int batchSize = 10000;
    [SerializeField] private float significanceLevel = 0.05f;
    [SerializeField] private bool useRandomSeedOnStart = true;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeRNG();
    }
    
    private void InitializeRNG()
    {
        int seedToUse = useRandomSeedOnStart ? 
            System.Environment.TickCount : seed;
            
        Debug.Log($"Inicializando CustomRandom con semilla: {seedToUse}");
        _rng = new ValidatedRandom(seedToUse, batchSize, significanceLevel);
    }
    
    // Equivalente a Random.value
    public static float value => _rng != null ? _rng.Random() : InitializeAndGetRandom();
    
    // Equivalente a Random.Range para flotantes
    public static float Range(float min, float max)
    {
        return min + value * (max - min);
    }
      // Equivalente a Random.Range para enteros
    public static int Range(int min, int max)
    {
        // Para enteros, Random.Range(min, max) en Unity es inclusivo para min y exclusivo para max.
        // _rng.RandomInt(min, max) es inclusivo en ambos extremos, así que ajustamos max-1
        if (min >= max) return min; // Evitar error si min es mayor o igual a max
        return _rng != null ? _rng.RandomInt(min, max - 1) : 
            min + (int)(InitializeAndGetRandom() * (max - min));
    }
    
    // Métodos adicionales
    public static T Choice<T>(IList<T> items)
    {
        if (items == null || items.Count == 0) return default(T);
        return items[Range(0, items.Count)];
    }
    
    public static void Shuffle<T>(IList<T> items)
    {
        if (items == null) return;
        int n = items.Count;
        while (n > 1)
        {
            n--;
            int k = Range(0, n + 1); // El rango aquí debe ser [0, n]
            T value = items[k];
            items[k] = items[n];
            items[n] = value;
        }
    }
    
    // Para distribución normal (Gaussiana)
    public static float Gaussian(float mean = 0, float stdDev = 1)
    {
        float u1 = value;
        float u2 = value;
        
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * 
                             Mathf.Sin(2.0f * Mathf.PI * u2);
                             
        return mean + stdDev * randStdNormal;
    }
    
    private static float InitializeAndGetRandom()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("CustomRandom_AutoInstance");
            _instance = go.AddComponent<CustomRandom>();
            // Awake se llamará automáticamente, inicializando _rng
        }
        // Si _rng aún es nulo después de lo anterior, algo falló en la inicialización de _instance
        if (_rng == null) {
            Debug.LogError("CustomRandom RNG no está inicializado.");
            return 0f; // O lanzar una excepción
        }
        return _rng.Random();
    }
    
    // Establecer semilla manualmente
    public static void SetSeed(int newSeed)
    {
        if (_instance == null) // Asegurarse de que la instancia exista
        {
            GameObject go = new GameObject("CustomRandom_AutoInstance");
            _instance = go.AddComponent<CustomRandom>();
        }
        _instance.seed = newSeed;
        _instance.useRandomSeedOnStart = false;
        _instance.InitializeRNG(); // Esto reinicializará _rng con la nueva semilla
        Debug.Log($"CustomRandom semilla establecida a: {newSeed}");
    }
}
