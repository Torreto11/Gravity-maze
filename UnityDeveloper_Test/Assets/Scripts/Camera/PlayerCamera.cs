using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2f;
    private PlayerInputManager inputManager;

    void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = inputManager.lookInput.x * sensitivity;
        float mouseY = inputManager.lookInput.y * sensitivity;

        // Horizontal rotation (yaw)
        transform.Rotate(Vector3.up * mouseX, Space.World);

        // Vertical rotation (pitch)
        Vector3 angles = transform.eulerAngles;
        angles.x -= mouseY;
        if (angles.x > 180) angles.x -= 360;
        angles.x = Mathf.Clamp(angles.x, -80f, 80f);

        transform.eulerAngles = new Vector3(angles.x, transform.eulerAngles.y, 0f);
    }
}