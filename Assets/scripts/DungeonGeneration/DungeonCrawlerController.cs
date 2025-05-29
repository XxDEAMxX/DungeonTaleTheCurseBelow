using System.Collections.Generic;
using UnityEngine;

// Se define un conjunto de direcciones posibles que puede tomar un caminante
public enum Direction
{
    top = 0,    // Arriba
    left = 1,   // Izquierda
    down = 2,   // Abajo
    Right = 3   // Derecha
}

// Esta clase se encarga de generar un mapa de mazmorra con caminatas aleatorias
public class DungeonCrawlerController : MonoBehaviour
{
    // Lista de todas las posiciones que han sido visitadas durante la generación
    public static List<Vector2Int> positionsVisited = new List<Vector2Int>();

    // Diccionario que asocia cada dirección con un desplazamiento en el plano (x, y)
    private static readonly Dictionary<Direction, Vector2Int> directionOffsets = new Dictionary<Direction, Vector2Int>
    {
        { Direction.top, Vector2Int.up },       // (0, 1)
        { Direction.left, Vector2Int.left },     // (-1, 0)
        { Direction.down, Vector2Int.down },     // (0, -1)
        { Direction.Right, Vector2Int.right }    // (1, 0)
    };

    // Función principal que genera la mazmorra y devuelve las posiciones visitadas
    public static List<Vector2Int> GenerateDungeon(DungeonGeneratorData dungeonGeneratorData)
    {
        // Se limpia la lista antes de comenzar una nueva generación
        positionsVisited.Clear();

        // Se crea una lista que almacenará los "caminantes" que exploran la mazmorra
        List<DangeonCrawler> dungeonCrawlers = new List<DangeonCrawler>();

        // Se inicializan los caminantes en la posición (0, 0)
        for (int i = 0; i < dungeonGeneratorData.numberOfCrawlers; i++)
        {
            dungeonCrawlers.Add(new DangeonCrawler(Vector2Int.zero));
        }

        // Se determina cuántas veces se moverán los caminantes, eligiendo un número aleatorio entre un mínimo y un máximo
        int iterations = CustomRandom.Range(dungeonGeneratorData.iterationMin, dungeonGeneratorData.iterationMax);

        // Se repite el proceso por la cantidad de iteraciones
        for (int i = 0; i < iterations; i++)
        {
            // Cada caminante se mueve una vez por iteración
            foreach (DangeonCrawler dangeonCrawler in dungeonCrawlers)
            {
                // El caminante se mueve en una dirección aleatoria y se guarda su nueva posición
                Vector2Int newPosition = dangeonCrawler.Move(directionOffsets);
                positionsVisited.Add(newPosition); // Se agrega esa posición a la lista de visitadas
            }
        }

        // Se retorna la lista de todas las posiciones recorridas, que representa el mapa generado
        return positionsVisited;
    }
}
