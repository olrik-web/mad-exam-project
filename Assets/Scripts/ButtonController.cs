using UnityEngine;
using UnityEngine.UI;

public enum ButtonType
{
    START_GAME,
    PAUSE_MENU,
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

    // Start is called before the first frame update
    private void Start()
    {
        // Get the button component and add a listener to it.
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
        // Get the canvas manager reference.
        canvasManager = CanvasManager.GetInstance();
        // Get the game manager reference.
        gameManager = GameManager.instance;
    }

    private void OnButtonClicked()
    {
        // Switch statement to determine what to do when the button is clicked.
        switch (buttonType)
        {
            // If the button is the start game button, then start the game.
            case ButtonType.START_GAME:
                // Call the resume game function in the game manager.
                gameManager.StartGame();
                // Unfreeze the player's movement.
                gameManager.player.UnFreezeMovement();
                // Switch to the game canvas.
                canvasManager.SwitchCanvas(CanvasType.GameUI);
                break;
            // If the button is the pause menu button, then go back to the main menu.
            case ButtonType.PAUSE_MENU:
                // Call the pause game function on the game manager. 
                gameManager.PauseGame();
                // Set the game manager's hasGameBeenPaused variable to true.
                gameManager.hasGameBeenPaused = true;
                // Switch to the main menu canvas.
                canvasManager.SwitchCanvas(CanvasType.PauseMenu);
                break;
            // If the button is the resume game button, then resume the game.
            case ButtonType.RESUME_GAME:
                // Call the resume game function in the game manager.
                gameManager.ResumeGame();
                // Unfreeze the player's movement.
                gameManager.player.UnFreezeMovement();
                // Switch to the game canvas.
                canvasManager.SwitchCanvas(CanvasType.GameUI);
                break;
            // If the button is the quit game button, then quit the game.
            case ButtonType.QUIT_GAME:
                // Quit the game.
                Application.Quit();
                break;
            // If the button is the open shop button, then open the shop.
            case ButtonType.OPEN_SHOP:
                // Open the shop.
                canvasManager.SwitchCanvas(CanvasType.Shop);
                break;
        }
    }
}