using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// TODO: Split this class into multiple classes. Each class should be responsible for a single task. E.g. a class for spawning customers.
// TODO: Add a tutorial to the game.
// TODO: Add a settings menu to the game.
// TODO: Add a level up system to the game.
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerController player;
    public List<ItemOrder> itemsToOrder; // The items that can be ordered by the customers.
    public List<GameObject> availableChairs; // The chairs that are available for the customers to sit on.
    public TextMeshProUGUI walletText; // The text that displays the player's wallet.
    public TextMeshProUGUI dayTimeText; // The text that displays the time left in the day.
    public TextMeshProUGUI dayText; // The text that displays the day number.
    public TextMeshProUGUI endScreenText; // The text that displays the end screen text.
    private int wallet; // The player's wallet.
    private int cashEarned = 0; // The amount of money earned in the current day.
    private bool isDayOver = false; // Is the day over?
    public List<Customer> customers = new List<Customer>(); // The list of customers in the restaurant.
    public float dayLength = 360f; // 360 seconds = 6 minutes
    public float timeLeft; // The time left in the day.
    public int dayCounter; // Counter for the number of days that have passed. 
    private CanvasManager canvasManager; // Canvas manager reference.
    public bool hasGameBeenPaused = false; // Has the game been paused?
    private AdManager adManager; // Ad manager reference.

    // Get the instance of the game manager.
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

        // Initialize the itemsToOrder list and the availableChairs list.
        itemsToOrder = new List<ItemOrder>();
        availableChairs = new List<GameObject>();

        // Find all the objects in the scene with the tag "CustomerChair" and add them to the available chairs list.
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

    // StartSpawningCustomers() is called when the player starts the game.
    public void StartSpawningCustomers()
    {
        Debug.Log("Spawning customers");
        StartCoroutine(SpawnCustomer());
    }

    // SpawnCustomer() is a coroutine that spawns a customer at a random table every 20-60 seconds.
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
                // Spawn a customer at the chair's position.
                GameObject customer = Instantiate(Resources.Load("Prefabs/Customer") as GameObject, randomChair.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                // Get the customer component.
                Customer tempCustomer = customer.GetComponent<Customer>();
                // Add the customer to the customers list.
                customers.Add(tempCustomer);
                // Set the customer's chair to the random chair.
                tempCustomer.chair = randomChair;
                // Set the chair's itemPlaceable to occupied.
                chairItemPlaceable.isOccupied = true;
                // Set the customers parent to the chair.
                customer.transform.SetParent(randomChair.transform);
                // Start the customer's start script.
                tempCustomer.StartScript();
            }
        }
    }
    // StopSpawningCustomers() is called when the player ends the game.
    public void StopSpawningCustomers()
    {
        Debug.Log("Stopping customer spawning coroutine");
        StopCoroutine(SpawnCustomer());
    }
    // AddToWallet() adds the specified amount to the player's wallet.
    public void AddToWallet(int amount)
    {
        cashEarned += amount;
        wallet += amount;
        walletText.text = "Coins: " + wallet;
    }

    // RemoveFromWallet() removes the specified amount from the player's wallet.
    public void RemoveFromWallet(int amount)
    {
        wallet -= amount;
        walletText.text = "Coins: " + wallet;
    }

    // GetWallet() returns the player's wallet amount.
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

    // EndDay() is called when the day timer runs out or when the player ends the day.
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

    // PauseGame() is called when the player pauses the game by pressing the pause button. 
    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    // ResumeGame() is called when the player resumes the game by pressing the resume button.
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
