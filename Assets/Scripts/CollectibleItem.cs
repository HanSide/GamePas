using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private int quantity = 1;
    [TextArea]
    [SerializeField] private string itemDescription;
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;

        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager.Instance is null!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (inventoryManager != null)
            {
                inventoryManager.AddItem(itemName, quantity, itemSprite, itemDescription);
            }
            else
            {
                Debug.LogError("InventoryManager is null!");
            }

            if (MultiCollectibleCounter.Instance != null)
            {
                MultiCollectibleCounter.Instance.AddCollectible(itemName, quantity);
            }
            else
            {
                Debug.LogWarning("MultiCollectibleCounter.Instance is null! Counter UI won't update.");
            }
            Destroy(gameObject);
        }
    }
}