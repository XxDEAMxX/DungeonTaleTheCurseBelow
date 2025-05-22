using System;
using System.Collections.Generic;

namespace RandomTester
{
    // Clases de núcleo
    public interface IPRNG
    {
        float Random();
    }
    
    public class LinearCongruenceRandom : IPRNG
    {
        private const long a = 630360016;
        // Reemplazamos Math.Pow(2, 31) - 1 por su valor explícito: 2147483647
        private const long m = 2147483647; // (2^31 - 1)
        private const long c = 0;
        private long seed;
        
        public LinearCongruenceRandom(int seedValue = 0)
        {
            seed = seedValue == 0 ? DateTime.Now.Ticks % m : seedValue % m;
            if (seed == 0) seed = 1;  // La semilla nunca debe ser 0
        }
        
        public float Random()
        {
            seed = (a * seed + c) % m;
            return (float)seed / m;
        }
        
        public int RandomInt(int min, int max)
        {
            return min + (int)(Random() * (max - min + 1));
        }
        
        public double Uniform(double min, double max)
        {
            return min + Random() * (max - min);
        }
    }

    public class ValidatedRandom : IPRNG
    {
        private LinearCongruenceRandom generator;
        
        public ValidatedRandom(int seed = 0)
        {
            generator = new LinearCongruenceRandom(seed);
        }
        
        public float Random()
        {
            return generator.Random();
        }
        
        public int RandomInt(int min, int max)
        {
            return min + (int)(Random() * (max - min + 1));
        }
        
        public double Uniform(double min, double max) 
        {
            return min + Random() * (max - min);
        }
        
        public double Gauss(double mu = 0, double sigma = 1)
        {
            double u1 = Random();
            double u2 = Random();
            
            // Algoritmo Box-Muller
            double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mu + z0 * sigma;
        }
        
        public T Choice<T>(List<T> items)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("La lista está vacía");
                
            int index = (int)(Random() * items.Count);
            return items[index];
        }
        
