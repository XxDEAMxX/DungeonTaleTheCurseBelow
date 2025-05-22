using System;
using System.Collections.Generic;
using System.Text;
using RandomModels.Core;
using RandomModels.StatisticalTests;
using UnityEngine;

namespace RandomModels
{
    /// <summary>
    /// Clase independiente para probar el rendimiento y la calidad del generador de números aleatorios.
    /// Esta clase puede ejecutarse sin necesidad de estar en una escena de Unity.
    /// </summary>
    public class RandomTester
    {
        // Parámetros de la prueba
        private const int NumTests = 5;         // Número de pruebas a realizar
        private const int SampleSize = 10000;   // Tamaño de la muestra para cada prueba
        private const float Alpha = 0.05f;      // Nivel de significancia
        
        // Contadores de pruebas exitosas
        private int _validatedPassCount = 0;
        private int _directPassCount = 0;

        /// <summary>
        /// Ejecuta todas las pruebas y muestra estadísticas detalladas.
        /// </summary>
        public void RunAllTests()
        {
            Debug.Log("========= INICIANDO PRUEBAS DE CALIDAD DEL GENERADOR =========");
            Debug.Log($"Ejecutando {NumTests} pruebas con muestras de {SampleSize} números");
            Debug.Log("-----------------------------------------------------------");

            for (int i = 0; i < NumTests; i++)
            {
                // Usar semilla diferente para cada prueba
                int seed = (i + 1) * 12345;
                
                Debug.Log($"\n[Prueba #{i+1}] Semilla: {seed}");
                
                // Probar el generador validado
                var validatedResults = TestValidatedGenerator(seed);
                if (validatedResults.allPassed) _validatedPassCount++;
                
                // Probar el generador directo
                var directResults = TestDirectGenerator(seed);
                if (directResults.allPassed) _directPassCount++;
                
                // Mostrar comparativa
                Debug.Log("\n--- Comparativa de pruebas ---");
                Debug.Log($"Validado: {(validatedResults.allPassed ? "TODOS PASARON ✓" : "ALGUNAS FALLARON ✗")}");
                Debug.Log($"Directo:  {(directResults.allPassed ? "TODOS PASARON ✓" : "ALGUNAS FALLARON ✗")}");
            }
            
            // Mostrar estadísticas finales
            Debug.Log("\n========= RESULTADOS FINALES =========");
            Debug.Log($"Validado: {_validatedPassCount}/{NumTests} pruebas exitosas ({(float)_validatedPassCount/NumTests*100}%)");
            Debug.Log($"Directo: {_directPassCount}/{NumTests} pruebas exitosas ({(float)_directPassCount/NumTests*100}%)");
            
            // Demostrar generación de valores en diferentes rangos
            DemonstrateRandomGeneration();
        }
        
        /// <summary>
        /// Prueba el generador validado y devuelve los resultados de las pruebas estadísticas.
        /// </summary>
        private (bool allPassed, bool chiPassed, bool ksPassed, bool variancePassed, bool pokerPassed) 
            TestValidatedGenerator(int seed)
        {
            Debug.Log("  Probando ValidatedRandom...");
            
            // Inicializar generador con semilla
            var rng = new ValidatedRandom(seed: seed, batchSize: SampleSize);
            var numbers = new List<float>(SampleSize);
            
            // Generar muestra
            for (int i = 0; i < SampleSize; i++)
            {
                numbers.Add(rng.Random());
            }
            
            // Ejecutar pruebas estadísticas
            return RunStatisticalTests(numbers, "    Validado");
        }
        
        /// <summary>
        /// Prueba el generador directo y devuelve los resultados de las pruebas estadísticas.
        /// </summary>
        private (bool allPassed, bool chiPassed, bool ksPassed, bool variancePassed, bool pokerPassed) 
            TestDirectGenerator(int seed)
        {
            Debug.Log("  Probando LinearCongruenceRandom...");
            
            // Inicializar generador con semilla
            var rng = new LinearCongruenceRandom(seedValue: seed);
            var numbers = new List<float>(SampleSize);
            
            // Generar muestra
            for (int i = 0; i < SampleSize; i++)
            {
                numbers.Add(rng.Random());
            }
            
            // Ejecutar pruebas estadísticas
            return RunStatisticalTests(numbers, "    Directo");
        }
        
