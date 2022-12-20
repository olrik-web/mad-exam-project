using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemIngredient : ItemClass
{
    public bool canChop = false;
    public bool canCook = false;
    public bool isChopped = false;
    public bool isCooked = false;
    public int chopTime = 3;
    public int cookTime = 5;

    public bool isChopping = false;
    public bool isCooking = false;

    public void Chop(GameObject choppingBoard)
    {
        if (canChop && !isChopping)
        {
            StartCoroutine(ChopCoroutine(choppingBoard));
        }
    }

    public void Cook(GameObject stove)
    {
        if (canCook && !isCooking)
        {
            StartCoroutine(CookCoroutine(stove));
        }
    }

    IEnumerator ChopCoroutine(GameObject choppingBoard)
    {
        isChopping = true;
        // Play chopping sound
        AudioManager.GetInstance().Play("Knife chopping");

        yield return new WaitForSeconds(chopTime);
        isChopped = true;
        isChopping = false;
        // Destory the item after it has been chopped and instantiate a new chopped item in the Ingredients folder.
        Destroy(gameObject);
        Debug.Log("Chopped " + itemName);
        // Instantiate the chopped item.
        GameObject choppedItem = Instantiate(Resources.Load("Ingredients/Chopped " + itemName), transform.position, transform.rotation) as GameObject;
        // Set the chopped item's parent to the chopping board.
        choppedItem.transform.SetParent(choppingBoard.transform);
    }

    IEnumerator CookCoroutine(GameObject stove)
    {
        isCooking = true;
        // Play chopping sound
        AudioManager.GetInstance().Play("Cooking soup");

        yield return new WaitForSeconds(cookTime);
        isCooked = true;
        isCooking = false;
        // Destory the item after it has been cooked and instantiate a new cooked item.
        Destroy(gameObject);
        GameObject cookedItem = Instantiate(Resources.Load("Orders/Cooked " + itemName), transform.position, transform.rotation) as GameObject;
        // Set the cooked item's parent to the stove.
        cookedItem.transform.SetParent(stove.transform);
    }

}
