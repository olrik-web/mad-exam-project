using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float speed = 1f;
    private float zoomSpeed = 0.1f;
    private float zoomMin = 30f;

    private float zoomMax = 120f;
    private Vector3 defaultPosition = new Vector3(0, 16, -10);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the player drags the screen using touch, then move the camera. Don't move the camera if the player is touching a UI element.
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            // Get the touch.
            Touch touch = Input.GetTouch(0);
            // Get the touch position.
            Vector2 touchPosition = touch.position;
            // Get the touch delta position.
            Vector2 touchDeltaPosition = touch.deltaPosition;

            // Move the camera on the x and z axis smoothly.
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - touchDeltaPosition.x * speed * Time.deltaTime, transform.position.y, transform.position.z - touchDeltaPosition.y * speed * Time.deltaTime), 0.5f);
        }
        // If the player pinches the screen, then zoom in or out.
        if (Input.touchCount == 2)
        {
            // Get the touch positions.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Zoom in or out depending on the difference by changing the field of view.
            Camera.main.fieldOfView += deltaMagnitudeDiff * zoomSpeed;

            // Clamp the field of view to make sure it's between 30 and 120.
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, zoomMin, zoomMax);
        }
    }

    // Method to reset the camera position.
    public void ResetCameraPosition()
    {
        // Reset the camera position.
        transform.position = defaultPosition;

        // Reset the field of view.
        Camera.main.fieldOfView = 60f;
    }
}
