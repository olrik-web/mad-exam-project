using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    string gameID = "5059209";

    bool testMode = true;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes the Unity Ad service
        Advertisement.Initialize(gameID, testMode);
    }

    public void ShowInterstitialAd()
    {
        if (Advertisement.isInitialized)
        {
            // Displays an Interstitial ad from the Unity Ad service
            Advertisement.Show("Interstitial_Android");
        }
    }
}
