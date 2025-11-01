using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private int quantity = 1;

    //private InventoryManager inventoryManager;

    void Start()
    {
        //inventoryManager = FindAnyObjectByType<InventoryManager>();
        //if (inventoryManager == null)
        //{
        //    Debug.LogError("Inventory manager has not assigned");
        //}
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Player"))
        //{
        //    if (inventoryManager != null)
        //    { 
        //        // Add the item to the inventory
        //        inventoryManager.AddItem(itemName, quantity, itemSprite);

        //        // Destroy the collectible item from the scene
        //        Destroy(gameObject);

        //    }

        //}
    }
}