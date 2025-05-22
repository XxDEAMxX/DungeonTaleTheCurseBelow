using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Necesario para Image, Button
using TMPro; // Necesario para TMP_Text
using RandomModels.Core;
using RandomModels.StatisticalTests;

public class RandomQualityMonitor : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private RectTransform histogramContainer;
    [SerializeField] private TMP_Text resultsText;
    [SerializeField] private Button testButton;

    [Header("Configuración")]
    [SerializeField] private int sampleSize = 10000;
    [SerializeField] private int histogramBins = 20;

    private RectTransform[] histogramBars;

    private void Start()
    {
        if (histogramContainer != null)
        {
            InitializeHistogram();
        }
        else
        {
            Debug.LogWarning("Histogram Container no asignado en RandomQualityMonitor.");
        }

        if (testButton != null)
        {
            testButton.onClick.AddListener(RunQualityTest);
        }
        else
        {
            Debug.LogWarning("Test Button no asignado en RandomQualityMonitor.");
        }
        
        if (resultsText == null)
        {
            Debug.LogWarning("Results Text no asignado en RandomQualityMonitor.");
        }
    }

    public void RunQualityTest()
    {
        Debug.Log("RandomQualityMonitor: Ejecutando prueba de calidad...");
        List<float> sample = new List<float>(sampleSize);
        for (int i = 0; i < sampleSize; i++)
        {
            sample.Add(CustomRandom.value); // Prueba CustomRandom por defecto
        }

        if (histogramContainer != null && histogramBars != null)
        {
            UpdateHistogram(sample);
        }

        bool allPassed = RunStatisticalTests(sample);
        UpdateResultsText(sample, allPassed);
        Debug.Log($"RandomQualityMonitor: Prueba de calidad completada. ¿Todas pasaron?: {allPassed}");
    }

    private void InitializeHistogram()
    {
        foreach (Transform child in histogramContainer)
        {
            Destroy(child.gameObject);
        }

        histogramBars = new RectTransform[histogramBins];
        if (histogramContainer.rect.width <= 0) {
            Debug.LogError("El ancho del contenedor del histograma es 0 o negativo. No se pueden crear barras.");
            return;
        }
        float barWidth = histogramContainer.rect.width / histogramBins;


        for (int i = 0; i < histogramBins; i++)
        {
            GameObject barObj = new GameObject($"Bar_{i}");
            barObj.transform.SetParent(histogramContainer, false);

            Image barImage = barObj.AddComponent<Image>();
            barImage.color = new Color(0.2f, 0.6f, 1.0f, 0.8f);

            RectTransform rectTransform = barObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(barWidth - 2, 10);
            rectTransform.anchoredPosition = new Vector2(i * barWidth + 1, 0);

            histogramBars[i] = rectTransform;
        }
    }

    private void UpdateHistogram(List<float> numbers)
    {
        if (numbers == null || numbers.Count == 0 || histogramBars == null) return;

        int[] frequencies = new int[histogramBins];
        foreach (float n in numbers)
        {
            int bin = Mathf.Clamp(Mathf.FloorToInt(n * histogramBins), 0, histogramBins - 1);
            frequencies[bin]++;
        }

        int maxFreq = 0;
        foreach (int freq in frequencies)
        {
            if (freq > maxFreq) maxFreq = freq;
        }
        if (maxFreq == 0) maxFreq = 1;

        if (histogramContainer.rect.height <= 0) {
             Debug.LogWarning("La altura del contenedor del histograma es 0 o negativa. Las barras no serán visibles.");
             return;
        }
        float maxHeight = histogramContainer.rect.height;

        for (int i = 0; i < histogramBins; i++)
        {
            if (histogramBars[i] == null) continue;
            float normalizedHeight = (float)frequencies[i] / maxFreq;
            float height = normalizedHeight * maxHeight;
            histogramBars[i].sizeDelta = new Vector2(histogramBars[i].sizeDelta.x, Mathf.Max(5, height));
        }
    }

    private bool RunStatisticalTests(List<float> numbers)
    {
        if (numbers == null || numbers.Count == 0) {
            Debug.LogWarning("No hay números para realizar pruebas estadísticas.");
            return false;
        }

        var chiTest = new ChiSquareTest(numbers); chiTest.EvaluateTest();
        var ksTest = new KsTest(numbers); ksTest.CheckTest(); // Asumiendo que CheckTest() es el método principal
        var varianceTest = new VarianceTest(numbers); varianceTest.EvaluateTest();
        var pokerTest = new PokerTest(numbers); pokerTest.CheckPoker(); // Asumiendo que CheckPoker() es el método principal

        bool allPassed = chiTest.Passed && ksTest.Passed && varianceTest.Passed && pokerTest.Passed;

        // Modificamos el log para mostrar los valores disponibles
        string logMessage = $"Pruebas de calidad (muestra de {numbers.Count}):\n" +
                            $"Chi-cuadrado: {(chiTest.Passed ? "PASÓ ✓" : "FALLÓ ✗")} (Valor: {chiTest.ChiSquareValue:F2}, Crítico: {chiTest.CriticalValue:F2})\n" +
                            $"KS: {(ksTest.Passed ? "PASÓ ✓" : "FALLÓ ✗")} (DMax: {ksTest.DMax:F4}, Crítico: {ksTest.DMaxCritical:F4})\n" +
                            $"Varianza: {(varianceTest.Passed ? "PASÓ ✓" : "FALLÓ ✗")} (Varianza: {varianceTest.Variance:F4}, Limites: [{varianceTest.LowerLimit:F4} - {varianceTest.UpperLimit:F4}])\n" +
                            $"Póker: {(pokerTest.Passed ? "PASÓ ✓" : "FALLÓ ✗")} (Suma Chi: {pokerTest.TotalSum:F2}, Crítico: {pokerTest.ChiReverse:F2})";
        Debug.Log(logMessage);
        return allPassed;
    }

    private void UpdateResultsText(List<float> numbers, bool allTestsPassed)
    {
        if (resultsText == null) return;
        if (numbers == null || numbers.Count == 0) {
            resultsText.text = "No hay datos para mostrar.";
            return;
        }

        float sum = 0; float minVal = float.MaxValue; float maxVal = float.MinValue;
        foreach (float n in numbers) { sum += n; minVal = Mathf.Min(minVal, n); maxVal = Mathf.Max(maxVal, n); }
        float mean = sum / numbers.Count;
        float sumSquaredDiff = 0;
        foreach (float n in numbers) { sumSquaredDiff += (n - mean) * (n - mean); }
        float variance = (numbers.Count > 1) ? sumSquaredDiff / (numbers.Count -1) : 0; // Varianza muestral

        string statusSymbol = allTestsPassed ? "<color=green>✓ TODAS PASARON</color>" : "<color=red>✗ ALGUNA FALLÓ</color>";
        resultsText.text = $"<b>Resultados (n={numbers.Count})</b>\n" +
                           $"Media: {mean:F4} (Ideal: 0.5)\n" +
                           $"Varianza Muestral: {variance:F4} (Ideal Dist. Uni[0,1]: ~0.0833)\n" +
                           $"Mínimo: {minVal:F4}\n" +
                           $"Máximo: {maxVal:F4}\n" +
                           $"Pruebas Estadísticas: {statusSymbol}";
    }
}
