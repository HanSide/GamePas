using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    // Data yang disimpandi setiap slot inventory
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDesc;


    [SerializeField] public GameObject selectedItem;
    private bool isSelected;



    // Referensi ke komponen UI
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] public TextMeshProUGUI ItemDescNameText;
    [SerializeField] public TextMeshProUGUI ItemDescText;
    [SerializeField] public Image ItemDescImage;


    void Start()
    {
        if (ItemDescImage == null)
            ItemDescImage = FindAnyObjectByType<InventoryManager>().itemDescImage;
        if (ItemDescNameText == null)
            ItemDescNameText = FindAnyObjectByType<InventoryManager>().itemDescNameText;
        if (ItemDescText == null)
            ItemDescText = FindAnyObjectByType<InventoryManager>().itemDescText;
    }


    public void AddItem(string newItemName, int newQuantity, Sprite newItemSprite, string itemDescription)
    {
        if (!isFull)
        {
            itemName = newItemName;
            quantity += newQuantity;
            itemSprite = newItemSprite;
            itemDesc = itemDescription;
            isFull = true;
        }
        else if (itemName == newItemName)
        {
            quantity += newQuantity;
        }


        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = newItemSprite;
        itemImage.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

    }

    public void OnLeftClick()
    {

        isSelected = !isSelected;
        selectedItem.SetActive(isSelected);

        if (ItemDescNameText != null && ItemDescText != null && ItemDescImage != null)
        {
            if (isSelected)
            {
                ItemDescNameText.text = itemName;
                ItemDescText.text = itemDesc;
                ItemDescImage.sprite = itemSprite;
                ItemDescImage.enabled = true;
            }
            else
            {
                ItemDescNameText.text = "";
                ItemDescText.text = "";
                ItemDescImage.sprite = null;
                ItemDescImage.enabled = false;
            }
        }
    }

    public void OnRightClick()
    {
        Debug.Log("kanan");
    }
}