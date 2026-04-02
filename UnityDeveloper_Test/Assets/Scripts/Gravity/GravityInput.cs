using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DirectionalGravityController))]
public class GravityInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerModel; // The visual model

    private DirectionalGravityController gravityController;

    // Gravity directions relative to player model
    private Vector3[] gravityVectors;

    // Lookup table: currentPlane × buttonIndex (Up=0, Down=1, Left=2, Right=3)
    private int[,] nextPlane = new int[6, 4]
    {
        {2,3,4,5}, // Base
        {2,3,5,4}, // Top
        {1,0,4,5}, // Forward
        {0,1,4,5}, // Back
        {2,3,1,0}, // Left
        {2,3,0,1}  // Right
    };

    private int currentPlane = 0; // Start at Base

    void Start()
    {
        gravityController = GetComponent<DirectionalGravityController>();

        // Initialize gravity vectors based on playerModel axes
        gravityVectors = new Vector3[6]
        {
            -playerModel.up,     // 0 Base
            playerModel.up,      // 1 Top
            playerModel.forward, // 2 Forward
            -playerModel.forward,// 3 Back
            -playerModel.right,  // 4 Left
            playerModel.right    // 5 Right
        };

        ApplyGravity();
        Debug.Log($"[Start] CurrentPlane: {currentPlane}, Gravity: {gravityVectors[currentPlane]}");
    }

    // Each button mapped in Input System to a separate method
    public void OnUpButton(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        ChangePlane(0);
    }

    public void OnDownButton(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        ChangePlane(1);
    }

    public void OnLeftButton(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        ChangePlane(2);
    }

    public void OnRightButton(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        ChangePlane(3);
    }

    // Updates plane based on the button pressed
    private void ChangePlane(int buttonIndex)
    {
        currentPlane = nextPlane[currentPlane, buttonIndex];
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        Vector3 newGravity = gravityVectors[currentPlane];
        gravityController.SetGravityDirection(newGravity);
        Debug.Log($"[Gravity] Plane: {currentPlane}, Gravity: {newGravity}");
    }
}