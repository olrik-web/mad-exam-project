using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float walkSpeed = 0.5f;
    public Animator anim;

    private CanvasManager canvasManager;

    private float runSpeed = 2f;
    private bool isRunning = false;
    public Joystick joystick;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canvasManager = CanvasManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player presses the "I" key, disable or enable the shop canvas.
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Pressed I key.");
            // If the shop canvas is enabled, disable it.
            if (canvasManager.lastActiveCanvas.canvasType == CanvasType.Shop)
            {
                canvasManager.SwitchCanvas(CanvasType.GameUI);
            }
            else
            {
                canvasManager.SwitchCanvas(CanvasType.Shop);
            }
        }

        // If the player presses the "Shift" key, enable or disable running.
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Pressed Shift key.");
            isRunning = !isRunning;
        }
    }

    void FixedUpdate()
    {
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
                    isRunning = true;
                }
                else
                {
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
                if (isRunning)
                {
                    rb.MovePosition(rb.position + moveDirection * runSpeed * Time.fixedDeltaTime);
                    // Adjust the animator parameters. 
                    anim.SetFloat("Speed", runSpeed);
                }
                else
                {
                    rb.MovePosition(rb.position + moveDirection * walkSpeed * Time.fixedDeltaTime);
                    // Adjust the animator parameters. 
                    anim.SetFloat("Speed", walkSpeed);
                }
            }
            else
            {
                // Adjust the animator parameters. 
                anim.SetFloat("Speed", 0f);
            }
        }
    }

    public void FreezeMovement(int freezeTime = 0)
    {
        anim.SetFloat("Speed", 0f);
        this.enabled = false;
        // If a freeze time is specified, unfreeze the player after the specified time.
        if (freezeTime > 0)
        {
            StartCoroutine(EnablePlayerMovement(freezeTime));
        }
    }

    IEnumerator EnablePlayerMovement(int freezeTime = 1)
    {
        // Wait for the item to finish chopping
        yield return new WaitForSeconds(freezeTime);

        // Enable the player's movement
        this.enabled = true;
    }

    public void UnFreezeMovement()
    {
        if (!this.enabled)
        {
            this.enabled = true;
        }
    }
}