        /// <summary>
        /// Ejecuta todas las pruebas estadísticas en un conjunto de números.
        /// </summary>
        private (bool allPassed, bool chiPassed, bool ksPassed, bool variancePassed, bool pokerPassed) 
            RunStatisticalTests(List<float> numbers, string prefix)
        {
            // Prueba Chi-Cuadrado
            var chiTest = new ChiSquareTest(numbers, alpha: Alpha);
            chiTest.EvaluateTest();
            bool chiPassed = chiTest.Passed;
            
            // Prueba Kolmogorov-Smirnov
            var ksTest = new KsTest(numbers, alpha: Alpha);
            ksTest.CheckTest();
            bool ksPassed = ksTest.Passed;
            
            // Prueba de Varianza
            var varianceTest = new VarianceTest(numbers, alpha: Alpha);
            varianceTest.EvaluateTest();
            bool variancePassed = varianceTest.Passed;
            
            // Prueba de Póker
            var pokerTest = new PokerTest(numbers, alpha: Alpha);
            pokerTest.CheckPoker();
            bool pokerPassed = pokerTest.Passed;
            
            // Calcular estadísticas básicas
            float mean = 0;
            foreach (var n in numbers) mean += n;
            mean /= numbers.Count;
            
            float variance = 0;
            foreach (var n in numbers) variance += (n - mean) * (n - mean);
            variance /= numbers.Count;
            
            // Mostrar resultados
            Debug.Log($"{prefix} | Media: {mean:F6}, Varianza: {variance:F6}");
            Debug.Log($"{prefix} | Chi-cuadrado: {(chiPassed ? "PASÓ ✓" : "FALLÓ ✗")} ({chiTest.ChiSquareValue:F4})");
            Debug.Log($"{prefix} | KS: {(ksPassed ? "PASÓ ✓" : "FALLÓ ✗")} ({ksTest.DMax:F4})");
            Debug.Log($"{prefix} | Varianza: {(variancePassed ? "PASÓ ✓" : "FALLÓ ✗")} ({varianceTest.Variance:F4})");
            Debug.Log($"{prefix} | Póker: {(pokerPassed ? "PASÓ ✓" : "FALLÓ ✗")} ({pokerTest.TotalSum:F4})");
            
            bool allPassed = chiPassed && ksPassed && variancePassed && pokerPassed;
            return (allPassed, chiPassed, ksPassed, variancePassed, pokerPassed);
        }
        
        /// <summary>
        /// Demuestra la generación de valores aleatorios en diferentes rangos.
        /// </summary>
        private void DemonstrateRandomGeneration()
        {
            Debug.Log("\n========= DEMOSTRACIÓN DE GENERACIÓN =========");
            
            // Crear instancia del generador validado
            var rng = new ValidatedRandom(seed: 12345);
            
            // 1. Generar números flotantes entre 0 y 1
            Debug.Log("10 números aleatorios en [0, 1):");
            for (int i = 0; i < 10; i++)
            {
                Debug.Log($"  {rng.Random():F8}");
            }
            
            // 2. Generar enteros en un rango
            Debug.Log("\n20 enteros aleatorios entre 1 y 6 (simulación de dado):");
            var frequencies = new int[6];
            for (int i = 0; i < 20; i++)
            {
                int dice = rng.RandomInt(1, 6);
                frequencies[dice - 1]++;
                Debug.Log($"  Tirada {i+1}: {dice}");
            }
            
            // Mostrar frecuencias
            Debug.Log("\nFrecuencias del dado:");
            for (int i = 0; i < 6; i++)
            {
                string bar = new string('*', frequencies[i]);
                Debug.Log($"  {i+1}: {frequencies[i]} {bar}");
            }
            
            // 3. Distribución normal
            Debug.Log("\n10 números con distribución normal (media=0, desv=1):");
            for (int i = 0; i < 10; i++)
            {
                Debug.Log($"  {rng.Gauss():F8}");
            }
            
            // 4. Elección aleatoria
            List<string> options = new List<string> { "Rojo", "Verde", "Azul", "Amarillo", "Negro" };
            Debug.Log("\nSelección aleatoria de colores:");
            for (int i = 0; i < 5; i++)
            {
                string selected = rng.Choice(options);
                Debug.Log($"  Elección {i+1}: {selected}");
            }
            
            // 5. Demostrar que semillas iguales producen las mismas secuencias
            Debug.Log("\nVerificar reproducibilidad con semillas iguales:");
            var rng1 = new ValidatedRandom(seed: 54321);
            var rng2 = new ValidatedRandom(seed: 54321);
            
            List<float> numbers1 = new List<float>();
            List<float> numbers2 = new List<float>();
            
            for (int i = 0; i < 5; i++)
            {
                float n1 = rng1.Random();
                float n2 = rng2.Random();
                numbers1.Add(n1);
                numbers2.Add(n2);
            }
            
            for (int i = 0; i < 5; i++)
            {
                Debug.Log($"  Secuencia 1: {numbers1[i]:F8}, Secuencia 2: {numbers2[i]:F8}, " +
                          $"Iguales: {Math.Abs(numbers1[i] - numbers2[i]) < 0.00001}");
            }
            
            Debug.Log("\n========= PRUEBAS COMPLETADAS =========");
        }
    }
}