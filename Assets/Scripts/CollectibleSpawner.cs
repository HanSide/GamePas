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
        List<Vector2Int> spawnPoints = floorPositions.ToList();

        spawnPoints.Remove(playerStartPosition);

        foreach (var collectible in collectibles)
        {
            if (collectible.prefab == null || collectible.amountToSpawn <= 0) continue;
            int spawnCount = Mathf.Min(collectible.amountToSpawn, spawnPoints.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                Vector2Int spawnTile = spawnPoints[randomIndex];

                Vector3 spawnWorldPos = new Vector3(spawnTile.x + 0.5f, spawnTile.y + 0.5f, 0);

                Instantiate(collectible.prefab, spawnWorldPos, Quaternion.identity);

                spawnPoints.RemoveAt(randomIndex);

                if (spawnPoints.Count == 0) break;
            }
        }

        if (spawnPoints.Count == 0)
            Debug.LogWarning("No more valid spawn points left after spawning!");
    }
}