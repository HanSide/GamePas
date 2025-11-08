using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class CollectibleEntry
    {
        public string name;
        public GameObject prefab;
        public int amountToSpawn = 3;
        public bool unlimitedSpawn = false;
        public int maxCollectiblesAlive = 10;
        public List<int> spawnInLevels;
    }

    [Header("Collectibles List")]
    public List<CollectibleEntry> collectibles = new List<CollectibleEntry>();

    [Header("Respawn Settings")]
    public float respawnInterval = 2f;

    private List<GameObject> activeCollectibles = new List<GameObject>();
    private List<Vector2Int> cachedSpawnTiles = new List<Vector2Int>();
    private Vector2Int cachedPlayerStart;
    private Coroutine respawnRoutine;

    public void SpawnCollectibles(HashSet<Vector2Int> floorTiles, Vector2Int playerStart, int level)
    {
        List<Vector2Int> spawnPoints = new List<Vector2Int>(floorTiles);
        spawnPoints.Remove(playerStart);

        foreach (var collectible in collectibles)
        {
            if (!collectible.spawnInLevels.Contains(level)) continue;
            if (collectible.prefab == null) continue;
            int spawnCount = Mathf.Min(collectible.amountToSpawn, spawnPoints.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                if (spawnPoints.Count == 0) break;

                int index = Random.Range(0, spawnPoints.Count);
                Vector2Int tile = spawnPoints[index];
                Vector3 worldPos = new Vector3(tile.x + 0.5f, tile.y + 0.5f, 0);

                GameObject item = Instantiate(collectible.prefab, worldPos, Quaternion.identity);
                activeCollectibles.Add(item);

                spawnPoints.RemoveAt(index);
            }

            Debug.Log($"Spawned {spawnCount} of {collectible.prefab.name} (Level {level})");
        }
    }
    public void UpdateCachedPositions(HashSet<Vector2Int> floorTiles, Vector2Int playerStart)
    {
        cachedSpawnTiles = new List<Vector2Int>(floorTiles);
        cachedSpawnTiles.Remove(playerStart);
        cachedPlayerStart = playerStart;
    }

    public void StartRespawn(HashSet<Vector2Int> floorTiles, Vector2Int playerStart)
    {
        UpdateCachedPositions(floorTiles, playerStart);

        if (respawnRoutine != null)
            StopCoroutine(respawnRoutine);

        respawnRoutine = StartCoroutine(RespawnLoop());
    }

    public void StopRespawn()
    {
        if (respawnRoutine != null)
            StopCoroutine(respawnRoutine);
    }

    private IEnumerator RespawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnInterval);
            TryRespawn();
        }
    }

    private void TryRespawn()
    {
        if (cachedSpawnTiles == null || cachedSpawnTiles.Count == 0) return;

        List<CollectibleEntry> valid = new List<CollectibleEntry>();

        foreach (var c in collectibles)
        {
            if (c.unlimitedSpawn)
            {
                valid.Add(c);
                continue;
            }

            int alive = CountAliveOfType(c.prefab.name);
            if (alive < c.maxCollectiblesAlive)
                valid.Add(c);
        }

        if (valid.Count == 0) return;

        CollectibleEntry selected = valid[Random.Range(0, valid.Count)];

        if (!selected.unlimitedSpawn)
        {
            int alive = CountAliveOfType(selected.prefab.name);
            if (alive >= selected.maxCollectiblesAlive) return;
        }

        Vector2Int tile = cachedSpawnTiles[Random.Range(0, cachedSpawnTiles.Count)];
        Vector3 pos = new Vector3(  tile.x + 0.5f, tile.y + 0.5f, 0);

        GameObject item = Instantiate(selected.prefab, pos, Quaternion.identity);
        activeCollectibles.Add(item);
    }

    private int CountAliveOfType(string name)
    {
        int count = 0;
        foreach (var obj in activeCollectibles)
        {
            if (obj != null && obj.name.Contains(name))
                count++;
        }
        return count;
    }
}
