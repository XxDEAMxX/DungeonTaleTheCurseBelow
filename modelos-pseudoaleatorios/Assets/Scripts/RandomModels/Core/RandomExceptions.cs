using System;

namespace RandomModels.Core
{
    /// <summary>
    /// Excepción lanzada cuando un lote de números aleatorios no pasa las pruebas de validación.
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException() : base() { }
        
        public ValidationException(string message) : base(message) { }
        
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Excepción lanzada cuando se produce un error en la configuración de un generador de números pseudoaleatorios.
    /// </summary>
    public class RandomConfigurationException : Exception
    {
        public RandomConfigurationException() : base() { }
        
        public RandomConfigurationException(string message) : base(message) { }
        
        public RandomConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Excepción base para otros errores de generación.
    /// </summary>
    public class GenerationException : Exception
    {
        public GenerationException() : base() { }
        public GenerationException(string message) : base(message) { }
        public GenerationException(string message, Exception innerException) : base(message, innerException) { }
    }
}