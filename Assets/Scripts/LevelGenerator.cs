using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public CollectibleSpawner collectibleSpawner;

    public GameObject gridPrefab;
    public Tile floorTile;
    public Tile wallTile;

    private Tilemap floorTilemap;
    private Tilemap wallTilemap;

    public int walkSteps = 200;
    public Vector2Int startPosition = Vector2Int.zero;

    [Header("Generation Settings")]
    public int minFloorTiles = 100; // Minimum number of floor tiles required
    public int stampSize = 1;       // How many tiles to place around the walker (1 for single tile, 2 for 3x3, etc.)
                                    // A stampSize of 1 means a 3x3 area (current tile + 1 in each direction)
                                    // A stampSize of 0 means just the current tile (original behavior)

    void Start()
    {
        GenerateLevelWithRetries();
    }

    void GenerateLevelWithRetries()
    {
        int attempts = 0;
        int maxAttempts = 100; // Prevent infinite loops

        while (attempts < maxAttempts)
        {
            // Clear any previous generation
            if (floorTilemap != null)
            {
                Destroy(floorTilemap.transform.parent.gameObject); // Destroy the old Grid instance
            }

            // Instantiate a new Grid prefab
            GameObject gridInstance = Instantiate(gridPrefab, Vector3.zero, Quaternion.identity);
            floorTilemap = gridInstance.transform.Find("Floor").GetComponent<Tilemap>();
            wallTilemap = gridInstance.transform.Find("Wall").GetComponent<Tilemap>();

            if (floorTilemap == null || wallTilemap == null)
            {
                Debug.LogError("Could not find 'Floor' or 'Wall' Tilemaps in the instantiated Grid prefab!");
                return; // Stop if prefab is not set up correctly
            }

            // Run generation and check conditions
            HashSet<Vector2Int> floorPositions = GenerateFloor();
            GenerateWalls(floorPositions);

            if (floorPositions.Count >= minFloorTiles)
            {
                Debug.Log($"Level generated successfully after {attempts + 1} attempts. Floor tiles: {floorPositions.Count}");

                if (enemySpawner != null)
                {
                    enemySpawner.SpawnEnemies(floorPositions, startPosition);
                }
                else
                {
                    Debug.LogWarning("EnemySpawner reference is missing! No enemies will be spawned.");
                }

                if (collectibleSpawner != null)
                {
                    collectibleSpawner.SpawnCollectibles(floorPositions, startPosition);
                }
                else
                {
                    Debug.LogWarning("CollectibleSpawner reference is missing! No collectibles will be spawned.");
                }

                return; // Level is good, stop trying
            }
            else
            {
                Debug.Log($"Generated level too small ({floorPositions.Count} tiles). Retrying...");
                attempts++;
            }
        }
        Debug.LogError("Failed to generate a level that meets criteria after max attempts.");
    }


    private HashSet<Vector2Int> GenerateFloor()
    {
        Vector2Int currentPos = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < walkSteps; i++)
        {
            // Place tiles in a 'stamp' around the current position
            for (int x = -stampSize; x <= stampSize; x++)
            {
                for (int y = -stampSize; y <= stampSize; y++)
                {
                    Vector2Int stampedPos = currentPos + new Vector2Int(x, y);
                    floorPositions.Add(stampedPos);
                    floorTilemap.SetTile((Vector3Int)stampedPos, floorTile);
                }
            }
            currentPos += GetRandomDirection();
        }
        return floorPositions;
    }

    private void GenerateWalls(HashSet<Vector2Int> floorPositions)
    {
        // Get all floor positions that have an empty neighbor
        HashSet<Vector2Int> wallCandidatePositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in GetCardinalAndDiagonalDirections()) // Check diagonals for better wall coverage
            {
                Vector2Int neighborPos = position + direction;
                if (!floorPositions.Contains(neighborPos))
                {
                    wallCandidatePositions.Add(neighborPos);
                }
            }
        }

        // Apply wall tiles to unique wall candidates
        foreach (var wallPos in wallCandidatePositions)
        {
            wallTilemap.SetTile((Vector3Int)wallPos, wallTile);
        }
    }

    private Vector2Int GetRandomDirection()
    {
        int choice = Random.Range(0, 4);
        switch (choice)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.down;
            case 2: return Vector2Int.left;
            case 3: return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }

    private List<Vector2Int> GetCardinalAndDiagonalDirections()
    {
        return new List<Vector2Int>
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, // Cardinal
            new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1) // Diagonal
        };
    }
}