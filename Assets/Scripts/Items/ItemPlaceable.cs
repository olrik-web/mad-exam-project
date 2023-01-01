using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemPlaceable : ItemClass
{
    public int itemCost = 10; // The cost of the item.
    public bool isOccupied = false; // Is the item occupied? (Used for seating items)
}
