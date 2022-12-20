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
    public string itemName;
    // An image of the item.
    public Sprite itemImage;
    public ItemType itemType;

}
