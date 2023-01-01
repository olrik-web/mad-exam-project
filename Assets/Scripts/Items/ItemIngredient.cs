using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemIngredient : ItemClass
{
    public bool canChop = false; // Can the item be chopped?
    public bool canCook = false; // Can the item be cooked?
    public bool isChopped = false; // Has the item been chopped?
    public bool isCooked = false; // Has the item been cooked?
    public int chopTime = 3; // The time it takes to chop the item.
    public int cookTime = 5; // The time it takes to cook the item.
    public bool isChopping = false; // Is the item currently being chopped?
    public bool isCooking = false; // Is the item currently being cooked?

    // Chop the item at the chopping board.
    public void Chop(GameObject choppingBoard)
    {
        // If the item can be chopped and it is not currently being chopped, then start the chopping coroutine.
        if (canChop && !isChopping)
        {
            StartCoroutine(ChopCoroutine(choppingBoard));
        }
    }

    // Cook the item at the stove.
    public void Cook(GameObject stove)
    {
        // If the item can be cooked and it is not currently being cooked, then start the cooking coroutine.
        if (canCook && !isCooking)
        {
            StartCoroutine(CookCoroutine(stove));
        }
    }

    // Chop Coroutine - Chops the item at the chopping board.
    IEnumerator ChopCoroutine(GameObject choppingBoard)
    {
        // Set the isChopping variable to true.
        isChopping = true;
        // Play chopping sound
        AudioManager.GetInstance().Play("Knife chopping");

        // Wait for the chop time to pass.
        yield return new WaitForSeconds(chopTime);
        // Set the isChopped variable to true and isChopping to false.
        isChopped = true;
        isChopping = false;
        // Destory the item after it has been chopped and instantiate a new chopped item in the Ingredients folder.
        Destroy(gameObject);
        // Instantiate the chopped item.
        GameObject choppedItem = Instantiate(Resources.Load("Ingredients/Chopped " + itemName), transform.position, transform.rotation) as GameObject;
        // Set the chopped item's parent to the chopping board.
        choppedItem.transform.SetParent(choppingBoard.transform);
    }

    // Cook Coroutine - Cooks the item at the stove.
    IEnumerator CookCoroutine(GameObject stove)
    {
        // Set the isCooking variable to true.
        isCooking = true;
        // Play chopping sound
        AudioManager.GetInstance().Play("Cooking soup");

        // Wait for the cook time to pass.
        yield return new WaitForSeconds(cookTime);
        // Set the isCooked variable to true and isCooking to false.
        isCooked = true;
        isCooking = false;
        // Destory the item after it has been cooked and instantiate a new cooked item.
        Destroy(gameObject);
        GameObject cookedItem = Instantiate(Resources.Load("Orders/Cooked " + itemName), transform.position, transform.rotation) as GameObject;
        // Set the cooked item's parent to the stove.
        cookedItem.transform.SetParent(stove.transform);
    }

}
