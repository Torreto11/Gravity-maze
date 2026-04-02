using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerHorizontalMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform cameraTransform; // assign your camera
    [SerializeField] private PlayerInputManager inputManager;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 input = inputManager.moveInput;

        if (input.sqrMagnitude < 0.01f) return;

        // Convert input to camera-relative direction
        Vector3 move = new Vector3(input.x, 0f, input.y);
        move = cameraTransform.TransformDirection(move);
        move.y = 0f; // flatten
        move.Normalize();

        controller.Move(move * speed * Time.deltaTime);
    }
}