using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomModels.StatisticalTests
{
    /// <summary>
    /// Implementa la prueba de Varianza para validar la dispersión de secuencias de números aleatorios.
    /// </summary>
    public class VarianceTest
    {
        private readonly List<float> numbers;
        private readonly int n;
        private float variance;
        private readonly float theoreticalVariance;
        private float lowerLimit;
        private float upperLimit;
        private readonly float alpha;
        private bool passed;

        /// <summary>
        /// Inicializa una nueva instancia de la prueba de Varianza.
        /// </summary>
        /// <param name="numbers">Secuencia de números a evaluar</param>
        /// <param name="alpha">Nivel de significancia de la prueba</param>
        public VarianceTest(List<float> numbers, float alpha = 0.05f)
        {
            this.numbers = new List<float>(numbers);
            this.n = numbers.Count;
            this.variance = 0.0f;
            this.theoreticalVariance = 1.0f / 12.0f; // Varianza teórica para distribución uniforme [0,1]
            this.lowerLimit = 0.0f;
            this.upperLimit = 0.0f;
            this.alpha = alpha;
            this.passed = false;
        }

        /// <summary>
        /// Resultado de la prueba.
        /// </summary>
        public bool Passed => passed;

        /// <summary>
        /// La varianza calculada.
        /// </summary>
        public float Variance => variance;

        /// <summary>
        /// El límite inferior para la prueba.
        /// </summary>
        public float LowerLimit => lowerLimit;

        /// <summary>
        /// El límite superior para la prueba.
        /// </summary>
        public float UpperLimit => upperLimit;

        /// <summary>
        /// Calcula la varianza muestral.
        /// </summary>
        private void CalculateVariance()
        {
            float mean = numbers.Average();
            float sumSquaredDiff = numbers.Sum(x => (x - mean) * (x - mean));
            variance = sumSquaredDiff / n;
        }

        /// <summary>
        /// Calcula los límites de aceptación usando chi cuadrado.
        /// </summary>
        private void CalculateLimits()
        {
            int df = n - 1;
            
            // Tabla de valores críticos para chi-cuadrado
            // Usaremos aproximaciones para los límites
            float chiLower, chiUpper;
            
            // Aproximación para chi-cuadrado inversa en el límite inferior (alpha/2)
            chiLower = (float)Math.Max(0, df * (1 - 2.0f / (9.0f * df) - 
                                       1.96f * Math.Sqrt(2.0f / (9.0f * df))));
            
            // Aproximación para chi-cuadrado inversa en el límite superior (1-alpha/2)
            chiUpper = (float)(df * (1 - 2.0f / (9.0f * df) + 
                                    1.96f * Math.Sqrt(2.0f / (9.0f * df))));
            
            lowerLimit = (chiLower / df) * theoreticalVariance;
            upperLimit = (chiUpper / df) * theoreticalVariance;
        }

        /// <summary>
        /// Ejecuta la prueba completa de Varianza.
        /// </summary>
        public void EvaluateTest()
        {
            CalculateVariance();
            CalculateLimits();
            passed = lowerLimit <= variance && variance <= upperLimit;
        }
    }
}