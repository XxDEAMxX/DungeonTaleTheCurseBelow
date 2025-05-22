using System.Collections.Generic;
using UnityEngine;
using RandomModels.Core;
using RandomModels.StatisticalTests;

public class RandomMatrix : MonoBehaviour
{
    private static RandomMatrix _instance;
    
    [SerializeField] public int matrixSize = 50000; // Hecho público para posible configuración externa
    [SerializeField] private bool validateMatrix = true;
    [SerializeField] private int seed = 12345;
    
    private static float[] _randomValues;
    private static int _currentIndex = 0;
    private bool _isInitialized = false;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (!_isInitialized) // Evitar re-generar si ya se hizo por acceso estático prematuro
        {
            GenerateRandomMatrix();
        }
    }
    
    private void GenerateRandomMatrix()
    {
        Debug.Log($"Generando matriz de {matrixSize} números aleatorios con semilla {seed}...");
        
        _randomValues = new float[matrixSize];
        LinearCongruenceRandom rng = new LinearCongruenceRandom(seedValue: seed);
        
        for (int i = 0; i < matrixSize; i++)
        {
            _randomValues[i] = rng.Random();
        }
        
        if (validateMatrix)
        {
            ValidateMatrixValues();
        }
        
        _currentIndex = 0; // Reiniciar índice
        _isInitialized = true;
        Debug.Log("Matriz de números aleatorios generada correctamente.");
    }
    
    private void ValidateMatrixValues()
    {
        List<float> values = new List<float>(_randomValues);
        bool allTestsPassed = true;
        
        var chiTest = new ChiSquareTest(values); chiTest.EvaluateTest(); allTestsPassed &= chiTest.Passed;
        var ksTest = new KsTest(values); ksTest.CheckTest(); allTestsPassed &= ksTest.Passed;
        var varianceTest = new VarianceTest(values); varianceTest.EvaluateTest(); allTestsPassed &= varianceTest.Passed;
        var pokerTest = new PokerTest(values); pokerTest.CheckPoker(); allTestsPassed &= pokerTest.Passed;
        
        if (allTestsPassed)
            Debug.Log("Matriz de números aleatorios validada: TODAS LAS PRUEBAS PASADAS ✓");
        else
            Debug.LogWarning("Matriz de números aleatorios con posible baja calidad: ALGUNAS PRUEBAS FALLARON ✗");
    }
    
    private static void EnsureInitialized()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("RandomMatrix_AutoInstance");
            _instance = go.AddComponent<RandomMatrix>();
            // Awake() será llamado, lo que llamará a GenerateRandomMatrix()
        }
        else if (!_instance._isInitialized)
        {
            // Si la instancia existe pero no está inicializada (ej. Awake aún no se ha llamado completamente)
            _instance.GenerateRandomMatrix();
        }
    }

    public static float GetNextRandom()
    {
        EnsureInitialized();
        if (_randomValues == null || _randomValues.Length == 0)
        {
            Debug.LogError("RandomMatrix no tiene valores. Intentando regenerar.");
            _instance.GenerateRandomMatrix(); // Intento desesperado
            if (_randomValues == null || _randomValues.Length == 0) return 0f;
        }
        
        float value = _randomValues[_currentIndex];
        _currentIndex = (_currentIndex + 1) % _randomValues.Length;
        return value;
    }
    
    public static float Range(float min, float max)
    {
        return min + GetNextRandom() * (max - min);
    }
    
    public static int Range(int min, int max)
    {
        if (min >= max) return min;
        // Para enteros, Random.Range(min, max) de Unity es [min, max-1]
        // Multiplicamos por (max - min) para obtener un valor en [0, max-min-epsilon]
        // Luego casteamos a int, lo que trunca.
        // Esto nos da un entero en [0, max-min-1]. Sumamos min.
        return min + (int)(GetNextRandom() * (max - min));
    }
    
    public static void RegenerateMatrix(int newSeed)
    {
        EnsureInitialized();
        _instance.seed = newSeed;
        _instance.GenerateRandomMatrix();
        Debug.Log($"RandomMatrix regenerada con nueva semilla: {newSeed}");
    }
}
