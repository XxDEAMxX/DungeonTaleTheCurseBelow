using System;

namespace RandomModels.Core
{
    /// <summary>
    /// Generador de números pseudoaleatorios usando el algoritmo de congruencia lineal.
    /// Implementa el método de Schrage para evitar desbordamiento numérico.
    /// </summary>
    public class LinearCongruenceRandom : PRNG
    {
        // Parámetros optimizados para el método de Schrage
        public const int DEFAULT_M = 2147483647; // 2^31 - 1 (Mersenne prime)
        public const int DEFAULT_A = 48271;      // Multiplicador óptimo para este módulo
        public const int DEFAULT_C = 0;          // Generador multiplicativo puro
        
        private readonly int m;    // Módulo
        private readonly int a;    // Multiplicador
        private readonly int c;    // Incremento
        private readonly int q;    // Cociente para el método de Schrage
        private readonly int r;    // Resto para el método de Schrage
        private int _x;            // Estado interno del generador
        
        /// <summary>
        /// Inicializa una nueva instancia del generador con parámetros optimizados.
        /// </summary>
        /// <param name="seedValue">Semilla inicial (opcional)</param>
        public LinearCongruenceRandom(int? seedValue = null)
        {
            m = DEFAULT_M;
            a = DEFAULT_A;
            c = DEFAULT_C;
            q = m / a;  // Cociente para el método de Schrage
            r = m % a;  // Resto para el método de Schrage
            
            // Si no se proporciona semilla, usar el tiempo actual
            _x = seedValue ?? (int)(DateTime.Now.Ticks & 0x7FFFFFFF);
            
            ValidateParameters();
            
            // Descartar algunos valores iniciales para mejorar la distribución
            for (int i = 0; i < 20; i++)
            {
                Random();
            }
        }
        
        /// <summary>
        /// Valida que los parámetros del generador cumplan las condiciones necesarias.
        /// </summary>
        private void ValidateParameters()
        {
            if (m <= 0)
                throw new ArgumentException("El módulo m debe ser positivo");
            if (a <= 0)
                throw new ArgumentException("El multiplicador a debe ser positivo");
            if (c < 0)
                throw new ArgumentException("El incremento c debe ser no negativo");
            if (a >= m)
                throw new ArgumentException("El multiplicador a debe ser menor que m");
            if (c >= m)
                throw new ArgumentException("El incremento c debe ser menor que m");
            if (r >= q)
                throw new ArgumentException("El método de Schrage requiere que r < q");
        }
        
        /// <summary>
        /// Genera un número pseudoaleatorio en el rango [0.0, 1.0) usando el método de Schrage.
        /// </summary>
        /// <returns>Número pseudoaleatorio normalizado</returns>
        public override float Random()
        {
            // Implementación del método de Schrage para evitar desbordamiento
            int k = _x / q;
            _x = a * (_x - k * q) - k * r;
            
            if (_x < 0)
                _x += m;
                
            // Normalización para mejor distribución de dígitos decimales
            return (float)_x / (m - 1);  // Usar m-1 para incluir posibilidad de 1.0
        }
        
        /// <summary>
        /// Establece una nueva semilla para el generador.
        /// </summary>
        /// <param name="value">Valor de la semilla</param>
        public override void Seed(int value)
        {
            if (value <= 0)
                throw new ArgumentException("La semilla debe ser un entero positivo");
                
            _x = value % m;
            
            // Descartar algunos valores iniciales
            for (int i = 0; i < 10; i++)
            {
                Random();
            }
        }
    }
}