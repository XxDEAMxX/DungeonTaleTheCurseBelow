using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomModels.Core;
using RandomModels.StatisticalTests;

namespace RandomModels
{
    /// <summary>
    /// Componente de Unity para ejecutar y mostrar pruebas del generador de números aleatorios.
    /// Añade este script a cualquier GameObject en una escena de Unity para ejecutar pruebas.
    /// </summary>
    public class RandomTestRunner : MonoBehaviour
    {
        [Header("Configuración de Pruebas")]
        [SerializeField] private bool runOnStart = true;
        [SerializeField] private int sampleSize = 10000;
        [SerializeField] private int numberOfTests = 3;
        [SerializeField] private int initialSeed = 12345;
        
        [Header("Resultados")]
        [SerializeField] private bool showDetailedResults = true;
        
        void Start()
        {
            if (runOnStart)
            {
                RunAllTests();
            }
        }
        
        /// <summary>
        /// Ejecuta todas las pruebas configuradas
        /// </summary>
        public void RunAllTests()
        {
            Debug.Log("========= INICIANDO PRUEBAS DE GENERADOR =========");
            Debug.Log($"Tamaño de muestra: {sampleSize}");
            Debug.Log($"Número de pruebas: {numberOfTests}");
            Debug.Log("=================================================");
            
            int validatedPassCount = 0;
            int directPassCount = 0;
            
            // Ejecutar pruebas para cada semilla
            for (int i = 0; i < numberOfTests; i++)
            {
                int seed = initialSeed + i * 1000;
                Debug.Log($"\n[Prueba #{i+1}] Semilla: {seed}");
                
                // Probar generador validado
                Debug.Log("\n  --- Generador ValidatedRandom ---");
                bool validatedPassed = TestValidatedGenerator(seed);
                if (validatedPassed) validatedPassCount++;
                
                // Probar generador directo
                Debug.Log("\n  --- Generador LinearCongruenceRandom ---");
                bool directPassed = TestDirectGenerator(seed);
                if (directPassed) directPassCount++;
                
                // Mostrar resultado de esta prueba
                Debug.Log("\n  Resultado de esta prueba:");
                Debug.Log($"  Validado: {(validatedPassed ? "PASÓ ✓" : "FALLÓ ✗")}");
                Debug.Log($"  Directo: {(directPassed ? "PASÓ ✓" : "FALLÓ ✗")}");
            }
            
            // Mostrar estadísticas finales
            Debug.Log("\n========= RESULTADOS FINALES =========");
            float validatedPercent = (float)validatedPassCount / numberOfTests * 100;
            float directPercent = (float)directPassCount / numberOfTests * 100;
            
            Debug.Log($"Validado: {validatedPassCount}/{numberOfTests} pruebas exitosas ({validatedPercent:F1}%)");
            Debug.Log($"Directo: {directPassCount}/{numberOfTests} pruebas exitosas ({directPercent:F1}%)");
            
            // Mostrar demo de características adicionales
            DemoAdditionalFeatures();
        }
        
        /// <summary>
        /// Prueba el generador validado
        /// </summary>
        private bool TestValidatedGenerator(int seed)
        {
            // Crear generador y generar números
            var rng = new ValidatedRandom(seed: seed);
            var numbers = new List<float>(sampleSize);
            
            for (int i = 0; i < sampleSize; i++)
            {
                numbers.Add(rng.Random());
            }
            
            // Mostrar estadísticas básicas
            DisplayBasicStats(numbers);
            
            // Ejecutar y mostrar pruebas estadísticas
            return RunStatisticalTests(numbers);
        }
        
        /// <summary>
        /// Prueba el generador directo de congruencia lineal
        /// </summary>
        private bool TestDirectGenerator(int seed)
        {
            // Crear generador y generar números
            var rng = new LinearCongruenceRandom(seedValue: seed);
            var numbers = new List<float>(sampleSize);
            
            for (int i = 0; i < sampleSize; i++)
            {
                numbers.Add(rng.Random());
            }
            
            // Mostrar estadísticas básicas
            DisplayBasicStats(numbers);
            
            // Ejecutar y mostrar pruebas estadísticas
            return RunStatisticalTests(numbers);
        }
        
        /// <summary>
        /// Muestra estadísticas básicas de una lista de números
        /// </summary>
        private void DisplayBasicStats(List<float> numbers)
        {
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
            
            // Mostrar resultados
            Debug.Log($"    Media: {mean:F6} (Teórica: 0.5)");
            Debug.Log($"    Varianza: {variance:F6} (Teórica: 0.0833)");
            Debug.Log($"    Rango: [{min:F6}, {max:F6}]");
        }
        
