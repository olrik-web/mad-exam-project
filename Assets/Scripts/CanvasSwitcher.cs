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
        menuButton = GetComponent<Button>();
        menuButton.onClick.AddListener(OnMenuButtonClicked);
        canvasManager = CanvasManager.GetInstance();
    }

    private void OnMenuButtonClicked()
    {
        canvasManager.SwitchCanvas(canvasType);
    }
}