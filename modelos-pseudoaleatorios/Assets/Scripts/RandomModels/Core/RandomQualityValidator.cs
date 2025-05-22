using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RandomModels.StatisticalTests;

namespace RandomModels.Core
{
    /// <summary>
    /// Clase para validar y demostrar la calidad de los números aleatorios generados.
    /// Utiliza todas las pruebas estadísticas para validar exhaustivamente la calidad.
    /// </summary>
    public class RandomQualityValidator : MonoBehaviour
    {
        [SerializeField] private int seedValue = 12345;
        [SerializeField] private int sampleSize = 10000;
        [SerializeField] private float significanceLevel = 0.05f;
        [SerializeField] private bool showDetailedResults = true;

        private ValidatedRandom _validatedRandom;
        private LinearCongruenceRandom _linearCongruenceRandom;
        private List<float> _validatedNumbers;
        private List<float> _directNumbers;
        
        // Resultados de las pruebas
        private bool _validatedChiSquarePassed;
        private bool _validatedKsPassed;
        private bool _validatedVariancePassed;
        private bool _validatedPokerPassed;
        
        private bool _directChiSquarePassed;
        private bool _directKsPassed;
        private bool _directVariancePassed;
        private bool _directPokerPassed;
        
        private float _validatedChiSquareValue;
        private float _validatedKsValue;
        private float _validatedVarianceValue;
        private float _validatedPokerValue;
        
        private float _directChiSquareValue;
        private float _directKsValue;
        private float _directVarianceValue;
        private float _directPokerValue;
        
        /// <summary>
        /// Inicializa los generadores y ejecuta las pruebas.
        /// </summary>
        private void Start()
        {
            // Crear instancias de los generadores
            _validatedRandom = new ValidatedRandom(seed: seedValue, batchSize: sampleSize, significanceLevel: significanceLevel);
            _linearCongruenceRandom = new LinearCongruenceRandom(seedValue: seedValue);
            
            // Generar muestras
            GenerateNumbers();
            
            // Ejecutar pruebas
            RunTests();
            
            // Mostrar resultados
            DisplayResults();
        }

        /// <summary>
        /// Genera dos conjuntos de números: uno con ValidatedRandom (pre-validado) 
        /// y otro directamente con LinearCongruenceRandom.
        /// </summary>
        private void GenerateNumbers()
        {
            _validatedNumbers = new List<float>(sampleSize);
            _directNumbers = new List<float>(sampleSize);
            
            for (int i = 0; i < sampleSize; i++)
            {
                _validatedNumbers.Add(_validatedRandom.Random());
                _directNumbers.Add(_linearCongruenceRandom.Random());
            }
        }

        /// <summary>
        /// Ejecuta todas las pruebas estadísticas en ambos conjuntos de números.
        /// </summary>
        private void RunTests()
        {
            // Prueba Chi-Cuadrado
            var validatedChiTest = new ChiSquareTest(_validatedNumbers, alpha: significanceLevel);
            validatedChiTest.EvaluateTest();
            _validatedChiSquarePassed = validatedChiTest.Passed;
            _validatedChiSquareValue = validatedChiTest.ChiSquareValue;
            
            var directChiTest = new ChiSquareTest(_directNumbers, alpha: significanceLevel);
            directChiTest.EvaluateTest();
            _directChiSquarePassed = directChiTest.Passed;
            _directChiSquareValue = directChiTest.ChiSquareValue;
            
            // Prueba Kolmogorov-Smirnov
            var validatedKsTest = new KsTest(_validatedNumbers, alpha: significanceLevel);
            validatedKsTest.CheckTest();
            _validatedKsPassed = validatedKsTest.Passed;
            _validatedKsValue = validatedKsTest.DMax;
            
            var directKsTest = new KsTest(_directNumbers, alpha: significanceLevel);
            directKsTest.CheckTest();
            _directKsPassed = directKsTest.Passed;
            _directKsValue = directKsTest.DMax;
            
            // Prueba de Varianza
            var validatedVarianceTest = new VarianceTest(_validatedNumbers, alpha: significanceLevel);
            validatedVarianceTest.EvaluateTest();
            _validatedVariancePassed = validatedVarianceTest.Passed;
            _validatedVarianceValue = validatedVarianceTest.Variance;
            
            var directVarianceTest = new VarianceTest(_directNumbers, alpha: significanceLevel);
            directVarianceTest.EvaluateTest();
            _directVariancePassed = directVarianceTest.Passed;
            _directVarianceValue = directVarianceTest.Variance;
            
            // Prueba de Póker
            var validatedPokerTest = new PokerTest(_validatedNumbers, alpha: significanceLevel);
            validatedPokerTest.CheckPoker();
            _validatedPokerPassed = validatedPokerTest.Passed;
            _validatedPokerValue = validatedPokerTest.TotalSum;
            
            var directPokerTest = new PokerTest(_directNumbers, alpha: significanceLevel);
            directPokerTest.CheckPoker();
            _directPokerPassed = directPokerTest.Passed;
            _directPokerValue = directPokerTest.TotalSum;
        }

