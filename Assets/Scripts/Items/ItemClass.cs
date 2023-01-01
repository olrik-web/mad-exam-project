using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Placeable,
    Ingredient,
    Seating,
    Decoration,
}

public class ItemClass : MonoBehaviour
{
    public string itemName; // The name of the item.
    public Sprite itemImage; // An image of the item.
    public ItemType itemType; // The type of the item.

}
