using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DirectionalGravityController : MonoBehaviour
{
    [Header("Gravity Settings")]
    public Vector3 GravityVector = Vector3.down;  // Current gravity direction
    public float GravityStrength = 9.8f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Move player along current gravity
        if (GravityVector.sqrMagnitude > 0f)
        {
            controller.Move(GravityVector.normalized * GravityStrength * Time.deltaTime);
        }
    }

    // Call this to set new gravity
    public void SetGravityDirection(Vector3 newGravity)
    {
        GravityVector = newGravity.normalized;
        Debug.Log($"[GravityController] Gravity applied: {GravityVector}");
    }
}