        /// <summary>
        /// Muestra los resultados de las pruebas en la consola.
        /// </summary>
        private void DisplayResults()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("======= RESULTADOS DE VALIDACIÓN DE CALIDAD =======");
            sb.AppendLine($"Tamaño de la muestra: {sampleSize}");
            sb.AppendLine($"Nivel de significancia: {significanceLevel}");
            sb.AppendLine();
            
            sb.AppendLine("--- Generador con Validación Previa (ValidatedRandom) ---");
            sb.AppendLine($"Chi-cuadrado: {(_validatedChiSquarePassed ? "PASÓ ✓" : "FALLÓ ✗")}");
            sb.AppendLine($"Kolmogorov-Smirnov: {(_validatedKsPassed ? "PASÓ ✓" : "FALLÓ ✗")}");
            sb.AppendLine($"Varianza: {(_validatedVariancePassed ? "PASÓ ✓" : "FALLÓ ✗")}");
            sb.AppendLine($"Póker: {(_validatedPokerPassed ? "PASÓ ✓" : "FALLÓ ✗")}");
            
            if (showDetailedResults)
            {
                sb.AppendLine($"  Valor Chi-cuadrado: {_validatedChiSquareValue}");
                sb.AppendLine($"  Valor KS (Dmax): {_validatedKsValue}");
                sb.AppendLine($"  Valor Varianza: {_validatedVarianceValue}");
                sb.AppendLine($"  Valor Póker: {_validatedPokerValue}");
            }
            
            sb.AppendLine();
            sb.AppendLine("--- Generador Directo (LinearCongruenceRandom) ---");
            sb.AppendLine($"Chi-cuadrado: {(_directChiSquarePassed ? "PASÓ ✓" : "FALLÓ ✗")}");
            sb.AppendLine($"Kolmogorov-Smirnov: {(_directKsPassed ? "PASÓ ✓" : "FALLÓ ✗")}");
            sb.AppendLine($"Varianza: {(_directVariancePassed ? "PASÓ ✓" : "FALLÓ ✗")}");
            sb.AppendLine($"Póker: {(_directPokerPassed ? "PASÓ ✓" : "FALLÓ ✗")}");
            
            if (showDetailedResults)
            {
                sb.AppendLine($"  Valor Chi-cuadrado: {_directChiSquareValue}");
                sb.AppendLine($"  Valor KS (Dmax): {_directKsValue}");
                sb.AppendLine($"  Valor Varianza: {_directVarianceValue}");
                sb.AppendLine($"  Valor Póker: {_directPokerValue}");
            }
            
            sb.AppendLine("===================================================");
            
            Debug.Log(sb.ToString());
        }

        /// <summary>
        /// Regenera y valida nuevos conjuntos de números aleatorios.
        /// Puede ser llamado desde un botón en la interfaz de usuario.
        /// </summary>
        public void RegenerateAndValidate()
        {
            GenerateNumbers();
            RunTests();
            DisplayResults();
        }
        
        /// <summary>
        /// Compara la distribución de frecuencias de ambos generadores y muestra histogramas de los resultados.
        /// </summary>
        public void AnalyzeDistribution()
        {
            const int bins = 20;
            int[] validatedFrequency = new int[bins];
            int[] directFrequency = new int[bins];
            
            foreach (var n in _validatedNumbers)
            {
                int bin = Math.Min(bins - 1, (int)(n * bins));
                validatedFrequency[bin]++;
            }
            
            foreach (var n in _directNumbers)
            {
                int bin = Math.Min(bins - 1, (int)(n * bins));
                directFrequency[bin]++;
            }
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("===== DISTRIBUCIÓN DE FRECUENCIAS =====");
            sb.AppendLine($"Intervalos de tamaño {1.0/bins:F2}:");
            
            float expectedFreq = (float)sampleSize / bins;
            for (int i = 0; i < bins; i++)
            {
                float lowerBound = (float)i / bins;
                float upperBound = (float)(i + 1) / bins;
                
                string histoValidated = new string('█', (int)(validatedFrequency[i] * 50 / expectedFreq));
                string histoDirect = new string('█', (int)(directFrequency[i] * 50 / expectedFreq));
                
                sb.AppendLine($"[{lowerBound:F2}, {upperBound:F2}]");
                sb.AppendLine($"  Validado: {validatedFrequency[i],5} {histoValidated}");
                sb.AppendLine($"  Directo:  {directFrequency[i],5} {histoDirect}");
            }
            
            sb.AppendLine("======================================");
            Debug.Log(sb.ToString());
        }
    }
}