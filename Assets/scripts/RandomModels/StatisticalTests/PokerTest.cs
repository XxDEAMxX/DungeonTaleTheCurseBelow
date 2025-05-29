using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomModels.StatisticalTests
{
    /// <summary>
    /// Implementa la prueba de póker para validar la independencia entre dígitos
    /// de secuencias de números aleatorios.
    /// </summary>
    public class PokerTest
    {
        private readonly List<float> numbers;
        private readonly int n;
        private readonly float[] probabilities = { 0.3024f, 0.504f, 0.108f, 0.072f, 0.009f, 0.0045f, 0.0001f };
        private readonly int[] observed = new int[7]; // Contador para cada categoría (mano de póker)
        private readonly List<float> expected = new List<float>();
        private readonly List<float> chiValues = new List<float>();
        private float totalSum;
        private float chiReverse;
        private bool passed;
        private readonly float alpha;        /// <summary>
        /// Inicializa una nueva instancia de la prueba de Póker.
        /// </summary>
        /// <param name="numbers">Secuencia de números a evaluar</param>
        /// <param name="alpha">Nivel de significancia de la prueba</param>
        public PokerTest(List<float> numbers, float alpha = 0.05f)
        {
            this.numbers = new List<float>(numbers ?? new List<float>());
            this.n = this.numbers.Count;
            this.totalSum = 0.0f;
            this.passed = false;
            this.alpha = alpha;
            
            // Inicializar el array observed con ceros
            for (int i = 0; i < observed.Length; i++)
            {
                observed[i] = 0;
            }
            
            // Valor crítico de chi-cuadrado para 6 grados de libertad y nivel de significancia 0.05
            this.chiReverse = 12.59f;
        }

        /// <summary>
        /// Resultado de la prueba.
        /// </summary>
        public bool Passed => passed;

        /// <summary>
        /// El valor chi-cuadrado calculado.
        /// </summary>
        public float TotalSum => totalSum;

        /// <summary>
        /// El valor crítico chi-cuadrado.
        /// </summary>
        public float ChiReverse => chiReverse;

        /// <summary>
        /// Determina si todos los dígitos son diferentes.
        /// </summary>
        private bool AllDiff(string numStr)
        {
            return numStr.Length == new HashSet<char>(numStr).Count;
        }

        /// <summary>
        /// Determina si todos los dígitos son iguales.
        /// </summary>
        private bool AllSame(string numStr)
        {
            return new HashSet<char>(numStr).Count == 1;
        }

        /// <summary>
        /// Determina si hay cuatro dígitos iguales.
        /// </summary>
        private bool FourOfAKind(string numStr)
        {
            var count = new Dictionary<char, int>();
            foreach (char c in numStr)
            {
                if (!count.ContainsKey(c))
                    count[c] = 0;
                count[c]++;
            }
            return count.Values.Count(v => v == 4) == 1;
        }

        /// <summary>
        /// Determina si hay dos pares.
        /// </summary>
        private bool TwoPairs(string numStr)
        {
            var count = new Dictionary<char, int>();
            foreach (char c in numStr)
            {
                if (!count.ContainsKey(c))
                    count[c] = 0;
                count[c]++;
            }
            return count.Values.Count(v => v == 2) == 2;
        }

        /// <summary>
        /// Determina si hay un trío y un par.
        /// </summary>
        private bool FullHouse(string numStr)
        {
            var count = new Dictionary<char, int>();
            foreach (char c in numStr)
            {
                if (!count.ContainsKey(c))
                    count[c] = 0;
                count[c]++;
            }
            return count.Values.Count(v => v == 3) == 1 && count.Values.Count(v => v == 2) == 1;
        }

        /// <summary>
        /// Determina si hay sólo un trío.
        /// </summary>
        private bool ThreeOfAKind(string numStr)
        {
            var count = new Dictionary<char, int>();
            foreach (char c in numStr)
            {
                if (!count.ContainsKey(c))
                    count[c] = 0;
                count[c]++;
            }
            return count.Values.Count(v => v == 3) == 1 && count.Values.Count(v => v == 1) == 2;
        }

        /// <summary>
        /// Determina si hay sólo un par.
        /// </summary>
        private bool OnePair(string numStr)
        {
            var count = new Dictionary<char, int>();
            foreach (char c in numStr)
            {
                if (!count.ContainsKey(c))
                    count[c] = 0;
                count[c]++;
            }
            return count.Values.Count(v => v == 2) == 1 && count.Values.Count(v => v == 1) == 3;
        }        /// <summary>
        /// Calcula las frecuencias observadas de cada mano de poker.
        /// </summary>
        private void CalculateObservedFrequencies()
        {
            foreach (float n in numbers)
            {
                // Convertir el número a cadena con exactamente 5 dígitos decimales
                string numStr = n.ToString("F5");
                string[] parts = numStr.Split('.');
                
                // Verificar que hay parte decimal
                if (parts.Length < 2)
                    continue;
                    
                string num = parts[1];
                
                // Asegurarse de que tengamos exactamente 5 dígitos
                if (num.Length != 5)
                    continue;

                if (AllDiff(num))
                    observed[0]++;
                else if (AllSame(num))
                    observed[6]++;
                else if (FourOfAKind(num))
                    observed[5]++;
                else if (FullHouse(num))
                    observed[4]++;
                else if (ThreeOfAKind(num))
                    observed[3]++;
                else if (TwoPairs(num))
                    observed[2]++;
                else if (OnePair(num))
                    observed[1]++;
            }
        }

        /// <summary>
        /// Calcula las frecuencias esperadas de cada mano de poker.
        /// </summary>
        private void CalculateExpectedFrequencies()
        {
            for (int i = 0; i < 7; i++)
            {
                expected.Add(probabilities[i] * n);
            }
        }        /// <summary>
        /// Calcula los valores de chi-cuadrado para cada categoría.
        /// </summary>
        private void CalculateChiValues()
        {
            for (int i = 0; i < observed.Length && i < probabilities.Length; i++)
            {
                float expectedVal = probabilities[i] * n;
                if (expectedVal > 0)
                {
                    float chiValue = ((observed[i] - expectedVal) * (observed[i] - expectedVal)) / expectedVal;
                    chiValues.Add(chiValue);
                }
                else
                {
                    chiValues.Add(0f);
                }
            }
        }

        /// <summary>
        /// Calcula la suma total de los valores de chi-cuadrado.
        /// </summary>
        private void CalculateTotalSum()
        {
            totalSum = chiValues.Sum();
        }        /// <summary>
        /// Ejecuta la prueba completa de Póker.
        /// </summary>
        public void CheckPoker()
        {
            if (numbers == null || numbers.Count == 0)
            {
                passed = false;
                return;
            }
            
            CalculateObservedFrequencies();
            CalculateExpectedFrequencies();
            CalculateChiValues();
            CalculateTotalSum();
            passed = totalSum < chiReverse;
        }
    }
}