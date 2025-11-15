using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies = 10;
    public bool enableAutoRespawn = false;
    public float respawnInterval = 0.1f;

    private Coroutine respawnCoroutine;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private HashSet<Vector2Int> cachedFloorPositions;
    private Vector2Int cachedPlayerStartPosition;

    public void SpawnEnemies(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        activeEnemies.Clear();
        cachedFloorPositions = new HashSet<Vector2Int>(floorPositions);
        cachedPlayerStartPosition = playerStartPosition;

        List<Vector2Int> spawnPoints = floorPositions.ToList();

        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                spawnPoints.Remove(playerStartPosition + new Vector2Int(x, y));
            }
        }

        if (spawnPoints.Count < numberOfEnemies)
        {
            numberOfEnemies = spawnPoints.Count;
        }

        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (spawnPoints.Count == 0) break;

            int randomIndex = Random.Range(0, spawnPoints.Count);
            Vector2Int spawnPosition = spawnPoints[randomIndex];
            Vector3 spawnWorldPosition = new Vector3(spawnPosition.x + 0.5f, spawnPosition.y + 0.5f, 0);

            GameObject enemy = Instantiate(enemyPrefab, spawnWorldPosition, Quaternion.identity);
            activeEnemies.Add(enemy);

            spawnPoints.RemoveAt(randomIndex);
        }
    }

    public void SpawnSingleEnemy(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        List<Vector2Int> spawnPoints = floorPositions.ToList();

        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                spawnPoints.Remove(playerStartPosition + new Vector2Int(x, y));
            }
        }

        if (spawnPoints.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Count);
        Vector2Int spawnPosition = spawnPoints[randomIndex];
        Vector3 spawnWorldPosition = new Vector3(spawnPosition.x + 0.5f, spawnPosition.y + 0.5f, 0);

        GameObject enemy = Instantiate(enemyPrefab, spawnWorldPosition, Quaternion.identity);
        activeEnemies.Add(enemy);
    }

    public void StartAutoRespawn(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }

        cachedFloorPositions = new HashSet<Vector2Int>(floorPositions);
        cachedPlayerStartPosition = playerStartPosition;

        enableAutoRespawn = true;
        respawnCoroutine = StartCoroutine(AutoRespawnRoutine());
    }

    private IEnumerator AutoRespawnRoutine()
    {
        int spawnCount = 0;

        while (enableAutoRespawn)
        {
            yield return new WaitForSeconds(respawnInterval);

            if (cachedFloorPositions == null || cachedFloorPositions.Count == 0)
            {
                break;
            }
            int beforeClean = activeEnemies.Count;
            activeEnemies.RemoveAll(e => e == null);
            int afterClean = activeEnemies.Count;
            spawnCount++;
            SpawnSingleEnemy(cachedFloorPositions, cachedPlayerStartPosition);
        }
    }

    public void StopRespawn()
    {
        enableAutoRespawn = false;

        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }
    }

    public void ClearActiveEnemies()
    {
        activeEnemies.Clear();
    }

    public int GetAliveEnemiesCount()
    {
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count;
    }
}