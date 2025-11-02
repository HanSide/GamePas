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
    public HashSet<Vector2Int> floorPositions;
    public Vector2Int playerStartPosition;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public float spawnIntervalLevel2 = 1f;

    private Coroutine spawnCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void GoToNextLevel()
    {
        currentLevel++;
        Debug.Log($"===> Level {currentLevel} started!");

        if (currentLevel == 2)
        {
            StartLevel2();

            if (levelGenerator != null)
            {
                levelGenerator.walkSteps += 100; 
                levelGenerator.GenerateLevelWithRetries();
            }
            else
            {
                Debug.LogWarning("LevelGenerator belum di-assign di LevelManager!");
            }
        }
        else
        {
            Debug.Log($"Level {currentLevel} belum punya rule khusus.");
        }
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
            }
        }
        else
        {
            Debug.LogWarning("EnemySpawner belum di-assign!");
        }
    }
    private IEnumerator SetupLevel2Delayed()
    {
        yield return new WaitForSeconds(0.5f);

        levelGenerator = FindObjectOfType<LevelGenerator>();
        enemySpawner = FindObjectOfType<EnemySpawner>();    

        if (levelGenerator != null)
        {
            levelGenerator.walkSteps += 100;
            levelGenerator.GenerateLevelWithRetries();
        }

        StartLevel2();
    }

    private IEnumerator SpawnEnemiesContinuously()
    {
        while (enemySpawner != null && enemySpawner.enableAutoRespawn)
        {
            enemySpawner.SpawnEnemies(floorPositions, playerStartPosition);
            yield return new WaitForSeconds(spawnIntervalLevel2);
        }

        spawnCoroutine = null;
    }

}
