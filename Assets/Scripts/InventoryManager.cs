using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class InventoryManager : MonoBehaviour
{
    public InputAction OpenInventory;
    public GameObject InventoryMenu;
    private bool menuActivated;
    public GameObject itemSlotPrefab;
    public Transform inventorySlotsParent;
    public TextMeshProUGUI itemDescNameText;
    public TextMeshProUGUI itemDescText;
    public Image itemDescImage;

    private List<ItemSlot> itemSlots = new List<ItemSlot>();

    public static InventoryManager Instance { get; private set; }

    void Awake()
    {
        itemDescImage.enabled = false;
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

        OpenInventory = new InputAction("OpenInventory", binding: "<Keyboard>/I");
    }

    void OnEnable()
    {
        OpenInventory.Enable();
        OpenInventory.performed += ToggleInventory;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        OpenInventory.performed -= ToggleInventory;
        OpenInventory.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        menuActivated = !menuActivated;
        InventoryMenu.SetActive(menuActivated);
        Time.timeScale = menuActivated ? 0f : 1f;
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {

        foreach (var slot in itemSlots)
        {
            if (slot.itemName == itemName && slot.isFull)
            {
                slot.AddItem(itemName, quantity, itemSprite, itemDescription);
                Debug.Log($"Stacked {quantity} of {itemName} to inventory. Total: {slot.quantity}");
                return;
            }
        }


        GameObject newSlotObj = Instantiate(itemSlotPrefab, inventorySlotsParent);
        ItemSlot newSlot = newSlotObj.GetComponent<ItemSlot>();

        if (newSlot != null)
        {
            newSlot.AddItem(itemName, quantity, itemSprite, itemDescription);
            itemSlots.Add(newSlot);
            Debug.Log($"Created new slot and added {quantity} of {itemName} to inventory.");
        }
        else
        {
            Debug.LogError("ItemSlot component not found on prefab!");
            Destroy(newSlotObj);
        }
    }

    public void ClearDescriptionPanel()
    {
        if (itemDescImage != null)
            itemDescImage.enabled = false;
        if (itemDescNameText != null)
            itemDescNameText.text = "";
        if (itemDescText != null)
            itemDescText.text = "";
    }


    public int GetItemQuantity(string itemName)
    {
        int total = 0;
        foreach (var slot in itemSlots)
        {
            if (slot.itemName == itemName && slot.isFull)
            {
                total += slot.quantity;
            }
        }
        return total;
    }
    public bool HasItem(string itemName)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemName == itemName && slot.isFull && slot.quantity > 0)
            {
                return true;
            }
        }
        return false;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        Time.timeScale = 1f;
        menuActivated = false;
        if (InventoryMenu == null)
            InventoryMenu = GameObject.Find("InventoryMenu");

        if (inventorySlotsParent == null)
            inventorySlotsParent = GameObject.Find("InventorySlotsParent")?.transform;

        if (InventoryMenu != null)
            InventoryMenu.SetActive(false);
    }
}