using System.Collections;
using System.Collections.Generic;
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
    public float spawnIntervalLevel2 = 1f;

    private Coroutine spawnCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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

    public void GoToNextLevel()
    {
        currentLevel++;
        Debug.Log($"===> Level {currentLevel} started!");


        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (currentLevel == 2)
        {
            StartCoroutine(TransitionToLevel2());
        }
        else
        {
            Debug.Log($"Level {currentLevel} belum punya rule khusus.");
        }
    }

    // LevelManager.cs

    private IEnumerator TransitionToLevel2()
    {
        Debug.Log("=== TRANSITIONING TO LEVEL 2 ===");

        CleanupEnemies();
        CleanupCollectibles();

        RefreshReferences();

        
        if (levelGenerator != null)
        {
            Debug.Log("Generating new level...");
            levelGenerator.walkSteps += 100;
            levelGenerator.GenerateLevelWithRetries(); 

            yield return new WaitForSeconds(0.3f);

        }

        ResetPlayerPosition();

        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
        }

        StartLevel2(); 

        Debug.Log("=== LEVEL 2 READY ===");
    }

    private void StartLevel2()
    {
        if (enemySpawner != null)
        {
            enemySpawner.enableAutoRespawn = true;
            enemySpawner.respawnInterval = spawnIntervalLevel2;

            if (spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(SpawnEnemiesContinuously());
                Debug.Log("✓ Continuous enemy spawn started");
            }
        }
        else
        {
            Debug.LogWarning("EnemySpawner belum di-assign!");
        }
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

        PlayerBehavior playerBehavior = player.GetComponent<PlayerBehavior>();
        if (playerBehavior != null)
        {
            Debug.Log("✓ Player behavior reset");
        }
    }

    private IEnumerator SpawnEnemiesContinuously()
    {
        while (enemySpawner != null && enemySpawner.enableAutoRespawn)
        {
            if (floorPositions != null && floorPositions.Count > 0)
            {
                enemySpawner.SpawnEnemies(floorPositions, playerStartPosition);
                Debug.Log($"[Spawn] Enemies spawned at interval {spawnIntervalLevel2}s");
            }
            else
            {
                Debug.LogWarning("FloorPositions is null or empty!");
            }

            yield return new WaitForSeconds(spawnIntervalLevel2);
        }

        Debug.Log("Continuous spawn stopped");
        spawnCoroutine = null;
    }

    private void CleanupEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        Debug.Log($"✓ Cleaned up {enemies.Length} enemies");
    }
    private void CleanupCollectibles() 
    {
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible"); 

        foreach (GameObject collectible in collectibles)
        {
            Destroy(collectible);
        }

        Debug.Log($"✓ Cleaned up {collectibles.Length} old collectibles");
    }
    private void RefreshReferences()
    {


        if (collectibleSpawner == null) 
        {
            collectibleSpawner = FindObjectOfType<CollectibleSpawner>();
            Debug.Log("✓ CollectibleSpawner reference refreshed");
        }


        if (collectibleCounter == null)
        {
            collectibleCounter = FindObjectOfType<MultiCollectibleCounter>();
            Debug.Log("✓ MultiCollectibleCounter reference refreshed");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
    }
    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

}