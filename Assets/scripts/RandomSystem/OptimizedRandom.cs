using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomPool
{
    public string poolName;
    public int size = 1000;
    [HideInInspector] public float[] values; // Ocultar en inspector si se inicializa por código
    private int currentIndex = 0;
    private bool _isInitialized = false;
    
    public void Initialize(int seed)
    {
        var rng = new RandomModels.Core.LinearCongruenceRandom(seedValue: seed);
        values = new float[size];
        
        for (int i = 0; i < size; i++)
        {
            values[i] = rng.Random();
        }
        currentIndex = 0;
        _isInitialized = true;
        Debug.Log($"Pool '{poolName}' inicializado con {size} valores y semilla {seed}.");
    }
    
    public float GetNext()
    {
        if (!_isInitialized || values == null || values.Length == 0) {
            Debug.LogError($"Pool '{poolName}' no inicializado o vacío. Llamar a Initialize() primero.");
            return 0f; 
        }
        float value = values[currentIndex];
        currentIndex = (currentIndex + 1) % values.Length;
        return value;
    }
}

public class OptimizedRandom : MonoBehaviour
{
    private static OptimizedRandom _instance;
    
    [SerializeField] private int baseSeed = 12345;
    [SerializeField] private List<RandomPool> pools = new List<RandomPool>();
    
    private Dictionary<string, RandomPool> _poolDictionary = new Dictionary<string, RandomPool>();
    private bool _arePoolsInitialized = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (!_arePoolsInitialized) // Evitar reinicializar si ya se hizo
        {
            InitializePools();
        }
    }
    
    // OnValidate se llama en el editor cuando se cambian los valores
    private void OnValidate()
    {
        // Esto es más para configuración en el editor, no tanto para runtime.
        // La inicialización real de los pools debe ocurrir en Awake o Start.
        if (pools.Count == 0 && Application.isEditor) // Solo añadir predeterminados en el editor
        {
            Debug.Log("Añadiendo pools predeterminados a OptimizedRandom en OnValidate.");
            pools.Add(new RandomPool { poolName = "General", size = 2000 });
            pools.Add(new RandomPool { poolName = "Combat", size = 5000 });
            pools.Add(new RandomPool { poolName = "Loot", size = 3000 });
            pools.Add(new RandomPool { poolName = "EnemyMovement", size = 5000 });
            pools.Add(new RandomPool { poolName = "Spawner", size = 3000 });
        }
    }
    
    private void InitializePools()
    {
        _poolDictionary.Clear();

        // Añadir pools predeterminados si la lista está vacía en tiempo de ejecución
        if (pools.Count == 0)
        {
            Debug.LogWarning("La lista de pools en OptimizedRandom está vacía. Añadiendo pools predeterminados en tiempo de ejecución. Por favor, configure los pools en el Inspector para un control más preciso.");
            pools.Add(new RandomPool { poolName = "General", size = 2000 });
            pools.Add(new RandomPool { poolName = "Combat", size = 5000 });
            pools.Add(new RandomPool { poolName = "Loot", size = 3000 });
            pools.Add(new RandomPool { poolName = "EnemyMovement", size = 5000 });
            pools.Add(new RandomPool { poolName = "Spawner", size = 3000 });
        }
        
        for (int i = 0; i < pools.Count; i++)
        {
            if (string.IsNullOrEmpty(pools[i].poolName)) {
                Debug.LogWarning($"Pool en índice {i} no tiene nombre. Omitiendo.");
                continue;
            }
            int poolSeed = baseSeed + i * 1000; 
            pools[i].Initialize(poolSeed);
            
            if(!_poolDictionary.ContainsKey(pools[i].poolName))
            {
                _poolDictionary[pools[i].poolName] = pools[i];
            }
            else
            {
                Debug.LogWarning($"Pool con nombre duplicado '{pools[i].poolName}'. Usando el primero encontrado.");
            }
        }
        _arePoolsInitialized = true;
        Debug.Log($"Se inicializaron {_poolDictionary.Count} pools de números aleatorios en OptimizedRandom.");
    }
    
    private static void EnsureInstanceAndInitialized()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("OptimizedRandom_AutoInstance");
            _instance = go.AddComponent<OptimizedRandom>();
            // Awake() será llamado, inicializando los pools.
        }
        else if (!_instance._arePoolsInitialized)
        {
            // Si la instancia existe pero los pools no (ej. Awake aún no se ha llamado completamente)
            _instance.InitializePools();
        }
    }

    public static float Value(string poolName = "General")
    {
        EnsureInstanceAndInitialized();
        
        if (_instance._poolDictionary.TryGetValue(poolName, out RandomPool pool))
        {
            return pool.GetNext();
        }
        
        Debug.LogWarning($"Pool '{poolName}' no encontrado. Intentando usar 'General'. Asegúrate de que el pool está definido y nombrado correctamente en el componente OptimizedRandom.");
        if(_instance._poolDictionary.TryGetValue("General", out RandomPool generalPool))
        {
            return generalPool.GetNext();
        }
        else
        {
            Debug.LogError("Pool 'General' no encontrado. OptimizedRandom no está configurado correctamente o no tiene pools.");
            return 0f; 
        }
    }
    
    public static float Range(float min, float max, string poolName = "General")
    {
        return min + Value(poolName) * (max - min);
    }
    
    public static int Range(int min, int max, string poolName = "General")
    {
        if (min >= max) return min;
        return min + (int)(Value(poolName) * (max - min));
    }
    
    public static void RegeneratePools(int newBaseSeed)
    {
        EnsureInstanceAndInitialized();
        _instance.baseSeed = newBaseSeed;
        _instance.InitializePools();
        Debug.Log($"Todos los pools de OptimizedRandom regenerados con nueva baseSeed: {newBaseSeed}");
    }
}