        public void Shuffle<T>(List<T> items)
        {
            int n = items.Count;
            while (n > 1)
            {
                n--;
                int k = (int)(Random() * (n + 1));
                T value = items[k];
                items[k] = items[n];
                items[n] = value;
            }
        }
    }

    // Clases de pruebas estadísticas
    public class ChiSquareTest
    {
        private List<float> numbers;
        private int intervals;
        private double chiSquareValue;
        private double criticalValue;
        private bool passed;

        public ChiSquareTest(List<float> numbers, int intervals = 10)
        {
            this.numbers = numbers;
            this.intervals = intervals;
            EvaluateTest();
        }

        public bool EvaluateTest()
        {
            // Frecuencia esperada para cada intervalo
            double expectedFreq = numbers.Count / (double)intervals;
            
            // Contadores para cada intervalo
            int[] observed = new int[intervals];
            
            // Clasificar los números
            foreach (float num in numbers)
            {
                int intervalIndex = (int)(num * intervals);
                if (intervalIndex >= intervals) intervalIndex = intervals - 1;
                observed[intervalIndex]++;
            }
            
            // Calcular Chi-Cuadrado
            chiSquareValue = 0;
            for (int i = 0; i < intervals; i++)
            {
                chiSquareValue += Math.Pow(observed[i] - expectedFreq, 2) / expectedFreq;
            }
            
            // Valor crítico para α = 0.05 y df = intervals - 1
            // Esto es una aproximación simplificada
            criticalValue = intervals - 1 + Math.Sqrt(2 * (intervals - 1));
            
            passed = chiSquareValue < criticalValue;
            return passed;
        }

        public double ChiSquareValue => chiSquareValue;
        public double CriticalValue => criticalValue;
        public bool Passed => passed;
    }

    public class KsTest
    {
        private List<float> numbers;
        public double dmax;
        public double dmax_critical;
        private bool passed;

        public KsTest(List<float> numbers)
        {
            this.numbers = numbers;
            CheckTest();
        }

        public bool CheckTest()
        {
            int n = numbers.Count;
            
            // Ordenar los números
            List<float> sortedNumbers = new List<float>(numbers);
            sortedNumbers.Sort();
            
            // Calcular D+ y D-
            double dplus = 0;
            double dminus = 0;
            
            for (int i = 0; i < n; i++)
            {
                double expected = (i + 1.0) / n;
                double observed = sortedNumbers[i];
                
                double diffPlus = expected - observed;
                double diffMinus = observed - (i) / (double)n;
                
                dplus = Math.Max(dplus, diffPlus);
                dminus = Math.Max(dminus, diffMinus);
            }
            
            // Obtener D_max
            dmax = Math.Max(dplus, dminus);
            
            // Valor crítico para α = 0.05
            dmax_critical = 1.36 / Math.Sqrt(n);
            
            passed = dmax < dmax_critical;
            return passed;
        }

        public bool Passed => passed;
    }

    public class VarianceTest
    {
        private List<float> numbers;
        private double variance;
        private double lowerLimit;
        private double upperLimit;
        private bool passed;

        public VarianceTest(List<float> numbers)
        {
            this.numbers = numbers;
            EvaluateTest();
        }

        public bool EvaluateTest()
        {
            int n = numbers.Count;
            
            // Calcular media
            double mean = 0;
            foreach (var num in numbers)
            {
                mean += num;
            }
            mean /= n;
            
            // Calcular varianza
            variance = 0;
            foreach (var num in numbers)
            {
                variance += Math.Pow(num - mean, 2);
            }
            variance /= n;
            
            // Teóricamente, la varianza de una distribución uniforme [0,1] es 1/12 (≈ 0.08333)
            double theoreticalVariance = 1.0 / 12.0;
            
            // Establecer límites para α = 0.025 en cada cola (total 0.05)
            // Esto es una aproximación
            double z = 1.96;  // Valor z para un 95% de confianza
            double factor = z * Math.Sqrt(2.0 / n);
            
            lowerLimit = theoreticalVariance * (1 - factor);
            upperLimit = theoreticalVariance * (1 + factor);
            
            passed = variance >= lowerLimit && variance <= upperLimit;
            return passed;
        }

        public double Variance => variance;
        public double LowerLimit => lowerLimit;
        public double UpperLimit => upperLimit;
        public bool Passed => passed;
    }

    public class PokerTest
    {
        private List<float> numbers;
        private double totalSum;
        private double chiReverse;
        private bool passed;

        public PokerTest(List<float> numbers)
        {
            this.numbers = numbers;
            CheckPoker();
        }

        public bool CheckPoker()
        {
            int n = numbers.Count;
            
            // Convertir cada número a 3 decimales (000-999)
            int[] categories = new int[5];  // Diferentes categorías de "manos" de póker
            
            foreach (float num in numbers)
            {
                int value = (int)(num * 1000);
                
                // Convertir a los 3 dígitos
                int d1 = value / 100;
                int d2 = (value / 10) % 10;
                int d3 = value % 10;
                
                // Clasificar en categoría
                if (d1 == d2 && d2 == d3)
                {
                    categories[0]++;  // Todos iguales (AAA)
                }
                else if (d1 == d2 || d2 == d3 || d1 == d3)
                {
                    categories[1]++;  // Un par (AAB)
                }
                else
                {
                    categories[2]++;  // Todos diferentes (ABC)
                }
            }
            
            // Probabilidades teóricas para 3 dígitos
            double[] probabilities = { 0.01, 0.27, 0.72 };
            
            // Calcular estadístico Chi-cuadrado
            totalSum = 0;
            for (int i = 0; i < 3; i++)
            {
                double expected = n * probabilities[i];
                if (expected > 0)
                {
                    totalSum += Math.Pow(categories[i] - expected, 2) / expected;
                }
            }
            
            // Valor crítico para α = 0.05 y df = 2
            chiReverse = 5.991;
            
            passed = totalSum < chiReverse;
            return passed;
        }

        public double TotalSum => totalSum;
        public double ChiReverse => chiReverse;
        public bool Passed => passed;
    }

    // Clase principal de pruebas
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========= PRUEBA DE CALIDAD DEL GENERADOR DE NÚMEROS ALEATORIOS =========");
            
            // Configuración de pruebas
            int sampleSize = 10000;
            int numberOfTests = 3;
            int initialSeed = 12345;
            
            Console.WriteLine($"Tamaño de muestra: {sampleSize}");
            Console.WriteLine($"Número de pruebas: {numberOfTests}");
            Console.WriteLine("=================================================");
            
            int validatedPassCount = 0;
            int directPassCount = 0;
            
            // Ejecutar pruebas para cada semilla
            for (int i = 0; i < numberOfTests; i++)
            {
                int seed = initialSeed + i * 1000;
                Console.WriteLine($"\n[Prueba #{i+1}] Semilla: {seed}");
                
                // Probar generador validado
                Console.WriteLine("\n  --- Generador ValidatedRandom ---");
                bool validatedPassed = TestValidatedGenerator(seed, sampleSize);
                if (validatedPassed) validatedPassCount++;
                
                // Probar generador directo
                Console.WriteLine("\n  --- Generador LinearCongruenceRandom ---");
                bool directPassed = TestDirectGenerator(seed, sampleSize);
                if (directPassed) directPassCount++;
                
                // Mostrar resultado de esta prueba
                Console.WriteLine("\n  Resultado de esta prueba:");
                Console.WriteLine($"  Validado: {(validatedPassed ? "PASÓ ✓" : "FALLÓ ✗")}");
                Console.WriteLine($"  Directo: {(directPassed ? "PASÓ ✓" : "FALLÓ ✗")}");
            }
            
            // Mostrar estadísticas finales
            Console.WriteLine("\n========= RESULTADOS FINALES =========");
            float validatedPercent = (float)validatedPassCount / numberOfTests * 100;
            float directPercent = (float)directPassCount / numberOfTests * 100;
            
            Console.WriteLine($"Validado: {validatedPassCount}/{numberOfTests} pruebas exitosas ({validatedPercent:F1}%)");
            Console.WriteLine($"Directo: {directPassCount}/{numberOfTests} pruebas exitosas ({directPercent:F1}%)");
            
            // Mostrar demo de características adicionales
            DemoAdditionalFeatures();
            
            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        static bool TestValidatedGenerator(int seed, int sampleSize)
        {
            // Crear generador y generar números
            var rng = new ValidatedRandom(seed);
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

        static bool TestDirectGenerator(int seed, int sampleSize)
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

        static void DisplayBasicStats(List<float> numbers)
        {
            float sum = 0;
            float min = float.MaxValue;
            float max = float.MinValue;
            
            foreach (float n in numbers)
            {
                sum += n;
                min = Math.Min(min, n);
                max = Math.Max(max, n);
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
            Console.WriteLine($"    Media: {mean:F6} (Teórica: 0.5)");
            Console.WriteLine($"    Varianza: {variance:F6} (Teórica: 0.0833)");
            Console.WriteLine($"    Rango: [{min:F6}, {max:F6}]");
        }

        static bool RunStatisticalTests(List<float> numbers)
        {
            Console.WriteLine("    Pruebas estadísticas:");
            
            // Prueba Chi-Cuadrado
            var chiTest = new ChiSquareTest(numbers);
            bool chiPassed = chiTest.Passed;
            
            Console.WriteLine($"      Chi-cuadrado: {(chiPassed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                    $"(Valor: {chiTest.ChiSquareValue:F4}, Crítico: {chiTest.CriticalValue:F4})");
            
            // Prueba KS
            var ksTest = new KsTest(numbers);
            bool ksPassed = ksTest.Passed;
            
            Console.WriteLine($"      KS: {(ksPassed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                    $"(Dmax: {ksTest.dmax:F4}, Crítico: {ksTest.dmax_critical:F4})");
            
            // Prueba de Varianza
            var varianceTest = new VarianceTest(numbers);
            bool variancePassed = varianceTest.Passed;
            
            Console.WriteLine($"      Varianza: {(variancePassed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                    $"(Valor: {varianceTest.Variance:F4}, Límites: [{varianceTest.LowerLimit:F4}, {varianceTest.UpperLimit:F4}])");
            
            // Prueba de Póker
            var pokerTest = new PokerTest(numbers);
            bool pokerPassed = pokerTest.Passed;
            
            Console.WriteLine($"      Póker: {(pokerPassed ? "PASÓ ✓" : "FALLÓ ✗")} " +
                    $"(Valor: {pokerTest.TotalSum:F4}, Crítico: {pokerTest.ChiReverse:F4})");
            
            // Resultado final
            bool allPassed = chiPassed && ksPassed && variancePassed && pokerPassed;
            
            return allPassed;
        }

        static void DemoAdditionalFeatures()
        {
            Console.WriteLine("\n========= DEMOSTRACIÓN DE CARACTERÍSTICAS =========");
            
            // Crear generador con semilla fija para reproducibilidad
            var rng = new ValidatedRandom(seed: 12345);
            
            // 1. Números en rango [0,1)
            Console.WriteLine("\n5 números aleatorios en [0,1):");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"  {rng.Random():F8}");
            }
            
            // 2. Números enteros (simulación de dado)
            Console.WriteLine("\nSimulación de 10 tiradas de dado (1-6):");
            int[] diceFreq = new int[6];
            for (int i = 0; i < 10; i++)
            {
                int roll = rng.RandomInt(1, 6);
                diceFreq[roll - 1]++;
                Console.WriteLine($"  Tirada {i+1}: {roll}");
            }
            
            // 3. Distribución uniforme personalizada
            Console.WriteLine("\n5 números en distribución uniforme [10,20]:");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"  {rng.Uniform(10, 20):F4}");
            }
            
            // 4. Distribución normal
            Console.WriteLine("\n5 números en distribución normal (µ=50, σ=10):");
            for (int i = 0; i < 5; i++) 
            {
                Console.WriteLine($"  {rng.Gauss(50, 10):F4}");
            }
            
            // 5. Elección aleatoria
            List<string> frutas = new List<string> { "Manzana", "Naranja", "Plátano", "Uvas", "Kiwi" };
            Console.WriteLine("\nSelección aleatoria de 4 frutas:");
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine($"  Selección {i+1}: {rng.Choice(frutas)}");
            }
            
            // 6. Mezcla aleatoria
            Console.WriteLine($"\nLista original: [{string.Join(", ", frutas)}]");
            rng.Shuffle(frutas);
            Console.WriteLine($"Lista mezclada: [{string.Join(", ", frutas)}]");
            
            // 7. Prueba de reproducibilidad
            Console.WriteLine("\nPrueba de reproducibilidad (misma semilla = misma secuencia):");
            var rng1 = new ValidatedRandom(seed: 54321);
            var rng2 = new ValidatedRandom(seed: 54321);
            
            for (int i = 0; i < 3; i++)
            {
                float val1 = rng1.Random();
                float val2 = rng2.Random();
                bool equal = Math.Abs(val1 - val2) < 1e-6;
                Console.WriteLine($"  {val1:F8} vs {val2:F8}: {(equal ? "Iguales ✓" : "Diferentes ✗")}");
            }
            
            Console.WriteLine("\n========= FIN DE PRUEBAS =========");
        }
    }
}
