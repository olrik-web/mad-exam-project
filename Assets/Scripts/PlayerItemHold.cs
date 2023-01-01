using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerItemHold : MonoBehaviour
{
    private GameObject heldItem; // The item that the player is holding.
    private bool holdingItem = false; // Is the player holding an item?
    private float holdDistance = 2f; // The distance at which the player can hold an item.
    private PlayerController player; // The player controller component.
    private CanvasManager canvasManager; // The canvas manager component.
    private AudioManager audioManager; // The audio manager component.


    // Start is called before the first frame update
    void Start()
    {
        // Get the player controller component.
        player = GetComponent<PlayerController>();
        // Get the canvas manager component.
        canvasManager = CanvasManager.GetInstance();
        // Get the audio manager component.
        audioManager = GameObject.FindObjectOfType<AudioManager>();
    }

    // Interact function when pressing the interact button on mobile. This is the same function as the Update function above. 
    public void Interact()
    {
        // If the last active canvas is the game UI, allow the player to interact with items.
        if (canvasManager.lastActiveCanvas.canvasType == CanvasType.GameUI)
        {
            // Check if the player is holding an item.
            if (holdingItem)
            {
                // Get the item class of the item that the player is holding.
                ItemClass tempItemPlaceable = heldItem.GetComponent<ItemClass>();
                // Check if the item is a item that has to be placed on the ground.
                if (tempItemPlaceable.itemType == ItemType.Placeable || tempItemPlaceable.itemType == ItemType.Seating)
                {
                    // Place the item on the ground.
                    PlaceItem();
                    return;
                }
                else if (tempItemPlaceable.itemType == ItemType.Decoration)
                {
                    // Place the item on the ground.
                    DropItem(0.5f, 0f);
                    return;
                }
                // Check if we are standing in front of a chopping block, if so, chop the item. Else if we are standing in front of a stove, cook the item.
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, holdDistance))
                {
                    // Get the ItemPlaceable component of the object we are looking at.
                    ItemPlaceable itemPlaceable = hit.transform.GetComponent<ItemPlaceable>();
                    if (hit.collider.gameObject.tag == "ChoppingBlock")
                    {
                        ItemIngredient tempItem = heldItem.GetComponent<ItemIngredient>();
                        // Chop the item if it can be chopped and freeze the player's movement while chopping.
                        if (tempItem && tempItem.canChop && !itemPlaceable.isOccupied)
                        {
                            tempItem.Chop(hit.collider.gameObject);
                            DropItem();
                            player.FreezeMovement(tempItem.chopTime);
                        }
                    }
                    else if (hit.collider.gameObject.tag == "Stove")
                    {
                        ItemIngredient tempItem = heldItem.GetComponent<ItemIngredient>();
                        // Cook the item if it can be cooked. 
                        if (tempItem && tempItem.canCook && !itemPlaceable.isOccupied)
                        {
                            tempItem.Cook(hit.collider.gameObject);
                            DropItem();
                        }
                    }
                    else if (hit.collider.gameObject.tag == "CustomerChair")
                    {
                        ItemOrder tempItem = heldItem.GetComponent<ItemOrder>();
                        // Serve the item if it can be served.
                        if (tempItem && tempItem.canServe)
                        {
                            // Get the customer component gameobject which is a child of the chair.
                            Customer customer = hit.collider.gameObject.transform.GetChild(0).GetComponent<Customer>();
                            if (customer)
                            {
                                bool itemServed = customer.ServeItem(tempItem);
                                if (itemServed)
                                {
                                    audioManager.Play("Serve");
                                    Debug.Log("Served " + tempItem.itemName);
                                    DropItemAndDestory();
                                }
                            }

                        }
                    }
                    // If we are standing in front of a trash can, destroy the item.
                    else if (hit.collider.gameObject.tag == "Trash")
                    {
                        DropItemAndDestory();
                    }
                    else
                    {
                        Debug.Log("Can't chop or cook here");
                        DropItem();
                    }
                }
                else
                {
                    DropItem();
                }
            }
            // If the player is not holding an item, check if we are standing in front of an ingredient box or a chopping block.
            else
            {
                RaycastHit hit;
                Debug.DrawRay(transform.position, transform.forward * holdDistance, Color.red, 2f);
                if (Physics.Raycast(transform.position, transform.forward, out hit, holdDistance))
                {
                    // If we are standing in front of an ingredient box, spawn an item from the box.
                    if (hit.collider.gameObject.tag == "IngredientBox")
                    {
                        // Check if the box has an item child. If so, pick it up. Else, spawn a new item.
                        if (hit.collider.gameObject.transform.childCount > 0)
                        {
                            // Find the first child with the tag "Item".
                            Transform item = hit.collider.gameObject.transform.Find("Item");
                            if (item)
                            {
                                PickUpSpecificItem(item.gameObject);
                            }
                        }
                        else
                        {
                            ItemClass tempItem = hit.collider.gameObject.GetComponent<IngredientBox>().SpawnItem();
                            if (tempItem != null)
                            {
                                PickUpItemFromBox(tempItem);
                            }
                        }
                    }
                    else if (hit.collider.gameObject.tag == "ChoppingBlock")
                    {
                        // Check if the chopping block has an item child. If so, pick it up.
                        if (hit.collider.gameObject.transform.childCount > 0)
                        {
                            // Find the first child with the tag "Item".
                            Transform item = hit.collider.gameObject.transform.Find("Item");
                            if (item)
                            {
                                PickUpSpecificItem(item.gameObject);
                            }
                        }
                    }
                    else if (hit.collider.gameObject.tag == "Stove")
                    {
                        // Check if the stove has an item child. If so, pick it up.
                        if (hit.collider.gameObject.transform.childCount > 0)
                        {
                            // Find the first child with the tag "Item".
                            Transform item = hit.collider.gameObject.transform.Find("Item");
                            if (item)
                            {
                                PickUpSpecificItem(item.gameObject);
                            }
                        }
                    }
                }
                PickUpItem();
            }
        }

    }

    void PickUpItemFromBox(ItemClass item)
    {
        heldItem = item.gameObject;
        holdingItem = true;
    }

    // Pick up an item from the shop and set it as the held item.
    public void CarryItemFromShop(ItemClass item)
    {
        // Instantiate the item and set it as the held item.
        heldItem = Instantiate(item.gameObject, transform.position, Quaternion.identity);
        holdingItem = true;

        // Disable the item's collider. 
        heldItem.GetComponent<Collider>().enabled = false;

        // Make the item a child of the player.
        heldItem.transform.parent = transform;

        // Set the item's position to just in front of the player.
        heldItem.transform.localPosition = new Vector3(0, 0.5f, 1f);

        // Switch canvas back to game UI. 
        canvasManager.SwitchCanvas(CanvasType.GameUI);
    }

    // Place the held item on the ground in front of the player.
    void PlaceItem()
    {
        // Get the nearest grid position.
        Vector3 gridPosition = GetNearestGridPosition(true);

        // Get the ItemClass component of the held item.
        ItemClass itemClass = heldItem.GetComponent<ItemClass>();

        if (!IsGridPositionOccupied(gridPosition))
        {

            // Place the item on the ground in front of the player. Round the position to the nearest 0.5.
            // heldItem.transform.position = new Vector3(Mathf.Round(transform.position.x + transform.forward.x), 0.5f, Mathf.Round(transform.position.z + transform.forward.z));
            heldItem.transform.position = gridPosition;

            // Set the item's rotation to nearest 90 degrees.
            heldItem.transform.rotation = Quaternion.Euler(0, Mathf.Round(transform.rotation.eulerAngles.y / 90) * 90, 0);

            // Enable the item's collider. 
            heldItem.GetComponent<Collider>().enabled = true;

            if (itemClass.itemType == ItemType.Seating)
            {
                GameManager.instance.availableChairs.Add(heldItem);
            }

            // Make the item no longer a child of the player.
            heldItem.transform.parent = null;

            // Set the held item to null.
            heldItem = null;

            // Set the holding item bool to false.
            holdingItem = false;
        }
    }

    // Pick up the closest item to the player.
    void PickUpItem()
    {
        // Get the closest item to the player
        GameObject closestItem = null;
        float closestDistance = 0f;
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            float distance = Vector3.Distance(transform.position, item.transform.position);
            if (closestItem == null || distance < closestDistance)
            {
                closestItem = item;
                closestDistance = distance;
            }
        }


        // Check if the closest item is close enough to pick it up
        if (closestItem != null && closestDistance <= holdDistance)
        {
            // Check if closest item is an ingredient or order.
            if (closestItem.TryGetComponent<ItemIngredient>(out ItemIngredient ingredient))
            {
                if (ingredient.isCooking || ingredient.isChopping)
                {
                    Debug.Log("Can't pick up item while it is cooking or chopping");
                    return;
                }
            }
            // Check if closest item is on a table. If so get the table's ItemPlaceable component and set the occupied bool to false.
            if (closestItem.transform.parent != null && closestItem.transform.GetComponentInParent<ItemPlaceable>())
            {
                closestItem.transform.parent.gameObject.GetComponent<ItemPlaceable>().isOccupied = false;
            }
            // Set the held item to the closest item
            heldItem = closestItem;
            holdingItem = true;

            // Remove the item's rigidbody and collider components
            heldItem.GetComponent<Rigidbody>().isKinematic = true;
            heldItem.GetComponent<Collider>().enabled = false;

            // Set the item's parent to the player
            heldItem.transform.SetParent(transform);

            // Set the item's position to the player's hand
            heldItem.transform.localPosition = new Vector3(0.5f, 0.5f, 0.5f);

        }
    }

    // Pick up a specific item. Used for picking up items from tables. 
    void PickUpSpecificItem(GameObject item)
    {
        if (item.TryGetComponent<ItemIngredient>(out ItemIngredient ingredient))
        {
            if (ingredient.isCooking || ingredient.isChopping)
            {
                Debug.Log("Can't pick up item while it is cooking or chopping");
                return;
            }
        }
        if (item.transform.parent != null && item.transform.GetComponentInParent<ItemPlaceable>())
        {
            item.transform.parent.gameObject.GetComponent<ItemPlaceable>().isOccupied = false;
        }
        // Set the held item to the closest item
        heldItem = item;
        holdingItem = true;

        // Remove the item's rigidbody and collider components
        heldItem.GetComponent<Rigidbody>().isKinematic = true;
        heldItem.GetComponent<Collider>().enabled = false;

        // Set the item's parent to the player
        heldItem.transform.SetParent(transform);

        // Set the item's position to the player's hand
        heldItem.transform.localPosition = new Vector3(0.5f, 0.5f, 0.5f);


    }

    void DropItem(float yPosTable = 1f, float yPosGround = 0.5f)
    {
        // If a table is in front of the player, drop the item on the table. Otherwise, drop the item on the ground.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, holdDistance))
        {
            ItemPlaceable tempTable = hit.collider.gameObject.GetComponent<ItemPlaceable>();
            if (tempTable && tempTable.isOccupied)
            {
                Debug.Log("Table is occupied");
                return;
            }
            else if (tempTable)
            {
                // Set the isOccupied flag.
                tempTable.isOccupied = true;
            }
            // Add the item's rigidbody and collider components
            if (heldItem.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = false;
            }
            heldItem.GetComponent<Collider>().enabled = true;

            // Remove the item's parent
            heldItem.transform.SetParent(null);

            // Set the item as a child of the table.
            heldItem.transform.parent = hit.collider.gameObject.transform;

            // Set the item's position to the above the table
            heldItem.transform.position = hit.collider.gameObject.transform.position + new Vector3(0f, yPosTable, 0f);
        }
        else
        {
            // Add the item's rigidbody and collider components
            if (heldItem.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = false;
            }
            heldItem.GetComponent<Collider>().enabled = true;

            // Remove the item's parent
            heldItem.transform.SetParent(null);
            // Set the item's position to the ground in front of the player
            heldItem.transform.position = transform.position + transform.forward * holdDistance + new Vector3(0f, yPosGround, 0f);
        }

        // Reset the held item
        heldItem = null;
        holdingItem = false;
    }

    void DropItemAndDestory()
    {
        // Remove the item's parent
        heldItem.transform.SetParent(null);
        Destroy(heldItem);

        // Reset the held item
        heldItem = null;
        holdingItem = false;
    }

    // Get the nearest grid position in front of the player using the player's forward vector.
    private Vector3 GetNearestGridPosition(bool inFrontOfPlayer)
    {
        if (inFrontOfPlayer)
        {
            // Get the nearest grid position in front of the player using the player's forward vector.
            Vector3 playerForward = transform.forward;
            Vector3 playerPosition = transform.position;
            // Get the nearest grid position in front of the player using the player's forward vector.
            Vector3 nearestGridPosition = new Vector3(Mathf.Round(playerPosition.x + playerForward.x), 0.5f, Mathf.Round(playerPosition.z + playerForward.z));
            return nearestGridPosition;
        }
        else
        {
            return new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        }
    }

    // Check if the grid position is occupied. 
    private bool IsGridPositionOccupied(Vector3 gridPosition)
    {
        // Check if the grid position is occupied.
        Collider[] colliders = Physics.OverlapBox(gridPosition, new Vector3(0.4f, 0.4f, 0.4f));
        // Ignore the player's collider, heldItem collider, and the collider of the plane.
        // Get the plane's collider
        Collider planeCollider = GameObject.FindWithTag("Plane").GetComponent<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.gameObject != heldItem && collider != planeCollider)
            {
                Debug.Log(collider.gameObject.name);

                return true;
            }
        }
        return false;
    }
}
