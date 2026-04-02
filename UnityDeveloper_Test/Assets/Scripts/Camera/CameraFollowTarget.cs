using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;   // Player root object
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);  // Camera height

    void LateUpdate()
    {
        // Follow player position only
        transform.position = playerRoot.position + offset;

        // Do NOT touch rotation — let Cinemachine handle it
    }
}