using UnityEngine;

namespace RandomModels
{
    /// <summary>
    /// Componente auxiliar para ejecutar todas las pruebas de calidad automáticamente cuando se inicia Unity.
    /// Este componente puede agregarse a cualquier GameObject en una escena para iniciar las pruebas.
    /// </summary>
    public class AutoTester : MonoBehaviour
    {
        [SerializeField] private bool runOnStart = true;

        /// <summary>
        /// Se ejecuta cuando se inicia el componente.
        /// </summary>
        private void Start()
        {
            if (runOnStart)
            {
                RunTests();
            }
        }

        /// <summary>
        /// Ejecuta todas las pruebas del generador de números aleatorios.
        /// </summary>
        public void RunTests()
        {
            Debug.Log("Iniciando pruebas automáticas del generador de números aleatorios...");
            
            // Crear e inicializar el tester
            RandomTester tester = new RandomTester();
            
            // Ejecutar todas las pruebas
            tester.RunAllTests();
        }
    }
}