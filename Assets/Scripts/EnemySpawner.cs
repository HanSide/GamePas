using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies = 10;

    public void SpawnEnemies(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        // Convert the floor positions to a list for easier random selection
        List<Vector2Int> spawnPoints = floorPositions.ToList();

        // Remove the player's start tile and its immediate neighbors to create a safe zone
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                spawnPoints.Remove(playerStartPosition + new Vector2Int(x, y));
            }
        }

        // Ensure we don't try to spawn more enemies than there are available tiles
        if (spawnPoints.Count < numberOfEnemies)
        {
            Debug.LogWarning("Not enough valid spawn points for the requested number of enemies! Spawning as many as possible.");
            numberOfEnemies = spawnPoints.Count;
        }

        // Loop to spawn the specified number of enemies
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Pick a random spawn point from the list
            int randomIndex = Random.Range(0, spawnPoints.Count);
            Vector2Int spawnPosition = spawnPoints[randomIndex];

            // Convert the tile position to a world position (centered in the tile)
            Vector3 spawnWorldPosition = new Vector3(spawnPosition.x + 0.5f, spawnPosition.y + 0.5f, 0);

            // Create the enemy instance
            Instantiate(enemyPrefab, spawnWorldPosition, Quaternion.identity);

            // Remove this point from the list so we don't spawn another enemy on the same tile
            spawnPoints.RemoveAt(randomIndex);
        }
    }
}