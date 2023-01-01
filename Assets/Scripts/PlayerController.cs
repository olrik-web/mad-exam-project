using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb; // The rigidbody of the player.
    private float walkSpeed = 0.5f; // The speed of the player when walking.
    public Animator anim; // The animator of the player.
    private CanvasManager canvasManager; // The canvas manager.
    private float runSpeed = 2f; // The speed of the player when running.
    private bool isRunning = false; // Is the player running?
    public Joystick joystick; // The joystick used to move the player.

    // Start is called before the first frame update
    void Start()
    {
        // Get the rigidbody of the player.
        rb = GetComponent<Rigidbody>();
        // Get the canvas manager reference.
        canvasManager = CanvasManager.GetInstance();
    }

    // FixedUpdate is called once per physics update.
    void FixedUpdate()
    {
        // If the last active canvas is the game UI, allow the player to move.
        if (canvasManager.lastActiveCanvas.canvasType == CanvasType.GameUI)
        {
            // Get the horizontal and vertical input.
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // If the player is using the joystick, get the horizontal and vertical input from the joystick.
            if (joystick != null)
            {
                // Get the horizontal and vertical input from the joystick.
                horizontal = joystick.Horizontal;
                vertical = joystick.Vertical;

                // If the joystick vertical or horizontal input is greater than 0.5, set the player to running.
                if (Mathf.Abs(vertical) > 0.5f || Mathf.Abs(horizontal) > 0.5f)
                {
                    // Set the player to running.
                    isRunning = true;
                }
                else
                {
                    // Set the player to walking.
                    isRunning = false;
                }
            }

            // Create a new vector3 with the horizontal and vertical input.
            Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
            if (moveDirection != Vector3.zero)
            {
                // Rotate the player to face the direction they are moving.
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
                // Move the player in the direction of the moveDirection vector.
                rb.MovePosition(rb.position + moveDirection * (isRunning ? runSpeed : walkSpeed) * Time.fixedDeltaTime);
                // Adjust the animator parameters. 
                anim.SetFloat("Speed", isRunning ? runSpeed : walkSpeed);
            }
            else
            {
                // Adjust the animator parameters. 
                anim.SetFloat("Speed", 0f);
            }
        }
    }

    // Freeze the player's movement for a specified amount of time.
    public void FreezeMovement(int freezeTime = 0)
    {
        // Set the Animator's Speed parameter to 0. Idle animation will play.
        anim.SetFloat("Speed", 0f);
        // Disable the player's movement.
        this.enabled = false;
        // If a freeze time is specified, unfreeze the player after the specified time.
        if (freezeTime > 0)
        {
            StartCoroutine(EnablePlayerMovement(freezeTime));
        }
    }

    // Enable the player's movement after a specified amount of time.
    IEnumerator EnablePlayerMovement(int freezeTime = 1)
    {
        // Wait for the item to finish chopping
        yield return new WaitForSeconds(freezeTime);

        // Enable the player's movement
        this.enabled = true;
    }

    // Unfreeze the player's movement.
    public void UnFreezeMovement()
    {
        if (!this.enabled)
        {
            this.enabled = true;
        }
    }
}
