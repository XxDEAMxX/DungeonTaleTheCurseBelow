using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomModels.StatisticalTests
{
    /// <summary>
    /// Implementa la prueba estadística Chi Cuadrado para evaluar uniformidad en secuencias de números aleatorios.
    /// </summary>
    public class ChiSquareTest
    {
        private readonly List<float> numbers;
        private readonly int nIntervals;
        private readonly int n;
        private List<(float Lower, float Upper)> intervals;
        private List<int> observedFreq;
        private float expectedFreq;
        private float chiSquare;
        private float criticalValue;
        private readonly float alpha;
        private bool passed;

        /// <summary>
        /// Inicializa una nueva instancia de la prueba Chi-cuadrado.
        /// </summary>
        /// <param name="numbers">Secuencia de números a evaluar</param>
        /// <param name="nIntervals">Número de intervalos para la prueba</param>
        /// <param name="alpha">Nivel de significancia de la prueba</param>
        public ChiSquareTest(List<float> numbers, int nIntervals = 10, float alpha = 0.05f)
        {
            this.numbers = new List<float>(numbers);
            this.nIntervals = nIntervals;
            this.n = numbers.Count;
            this.intervals = new List<(float Lower, float Upper)>();
            this.observedFreq = new List<int>();
            this.expectedFreq = (float)n / nIntervals;
            this.chiSquare = 0.0f;
            this.criticalValue = 0.0f;
            this.alpha = alpha;
            this.passed = false;
        }

        /// <summary>
        /// Resultado de la prueba.
        /// </summary>
        public bool Passed => passed;

        /// <summary>
        /// El valor Chi-cuadrado calculado.
        /// </summary>
        public float ChiSquareValue => chiSquare;

        /// <summary>
        /// El valor crítico para la prueba.
        /// </summary>
        public float CriticalValue => criticalValue;

        /// <summary>
        /// Calcula los intervalos para la prueba.
        /// </summary>
        private void CalculateIntervals()
        {
            float minVal = numbers.Min();
            float maxVal = numbers.Max();
            float intervalSize = (maxVal - minVal) / nIntervals;
            
            for (int i = 0; i < nIntervals; i++)
            {
                float lower = minVal + i * intervalSize;
                float upper = lower + intervalSize;
                intervals.Add((lower, upper));
            }
        }

        /// <summary>
        /// Calcula las frecuencias observadas en cada intervalo.
        /// </summary>
        private void CalculateFrequencies()
        {
            // Inicializar el contador de frecuencias
            observedFreq = new List<int>(new int[nIntervals]);
            
            foreach (float num in numbers)
            {
                for (int i = 0; i < intervals.Count; i++)
                {
                    var (lower, upper) = intervals[i];
                    if (num >= lower && (num < upper || (i == nIntervals - 1 && num == upper)))
                    {
                        observedFreq[i]++;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Calcula el estadístico chi cuadrado.
        /// </summary>
        private void CalculateChiSquare()
        {
            chiSquare = 0;
            foreach (int observed in observedFreq)
            {
                chiSquare += (float)Math.Pow(observed - expectedFreq, 2) / expectedFreq;
            }
        }

        /// <summary>
        /// Calcula el valor crítico de chi cuadrado.
        /// </summary>
        private void CalculateCriticalValue()
        {
            int degreesOfFreedom = nIntervals - 1;
            
            // Valores críticos aproximados comunes para la prueba chi-cuadrado
            // Para una aproximación más exacta, se podría implementar la función chi2inv
            Dictionary<int, float> criticalValues95 = new Dictionary<int, float>
            {
                { 1, 3.84f }, { 2, 5.99f }, { 3, 7.81f }, { 4, 9.49f }, 
                { 5, 11.07f }, { 6, 12.59f }, { 7, 14.07f }, { 8, 15.51f }, 
                { 9, 16.92f }, { 10, 18.31f }, { 11, 19.68f }, { 12, 21.03f },
                { 13, 22.36f }, { 14, 23.68f }, { 15, 25.00f }, { 16, 26.30f },
                { 17, 27.59f }, { 18, 28.87f }, { 19, 30.14f }, { 20, 31.41f },
                { 21, 32.67f }, { 22, 33.92f }, { 23, 35.17f }, { 24, 36.42f },
                { 25, 37.65f }, { 26, 38.89f }, { 27, 40.11f }, { 28, 41.34f },
                { 29, 42.56f }, { 30, 43.77f }
            };
            
            // Si grados de libertad está en la tabla, usar ese valor
            if (criticalValues95.ContainsKey(degreesOfFreedom))
            {
                criticalValue = criticalValues95[degreesOfFreedom];
            }
            else
            {
                // Aproximación para grados de libertad mayores
                // Asegurarse de que el resultado de Math.Sqrt (que es double) se convierta a float
                criticalValue = degreesOfFreedom + 1.64f * (float)Math.Sqrt(2 * degreesOfFreedom); // <--- CAST AÑADIDO AQUÍ
            }
        }

        /// <summary>
        /// Ejecuta la prueba completa de Chi Cuadrado.
        /// </summary>
        public void EvaluateTest()
        {
            CalculateIntervals();
            CalculateFrequencies();
            CalculateChiSquare();
            CalculateCriticalValue();
            passed = chiSquare <= criticalValue;
        }
    }
}