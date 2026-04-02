using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerVerticalMovement : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private PlayerInputManager inputManager;

    private CharacterController controller;
    private Vector3 verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Called by Input System when jump button is pressed
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * 2f);
        }
    }

    void Update()
    {
        // Only jump movement; gravity handled externally
        controller.Move(verticalVelocity * Time.deltaTime);

        if (IsGrounded() && verticalVelocity.y < 0)
            verticalVelocity.y = 0f; // reset jump velocity when grounded
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float rayLength = groundCheckDistance + 0.1f;
        return Physics.Raycast(origin, Vector3.down, rayLength, groundMask);
    }
}