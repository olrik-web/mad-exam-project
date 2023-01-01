using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    // The item in the shop slot.
    public ItemPlaceable item;

    // The image of the item in the shop slot.
    public Image itemImage;

    // The text of the item cost.
    public TextMeshProUGUI itemCostText;

    // Buy the item in the shop slot.
    public void BuyItem()
    {
        // Check if the player has enough money to buy the item.
        if (GameManager.instance.GetWallet() >= item.itemCost)
        {
            // Remove the item's cost from the player's wallet.
            GameManager.instance.RemoveFromWallet(item.itemCost);

            // Add the item to the player's hold item.
            GameManager.instance.player.GetComponent<PlayerItemHold>().CarryItemFromShop(item);
        }
    }
}
