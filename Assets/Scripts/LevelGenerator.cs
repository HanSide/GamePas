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
    public int minFloorTiles = 100;
    public int stampSize = 1;

    private HashSet<Vector2Int> currentFloorPositions;

    void Start()
    {
        GenerateLevelWithRetries();
    }
    public void GenerateLevelWithRetries()
    {
        int attempts = 0;
        int maxAttempts = 100;

        if (enemySpawner == null)
        {
            enemySpawner = FindAnyObjectByType<EnemySpawner>();
        }
        if (collectibleSpawner == null)
        {
            collectibleSpawner = FindAnyObjectByType<CollectibleSpawner>();
        }

        while (attempts < maxAttempts)
        {
            if (floorTilemap != null)
            {
                Destroy(floorTilemap.transform.parent.gameObject);
            }

            GameObject gridInstance = Instantiate(gridPrefab, Vector3.zero, Quaternion.identity);
            floorTilemap = gridInstance.transform.Find("Floor").GetComponent<Tilemap>();
            wallTilemap = gridInstance.transform.Find("Wall").GetComponent<Tilemap>();

            if (floorTilemap == null || wallTilemap == null)
            {
                Debug.LogError("Could not find 'Floor' or 'Wall' Tilemaps!");
                return;
            }

            HashSet<Vector2Int> floorPositions = GenerateFloor();
            GenerateWalls(floorPositions);
            currentFloorPositions = floorPositions;

            if (floorPositions.Count >= minFloorTiles)
            {
                Debug.Log($"Level generated successfully. Floor tiles: {floorPositions.Count}");

                if (enemySpawner != null)
                {
                    enemySpawner.SpawnEnemies(floorPositions, startPosition);
                }

                int currentLevel = LevelManager.Instance?.currentLevel ?? 1;

                if (collectibleSpawner != null)
                {
                    if (currentLevel < 2)
                    {
                        collectibleSpawner.SpawnCollectibles(floorPositions, startPosition, currentLevel);
                    }
                    else
                    {
                        Debug.Log($"Skipping initial collectible spawn for level {currentLevel} (will use respawn system)");

                        collectibleSpawner.UpdateCachedPositions(floorPositions, startPosition);
                    }
                }

                return;
            }
            else
            {
                Debug.Log($"Generated level too small. Retrying...");
                attempts++;
            }
        }
        Debug.LogError("Failed to generate level.");
    }

    private HashSet<Vector2Int> GenerateFloor()
    {
        Vector2Int currentPos = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < walkSteps; i++)
        {
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
        HashSet<Vector2Int> wallCandidatePositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in GetCardinalAndDiagonalDirections())
            {
                Vector2Int neighborPos = position + direction;
                if (!floorPositions.Contains(neighborPos))
                {
                    wallCandidatePositions.Add(neighborPos);
                }
            }
        }

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
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
        };
    }

    public HashSet<Vector2Int> GetFloorPositions()
    {
        return currentFloorPositions;
    }

    public Vector2Int GetPlayerStartPosition()
    {
        return startPosition;
    }

    public Vector3 GetPlayerStartWorldPosition()
    {
        return new Vector3(startPosition.x + 0.5f, startPosition.y + 0.5f, 0);
    }
}