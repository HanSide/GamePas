using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public int numberOfEnemies = 10;

    [Header("Auto Respawn (Level 2)")]
    public bool enableAutoRespawn = false;
    public float respawnInterval = 1f;
    public int maxEnemiesAlive = 20; 

    private Coroutine respawnCoroutine;
    private List<GameObject> activeEnemies = new List<GameObject>();

 
    public void SpawnEnemies(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
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
            Debug.LogWarning($"Not enough spawn points! Spawning {spawnPoints.Count} instead of {numberOfEnemies}");
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

        Debug.Log($"Spawned {numberOfEnemies} enemies");
    }
    public void SpawnSingleEnemy(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        activeEnemies.RemoveAll(e => e == null);

        if (activeEnemies.Count >= maxEnemiesAlive)
        {
            Debug.Log($"Max enemies reached ({maxEnemiesAlive}). Skipping spawn.");
            return;
        }

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
            Debug.LogWarning("No valid spawn points!");
            return;
        }

        int randomIndex = Random.Range(0, spawnPoints.Count);
        Vector2Int spawnPosition = spawnPoints[randomIndex];
        Vector3 spawnWorldPosition = new Vector3(spawnPosition.x + 0.5f, spawnPosition.y + 0.5f, 0);

        GameObject enemy = Instantiate(enemyPrefab, spawnWorldPosition, Quaternion.identity);
        activeEnemies.Add(enemy);

        Debug.Log($"Spawned 1 enemy. Total alive: {activeEnemies.Count}/{maxEnemiesAlive}");
    }
    public void StartAutoRespawn(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }

        enableAutoRespawn = true;
        respawnCoroutine = StartCoroutine(AutoRespawnRoutine(floorPositions, playerStartPosition));
        Debug.Log("✓ Auto respawn started");
    }

    private IEnumerator AutoRespawnRoutine(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        while (enableAutoRespawn)
        {
            yield return new WaitForSeconds(respawnInterval);

            SpawnSingleEnemy(floorPositions, playerStartPosition);
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

        Debug.Log("✓ Auto respawn stopped");
    }

    public int GetAliveEnemiesCount()
    {
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count;
    }
}