        /// <summary>
        /// Ejecuta todas las pruebas estadísticas en una lista de números
        /// </summary>
        private bool RunStatisticalTests(List<float> numbers)
        {
            Debug.Log("    Pruebas estadísticas:");
            
            // Prueba Chi-Cuadrado
            var chiTest = new ChiSquareTest(numbers);
            chiTest.EvaluateTest();
            bool chiPassed = chiTest.Passed;
            
            Debug.Log($"      Chi-cuadrado: {(chiPassed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                    $"(Valor: {chiTest.ChiSquareValue:F4}, Crítico: {chiTest.CriticalValue:F4})");
            
            // Prueba KS
            var ksTest = new KsTest(numbers);
            ksTest.CheckTest();
            bool ksPassed = ksTest.Passed;
            
            Debug.Log($"      KS: {(ksPassed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                    $"(Dmax: {ksTest.DMax:F4}, Crítico: {ksTest.DMaxCritical:F4})");
            
            // Prueba de Varianza
            var varianceTest = new VarianceTest(numbers);
            varianceTest.EvaluateTest();
            bool variancePassed = varianceTest.Passed;
            
            Debug.Log($"      Varianza: {(variancePassed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                    $"(Valor: {varianceTest.Variance:F4}, Límites: [{varianceTest.LowerLimit:F4}, {varianceTest.UpperLimit:F4}])");
            
            // Prueba de Póker
            var pokerTest = new PokerTest(numbers);
            pokerTest.CheckPoker();
            bool pokerPassed = pokerTest.Passed;
            
            Debug.Log($"      Póker: {(pokerPassed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                    $"(Valor: {pokerTest.TotalSum:F4}, Crítico: {pokerTest.ChiReverse:F4})");
            
            // Resultado final
            bool allPassed = chiPassed && ksPassed && variancePassed && pokerPassed;
            
            return allPassed;
        }
        
        /// <summary>
        /// Demuestra características adicionales del generador
        /// </summary>
        private void DemoAdditionalFeatures()
        {
            Debug.Log("\n========= DEMOSTRACIÓN DE CARACTERÍSTICAS =========");
            
            // Crear generador con semilla fija para reproducibilidad
            var rng = new ValidatedRandom(seed: 12345);
            
            // 1. Números en rango [0,1)
            Debug.Log("\n5 números aleatorios en [0,1):");
            for (int i = 0; i < 5; i++)
            {
                Debug.Log($"  {rng.Random():F8}");
            }
            
            // 2. Números enteros (simulación de dado)
            Debug.Log("\nSimulación de 10 tiradas de dado (1-6):");
            int[] diceFreq = new int[6];
            for (int i = 0; i < 10; i++)
            {
                int roll = rng.RandomInt(1, 6);
                diceFreq[roll - 1]++;
                Debug.Log($"  Tirada {i+1}: {roll}");
            }
            
            // 3. Distribución uniforme personalizada
            Debug.Log("\n5 números en distribución uniforme [10,20]:");
            for (int i = 0; i < 5; i++)
            {
                Debug.Log($"  {rng.Uniform(10, 20):F4}");
            }
            
            // 4. Distribución normal
            Debug.Log("\n5 números en distribución normal (µ=50, σ=10):");
            for (int i = 0; i < 5; i++) 
            {
                Debug.Log($"  {rng.Gauss(50, 10):F4}");
            }
            
            // 5. Elección aleatoria
            List<string> frutas = new List<string> { "Manzana", "Naranja", "Plátano", "Uvas", "Kiwi" };
            Debug.Log("\nSelección aleatoria de 4 frutas:");
            for (int i = 0; i < 4; i++)
            {
                Debug.Log($"  Selección {i+1}: {rng.Choice(frutas)}");
            }
            
            // 6. Mezcla aleatoria
            Debug.Log($"\nLista original: [{string.Join(", ", frutas)}]");
            rng.Shuffle(frutas);
            Debug.Log($"Lista mezclada: [{string.Join(", ", frutas)}]");
            
            // 7. Prueba de reproducibilidad
            Debug.Log("\nPrueba de reproducibilidad (misma semilla = misma secuencia):");
            var rng1 = new ValidatedRandom(seed: 54321);
            var rng2 = new ValidatedRandom(seed: 54321);
            
            for (int i = 0; i < 3; i++)
            {
                float val1 = rng1.Random();
                float val2 = rng2.Random();
                bool equal = Mathf.Approximately(val1, val2);
                Debug.Log($"  {val1:F8} vs {val2:F8}: {(equal ? "Iguales ✓" : "Diferentes ✗")}");
            }
            
            Debug.Log("\n========= FIN DE PRUEBAS =========");
        }
    }
}