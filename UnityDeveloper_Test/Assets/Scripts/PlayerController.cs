using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Debug.Log("PlayerController is running");
    }

    //WASD movement using new Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log($"Move Input: {moveInput}");

        /*if (controller.isGrounded)
            Debug.Log("Touched");
        else
            Debug.Log("Air");*/
    }

    //Jump and grounded check
    public void OnJump(InputAction.CallbackContext context)
    {
       /* Debug.Log($"Jumping {context.performed} - Is Grounded: {controller.isGrounded}");
        if(context.performed && controller.isGrounded)
        {
            Debug.Log("Jumped");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
       */
    }
    
    void Update()
    {
        //converting vector2 to vector3 (X = left/right, Z = forward/back)
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // Horizontal movements
        controller.Move(move * speed * Time.deltaTime);

        // gravity
        velocity.y += gravity * Time.deltaTime;

        // Vertical movements (Jump)
        controller.Move(velocity * Time.deltaTime);
    }
}
