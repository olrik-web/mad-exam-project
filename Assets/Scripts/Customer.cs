using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Customer : MonoBehaviour
{
    public ItemOrder itemToServe; // The item the customer wants to be served.
    public bool isServed = false; // Has the customer been served?
    public int tipAmount; // The amount of money the customer will tip.
    public GameObject chair; // The chair the customer is sitting on.
    public GameObject itemToServeText; // The text that displays the item the customer wants to be served.
    public OrderLine orderLine; // The order line.

    // StartScript is called when the customer is spawned. It starts the OrderItems coroutine. 
    public void StartScript()
    {
        // Get the order line reference using the tag.
        orderLine = GameObject.FindGameObjectWithTag("OrderLineUI").GetComponent<OrderLine>();
        // Add items to the itemsToOrder list.   
        StartCoroutine(OrderItems());
    }
    // ServeItem is called when the player serves an item to the customer. 
    public bool ServeItem(ItemClass item)
    {
        bool served = false;
        // Check if the customer has ordered an item.
        if (itemToServe != null)
        {
            // Check if the item the player served is the same as the item the customer ordered.
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
                Debug.Log("Customer wanted " + itemToServe.itemName + " but was served " + item.itemName);
            }
        }
        else
        {
            Debug.Log("Customer was served an item but they didn't order anything");
        }
        return served;
    }

    // OrderItems is called when the customer is spawned. It waits for a random amount of time between 5 and 10 seconds and then orders an item.
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
    // Leave is called when the customer is served or when the customer leaves the restaurant.
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
