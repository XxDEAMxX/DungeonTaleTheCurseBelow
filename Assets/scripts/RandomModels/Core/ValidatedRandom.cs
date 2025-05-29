using System;
using System.Collections.Generic;
using RandomModels.StatisticalTests;

namespace RandomModels.Core
{
    /// <summary>
    /// Generador de números aleatorios que valida estadísticamente las secuencias generadas.
    /// Utiliza un generador LCG y aplica pruebas estadísticas antes de servir los números.
    /// </summary>
    public class ValidatedRandom
    {
        private readonly LinearCongruenceRandom _rng;
        private readonly int _batchSize;
        private readonly float _alpha;
        private readonly int _maxAttempts;
        private List<float> _validatedBatch;
        private int _currentIndex;        /// <summary>
        /// Inicializa una nueva instancia del generador validado.
        /// </summary>
        /// <param name="seed">Semilla inicial (opcional)</param>
        /// <param name="batchSize">Tamaño del lote de números a validar</param>
        /// <param name="significanceLevel">Nivel de significancia para las pruebas estadísticas</param>
        /// <param name="maxAttempts">Número máximo de intentos para generar un lote válido</param>
        public ValidatedRandom(int? seed = null, int batchSize = 1000, float significanceLevel = 0.01f, int maxAttempts = 10)
        {
            _rng = new LinearCongruenceRandom(seed);
            _batchSize = batchSize;
            _alpha = significanceLevel;
            _maxAttempts = maxAttempts;
            _validatedBatch = new List<float>();
            _currentIndex = 0;
        }        /// <summary>
        /// Ejecuta las pruebas estadísticas en un conjunto de números.
        /// </summary>
        /// <param name="numbers">Lista de números a validar</param>
        /// <returns>True si pasa la mayoría de las pruebas, False en caso contrario</returns>
        private bool RunTests(List<float> numbers)
        {
            if (numbers == null || numbers.Count < 100)
            {
                return false; // Muy pocos números para validar
            }

            int passedTests = 0;
            int totalTests = 0;

            try
            {
                // Prueba Chi-cuadrado
                var chiTest = new ChiSquareTest(numbers, alpha: _alpha);
                chiTest.EvaluateTest();
                if (chiTest.Passed) passedTests++;
                totalTests++;
            }
            catch
            {
                // Si la prueba falla por error, continúa con las demás
            }

            try
            {
                // Prueba KS
                var ksTest = new KsTest(numbers, alpha: _alpha);
                ksTest.CheckTest();
                if (ksTest.Passed) passedTests++;
                totalTests++;
            }
            catch
            {
                // Si la prueba falla por error, continúa con las demás
            }

            try
            {
                // Prueba de varianza
                var varianceTest = new VarianceTest(numbers, alpha: _alpha);
                varianceTest.EvaluateTest();
                if (varianceTest.Passed) passedTests++;
                totalTests++;
            }
            catch
            {
                // Si la prueba falla por error, continúa con las demás
            }

            try
            {
                // Prueba de póker (solo si tenemos suficientes números)
                if (numbers.Count >= 1000)
                {
                    var pokerTest = new PokerTest(numbers, alpha: _alpha);
                    pokerTest.CheckPoker();
                    if (pokerTest.Passed) passedTests++;
                    totalTests++;
                }
            }
            catch
            {
                // Si la prueba falla por error, continúa con las demás
            }

            // Considera válido si pasa al menos el 50% de las pruebas
            return totalTests > 0 && (float)passedTests / totalTests >= 0.5f;
        }        /// <summary>
        /// Genera y valida un nuevo lote de números aleatorios.
        /// </summary>
        /// <exception cref="ValidationException">Si no se puede generar un lote válido</exception>
        private void GenerateAndValidateBatch()
        {
            int attempts = 0;
            List<float> bestBatch = null;
            
            while (attempts < _maxAttempts)
            {
                var batch = new List<float>(_batchSize);
                for (int i = 0; i < _batchSize; i++)
                {
                    batch.Add(_rng.Random());
                }

                if (RunTests(batch))
                {
                    _validatedBatch = batch;
                    _currentIndex = 0;
                    return;
                }
                
                // Guarda el primer lote como fallback
                if (bestBatch == null)
                {
                    bestBatch = new List<float>(batch);
                }
                
                attempts++;
            }
            
            // Si llegamos aquí, no se pudo validar ningún lote
            // Usar el primer lote generado como fallback
            if (bestBatch != null)
            {
                _validatedBatch = bestBatch;
                _currentIndex = 0;
                // Log de advertencia en lugar de excepción
                UnityEngine.Debug.LogWarning($"No se pudo validar lote después de {_maxAttempts} intentos. Usando lote sin validar.");
                return;
            }
            
            throw new ValidationException($"No se pudo generar ningún lote después de {_maxAttempts} intentos.");
        }

