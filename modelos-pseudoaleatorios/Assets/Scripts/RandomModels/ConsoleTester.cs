using System;
using System.Collections.Generic;
using RandomModels.Core;
using RandomModels.StatisticalTests;

namespace RandomModels
{
    /// <summary>
    /// Programa de consola para probar el generador de números aleatorios
    /// sin necesidad de ejecutarlo dentro del editor de Unity.
    /// </summary>
    public class ConsoleTester
    {
        // Parámetros de la prueba
        private const int NumTests = 3;
        private const int SampleSize = 10000;
        private const float Alpha = 0.05f;

        public static void Main(string[] args)
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine("PRUEBA DEL GENERADOR DE NÚMEROS ALEATORIOS (Consola)");
            Console.WriteLine("=====================================================");
            
            // Ejecutar pruebas para diferentes semillas
            for (int i = 0; i < NumTests; i++)
            {
                int seed = (i + 1) * 12345;
                Console.WriteLine($"\n[Prueba #{i+1}] Semilla: {seed}");
                
                // Pruebas con generador directo
                TestGenerator(seed, "LinearCongruenceRandom");
                
                // Pruebas con generador validado
                TestGenerator(seed, "ValidatedRandom");
            }
            
            // Demostración adicional
            DemonstrateRandomFeatures();
            
            Console.WriteLine("\n¡Pruebas completadas con éxito!");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
        
        /// <summary>
        /// Prueba un generador específico con la semilla proporcionada
        /// </summary>
        /// <param name="seed">Semilla para el generador</param>
        /// <param name="generatorType">Tipo de generador a probar</param>
        private static void TestGenerator(int seed, string generatorType)
        {
            Console.WriteLine($"\n  Probando {generatorType}...");
            
            // Generar la secuencia de números
            List<float> numbers = new List<float>(SampleSize);
            
            if (generatorType == "LinearCongruenceRandom")
            {
                var generator = new LinearCongruenceRandom(seedValue: seed);
                for (int i = 0; i < SampleSize; i++)
                {
                    numbers.Add(generator.Random());
                }
            }
            else if (generatorType == "ValidatedRandom") 
            {
                var generator = new ValidatedRandom(seed: seed);
                for (int i = 0; i < SampleSize; i++) 
                {
                    numbers.Add(generator.Random());
                }
            }
            
            // Calcular estadísticas básicas
            float sum = 0;
            float min = float.MaxValue;
            float max = float.MinValue;
            
            foreach (float n in numbers)
            {
                sum += n;
                min = Math.Min(min, n);
                max = Math.Max(max, n);
            }
            
            float mean = sum / SampleSize;
            
            // Calcular varianza
            float sumSquaredDiff = 0;
            foreach (float n in numbers)
            {
                sumSquaredDiff += (n - mean) * (n - mean);
            }
            float variance = sumSquaredDiff / SampleSize;
            
            Console.WriteLine($"    Media: {mean:F6} (Teórica: 0.5)");
            Console.WriteLine($"    Varianza: {variance:F6} (Teórica: 0.0833)");
            Console.WriteLine($"    Rango: [{min:F6}, {max:F6}]");
            
            // Ejecutar pruebas estadísticas
            RunStatisticalTests(numbers);
        }
        
        /// <summary>
        /// Ejecuta todas las pruebas estadísticas en un conjunto de números
        /// </summary>
        private static void RunStatisticalTests(List<float> numbers)
        {
            Console.WriteLine("\n    Pruebas estadísticas:");
            
            // Prueba Chi-Cuadrado
            var chiTest = new ChiSquareTest(numbers, alpha: Alpha);
            chiTest.EvaluateTest();
            Console.WriteLine($"      Chi-cuadrado: {(chiTest.Passed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                            $"(Valor: {chiTest.ChiSquareValue:F4}, Crítico: {chiTest.CriticalValue:F4})");
            
            // Prueba Kolmogorov-Smirnov
            var ksTest = new KsTest(numbers, alpha: Alpha);
            ksTest.CheckTest();
            Console.WriteLine($"      KS: {(ksTest.Passed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                            $"(Dmax: {ksTest.DMax:F4}, Crítico: {ksTest.DMaxCritical:F4})");
            
            // Prueba de Varianza
            var varianceTest = new VarianceTest(numbers, alpha: Alpha);
            varianceTest.EvaluateTest();
            Console.WriteLine($"      Varianza: {(varianceTest.Passed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                            $"(Valor: {varianceTest.Variance:F4}, Límites: [{varianceTest.LowerLimit:F4}, {varianceTest.UpperLimit:F4}])");
            
            // Prueba de Póker
            var pokerTest = new PokerTest(numbers, alpha: Alpha);
            pokerTest.CheckPoker();
            Console.WriteLine($"      Póker: {(pokerTest.Passed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                            $"(Valor: {pokerTest.TotalSum:F4}, Crítico: {pokerTest.ChiReverse:F4})");
            
            // Resultado final
            bool allPassed = chiTest.Passed && ksTest.Passed && 
                            varianceTest.Passed && pokerTest.Passed;
            Console.WriteLine($"\n    Resultado global: {(allPassed ? "TODAS LAS PRUEBAS PASARON ✓" : "ALGUNAS PRUEBAS FALLARON ✗")}");
        }
        
        /// <summary>
        /// Demuestra características adicionales del generador validado
        /// </summary>
        private static void DemonstrateRandomFeatures()
        {
            Console.WriteLine("\n====== DEMOSTRACIÓN DE FUNCIONALIDADES ======");
            
            var rng = new ValidatedRandom(seed: 12345);
            
            // Distribución uniforme en rangos personalizados
            Console.WriteLine("\nNúmeros entre 10 y 20:");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"  {rng.Uniform(10, 20):F4}");
            }
            
            // Distribución normal
            Console.WriteLine("\nNúmeros con distribución normal (µ=100, σ=15):");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"  {rng.Gauss(100, 15):F4}");
            }
            
            // Aleatorización de listas
            List<string> opciones = new List<string> { "A", "B", "C", "D", "E" };
            Console.WriteLine("\nMezcla aleatoria:");
            Console.WriteLine($"  Original: [{string.Join(", ", opciones)}]");
            
            rng.Shuffle(opciones);
            Console.WriteLine($"  Mezclada: [{string.Join(", ", opciones)}]");
            
            // Sampling sin reemplazo
            Console.WriteLine("\nMuestreo de 3 elementos (sin reemplazo):");
            var muestra = rng.Sample(opciones, 3);
            Console.WriteLine($"  Muestra: [{string.Join(", ", muestra)}]");
            
            // Sampling con reemplazo (simulado con Choice)
            Console.WriteLine("\nMuestreo con reemplazo (5 elecciones):");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"  Elección {i+1}: {rng.Choice(opciones)}");
            }
            
            // Prueba de reproducibilidad
            Console.WriteLine("\nPrueba de reproducibilidad con misma semilla:");
            var rng1 = new ValidatedRandom(seed: 54321);
            var rng2 = new ValidatedRandom(seed: 54321);
            
            for (int i = 0; i < 3; i++)
            {
                float n1 = rng1.Random();
                float n2 = rng2.Random();
                Console.WriteLine($"  Gen1: {n1:F8}, Gen2: {n2:F8}, Iguales: {Math.Abs(n1-n2) < 0.0000001}");
            }
        }
    }
}