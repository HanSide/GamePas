using UnityEngine;
using TMPro;
using System.Collections.Generic;
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
    [SerializeField] private List<CollectibleType> collectibles = new List<CollectibleType>();
    public static MultiCollectibleCounter Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ini bikin gak ke-destroy pas ganti scene

            // biar UI bisa update otomatis tiap pindah scene
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ResetUI();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // cari lagi semua TMP_Text di scene baru
        // kamu bisa pasang ulang reference ke UI-nya (misal pakai tag "CollectibleUI")
        foreach (var collectible in collectibles)
        {
            if (collectible.counterText == null)
            {
                var ui = GameObject.Find(collectible.itemName + "Counter"); // pastikan nama GameObject-nya sama
                if (ui != null)
                    collectible.counterText = ui.GetComponent<TMP_Text>();
            }

            UpdateUI(collectible);
        }
    }

    private void ResetUI()
    {
        foreach (var collectible in collectibles)
        {
            collectible.currentAmount = Mathf.Clamp(collectible.currentAmount, 0, collectible.maxAmount);
            UpdateUI(collectible);
        }
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

    private void UpdateUI(CollectibleType collectible)
    {
        if (collectible.counterText != null)
            collectible.counterText.text = $"{collectible.currentAmount}/{collectible.maxAmount}";
    }

    private void OnTypeCompleted(string itemName)
    {
        Debug.Log($"🎉 Semua {itemName} terkumpul!");
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
        Debug.Log("Semua collectible lengkap! Pindah ke level berikutnya...");
        LevelManager.Instance?.GoToNextLevel();
    }

    public int GetCurrentAmount(string itemName)
    {
        var collectible = collectibles.Find(c => c.itemName == itemName);
        return collectible != null ? collectible.currentAmount : 0;
    }

    public int GetMaxAmount(string itemName)
    {
        var collectible = collectibles.Find(c => c.itemName == itemName);
        return collectible != null ? collectible.maxAmount : 0;
    }
}
