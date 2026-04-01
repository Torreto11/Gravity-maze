using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRotate : MonoBehaviour
{

    // Stores mouse movement input (X = horizontal, Y = vertical)
    private Vector2 lookInput;

    // Used to ignore the first input (prevents sudden camera snap at start)
    private bool firstInput = true;

    [SerializeField] private float sensitivity = 2f;

    // Mouse Movement
    public void OnLook(InputAction.CallbackContext context)
    {

        // To skip intial snap of camera
        if (firstInput)
        {
            lookInput = Vector2.zero;
            firstInput = false;
            return;
        }

        lookInput = context.ReadValue<Vector2>();
        Debug.Log("Look Input: " + lookInput);
    }

    void Update()
    {
        // Multiply input for smooth rotation
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        // Horizontal rotation (world space)
        transform.Rotate(Vector3.up * mouseX, Space.World);
        
        //get current rotation
        Vector3 angles = transform.eulerAngles;

        // inverts Y
        angles.x -= mouseY;

        // clamp to prevent flipping
        if (angles.x > 180) angles.x -= 360;
        {
            angles.x = Mathf.Clamp(angles.x, -80f, 80f);
        }

        //apply final rotation (z set to 0 to avoid tilt)
        transform.eulerAngles = new Vector3(angles.x, transform.eulerAngles.y, 0f);
    }
}