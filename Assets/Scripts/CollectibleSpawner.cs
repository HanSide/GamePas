using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CollectibleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class CollectibleEntry
    {
        public GameObject prefab;
        public int amountToSpawn;
    }

    [Header("Collectible Settings")]
    public List<CollectibleEntry> collectibles = new List<CollectibleEntry>();

    public void SpawnCollectibles(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        // Convert the floor positions to a list for easier random selection
        List<Vector2Int> spawnPoints = floorPositions.ToList();

        // Remove the player's start position to avoid spawning collectibles there
        spawnPoints.Remove(playerStartPosition);

        // Loop through each collectible type
        foreach (var collectible in collectibles)
        {
            // Safety check
            if (collectible.prefab == null || collectible.amountToSpawn <= 0) continue;

            // Make sure there are still spawn points left
            int spawnCount = Mathf.Min(collectible.amountToSpawn, spawnPoints.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                Vector2Int spawnTile = spawnPoints[randomIndex];

                Vector3 spawnWorldPos = new Vector3(spawnTile.x + 0.5f, spawnTile.y + 0.5f, 0);

                Instantiate(collectible.prefab, spawnWorldPos, Quaternion.identity);

                // Remove this tile so we don’t overlap spawns
                spawnPoints.RemoveAt(randomIndex);

                // If no spawn points left, break early
                if (spawnPoints.Count == 0) break;
            }
        }

        if (spawnPoints.Count == 0)
            Debug.LogWarning("No more valid spawn points left after spawning!");
    }
}