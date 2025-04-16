using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("ITEM DATA")]
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    [SerializeField] private int maxNumberOfItems;

    [Header("EQUIPMENT SLOT")]
    public EquipmentSlotType slotType = EquipmentSlotType.None;

    [Header("ITEM SLOT")]
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] private Sprite defaultSlotSprite; // NEW: Default background sprite

    [Header("ITEM DESCRIPTION SLOT")]
    public GameObject itemDescriptionPanel;
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    [Header("Slot Type")]
    public bool isEquipmentSlot = false;


    [Header("SHADER SELECTED SLOT")]
    public GameObject selectedShader;
    public bool thisItemSelected;

    [Header("BUTTONS")]
    public Button equipButton;
    public Button unEquipButton;
    public Button deleteButton;
    public Button useButton;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError(" InventoryManager not found! Ensure InventoryMenu is active at runtime.");
        }

        itemDescriptionPanel.SetActive(false);
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, EquipmentSlotType newSlotType)
    {
        if (isFull) return quantity;  // If the slot is full, return the remaining quantity

        // Check if the item already exists in the slot
        if (this.itemName == itemName)
        {
            // Update quantity and handle item stack limit
            this.quantity += quantity;

            if (this.quantity >= maxNumberOfItems)
            {
                int extraItems = this.quantity - maxNumberOfItems;
                this.quantity = maxNumberOfItems;
                isFull = true;
                quantityText.text = this.quantity.ToString();
                quantityText.enabled = true;
                return extraItems;  // Return excess items
            }
            else
            {
                quantityText.text = this.quantity.ToString();
                quantityText.enabled = true;
                return 0;  // No excess items
            }
        }

        // If the item does not exist in this slot, update the slot with the new item
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;
        this.itemDescription = itemDescription;
        this.slotType = newSlotType;

        this.quantity = quantity;
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;

        return 0;  // No excess items
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemName)) return;

        if (thisItemSelected)
        {
            DeselectItem();
        }
        else
        {
            SelectItem();
        }
    }

    public void SelectItem()
    {
        if (inventoryManager == null)
        {
            Debug.LogError(" InventoryManager is NULL in SelectItem()!");
            return;
        }
        if (thisItemSelected)
        {
            inventoryManager.UseItem(itemName);
        }

        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;

        itemDescriptionPanel.SetActive(true);
        ItemDescriptionNameText.text = itemName;
        ItemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;

        PositionDescriptionPanel();

        // Try to get ItemSO or EquipmentSO
        ItemSO itemData = inventoryManager.GetItemSO(itemName);
        EquipmentSO equipmentData = inventoryManager.GetEquipmentSO(itemName);

        // Hide buttons by default
        equipButton.gameObject.SetActive(false);
        unEquipButton.gameObject.SetActive(false);
        useButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(true); // Always show delete

        // Show Equip button for equipment
        if (equipmentData != null)
        {
            equipButton.gameObject.SetActive(true);
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(EquipItem);
        }

        // Show Use button for usable items
        if (itemData != null && itemData.isUsable)
        {
            useButton.gameObject.SetActive(true);
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(UseItem);
        }
    }


    public void EquipItem()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is NULL in EquipItem()!");
            return;
        }

        // Get item data (the ScriptableObject)
        EquipmentSO equipmentData = inventoryManager.GetEquipmentSO(itemName);
        if (equipmentData == null)
        {
            Debug.LogError("No ItemSO found for item: " + itemName);
            return;
        }
            EquipmentSlotType itemSlotType = equipmentData.slotType;
        

        if (itemSlotType == EquipmentSlotType.None)
        {
            Debug.Log("Item is not equipable!");
            return;
        }

        EquippedSlot targetSlot = inventoryManager.GetEquipmentSlot(itemSlotType);
        if (targetSlot == null)
        {
            Debug.LogError($"No equipment slot found for type {itemSlotType}! Check InventoryManager.");
            return;
        }

        if (targetSlot.IsEquipped)
        {
            string oldItem = targetSlot.ItemName;
            Sprite oldSprite = targetSlot.ItemSprite;
            string oldDescription = targetSlot.ItemDescription;
        }
        else
        {
            targetSlot.EquipItem(itemSprite, itemName, itemDescription);
            RemoveItem(); // Remove equipped item from inventory
        }

        itemDescriptionPanel.SetActive(false);
    }


    public void SetItem(string newItemName, Sprite newItemSprite, EquipmentSlotType newSlotType)
    {
        itemName = newItemName;
        itemSprite = newItemSprite;
        itemImage.sprite = newItemSprite;
        if (isEquipmentSlot)
        {
            itemImage.color = Color.white;
        }
        isFull = true;
        quantity = 1;
        quantityText.enabled = false;
        this.slotType = newSlotType;

    }

    public void DeleteItem()
    {
        RemoveItem();
        itemDescriptionPanel.SetActive(false);
    }

    public void UseItem()
    {
        if (inventoryManager == null)
        {
            Debug.LogError(" InventoryManager is NULL in UseItem()!");
            return;
        }

        inventoryManager.UseItem(itemName);

        quantity--;

        if (quantity <= 0)
        {
            RemoveItem();
        }
        else
        {
            quantityText.text = quantity.ToString();
        }

        itemDescriptionPanel.SetActive(false);
    }

    public void RemoveItem()
    {
        itemName = "";
        itemSprite = null;
        quantity = 0;
        isFull = false;

        if (defaultSlotSprite != null)
        {
            itemImage.sprite = defaultSlotSprite;
        }
        else
        {
            Debug.LogWarning(" Default slot sprite not set on: " + gameObject.name);
            itemImage.sprite = null;
        }

        if (isEquipmentSlot)
        {
            itemImage.color = Color.black;
        }

        quantityText.text = "";
    }

    private void PositionDescriptionPanel()
    {
        RectTransform panelRect = itemDescriptionPanel.GetComponent<RectTransform>();
        RectTransform slotRect = GetComponent<RectTransform>();

        Vector3 slotScreenPos = slotRect.position;

        float offsetX = 150f;
        float offsetY = -550f;

        Vector3 newPos = slotScreenPos + new Vector3(offsetX, offsetY, 0);

        Vector3[] panelCorners = new Vector3[4];
        panelRect.GetWorldCorners(panelCorners);
        float panelWidth = panelCorners[2].x - panelCorners[0].x;
        float panelHeight = panelCorners[2].y - panelCorners[0].y;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (newPos.x + panelWidth > screenWidth)
            newPos.x = screenWidth - panelWidth - 550;

        if (newPos.y - panelHeight < 0)
            newPos.y = panelHeight + 550;

        if (newPos.x < 0)
            newPos.x = 550;

        if (newPos.y > screenHeight)
            newPos.y = screenHeight - 550;

        panelRect.position = newPos;
    }

    public void DeselectItem()
    {
        selectedShader.SetActive(false);
        thisItemSelected = false;
        itemDescriptionPanel.SetActive(false);
    }
    public void DropItem()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is NULL in DropItem()!");
            return;
        }

        inventoryManager.DropItem(itemName, itemSprite, itemDescription, slotType);

        quantity--;

        if (quantity <= 0)
        {
            RemoveItem();
        }
        else
        {
            quantityText.text = quantity.ToString();
        }

        itemDescriptionPanel.SetActive(false);
    }

}