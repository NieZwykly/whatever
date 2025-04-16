using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI; 

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject AvatarMenu;
    public Button inventoryButton;
    public ItemSlot [] itemSlot;
    public EquippedSlot [] equippedSlot;

    public ItemSO[] itemSOs;
    public EquipmentSO[] equipmentSOs;

    private bool menuActivated = false;
    public bool avatarActivated = false;


    public EquippedSlot helmetSlot;
    public EquippedSlot armorSlot;
    public EquippedSlot legsSlot;
    public EquippedSlot backpackSlot;
    public EquippedSlot weaponSlot;
    public EquippedSlot bootsSlot;

    private Dictionary<EquipmentSlotType, EquippedSlot> equipmentSlots = new Dictionary<EquipmentSlotType, EquippedSlot>();

    void Start()
    {
        RegisterEquipmentSlot(EquipmentSlotType.Helmet, helmetSlot);
        RegisterEquipmentSlot(EquipmentSlotType.Armor, armorSlot);
        RegisterEquipmentSlot(EquipmentSlotType.Legs, legsSlot);
        RegisterEquipmentSlot(EquipmentSlotType.Backpack, backpackSlot);
        RegisterEquipmentSlot(EquipmentSlotType.Weapon, weaponSlot);
        RegisterEquipmentSlot(EquipmentSlotType.Boots, bootsSlot);

        if (InventoryMenu == null)
        {
            Debug.LogError("InventoryMenu is not assigned in the Inspector!");
            return;
        }

        InventoryMenu.SetActive(true);  // Temporarily enable to be found
        Invoke("HideInventory", 0.1f); // Hide after a small delay
        }

    public ItemSO GetItemSO(string itemName)
    {
        foreach (var item in itemSOs)
        {
        if (item.itemName == itemName)
            return item;
        }
        return null;
    }
    public EquipmentSO GetEquipmentSO(string itemName)
    {
        foreach (var item in equipmentSOs)
        {
            if (item.itemName == itemName)
                return item;
        }
        return null;
    }

    public void RegisterEquipmentSlot(EquipmentSlotType slotType, EquippedSlot slot)
    {
        if (!equipmentSlots.ContainsKey(slotType))
        {
            equipmentSlots[slotType] = slot;
        }
    }

    public EquippedSlot GetEquipmentSlot(EquipmentSlotType slotType)
    {
        return equipmentSlots.ContainsKey(slotType) ? equipmentSlots[slotType] : null;
    }

    void HideInventory()
    {
        InventoryMenu.SetActive(false);
    }

    public void ToggleInventory()
    {
        // If avatar is open, close it first
        if (AvatarMenu.activeSelf)
        {
            AvatarMenu.SetActive(false);
            avatarActivated = false;
        }

        menuActivated = !menuActivated;
        InventoryMenu.SetActive(menuActivated);

        if (!menuActivated)
        {
            DeselectAllSlots();
        }
    }

    public void ToggleAvatar()
    {
        // If inventory is open, close it first
        if (InventoryMenu.activeSelf)
        {
            InventoryMenu.SetActive(false);
            menuActivated = false;
        }

        avatarActivated = !avatarActivated;
        AvatarMenu.SetActive(avatarActivated);
    }

    public void UseItem(string itemName)
    {
        for (int i = 0; i < itemSOs.Length; i++)
        {
            if (itemSOs[i].itemName == itemName)
            {
                itemSOs[i].UseItem();
            }
        }
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if ((!itemSlot[i].isFull && itemSlot[i].itemName == itemName) || itemSlot[i].quantity == 0)
            {
                EquipmentSlotType type = itemSOs.FirstOrDefault(x => x.itemName == itemName)?.slotType ?? EquipmentSlotType.None;
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription, type);

                if (leftOverItems > 0)
                    leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription);

                return leftOverItems;
            }
        }
        return quantity;
    }

    public void EquipItemToSlot(string itemName, EquipmentSlotType slotType)
    {
        // Find the corresponding item in the inventory (ItemSlot)
        foreach (var slot in itemSlot)
        {
            if (slot.itemName == itemName)
            {
                // Equip the item in the corresponding equipment slot
                EquippedSlot targetSlot = GetEquipmentSlot(slotType);
                if (targetSlot != null)
                {
                    targetSlot.EquipItem(slot.itemSprite, slot.itemName, slot.itemDescription);

                    // Remove the item from the inventory
                    slot.RemoveItem();
                }
                else
                {
                    Debug.LogError($"No equipped slot found for type {slotType}!");
                }
                break;
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
            itemSlot[i].itemDescriptionPanel.SetActive(false); // Hide description
        }
        for (int i = 0; i < equippedSlot.Length; i++)
        {
            equippedSlot[i].selectedShader.SetActive(false);
            equippedSlot[i].thisItemSelected = false;
            equippedSlot[i].itemDescriptionPanel.SetActive(false); // Hide description
        }
    }
    public void DropItem(string itemName, Sprite itemSprite, string itemDescription, EquipmentSlotType slotType)
    {
        GameObject itemToDrop = new GameObject(itemName);
        Item newItem = itemToDrop.AddComponent<Item>();
        newItem.quantity = 1;
        newItem.itemName = itemName;
        newItem.sprite = itemSprite;
        newItem.itemDescription = itemDescription;
        newItem.slotType = slotType;

        SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
        sr.sprite = itemSprite;
        sr.sortingOrder = 0;
        sr.sortingLayerName = "ForeGround";

        itemToDrop.AddComponent<BoxCollider2D>();

        itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(4f, 1f, 0f);
    }
}