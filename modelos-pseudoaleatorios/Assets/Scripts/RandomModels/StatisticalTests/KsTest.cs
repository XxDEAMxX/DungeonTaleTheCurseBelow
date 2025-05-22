using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomModels.StatisticalTests
{
    /// <summary>
    /// Implementa la prueba de Kolmogorov-Smirnov (KS) para validar secuencias de números aleatorios.
    /// </summary>
    public class KsTest
    {
        private readonly List<float> numbers;
        private readonly int n;
        private readonly int nIntervals;
        private float average;
        private float dMax;
        private float dMaxCritical;
        private float min;
        private float max;
        private List<int> oi;
        private List<int> oia;
        private List<float> probOi;
        private List<float> oiaA;
        private List<float> probEsp;
        private List<float> diff;
        private bool passed;
        private readonly float alpha;
        private List<(float Lower, float Upper)> intervals;

        /// <summary>
        /// Inicializa una nueva instancia de la prueba Kolmogorov-Smirnov (KS).
        /// </summary>
        /// <param name="numbers">Secuencia de números a evaluar</param>
        /// <param name="nIntervals">Número de intervalos para la prueba</param>
        /// <param name="alpha">Nivel de significancia de la prueba</param>
        public KsTest(List<float> numbers, int nIntervals = 10, float alpha = 0.05f)
        {
            this.numbers = new List<float>(numbers);
            this.n = numbers.Count;
            this.nIntervals = nIntervals;
            this.average = 0;
            this.dMax = 0;
            this.dMaxCritical = 0;
            this.min = 0;
            this.max = 0;
            this.oi = new List<int>();
            this.oia = new List<int>();
            this.probOi = new List<float>();
            this.oiaA = new List<float>();
            this.probEsp = new List<float>();
            this.diff = new List<float>();
            this.passed = false;
            this.alpha = alpha;
            this.intervals = new List<(float Lower, float Upper)>();
        }

        /// <summary>
        /// Resultado de la prueba.
        /// </summary>
        public bool Passed => passed;

        /// <summary>
        /// El valor máximo de diferencia calculado.
        /// </summary>
        public float DMax => dMax;

        /// <summary>
        /// El valor crítico para la prueba KS.
        /// </summary>
        public float DMaxCritical => dMaxCritical;

        /// <summary>
        /// Calcula la sumatoria acumulada de las frecuencias observadas.
        /// </summary>
        private void CalculateOia()
        {
            int cumFreq = 0;
            foreach (var freq in oi)
            {
                cumFreq += freq;
                oia.Add(cumFreq);
            }
        }

        /// <summary>
        /// Calcula el valor mínimo de la secuencia.
        /// </summary>
        private void CalculateMin()
        {
            if (n > 0)
            {
                min = numbers.Min();
            }
        }

        /// <summary>
        /// Calcula el valor máximo de la secuencia.
        /// </summary>
        private void CalculateMax()
        {
            if (n > 0)
            {
                max = numbers.Max();
            }
        }

        /// <summary>
        /// Calcula el promedio de la secuencia.
        /// </summary>
        private void CalculateAverage()
        {
            if (n > 0)
            {
                average = numbers.Average();
            }
        }

        /// <summary>
        /// Calcula los intervalos utilizados para la prueba KS.
        /// </summary>
        private void CalculateIntervals()
        {
            if (n > 0)
            {
                float intervalSize = (max - min) / nIntervals;
                float initial = min;
                for (int i = 0; i < nIntervals; i++)
                {
                    var newInterval = (initial, initial + intervalSize);
                    intervals.Add(newInterval);
                    initial = newInterval.Item2;
                }
            }
        }

        /// <summary>
        /// Calcula las frecuencias observadas (oi) en cada intervalo.
        /// </summary>
        private void CalculateOi()
        {
            // Ordenar la secuencia para facilitar el cálculo
            var sortedNumbers = new List<float>(numbers);
            sortedNumbers.Sort();
            
            // Inicializar los contadores para cada intervalo
            oi = Enumerable.Repeat(0, nIntervals).ToList();
            
            // Para cada valor, determinar en qué intervalo se encuentra
            foreach (var valor in sortedNumbers)
            {
                for (int i = 0; i < intervals.Count; i++)
                {
                    var (lower, upper) = intervals[i];
                    if (lower <= valor && valor < upper)
                    {
                        oi[i]++;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Calcula las probabilidades observadas en cada intervalo.
        /// </summary>
        private void CalculateProbOi()
        {
            foreach (var freq in oia)
            {
                probOi.Add((float)freq / n);
            }
        }

        /// <summary>
        /// Calcula la frecuencia acumulada esperada.
        /// </summary>
        private void CalculateOiaA()
        {
            float n1 = (float)n / nIntervals;
            for (int i = 0; i < nIntervals; i++)
            {
                oiaA.Add(n1 * (i + 1));
            }
        }

        /// <summary>
        /// Calcula las probabilidades esperadas.
        /// </summary>
        private void CalculateProbEsp()
        {
            foreach (var freq in oiaA)
            {
                probEsp.Add(freq / n);
            }
        }

        /// <summary>
        /// Calcula la diferencia absoluta entre las probabilidades observadas y esperadas.
        /// </summary>
        private void CalculateDiff()
        {
            for (int i = 0; i < probEsp.Count; i++)
            {
                diff.Add(Math.Abs(probEsp[i] - probOi[i]));
            }
        }

        /// <summary>
        /// Calcula el valor crítico de KS según el tamaño de la muestra.
        /// </summary>
        private void CalculateKS()
        {
            if (n <= 50 && n > 0)
            {
                // Para n ≤ 50, usar valores tabulados aproximados
                dMaxCritical = (float)(1.36 / Math.Sqrt(n));
            }
            else
            {
                // Para n > 50, usar la aproximación asintótica
                dMaxCritical = (float)(1.36 / Math.Sqrt(n));
            }
        }

        /// <summary>
        /// Ejecuta todas las etapas de la prueba KS.
        /// </summary>
        public void CheckTest()
        {
            CalculateMin();
            CalculateMax();
            CalculateAverage();
            CalculateIntervals();
            CalculateOi();
            CalculateOia();
            CalculateProbOi();
            CalculateOiaA();
            CalculateProbEsp();
            CalculateDiff();
            dMax = diff.Max();
            CalculateKS();
            passed = dMax <= dMaxCritical;
        }
    }
}