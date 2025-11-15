using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        StartCoroutine(FindInventoryManager());
    }

    private IEnumerator FindInventoryManager()
    {
        yield return null;

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
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(itemName, quantity, itemSprite, itemDescription);
            }
            else
            {
                Debug.LogError("InventoryManager.Instance is null when collecting!");
            }

            if (MultiCollectibleCounter.Instance != null)
            {
                MultiCollectibleCounter.Instance.AddCollectible(itemName, quantity);
            }

            Destroy(gameObject);
        }
    }
}