using UnityEngine;

public class GravityThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public GravityCharacterController gravityController;

    [Header("Orbit")]
    public float distance = 4f;
    public float heightOffset = 1.5f;
    public float mouseSensitivity = 2f;
    public float minPitch = -75f;
    public float maxPitch = 75f;

    [Header("Smoothing")]
    public float positionSmooth = 12f;
    public float rotationSmooth = 12f;

    private float yaw;
    private float pitch;

    private Vector3 planarForward;
    private Vector3 lastGravityUp;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (gravityController != null)
        {
            lastGravityUp = gravityController.targetGravityUp;
        }
        else
        {
            lastGravityUp = Vector3.up;
        }

        planarForward = Vector3.ProjectOnPlane(transform.forward, lastGravityUp).normalized;

        if (planarForward.sqrMagnitude < 0.001f)
            planarForward = Vector3.forward;
    }

    private void LateUpdate()
    {
        if (target == null || gravityController == null)
            return;

        Vector3 currentUp = gravityController.targetGravityUp;

        HandleGravityRotation(currentUp);
        HandleMouseLook(currentUp);
        UpdateCamera(currentUp);
    }

    private void HandleGravityRotation(Vector3 currentUp)
    {
        if (Vector3.Dot(lastGravityUp, currentUp) < 0.9999f)
        {
            Quaternion gravityDelta = Quaternion.FromToRotation(lastGravityUp, currentUp);

            planarForward = gravityDelta * planarForward;
            transform.rotation = gravityDelta * transform.rotation;

            lastGravityUp = currentUp;
        }
    }

    private void HandleMouseLook(Vector3 currentUp)
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (Mathf.Abs(mouseX) > 0.0001f)
        {
            Quaternion yawRotation = Quaternion.AngleAxis(mouseX, currentUp);
            planarForward = yawRotation * planarForward;
        }

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        planarForward = Vector3.ProjectOnPlane(planarForward, currentUp).normalized;

        if (planarForward.sqrMagnitude < 0.001f)
        {
            planarForward = Vector3.ProjectOnPlane(transform.forward, currentUp).normalized;
        }
    }

    private void UpdateCamera(Vector3 currentUp)
    {
        Vector3 right = Vector3.Cross(currentUp, -planarForward).normalized;
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, right);

        Vector3 lookDirection = (pitchRotation * planarForward).normalized;

        Vector3 focusPoint = target.position + gravityController.CurrentUp * heightOffset;
        Vector3 desiredPosition = focusPoint - lookDirection * distance;
        Quaternion desiredRotation = Quaternion.LookRotation(lookDirection, currentUp);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            positionSmooth * Time.deltaTime
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmooth * Time.deltaTime
        );
    }
}