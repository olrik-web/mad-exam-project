using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBox : MonoBehaviour
{
    public ItemClass item; // The item that the ingredient box will spawn.

    // Spawn an item from the ingredient box.
    public ItemClass SpawnItem()
    {
        // Instantiate a new item from the item variable.
        GameObject newItem = Instantiate(item.gameObject, transform.position, transform.rotation);
        // Set the parent of the new item to the ingredient box.
        newItem.transform.SetParent(transform);
        // Return the new item.
        return newItem.GetComponent<ItemClass>();
    }
    
}
