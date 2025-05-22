using UnityEngine;

// Clase de extensión estática para el movimiento de enemigos usando OptimizedRandom.
public static class EnemyRandomExtension
{
    // Constante para el nombre del pool, para evitar errores de tipeo.
    private const string EnemyMovementPoolName = "EnemyMovement";

    public static Vector2 GetRandomDirection()
    {
        // Asegurarse de que OptimizedRandom esté disponible.
        // OptimizedRandom.Value se encargará de instanciarlo si es necesario.
        float x = OptimizedRandom.Range(-1f, 1f, EnemyMovementPoolName);
        float y = OptimizedRandom.Range(-1f, 1f, EnemyMovementPoolName);
        return new Vector2(x, y).normalized;
    }
    
    public static float GetWanderTime()
    {
        return OptimizedRandom.Range(1f, 2f, EnemyMovementPoolName);
    }
    
    public static float GetWaitTime()
    {
        return OptimizedRandom.Range(0.5f, 1.5f, EnemyMovementPoolName);
    }
}
