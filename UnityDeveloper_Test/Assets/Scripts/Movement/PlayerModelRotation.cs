using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerModelRotation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputManager inputManager;  // Reads WASD input
    [SerializeField] private Transform playerModel;            // Visual model
    [SerializeField] private Transform cameraTransform;        // Camera for camera-relative movement

    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 10f;        // Smooth rotation speed

    void Update()
    {
        Vector2 moveInput = inputManager.moveInput;

        // Only rotate if there is movement input
        if (moveInput.sqrMagnitude > 0.01f)
        {
            // Convert 2D input to world-space movement direction relative to camera
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
            moveDir = cameraTransform.TransformDirection(moveDir);
            moveDir.y = 0f; // flatten to horizontal
            moveDir.Normalize();

            // Smoothly rotate the player model to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}