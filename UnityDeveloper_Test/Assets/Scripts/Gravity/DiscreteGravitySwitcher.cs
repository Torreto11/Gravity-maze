using UnityEngine;

[RequireComponent(typeof(GravityCharacterController))]
public class DiscreteGravitySwitcher : MonoBehaviour
{
    [Header("References")]
    public GravityCharacterController controller;
    public Transform cameraReference;

    [Header("Options")]
    public bool clearVerticalVelocityOnSwitch = true;

    private void Reset()
    {
        controller = GetComponent<GravityCharacterController>();
    }

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<GravityCharacterController>();
    }

    private void Update()
    {
        if (controller == null || cameraReference == null)
            return;

        if (Input.GetKeyDown(KeyCode.K)) // RIGHT
        {
            Debug.Log("K pressed - gravity right");
            RotateRelativeToView(-90f);
        }
        else if (Input.GetKeyDown(KeyCode.H)) // LEFT
        {
            Debug.Log("H pressed - gravity left");
            RotateRelativeToView(90f);
        }
        else if (Input.GetKeyDown(KeyCode.U)) // UP
        {
            Debug.Log("U pressed - gravity up");
            RotateRelativeToView(180f);
        }
        else if (Input.GetKeyDown(KeyCode.J)) // DOWN
        {
            Debug.Log("J pressed - gravity down");
            ResetToWorldDown();
        }
    }

    private void RotateRelativeToView(float degrees)
    {
        Vector3 currentUp = controller.targetGravityUp;

        Vector3 rotationAxis = Vector3.ProjectOnPlane(cameraReference.forward, currentUp).normalized;

        if (rotationAxis.sqrMagnitude < 0.0001f)
            rotationAxis = Vector3.ProjectOnPlane(transform.forward, currentUp).normalized;

        if (rotationAxis.sqrMagnitude < 0.0001f)
            rotationAxis = Vector3.forward;

        Vector3 rotatedUp = Quaternion.AngleAxis(degrees, rotationAxis) * currentUp;
        rotatedUp = SnapToAxis(rotatedUp);

        controller.RotateGravity(rotatedUp);

        if (clearVerticalVelocityOnSwitch)
            controller.ClearVelocityAlongGravity();

        Debug.Log("New gravity up: " + rotatedUp);
    }

    private void ResetToWorldDown()
    {
        controller.RotateGravity(Vector3.up);

        if (clearVerticalVelocityOnSwitch)
            controller.ClearVelocityAlongGravity();

        Debug.Log("Gravity reset to world up");
    }

    private Vector3 SnapToAxis(Vector3 v)
    {
        v.Normalize();

        float ax = Mathf.Abs(v.x);
        float ay = Mathf.Abs(v.y);
        float az = Mathf.Abs(v.z);

        if (ax >= ay && ax >= az)
            return new Vector3(Mathf.Sign(v.x), 0f, 0f);

        if (ay >= ax && ay >= az)
            return new Vector3(0f, Mathf.Sign(v.y), 0f);

        return new Vector3(0f, 0f, Mathf.Sign(v.z));
    }
}