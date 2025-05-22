using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RandomModels.Core;

/// <summary>
/// Clase de demostración para visualizar la calidad de los números aleatorios generados.
/// Esta clase puede adjuntarse a un GameObject en la escena para mostrar resultados visuales.
/// </summary>
public class RandomQualityDemo : MonoBehaviour
{
    [Header("Configuración del generador")]
    [SerializeField] private int seedValue = 12345;
    [SerializeField] private int sampleSize = 10000;
    
    [Header("Referencias UI")]
    [SerializeField] private RectTransform histogramContainer;
    [SerializeField] private TMP_Text resultsText;
    [SerializeField] private Button regenerateButton;
    
    private ValidatedRandom _validatedRandom;
    private RandomQualityValidator _validator;
    
    // Referencias para el histograma
    private RectTransform[] _histogramBars;
    private const int HistogramBins = 20;
    
    // Resultados de las pruebas
    private bool _allTestsPassed;
    
    private void Start()
    {
        // Inicializar el validador
        _validator = GetComponent<RandomQualityValidator>();
        if (_validator == null)
        {
            _validator = gameObject.AddComponent<RandomQualityValidator>();
        }
        
        // Inicializar el generador validado
        _validatedRandom = new ValidatedRandom(seed: seedValue, batchSize: sampleSize);
        
        // Configurar el botón
        if (regenerateButton != null)
        {
            regenerateButton.onClick.AddListener(RegenerateAndTest);
        }
        
        // Inicializar histograma si está disponible
        if (histogramContainer != null)
        {
            InitializeHistogram();
        }
        
        // Primera generación y prueba
        RegenerateAndTest();
    }
    
    /// <summary>
    /// Inicializa el histograma creando barras para visualizar la distribución.
    /// </summary>
    private void InitializeHistogram()
    {
        // Limpiar el histograma existente
        foreach (Transform child in histogramContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Crear nuevas barras para el histograma
        _histogramBars = new RectTransform[HistogramBins];
        
        float barWidth = histogramContainer.rect.width / HistogramBins;
        
        for (int i = 0; i < HistogramBins; i++)
        {
            GameObject barObj = new GameObject($"Bar_{i}");
            barObj.transform.SetParent(histogramContainer, false);
            
            Image barImage = barObj.AddComponent<Image>();
            barImage.color = new Color(0.2f, 0.6f, 1.0f, 0.8f);
            
            RectTransform rectTransform = barObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(barWidth - 2, 10); // Altura inicial pequeña con margen
            rectTransform.anchoredPosition = new Vector2(i * barWidth + 1, 0);
            
            _histogramBars[i] = rectTransform;
        }
    }
    
    /// <summary>
    /// Genera nuevos números aleatorios, ejecuta las pruebas y actualiza la visualización.
    /// </summary>
    public void RegenerateAndTest()
    {
        // Generar una muestra grande de números para analizar
        List<float> numbers = new List<float>(sampleSize);
        for (int i = 0; i < sampleSize; i++)
        {
            numbers.Add(_validatedRandom.Random());
        }
        
        // Actualizar el histograma
        UpdateHistogram(numbers);
        
        // Ejecutar las pruebas a través del validador
        if (_validator != null)
        {
            _validator.RegenerateAndValidate();
            _validator.AnalyzeDistribution();
            
            // La validadora imprime los resultados detallados en la consola
        }
        
        // Actualizar el texto de resultados
        UpdateResultsText(numbers);
    }
    
    /// <summary>
    /// Actualiza el histograma con los datos de una lista de números.
    /// </summary>
    private void UpdateHistogram(List<float> numbers)
    {
        if (histogramContainer == null || _histogramBars == null) return;
        
        // Calcular frecuencias
        int[] frequencies = new int[HistogramBins];
        
        foreach (float n in numbers)
        {
            int bin = Mathf.Min(HistogramBins - 1, Mathf.FloorToInt(n * HistogramBins));
            frequencies[bin]++;
        }
        
        // Encontrar la frecuencia máxima para normalización
        int maxFreq = 0;
        foreach (int freq in frequencies)
        {
            maxFreq = Mathf.Max(maxFreq, freq);
        }
        
        // Actualizar altura de las barras
        float maxHeight = histogramContainer.rect.height;
        for (int i = 0; i < HistogramBins; i++)
        {
            float barHeight = frequencies[i] * maxHeight / maxFreq;
            _histogramBars[i].sizeDelta = new Vector2(_histogramBars[i].sizeDelta.x, barHeight);
        }
    }
    
    /// <summary>
    /// Actualiza el texto con estadísticas básicas de la muestra.
    /// </summary>
    private void UpdateResultsText(List<float> numbers)
    {
        if (resultsText == null) return;
        
        // Calcular estadísticas básicas
        float sum = 0;
        float min = float.MaxValue;
        float max = float.MinValue;
        
        foreach (float n in numbers)
        {
            sum += n;
            min = Mathf.Min(min, n);
            max = Mathf.Max(max, n);
        }
        
        float mean = sum / numbers.Count;
        
        // Calcular varianza
        float sumSquaredDiff = 0;
        foreach (float n in numbers)
        {
            sumSquaredDiff += (n - mean) * (n - mean);
        }
        float variance = sumSquaredDiff / numbers.Count;
        
        // Actualizar texto
        string resultString = $"Estadísticas de la muestra (n={numbers.Count}):\n" +
                             $"Media: {mean:F6}\n" +
                             $"Varianza: {variance:F6}\n" +
                             $"Teórica: 0.0833 (1/12)\n" +
                             $"Mín: {min:F6}\n" +
                             $"Máx: {max:F6}\n" +
                             $"Ver consola para resultados completos";
                             
        resultsText.text = resultString;
    }
    
    /// <summary>
    /// Obtiene números aleatorios del generador validado.
    /// Este método puede ser utilizado por otros componentes que necesiten números aleatorios.
    /// </summary>
    public float GetRandomValue()
    {
        return _validatedRandom.Random();
    }
    
    /// <summary>
    /// Obtiene un entero aleatorio en el rango [min, max] (inclusivo).
    /// </summary>
    public int GetRandomInt(int min, int max)
    {
        return _validatedRandom.RandomInt(min, max);
    }
}