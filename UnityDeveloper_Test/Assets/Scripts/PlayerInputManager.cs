using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 lookInput;

    private bool firstLookInput = true;

    // Called by Input System when moving
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Called by Input System when looking
    public void OnLook(InputAction.CallbackContext context)
    {
        if (firstLookInput)
        {
            lookInput = Vector2.zero;
            firstLookInput = false;
            return;
        }
        lookInput = context.ReadValue<Vector2>();
    }

 
}