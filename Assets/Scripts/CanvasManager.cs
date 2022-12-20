using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum CanvasType
{
    MainMenu,
    GameUI,
    EndScreen,
    Shop
}

public class CanvasManager : Singleton<CanvasManager>
{
    List<CanvasController> canvasControllers;
    public CanvasController lastActiveCanvas;

    protected override void Awake()
    {
        // Get all the CanvasControllers (UIs) in the scene and store them in a list. We use the Linq library to do convert the array to a list.
        canvasControllers = GetComponentsInChildren<CanvasController>().ToList();

        // Set the last active canvas to be inactive. 
        if (lastActiveCanvas != null)
        {
            lastActiveCanvas.gameObject.SetActive(false);
        }

        // Set the GameUI CanvasController to active.
        SwitchCanvas(CanvasType.GameUI);
    }

    public void SwitchCanvas(CanvasType canvasType)
    {
        // Loop through all the CanvasControllers and set them to inactive.
        foreach (CanvasController canvasController in canvasControllers)
        {
            canvasController.gameObject.SetActive(false);
        }

        // Set the CanvasController with the specified CanvasType to active.
        CanvasController canvasControllerToActivate = canvasControllers.Find(x => x.canvasType == canvasType);
        if (canvasControllerToActivate != null)
        {
            canvasControllerToActivate.gameObject.SetActive(true);
            lastActiveCanvas = canvasControllerToActivate;
        }
        else
        {
            Debug.LogError("No CanvasController with the CanvasType " + canvasType + " found in the scene.");
        }
    }
}
