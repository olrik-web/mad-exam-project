using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderLine : MonoBehaviour
{
    public List<ItemOrder> itemsOrdered = new List<ItemOrder>();
    public List<OrderLineItem> itemsOrderedUIItems = new List<OrderLineItem>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddItemToOrder(ItemOrder item)
    {
        itemsOrdered.Add(item);
        // Update the UI.
        UpdateOrderLineUI();
    }

    public void RemoveItemFromOrder(ItemOrder item)
    {
        itemsOrdered.Remove(item);
        UpdateOrderLineUI();

    }

    public void UpdateOrderLineUI()
    {
        // Add the items to the order line UI.
        for (int i = 0; i < itemsOrderedUIItems.Count; i++)
        {
            if (i < itemsOrdered.Count)
            {
                if (itemsOrdered[i] != null)
                {
                    itemsOrderedUIItems[i].item = itemsOrdered[i];
                    itemsOrderedUIItems[i].itemText.text = itemsOrdered[i].itemName;
                    // Set the image soruce to the item's image.
                    itemsOrderedUIItems[i].itemImage.GetComponent<Image>().sprite = itemsOrdered[i].itemImage;

                    // Set the item's iamge aphla to 1.
                    itemsOrderedUIItems[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    // Set the item's iamge aphla to 1.
                    itemsOrderedUIItems[i].itemImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
            }
            else
            {
                itemsOrderedUIItems[i].item = null;
                itemsOrderedUIItems[i].itemText.text = "";
                itemsOrderedUIItems[i].itemImage.GetComponent<Image>().sprite = null;

                // Set the item's iamge aphla to 0.
                itemsOrderedUIItems[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);

                // Set the item's itemImage aphla to 0.
                itemsOrderedUIItems[i].itemImage.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }
    }
}
