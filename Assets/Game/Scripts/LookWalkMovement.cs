using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class LookWalkMovement : MonoBehaviour
{
    [SerializeField]
    private Camera vrCamera;
    [Tooltip("The minimum angle that the camera must be at to begin moving.")]
    [SerializeField]
    private float minimumMovementAngle = 30;
    [SerializeField]
    private float movementSpeed = 3;

    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (!(vrCamera.transform.eulerAngles.x >= minimumMovementAngle) ||
            !(vrCamera.transform.eulerAngles.x < 90.0f)) return;

        Vector3 direction = vrCamera.transform.TransformDirection(Vector3.forward);
        characterController.SimpleMove(direction * movementSpeed * Time.deltaTime);
    }
}
