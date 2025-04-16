using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class EquippedSlot : MonoBehaviour, IPointerClickHandler
{
    // SLOT APPEARANCE//
    [SerializeField] private Image slotImage; // Image for the equipped item slot (e.g., helmet, backpack)
    // SLOT DATA//
    [SerializeField] private EquipmentSlotType slotType; // Type of equipment slot (e.g., helmet, armor)
    private ItemSO equippedItemSO;


    [Header("GAME OBJECTS")]
    public GameObject selectedShader; // Visual indication for selected item
    public bool thisItemSelected; // Flag to check if the item is selected


    [Header("ITEM DESCRIPTION SLOT")]
    public GameObject itemDescriptionPanel; // Panel to show item description
    public Image itemDescriptionImage; // Image for item in description
    public TMP_Text ItemDescriptionNameText; // Text for item name
    public TMP_Text ItemDescriptionText; // Text for item description

    // ITEM DATA//
    private Sprite itemSprite; // Item sprite for visual representation
    private string itemName; // Name of the item
    private string itemDescription; // Description of the item

    // PROPERTIES//
    public bool IsEquipped => isEquipped; // Property to check if the item is equipped
    public string ItemName => itemName; // Property to get the item name
    public Sprite ItemSprite => itemSprite; // Property to get the item sprite
    public string ItemDescription => itemDescription; // Property to get the item description

    // OTHER VARIABLES//
    private bool isEquipped; // Flag to track if the item is equipped

    [Header("EQUIPPED SLOT IMAGE ICON")]
    public Sprite defaultSlotIcon;

    [Header("INVENTORY MANAGER")]
    public InventoryManager inventoryManager; // Reference to the InventoryManager

    [Header("BUTTONS")]
    public Button equipButton;
    public Button unEquipButton;
    public Button deleteButton;
    public Button useButton;
    public Button dropButton;

    // This function is called when an item slot is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemName)) return; // Avoid null or empty items

        if (thisItemSelected) // If the item is already selected, deselect it
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
            Debug.LogError("InventoryManager is NULL in SelectItem()!");
            return;
        }

        inventoryManager.DeselectAllSlots(); // Deselect all slots before selecting this one
        selectedShader.SetActive(true); // Show the selection highlight
        thisItemSelected = true; // Set flag to true

        itemDescriptionPanel.SetActive(true); // Show the description panel
        ItemDescriptionNameText.text = itemName; // Set item name
        ItemDescriptionText.text = itemDescription; // Set item description
        itemDescriptionImage.sprite = itemSprite; // Set item sprite for description

        PositionDescriptionPanel(); // Position the description panel correctly on screen

        unEquipButton.onClick.RemoveAllListeners();
        unEquipButton.onClick.AddListener(UnEquipItem);

        // Show only the Unequip button
        equipButton.gameObject.SetActive(false);
        unEquipButton.gameObject.SetActive(true); // Always show delete button for now
        useButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false); 
    }

    public void DeselectItem()
    {
        selectedShader.SetActive(false); // Hide the selection highlight
        thisItemSelected = false; // Set flag to false
        itemDescriptionPanel.SetActive(false); // Hide the description panel
        dropButton.gameObject.SetActive(true); // Always show delete button for now

        // Hide all buttons
        unEquipButton.gameObject.SetActive(false);
    }

    public void EquipItem(Sprite itemSprite, string itemName, string itemDescription)
    {
        if (isEquipped)
            UnEquipItem();

        // UPDATE IMAGE// 
        this.itemSprite = itemSprite;
        slotImage.sprite = this.itemSprite; // Update the slot image

        // UPDATE DATA//
        this.itemName = itemName;
        this.itemDescription = itemDescription; // Update item data

        // UPDATE PLAYER STATS //
        for (int i = 0; i < inventoryManager.equipmentSOs.Length; i++)
        {
            if (inventoryManager.equipmentSOs[i].itemName == this.itemName)
                inventoryManager.equipmentSOs[i].EquipItem();
        }

        isEquipped = true; // Mark as equipped
    }

    public void UnEquipItem()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is NULL in UnEquipItem()!");
            return;
        }

        ItemSO itemData = inventoryManager.GetItemSO(itemName);
        if (itemData == null)
        {
            Debug.LogError("No ItemSO found for item: " + itemName);
            return;
        }

        int quantityToAdd = 1;
        int leftover = quantityToAdd;

        // First try to add to an existing slot with the same item (if stackable)
        foreach (ItemSlot slot in inventoryManager.itemSlot)
        {
            if (slot.itemName == itemName && !slot.isFull)
            {
                leftover = slot.AddItem(itemName, quantityToAdd, itemSprite, itemDescription, itemData.slotType);
                if (leftover <= 0)
                    break;
            }
        }

        // If there's still leftover, try to add to a new empty slot
        if (leftover > 0)
        {
            foreach (ItemSlot slot in inventoryManager.itemSlot)
            {
                if (!slot.isFull && string.IsNullOrEmpty(slot.itemName))
                {
                    leftover = slot.AddItem(itemName, leftover, itemSprite, itemDescription, itemData.slotType);
                    if (leftover <= 0)
                        break;
                }
            }
        }

        // If we still have leftovers, inventory is full
        if (leftover > 0)
        {
            Debug.Log("Not enough space to unequip item.");
            return;
        }

        // Clear the equipped slot
        itemName = "";
        itemSprite = null;
        itemDescription = "";
        isEquipped = false;

        if (slotImage != null)
            slotImage.sprite = defaultSlotIcon;

        DeselectItem(); // Hide description panel, deselect visuals

        for (int i = 0; i < inventoryManager.equipmentSOs.Length; i++)
        {
            if (inventoryManager.equipmentSOs[i].itemName == this.itemName)
                inventoryManager.equipmentSOs[i].UnEquipItem();
        }
    }

    private void PositionDescriptionPanel()
    {
        RectTransform panelRect = itemDescriptionPanel.GetComponent<RectTransform>();
        RectTransform slotRect = GetComponent<RectTransform>();

        Vector3 slotScreenPos = slotRect.position;

        float offsetX = 150f; // Horizontal offset
        float offsetY = -550f; // Vertical offset

        Vector3 newPos = slotScreenPos + new Vector3(offsetX, offsetY, 0);

        // Check if the panel is off-screen and adjust its position
        Vector3[] panelCorners = new Vector3[4];
        panelRect.GetWorldCorners(panelCorners);
        float panelWidth = panelCorners[2].x - panelCorners[0].x;
        float panelHeight = panelCorners[2].y - panelCorners[0].y;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Adjust horizontal position if the panel goes off-screen
        if (newPos.x + panelWidth > screenWidth)
            newPos.x = screenWidth - panelWidth - 550;

        // Adjust vertical position if the panel goes off-screen
        if (newPos.y - panelHeight < 0)
            newPos.y = panelHeight + 550;

        // Ensure the panel is within the screen boundaries
        if (newPos.x < 0)
            newPos.x = 550;

        if (newPos.y > screenHeight)
            newPos.y = screenHeight - 550;

        panelRect.position = newPos; // Apply the new position to the panel
    }
}

public enum EquipmentSlotType
{
    None, // No specific slot
    Helmet, // Helmet slot
    Armor, // Armor slot
    Legs, // Legs slot
    Backpack, // Backpack slot
    Weapon, // Weapon slot
    Boots // Boots slot
}
