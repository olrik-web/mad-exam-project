using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItems : MonoBehaviour
{
    // All the items that can be bought in the shop.
    public List<ItemPlaceable> shopItems = new List<ItemPlaceable>();
    // Available slots in the shop. 
    public List<GameObject> shopSlots = new List<GameObject>();

    // The currently selected item.
    public GameObject selectedItem;

    public GameObject itemSelector;

    // Start is called before the first frame update
    void Start()
    {
        // Find all child objects with the tag "ShopSlot" and add them to the shopSlots list.
        foreach (Transform child in transform)
        {
            if (child.tag == "ShopSlot")
            {
                shopSlots.Add(child.gameObject);
            }
        }

        // Populate the shopItems list with all the prefabs in the Resources folder with the tag "ShopItem".
        Object[] tempItems = Resources.LoadAll("ShopItems", typeof(GameObject));
        foreach (GameObject item in tempItems)
        {
            shopItems.Add(item.GetComponent<ItemPlaceable>());
        }

        // Log the images of the items in the shop.
        foreach (ItemPlaceable item in shopItems)
        {
            Debug.Log(item.itemImage);
        }

        // Add the items to the shop slots.
        for (int i = 0; i < shopItems.Count; i++)
        {
            // Get the shop slot script.
            ShopSlot shopSlotScript = shopSlots[i].GetComponent<ShopSlot>();
            shopSlotScript.item = shopItems[i];
            // Set the item's image in the shop slot.
            shopSlotScript.itemImage.sprite = shopItems[i].itemImage;
            // Initialise the item's cost text.
            shopSlotScript.itemCostText = shopSlotScript.GetComponentInChildren<TextMeshProUGUI>();
            // Set the item's cost in the shop slot.
            shopSlots[i].GetComponent<ShopSlot>().itemCostText.text = shopItems[i].itemCost.ToString();
        }

        // Set the selected item to the first item in the shop.
        selectedItem = shopSlots[0];
    }

    // Set the selected item to the shop slot that the player touched.
    public void SetSelectedItem(GameObject shopSlot)
    {
        selectedItem = shopSlot;
        // Set the item selector's position to the shop slot that the player touched.
        itemSelector.transform.position = shopSlot.transform.position;
    }

    // BuyItem() is called when the player clicks the buy button.
    public void BuyItem()
    {
        // Get the shop slot script.
        ShopSlot shopSlotScript = selectedItem.GetComponent<ShopSlot>();

        if (shopSlotScript != null && shopSlotScript.item != null)
        {
            // Buy the item in the shop slot.
            shopSlotScript.BuyItem();
        }
    }
}
