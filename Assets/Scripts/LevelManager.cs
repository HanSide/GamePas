using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("References")]
    public EnemySpawner enemySpawner;
    public LevelGenerator levelGenerator;
    public Transform player;
    public HashSet<Vector2Int> floorPositions;
    public Vector2Int playerStartPosition;
    public CollectibleSpawner collectibleSpawner;
    public MultiCollectibleCounter collectibleCounter;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public float spawnIntervalLevel2 = 0.1f;

    private Coroutine spawnCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Player found and assigned!");
            }
            else
            {
                Debug.LogError("Player not found! Make sure Player has 'Player' tag.");
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void GoToNextLevel()
    {
        currentLevel++;
        Debug.Log($"===> Level {currentLevel} started!");

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        if (enemySpawner != null)
        {
            enemySpawner.StopRespawn();
        }

        if (currentLevel == 2)
        {
            StartCoroutine(TransitionToLevel2());
        }
    }

    private IEnumerator TransitionToLevel2()
    {
        Debug.Log("=== TRANSITIONING TO LEVEL 2 ===");
        Time.timeScale = 1f;

        Debug.Log($"CurrentLevel at transition start: {currentLevel}");

        CleanupEnemies();
        CleanupCollectibles();

        yield return null;

        if (levelGenerator != null)
        {
            Debug.Log("Generating new level...");
            levelGenerator.walkSteps += 100;
            levelGenerator.GenerateLevelWithRetries();

            yield return null;

            floorPositions = levelGenerator.GetFloorPositions();
            playerStartPosition = levelGenerator.GetPlayerStartPosition();

            Debug.Log($"New level generated. Floor tiles: {floorPositions?.Count ?? 0}, playerStart: {playerStartPosition}");
        }
        else
        {
            Debug.LogError("LevelGenerator is null!");
            yield break;
        }

        ResetPlayerPosition();

        if (collectibleSpawner != null)
        {
            collectibleSpawner.UpdateCachedPositions(floorPositions, playerStartPosition);
        }

        StartLevel2();

        Debug.Log("=== LEVEL 2 READY ===");
    }


    private void StartLevel2()
    {
        if (enemySpawner != null && floorPositions != null)
        {
            enemySpawner.respawnInterval = spawnIntervalLevel2;
                                                                
            enemySpawner.numberOfEnemies = 5;

            enemySpawner.SpawnEnemies(floorPositions, playerStartPosition);
            enemySpawner.StartAutoRespawn(floorPositions, playerStartPosition);

            Debug.Log("✓ Level 2 UNLIMITED enemy respawn started");
        }
        else
        {
            Debug.LogWarning("Cannot start Level 2: EnemySpawner or floorPositions is null!");
        }

        if (collectibleSpawner != null && floorPositions != null)
        {
            Debug.Log($"Spawning initial Level 2 collectibles...");
            collectibleSpawner.SpawnCollectibles(floorPositions, playerStartPosition, currentLevel);
            collectibleSpawner.StartRespawn(floorPositions, playerStartPosition);
            Debug.Log("✓ Level 2 collectible system started");
        }
        else
        {
            Debug.LogWarning("Cannot start collectible respawn!");
        }
    }

    private void CleanupEnemies()
    {
        if (enemySpawner != null)
        {
            enemySpawner.StopRespawn();
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        if (enemySpawner != null)
        {
            enemySpawner.ClearActiveEnemies();
        }

        Debug.Log($"✓ Cleaned up {enemies.Length} enemies");
    }
    private void CleanupCollectibles()
    {
        if (collectibleSpawner != null)
        {
            collectibleSpawner.StopRespawn();
        }

        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        foreach (GameObject collectible in collectibles)
        {
            Destroy(collectible);
        }
        Debug.Log($"✓ Cleaned up {collectibles.Length} old collectibles");
    }

    private void ResetPlayerPosition()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null! Cannot reset position.");
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player GameObject not found!");
                return;
            }
        }

        Vector3 newPosition = new Vector3(
            playerStartPosition.x + 0.5f,
            playerStartPosition.y + 0.5f,
            player.position.z
        );

        player.position = newPosition;
        Debug.Log($"✓ Player position reset to: {newPosition}");

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            Debug.Log("✓ Player velocity reset");
        }
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
            Debug.Log("✓ Player animator reset");
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        Debug.Log($"[LevelManager] Scene loaded: {scene.name}, TimeScale: {Time.timeScale}");
    }
}