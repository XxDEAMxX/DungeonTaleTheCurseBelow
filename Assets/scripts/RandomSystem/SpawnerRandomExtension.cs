using UnityEngine;

// Clase de extensión estática para Spawners, usando CustomRandom para calidad.
public static class SpawnerRandomExtension
{
    public static int GetSpawnCount(int minSpawn, int maxSpawn)
    {
        // CustomRandom.Range(min, max) para enteros es [min, max-1].
        // Para que maxSpawn sea inclusivo, el segundo argumento de Range debe ser maxSpawn + 1.
        if (minSpawn > maxSpawn) minSpawn = maxSpawn; // Evitar error
        return CustomRandom.Range(minSpawn, maxSpawn + 1);
    }
    
    public static int GetRandomPositionIndex(int availablePositionsCount)
    {
        if (availablePositionsCount <= 0)
        {
            Debug.LogWarning("GetRandomPositionIndex llamado con availablePositionsCount <= 0. Devolviendo 0.");
            return 0;
        }
        // CustomRandom.Range(0, N) devuelve un entero entre 0 y N-1, perfecto para índices.
        return CustomRandom.Range(0, availablePositionsCount);
    }
}
