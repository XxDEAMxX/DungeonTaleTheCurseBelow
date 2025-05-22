using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using RandomModels.Core;
using RandomModels.StatisticalTests;

namespace RandomModels.Tests
{
    /// <summary>
    /// Pruebas para el generador de números aleatorios de congruencia lineal.
    /// </summary>
    public class LinearCongruenceRandomTest
    {
        private const int SampleSize = 10000;
        private const float SignificanceLevel = 0.05f;
        
        [Test]
        public void TestRange()
        {
            // Configuración
            var rng = new LinearCongruenceRandom(seedValue: 12345);
            var generatedNumbers = new List<float>();
            
            // Acción
            for (int i = 0; i < 1000; i++)
            {
                float num = rng.Random();
                generatedNumbers.Add(num);
                
                // Verificar que cada número esté en el rango [0, 1)
                Assert.GreaterOrEqual(num, 0.0f, "Número debe ser mayor o igual a 0");
                Assert.Less(num, 1.0f, "Número debe ser menor que 1");
            }
            
            // Verificar valores extremos
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            
            foreach (var num in generatedNumbers)
            {
                minValue = Math.Min(minValue, num);
                maxValue = Math.Max(maxValue, num);
            }
            
            Assert.GreaterOrEqual(minValue, 0.0f, "El valor mínimo debe ser mayor o igual a 0");
            Assert.Less(maxValue, 1.0f, "El valor máximo debe ser menor que 1");
        }
        
        [Test]
        public void TestChiSquared()
        {
            // Configuración
            var rng = new LinearCongruenceRandom(seedValue: 12345);
            var numbers = new List<float>();
            
            // Generar números
            for (int i = 0; i < SampleSize; i++)
            {
                numbers.Add(rng.Random());
            }
            
            // Realizar prueba Chi-cuadrado
            var test = new ChiSquareTest(numbers);
            test.EvaluateTest();
            
            Debug.Log($"Chi Square Value: {test.ChiSquareValue}, Critical Value: {test.CriticalValue}");
            Assert.IsTrue(test.Passed, "La prueba Chi-cuadrado falló - la distribución no es uniforme");
        }
        
        [Test]
        public void TestKS()
        {
            // Configuración
            var rng = new LinearCongruenceRandom(seedValue: 12345);
            var numbers = new List<float>();
            
            // Generar números
            for (int i = 0; i < SampleSize; i++)
            {
                numbers.Add(rng.Random());
            }
            
            // Realizar prueba Kolmogorov-Smirnov
            var test = new KsTest(numbers);
            test.CheckTest();
            
            Debug.Log($"KS Dmax: {test.DMax}, Critical Value: {test.DMaxCritical}");
            Assert.IsTrue(test.Passed, "La prueba KS falló - la distribución no es uniforme");
        }
        
        [Test]
        public void TestVariance()
        {
            // Configuración
            var rng = new LinearCongruenceRandom(seedValue: 12345);
            var numbers = new List<float>();
            
            // Generar números
            for (int i = 0; i < SampleSize; i++)
            {
                numbers.Add(rng.Random());
            }
            
            // Realizar prueba de varianza
            var test = new VarianceTest(numbers);
            test.EvaluateTest();
            
            Debug.Log($"Variance: {test.Variance}, Lower Limit: {test.LowerLimit}, Upper Limit: {test.UpperLimit}");
            Assert.IsTrue(test.Passed, "La prueba de varianza falló - la dispersión no es adecuada");
        }
        
        [Test]
        public void TestPoker()
        {
            // Configuración
            var rng = new LinearCongruenceRandom(seedValue: 12345);
            var numbers = new List<float>();
            
            // Generar números
            for (int i = 0; i < SampleSize; i++)
            {
                numbers.Add(rng.Random());
            }
            
            // Realizar prueba de poker
            var test = new PokerTest(numbers);
            test.CheckPoker();
            
            Debug.Log($"Poker Chi Square: {test.TotalSum}, Critical Value: {test.ChiReverse}");
            Assert.IsTrue(test.Passed, "La prueba de poker falló - los números no son independientes");
        }
        
        [Test]
        public void TestSeedReproducibility()
        {
            // Configuración
            var rng1 = new LinearCongruenceRandom(seedValue: 12345);
            var rng2 = new LinearCongruenceRandom(seedValue: 12345);
            
            // Generar números
            var numbers1 = new List<float>();
            var numbers2 = new List<float>();
            
            for (int i = 0; i < 1000; i++)
            {
                numbers1.Add(rng1.Random());
                numbers2.Add(rng2.Random());
            }
            
            // Verificar que las secuencias sean idénticas
            for (int i = 0; i < numbers1.Count; i++)
            {
                Assert.AreEqual(numbers1[i], numbers2[i], 
                    $"Las secuencias no son idénticas en la posición {i}");
            }
        }
    }
}