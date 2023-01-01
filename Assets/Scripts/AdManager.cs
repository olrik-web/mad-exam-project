using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    string gameID = "5059209"; // The game ID from the Unity Ad service

    bool testMode = true; // Is the game in test mode?

    // Start is called before the first frame update
    void Start()
    {
        // Initializes the Unity Ad service
        Advertisement.Initialize(gameID, testMode);
    }

    // Display an interstitial ad when the function is called.
    public void ShowInterstitialAd()
    {
        if (Advertisement.isInitialized)
        {
            // Displays an Interstitial ad from the Unity Ad service
            Advertisement.Show("Interstitial_Android");
        }
    }
}
