using UnityEngine;
using TMPro;

public class CollectibleCounter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private int maxCollectibles = 5;

    private int currentCount = 0;

    public static CollectibleCounter Instance { get; private set; }

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateCounterUI();
    }


    public void AddCollectible(int amount = 1)
    {
        currentCount += amount;
        UpdateCounterUI();

        if (currentCount >= maxCollectibles)
        {
            Debug.Log("Semua collectible terkumpul!");
            OnAllCollected();
        }
    }

    public void ResetCounter()
    {
        currentCount = 0;
        UpdateCounterUI();
    }

    private void UpdateCounterUI()
    {
        if (counterText != null)
        {
            counterText.text = $"{currentCount}/{maxCollectibles}";
        }
    }

    private void OnAllCollected()
    {
        Debug.Log("All collectibles collected! Level complete!");
    }

    public int GetCurrentCount()
    {
        return currentCount;
    }

    // Getter untuk max
    public int GetMaxCount()
    {
        return maxCollectibles;
    }
}