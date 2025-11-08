using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CollectibleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class CollectibleEntry
    {
        public GameObject prefab;
        public int amountToSpawn;
        public List<int> spawnInLevels = new List<int> { 1 };
    }

    public List<CollectibleEntry> collectibles = new List<CollectibleEntry>();
    public bool enableRespawn = false;
    public float respawnInterval = 5f;
    public int maxCollectiblesAlive = 10;

    private List<GameObject> activeCollectibles = new List<GameObject>();
    private HashSet<Vector2Int> cachedFloorPositions;
    private Vector2Int cachedPlayerStartPosition;
    private int currentLevel = 1;

    public void SpawnCollectibles(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition, int level = 1)
    {
        currentLevel = level;
        cachedFloorPositions = floorPositions;
        cachedPlayerStartPosition = playerStartPosition;

        List<Vector2Int> spawnPoints = floorPositions.ToList();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                spawnPoints.Remove(playerStartPosition + new Vector2Int(x, y));
            }
        }

        foreach (var collectible in collectibles)
        {
            if (!collectible.spawnInLevels.Contains(level))
            {
                Debug.Log($"Skipping {collectible.prefab?.name} (not for level {level})");
                continue;
            }

            if (collectible.prefab == null || collectible.amountToSpawn <= 0) continue;

            int spawnCount = Mathf.Min(collectible.amountToSpawn, spawnPoints.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                if (spawnPoints.Count == 0) break;

                int randomIndex = Random.Range(0, spawnPoints.Count);
                Vector2Int spawnTile = spawnPoints[randomIndex];
                Vector3 spawnWorldPos = new Vector3(spawnTile.x + 0.5f, spawnTile.y + 0.5f, 0);

                GameObject item = Instantiate(collectible.prefab, spawnWorldPos, Quaternion.identity);
                activeCollectibles.Add(item);

                spawnPoints.RemoveAt(randomIndex);
            }

            Debug.Log($"Spawned {spawnCount} {collectible.prefab.name} for level {level}");
        }
    }

    public void UpdateCachedPositions(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        cachedFloorPositions = floorPositions;
        cachedPlayerStartPosition = playerStartPosition;
        Debug.Log($"✓ Cached positions updated: {floorPositions.Count} tiles, player at {playerStartPosition}");
    }

    public void StartRespawn(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        cachedFloorPositions = floorPositions;
        cachedPlayerStartPosition = playerStartPosition;
        enableRespawn = true;

        StartCoroutine(RespawnRoutine());
        Debug.Log($"✓ Collectible respawn started at player position: {playerStartPosition}");
    }

    private IEnumerator RespawnRoutine()
    {
        while (enableRespawn)
        {
            yield return new WaitForSeconds(respawnInterval);

            activeCollectibles.RemoveAll(item => item == null);

            if (activeCollectibles.Count < maxCollectiblesAlive)
            {
                SpawnRandomCollectible();
            }
        }
    }

    private void SpawnRandomCollectible()
    {
        if (cachedFloorPositions == null || cachedFloorPositions.Count == 0)
        {
            Debug.LogWarning("Cached floor positions is null or empty!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector2Int currentPlayerPos = cachedPlayerStartPosition; 

        if (player != null)
        {
            currentPlayerPos = Vector2Int.RoundToInt(player.transform.position);
        }

        List<CollectibleEntry> validCollectibles = collectibles.FindAll(c =>
            c.prefab != null &&
            c.spawnInLevels.Contains(currentLevel)
        );

        if (validCollectibles.Count == 0)
        {
            Debug.LogWarning("No valid collectibles to spawn!");
            return;
        }

        CollectibleEntry randomEntry = validCollectibles[Random.Range(0, validCollectibles.Count)];

        List<Vector2Int> spawnPoints = cachedFloorPositions.ToList();

        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                spawnPoints.Remove(currentPlayerPos + new Vector2Int(x, y));
            }
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No valid spawn points after removing safe zone!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Count);
        Vector2Int spawnTile = spawnPoints[randomIndex];
        Vector3 spawnWorldPos = new Vector3(spawnTile.x + 0.5f, spawnTile.y + 0.5f, 0);

        GameObject item = Instantiate(randomEntry.prefab, spawnWorldPos, Quaternion.identity);
        activeCollectibles.Add(item);

        Debug.Log($"Respawned {randomEntry.prefab.name} at {spawnWorldPos}. Total: {activeCollectibles.Count}/{maxCollectiblesAlive}"); 
    }

    public void StopRespawn()
    {
        enableRespawn = false;
        StopAllCoroutines();
        Debug.Log("✓ Collectible respawn stopped");
    }

    public int GetActiveCollectiblesCount()
    {
        activeCollectibles.RemoveAll(item => item == null);
        return activeCollectibles.Count;
    }
}