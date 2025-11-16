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

    // Static data untuk persist antar scene
    private static List<InventoryItemData> persistentInventoryData = new List<InventoryItemData>();

    public static InventoryManager Instance { get; private set; }

    [System.Serializable]
    public class InventoryItemData
    {
        public string itemName;
        public int quantity;
        public Sprite itemSprite;
        public string itemDescription;

        public InventoryItemData(string name, int qty, Sprite sprite, string desc)
        {
            itemName = name;
            quantity = qty;
            itemSprite = sprite;
            itemDescription = desc;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (itemDescImage != null)
            itemDescImage.enabled = false;

        OpenInventory = new InputAction("OpenInventory", binding: "<Keyboard>/E");

    }

    void Start()
    {
        if (persistentInventoryData.Count > 0)
        {
            LoadInventoryData();
        }
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

    void OnDestroy()
    {
        if (Instance == this)
        {
            SaveInventoryData();
            Instance = null;
        }
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        if (InventoryMenu == null) return;

        menuActivated = !menuActivated;
        InventoryMenu.SetActive(menuActivated);
        Time.timeScale = menuActivated ? 0f : 1f;
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        if (inventorySlotsParent == null)
        {
            Debug.LogError("[InventoryManager] inventorySlotsParent is null!");
            return;
        }

        foreach (var slot in itemSlots)
        {
            if (slot != null && slot.itemName == itemName && slot.isFull)
            {
                slot.AddItem(itemName, quantity, itemSprite, itemDescription);
                return;
            }
        }

        GameObject newSlotObj = Instantiate(itemSlotPrefab, inventorySlotsParent);
        ItemSlot newSlot = newSlotObj.GetComponent<ItemSlot>();

        if (newSlot != null)
        {
            newSlot.AddItem(itemName, quantity, itemSprite, itemDescription);
            itemSlots.Add(newSlot);
        }
        else
        {
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
            if (slot != null && slot.itemName == itemName && slot.isFull)
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
            if (slot != null && slot.itemName == itemName && slot.isFull && slot.quantity > 0)
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

        if (InventoryMenu != null)
        {
            InventoryMenu.SetActive(false);
        }
    }

    private void SaveInventoryData()
    {
        persistentInventoryData.Clear();

        foreach (var slot in itemSlots)
        {
            if (slot != null && slot.isFull && slot.quantity > 0)
            {
                persistentInventoryData.Add(new InventoryItemData(
                    slot.itemName,
                    slot.quantity,
                    slot.itemSprite,
                    slot.itemDesc
                ));
            }
        }
    }

    private void LoadInventoryData()
    {
        foreach (var data in persistentInventoryData)
        {
            AddItem(data.itemName, data.quantity, data.itemSprite, data.itemDescription);
        }
    }

    public void ResetInventory()
    {
        foreach (var slot in itemSlots)
        {
            if (slot != null && slot.gameObject != null)
            {
                Destroy(slot.gameObject);
            }
        }

        itemSlots.Clear();
        persistentInventoryData.Clear();

        ClearDescriptionPanel();

    }
}