using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CollectibleType
{
    public string itemName;
    public TMP_Text counterText;
    public int maxAmount;
    [HideInInspector] public int currentAmount;
}

public class MultiCollectibleCounter : MonoBehaviour
{
    [SerializeField] public List<CollectibleType> collectibles = new List<CollectibleType>();
    public static MultiCollectibleCounter Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void Start()
    {
        foreach (var collectible in collectibles)
        {
            UpdateUI(collectible);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void AddCollectible(string itemName, int amount = 1)
    {
        var collectible = collectibles.Find(c => c.itemName == itemName);

        if (collectible != null)
        {
            collectible.currentAmount += amount;
            collectible.currentAmount = Mathf.Clamp(collectible.currentAmount, 0, collectible.maxAmount);
            UpdateUI(collectible);

            Debug.Log($"[Counter] Collected {itemName}: {collectible.currentAmount}/{collectible.maxAmount}");

            if (collectible.currentAmount >= collectible.maxAmount) 
            {
                OnTypeCompleted(itemName);
            }

            CheckAllCompleted();
        }
        else
        {
            Debug.LogWarning($"Collectible '{itemName}' not found!");
        }
    }

    public void UpdateUI(CollectibleType collectible)
    {
        if (collectible.counterText != null)
            collectible.counterText.text = $"{collectible.currentAmount}/{collectible.maxAmount}";
    }

    private void OnTypeCompleted(string itemName)
    {
        Debug.Log($"Collectible {itemName} Completed");
    }

    private void CheckAllCompleted()
    {
        foreach (var collectible in collectibles)
        {
            if (collectible.currentAmount < collectible.maxAmount)
                return;
        }   

        OnAllCollectiblesCompleted();
    }

    private void OnAllCollectiblesCompleted()
    {
        Debug.Log("All CollectiblesCompleted");
        Time.timeScale = 1f;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.GoToNextLevel();
        }
        else
        {
            Debug.LogError("LevelManager Instance is null! Cannot proceed to next level.");
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ReassignUICounters());
    }

    private IEnumerator ReassignUICounters()
    {
        yield return null; 

        foreach (var collectible in collectibles)
        {
            if (collectible.counterText == null)
            {
                var ui = GameObject.Find(collectible.itemName + "Counter");
                if (ui != null)
                    collectible.counterText = ui.GetComponent<TMP_Text>();
            }
            UpdateUI(collectible);
        }
    }

}
