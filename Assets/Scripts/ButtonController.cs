using UnityEngine;
using UnityEngine.UI;

public enum ButtonType
{
    START_GAME,
    MAIN_MENU,
    RESUME_GAME,
    QUIT_GAME,
    OPEN_SHOP,

}

[RequireComponent(typeof(Button))]
public class ButtonController : MonoBehaviour
{
    public ButtonType buttonType;
    private Button button;
    private CanvasManager canvasManager;
    private GameManager gameManager;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
        canvasManager = CanvasManager.GetInstance();
        gameManager = GameManager.instance;
    }

    private void OnButtonClicked()
    {

        switch (buttonType)
        {
            case ButtonType.START_GAME:
                // Call the resume game function in the game manager.
                gameManager.StartGame();
                // If this is the first time switching to the game scene, then call the start spawning customers function.
                // Start the game. 
                gameManager.player.UnFreezeMovement();

                // Switch to the game canvas.
                canvasManager.SwitchCanvas(CanvasType.GameUI);
                break;
            case ButtonType.MAIN_MENU:
                // Go back to the main menu.
                // Call the pause game function on the game manager. 
                gameManager.PauseGame();
                // Set the game manager's hasGameBeenPaused variable to true.
                gameManager.hasGameBeenPaused = true;
                // Switch to the main menu canvas.
                canvasManager.SwitchCanvas(CanvasType.MainMenu);

                break;
            case ButtonType.RESUME_GAME:
                // Call the resume game function in the game manager.
                gameManager.ResumeGame();
                // Start the game. 
                gameManager.player.UnFreezeMovement();

                // Switch to the game canvas.
                canvasManager.SwitchCanvas(CanvasType.GameUI);
                break;
            case ButtonType.QUIT_GAME:
                // Quit the game.
                Application.Quit();
                break;

            case ButtonType.OPEN_SHOP:
                // Open the shop.
                canvasManager.SwitchCanvas(CanvasType.Shop);
                break;
        }
    }
}