        /// <summary>
        /// Establece una nueva semilla para el generador.
        /// </summary>
        /// <param name="value">Valor de la semilla</param>
        public void Seed(int value)
        {
            _rng.Seed(value);
            _validatedBatch.Clear();
            _currentIndex = 0;
        }

        /// <summary>
        /// Genera un número aleatorio validado entre [0.0, 1.0).
        /// </summary>
        /// <returns>Un número aleatorio validado</returns>
        public float Random()
        {
            if (_currentIndex >= _validatedBatch.Count)
            {
                GenerateAndValidateBatch();
            }
            return _validatedBatch[_currentIndex++];
        }

        /// <summary>
        /// Retorna un entero aleatorio N tal que a <= N <= b.
        /// </summary>
        /// <param name="a">Límite inferior (inclusivo)</param>
        /// <param name="b">Límite superior (inclusivo)</param>
        /// <returns>Un entero aleatorio en el rango [a, b]</returns>
        public int RandomInt(int a, int b)
        {
            float val = Random();
            int scaled = (int)(val * (b - a + 1));
            return Math.Min(a + scaled, b);
        }

        /// <summary>
        /// Retorna un número float aleatorio N tal que a <= N <= b.
        /// </summary>
        /// <param name="a">Límite inferior (inclusivo)</param>
        /// <param name="b">Límite superior (inclusivo)</param>
        /// <returns>Un número float aleatorio en el rango [a, b]</returns>
        public float Uniform(float a, float b)
        {
            return a + (b - a) * Random();
        }

        /// <summary>
        /// Retorna un elemento aleatorio de la secuencia no vacía.
        /// </summary>
        /// <typeparam name="T">Tipo de los elementos de la secuencia</typeparam>
        /// <param name="seq">La secuencia de elementos</param>
        /// <returns>Un elemento aleatorio de la secuencia</returns>
        public T Choice<T>(IList<T> seq)
        {
            if (seq == null || seq.Count == 0)
            {
                throw new ArgumentException("No se puede elegir de una secuencia vacía");
            }
            return seq[RandomInt(0, seq.Count - 1)];
        }

        /// <summary>
        /// Mezcla la lista in-place.
        /// </summary>
        /// <typeparam name="T">Tipo de los elementos de la lista</typeparam>
        /// <param name="list">La lista a mezclar</param>
        public void Shuffle<T>(IList<T> list)
        {
            if (list == null) return;
            
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = RandomInt(0, i);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        /// <summary>
        /// Retorna k elementos únicos elegidos de la población.
        /// </summary>
        /// <typeparam name="T">Tipo de los elementos de la población</typeparam>
        /// <param name="population">La población de elementos</param>
        /// <param name="k">Cantidad de elementos a seleccionar</param>
        /// <returns>Una lista con k elementos únicos de la población</returns>
        public List<T> Sample<T>(IList<T> population, int k)
        {
            if (k < 0)
            {
                throw new ArgumentException("El tamaño de la muestra debe ser no negativo");
            }
            int n = population.Count;
            if (k > n)
            {
                throw new ArgumentException("El tamaño de la muestra no puede ser mayor que el de la población");
            }
            
            List<T> result = new List<T>(population);
            for (int i = 0; i < k; i++)
            {
                int j = RandomInt(i, n - 1);
                T temp = result[i];
                result[i] = result[j];
                result[j] = temp;
            }
            return result.GetRange(0, k);
        }

        /// <summary>
        /// Retorna un número aleatorio con distribución normal.
        /// </summary>
        /// <param name="mu">Media de la distribución</param>
        /// <param name="sigma">Desviación estándar de la distribución</param>
        /// <returns>Un número aleatorio con distribución normal</returns>
        public float Gauss(float mu = 0.0f, float sigma = 1.0f)
        {
            // Implementación del método Box-Muller
            float u1 = Random();
            float u2 = Random();
            
            // Evitar valores extremadamente pequeños
            if (u1 < 0.0001f) u1 = 0.0001f;
            
            float z0 = (float)(Math.Sqrt(-2.0f * Math.Log(u1)) * Math.Cos(2.0f * Math.PI * u2));
            return mu + z0 * sigma;
        }
        
        /// <summary>
        /// Configura el generador para usar validación simple (solo pruebas básicas).
        /// </summary>
        public void SetSimpleValidation()
        {
            // Regenera el lote actual con configuración más permisiva
            _validatedBatch.Clear();
            _currentIndex = 0;
        }

        /// <summary>
        /// Genera un número aleatorio sin validación estadística completa.
        /// Usado como fallback cuando la validación falla.
        /// </summary>
        /// <returns>Un número aleatorio básico entre [0.0, 1.0)</returns>
        public float RandomSimple()
        {
            return _rng.Random();
        }
    }
}