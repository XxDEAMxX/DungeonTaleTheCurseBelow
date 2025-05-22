using System;
using System.Collections.Generic;

namespace RandomModels.Core
{
    /// <summary>
    /// Clase base abstracta para generadores de números pseudoaleatorios.
    /// </summary>
    public abstract class PRNG
    {
        /// <summary>
        /// Establece la semilla del generador.
        /// </summary>
        /// <param name="value">El valor de la semilla</param>
        public abstract void Seed(int value);
        
        /// <summary>
        /// Retorna un número float aleatorio en el rango [0.0, 1.0).
        /// </summary>
        /// <returns>Un número float aleatorio</returns>
        public abstract float Random();

        /// <summary>
        /// Retorna un entero aleatorio N tal que a <= N <= b.
        /// </summary>
        /// <param name="a">Límite inferior (inclusivo)</param>
        /// <param name="b">Límite superior (inclusivo)</param>
        /// <returns>Un entero aleatorio en el rango [a, b]</returns>
        public virtual int RandomInt(int a, int b)
        {
            return a + (int)(Random() * (b - a + 1));
        }

        /// <summary>
        /// Retorna un número float aleatorio N tal que a <= N <= b.
        /// </summary>
        /// <param name="a">Límite inferior (inclusivo)</param>
        /// <param name="b">Límite superior (inclusivo)</param>
        /// <returns>Un número float aleatorio en el rango [a, b]</returns>
        public virtual float Uniform(float a, float b)
        {
            return a + (b - a) * Random();
        }

        /// <summary>
        /// Retorna un elemento aleatorio de la secuencia no vacía.
        /// </summary>
        /// <typeparam name="T">Tipo de los elementos de la secuencia</typeparam>
        /// <param name="seq">La secuencia de elementos</param>
        /// <returns>Un elemento aleatorio de la secuencia</returns>
        public virtual T Choice<T>(IList<T> seq)
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
        public virtual void Shuffle<T>(IList<T> list)
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
        public virtual List<T> Sample<T>(IList<T> population, int k)
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
        public virtual float Gauss(float mu = 0.0f, float sigma = 1.0f)
        {
            // Implementación del método Box-Muller
            float u1 = Random();
            float u2 = Random();
            
            // Evitar valores extremadamente pequeños
            if (u1 < 0.0001f) u1 = 0.0001f;
            
            float z0 = (float)(Math.Sqrt(-2.0f * Math.Log(u1)) * Math.Cos(2.0f * Math.PI * u2));
            return mu + z0 * sigma;
        }
    }
}