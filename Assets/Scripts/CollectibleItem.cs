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
        Debug.Log("Nabrak: " + collision.name);
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Kena Player!");
            Destroy(gameObject);
        }
    }

}