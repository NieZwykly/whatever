using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public int quantity;
    public Sprite sprite;
    [TextArea] // add up box to write description in unity
    public string itemDescription;
    public EquipmentSlotType slotType;

    public InventoryManager inventoryManager;


    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found! Ensure InventoryMenu is active at runtime.");
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
           int leftOverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);
            if(leftOverItems <= 0)
            Destroy(gameObject);
            else
                quantity = leftOverItems;
        }
    }
}
