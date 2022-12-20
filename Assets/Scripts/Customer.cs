using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Customer : MonoBehaviour
{
    public ItemOrder itemToServe;
    public bool isServed = false;
    public int tipAmount;
    public GameObject chair;
    public GameObject itemToServeText;
    public OrderLine orderLine;
    public void StartScript()
    {
        // Get the order line reference using the tag.
        orderLine = GameObject.FindGameObjectWithTag("OrderLineUI").GetComponent<OrderLine>();
        // Add items to the itemsToOrder list.   
        StartCoroutine(OrderItems());
    }
    public bool ServeItem(ItemClass item)
    {
        bool served = false;
        if (itemToServe != null)
        {
            if (itemToServe != null && item.itemName == itemToServe.itemName)
            {
                isServed = true;
                // Set tip amount to a random amount between 1 and 5.
                tipAmount = Random.Range(1, 5);
                // Add money to the player's wallet.
                GameManager.instance.AddToWallet(tipAmount + itemToServe.itemPrice);
                // Make the customer leave the restaurant.
                Leave();

                served = true;
            }
            else
            {
                Debug.Log("Customer was served the wrong item");
                Debug.Log("Customer wanted " + itemToServe.itemName + " but was served " + item.itemName);
            }
        }
        else
        {
            Debug.Log("Customer was served an item but they didn't order anything");
        }
        return served;
    }

    IEnumerator OrderItems()
    {
        // Wait for a random amount of time between 5 and 10 seconds.
        yield return new WaitForSeconds(Random.Range(5, 10));
        // Get a random item from the itemsToOrder list.
        int randomItemIndex = Random.Range(0, GameManager.instance.itemsToOrder.Count);
        itemToServe = GameManager.instance.itemsToOrder[randomItemIndex];
        // Add the item to the order line.
        orderLine.AddItemToOrder(itemToServe);
    }
    public void Leave()
    {
        // Remove the customer from the available chairs list.
        GameManager.instance.availableChairs.Add(chair);
        // Get chair ItemPlaceable component.
        ItemPlaceable chairItemPlaceable = chair.GetComponent<ItemPlaceable>();
        // Set the chair's itemPlaceable to not occupied.
        chairItemPlaceable.isOccupied = false;

        // Remove the item from the order line.
        orderLine.RemoveItemFromOrder(itemToServe);

        // Remove the customer from the customers list.
        GameManager.instance.customers.Remove(this);

        // Destroy the customer.
        Destroy(gameObject);
    }
}
