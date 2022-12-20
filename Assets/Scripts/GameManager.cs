using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerController player;
    public List<ItemOrder> itemsToOrder;
    public List<GameObject> availableChairs;
    public TextMeshProUGUI walletText;
    public TextMeshProUGUI dayTimeText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI endScreenText;
    private int wallet;
    private int cashEarned = 0;
    private bool isDayOver = false;

    public List<Customer> customers = new List<Customer>();

    // A day in the game is 10 minutes long. This is the time in seconds. 
    public float dayLength = 600f; // 600 seconds = 10 minutes
    public float timeLeft;
    // Counter for the number of days that have passed. 
    public int dayCounter;

    // Canvas manager reference.
    private CanvasManager canvasManager;

    public bool hasGameBeenPaused = false;

    private AdManager adManager;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the canvas manager reference.
        canvasManager = CanvasManager.GetInstance();

        // Find the ad manager using the tag.
        adManager = GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>();

        PlayerPrefs.SetInt("DayCounter", 0);
        PlayerPrefs.SetInt("Wallet", 100);

        if (dayCounter == 0)
        {
            // TODO: Add a tutorial to the game.
            StartDay();

        }

        // If the game has not been paused yet or it is day 0 (preparation day), then start the game.
        if (!hasGameBeenPaused && dayCounter != 0)
        {
            StartGame();
        }

        itemsToOrder = new List<ItemOrder>();
        availableChairs = new List<GameObject>();

        // Find all the objects in the scene with the tag "CustomerChair" and add them to the tables list.
        GameObject[] tempChairs = GameObject.FindGameObjectsWithTag("CustomerChair");
        foreach (GameObject chair in tempChairs)
        {
            availableChairs.Add(chair);
        }

        // Populate the itemsToOrder list with all the prefabs in the Resources folder with the tag "Item".
        Object[] tempItems = Resources.LoadAll("Orders", typeof(GameObject));
        foreach (GameObject item in tempItems)
        {
            itemsToOrder.Add(item.GetComponent<ItemOrder>());
        }
    }

    public void StartSpawningCustomers()
    {
        Debug.Log("Spawning customers");
        StartCoroutine(SpawnCustomer());
    }

    public void StopSpawningCustomers()
    {
        Debug.Log("Stopping customer spawning coroutine");
        StopCoroutine(SpawnCustomer());
    }

    public IEnumerator SpawnCustomer()
    {
        while (true)
        {
            // Wait for a random amount of time between 20 and 60 seconds.
            yield return new WaitForSeconds(Random.Range(20, 60));

            // Spawn a customer at a random table if there is a table available.
            if (availableChairs.Count > 0 && !isDayOver)
            {
                int randomChairIndex = Random.Range(0, availableChairs.Count);
                GameObject randomChair = availableChairs[randomChairIndex];
                availableChairs.Remove(randomChair);
                // Get chair ItemPlaceable component.
                ItemPlaceable chairItemPlaceable = randomChair.GetComponent<ItemPlaceable>();
                GameObject customer = Instantiate(Resources.Load("Prefabs/Customer") as GameObject, randomChair.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                Customer tempCustomer = customer.GetComponent<Customer>();
                customers.Add(tempCustomer);
                Debug.Log("Customers: " + customers.Count);
                tempCustomer.chair = randomChair;
                // Set the chair's itemPlaceable to occupied.
                chairItemPlaceable.isOccupied = true;
                // Set the customers parent to the chair.
                customer.transform.SetParent(randomChair.transform);
                tempCustomer.StartScript();
            }
        }
    }
    public void AddToWallet(int amount)
    {
        cashEarned += amount;
        wallet += amount;
        walletText.text = "Coins: " + wallet;
    }

    public void RemoveFromWallet(int amount)
    {
        wallet -= amount;
        walletText.text = "Coins: " + wallet;
    }

    public int GetWallet()
    {
        return wallet;
    }

    // This function is called when the scene is loaded. 
    // It starts the day timer. When the timer runs out, the game ends and the player is taken to the end screen.
    // The end screen will display the player's score. 
    public void StartDayTimer()
    {
        StartDay();

        StartCoroutine(DayTimer());
    }

    private void StartDay()
    {
        isDayOver = false;

        // Set the wallet text to the player's wallet amount.
        wallet = PlayerPrefs.GetInt("Wallet", 100);
        walletText.text = wallet.ToString();

        // Set the day counter to player prefs if it exists, otherwise set it to 0.
        dayCounter = PlayerPrefs.GetInt("DayCounter", 0);

        // Set the day text to the current day.
        dayText.text = "Day " + dayCounter;

        // Play a random background music track.
        AudioManager.GetInstance().PlayRandomBackgroundMusic();
    }

    public void StopDayTimer()
    {
        Debug.Log("Day timer stopped");
        StopCoroutine(DayTimer());
    }

    IEnumerator DayTimer()
    {
        // For each second that passes, subtract 1 from the timeLeft variable.
        for (timeLeft = dayLength; timeLeft > 0; timeLeft--)
        {
            // Update the dayTimeText to display the time left in the day.
            dayTimeText.text = "Time left: " + timeLeft;
            yield return new WaitForSeconds(1);
        }

        // Process the end of the day.
        EndDay();
    }

    public void EndDay()
    {
        // When the day is over, set the isDayOver variable to true.
        isDayOver = true;

        // Stop the customer spawning coroutine.
        StopSpawningCustomers();

        StopDayTimer();

        // Stop playing the music.
        AudioManager.GetInstance().StopBackgroundMusicFadeOut();

        // Loop through the customers list and remove each customer from the scene.
        while (customers.Count > 0)
        {
            customers[0].Leave();
        }

        // Disable player movement and the player's ability to interact with objects.
        player.FreezeMovement();

        // Add the cash earned in the day to the player's wallet.
        PlayerPrefs.SetInt("Wallet", wallet);

        // Reset the cash earned for the day.
        cashEarned = 0;

        // Increment the day counter.
        dayCounter++;
        PlayerPrefs.SetInt("DayCounter", dayCounter);

        // Set the end screen text to display the completed day.
        endScreenText.text = "Day " + (dayCounter - 1) + " completed!";

        // Switch to the EndScreen canvas.
        canvasManager.SwitchCanvas(CanvasType.EndScreen);

        // Show an interstitial ad.
        adManager.ShowInterstitialAd();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void StartGame()
    {
        // Set the time scale to 1 so that the game runs at normal speed.
        Time.timeScale = 1;

        // Start the day timer.
        StartDayTimer();

        // Start spawning customers.
        StartSpawningCustomers();
    }
}
