using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public int numberOfEnemies = 10;
    public bool enableAutoRespawn = false; // checklist di inspector

    [Header("Respawn Settings (aktif kalau enableAutoRespawn = true)")]
    public float respawnInterval = 1f;

    private Coroutine respawnCoroutine;

    public void SpawnEnemies(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        List<Vector2Int> spawnPoints = floorPositions.ToList();

        // bikin area aman di sekitar player
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                spawnPoints.Remove(playerStartPosition + new Vector2Int(x, y));
            }
        }

        if (spawnPoints.Count < numberOfEnemies)
        {
            Debug.LogWarning("Kurang banyak tile buat spawn musuh! Spawn semampunya aja.");
            numberOfEnemies = spawnPoints.Count;
        }

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            Vector2Int spawnPosition = spawnPoints[randomIndex];
            Vector3 spawnWorldPosition = new Vector3(spawnPosition.x + 0.5f, spawnPosition.y + 0.5f, 0);
            Instantiate(enemyPrefab, spawnWorldPosition, Quaternion.identity);
            spawnPoints.RemoveAt(randomIndex);
        }

        // mulai respawn kalau aktif
        if (enableAutoRespawn && respawnCoroutine == null)
        {
            respawnCoroutine = StartCoroutine(AutoRespawn(floorPositions, playerStartPosition));
        }
    }

    private IEnumerator AutoRespawn(HashSet<Vector2Int> floorPositions, Vector2Int playerStartPosition)
    {
        while (enableAutoRespawn)
        {
            yield return new WaitForSeconds(respawnInterval);
            SpawnEnemies(floorPositions, playerStartPosition);
        }
    }

    public void StopRespawn()
    {
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }
    }
}
