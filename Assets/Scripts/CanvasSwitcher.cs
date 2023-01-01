using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CanvasSwitcher : MonoBehaviour
{

    public CanvasType canvasType;
    private CanvasManager canvasManager;
    private Button menuButton;

    private void Start()
    {
        // Get the button component and add a listener to it.
        menuButton = GetComponent<Button>();
        menuButton.onClick.AddListener(OnMenuButtonClicked);
        // Get the canvas manager reference.
        canvasManager = CanvasManager.GetInstance();
    }
    // Function to switch to the canvas that is assigned to the button.
    private void OnMenuButtonClicked()
    {
        canvasManager.SwitchCanvas(canvasType);
    }
}