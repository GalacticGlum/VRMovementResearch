/*
 * Author: Shon Verch
 * File Name: PlayerController.cs
 * Project Name: VRMovementResearch
 * Creation Date: 12/20/2017
 * Modified Date: 12/24/2017
 * Description: The movement controller for the player.
 */

using System.Collections;
using UnityEngine;

/// <summary>
/// The movement controller for the player.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    private Camera vrCamera;
    [SerializeField]
    private float movementSpeed = 50;
    [SerializeField]
    private PlayerMovementType movementType;

    [Header("Look Walk")]
    [Tooltip("The minimum angle that the camera must be at to begin moving.")]
    [SerializeField]
    private float minimumMovementAngle = 30;

    [Header("Teleport")]
    [SerializeField]
    private GameObject positionSelectionPrefab;
    [SerializeField]
    private Camera fadeCamera;
    [SerializeField]
    private float teleportFadeDuration = 0.5f;

    private GameObject teleportSelectionGameObject;
    private bool isInTeleportMode;

    private CharacterController characterController;

    /// <summary>
    /// Called on the frame when this <see cref="MonoBehaviour"/> is enabled, 
    /// before any of the Update methods are called.
    /// </summary>
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Called every fixed framerate frame.
    /// </summary>
    private void FixedUpdate()
    {
        switch (movementType)
        {
            case PlayerMovementType.FreeWalk:
                float joystickVerticalAxis = Input.GetAxis("LeftJoystickVertical");
                if (joystickVerticalAxis <= 0)
                {
                    MoveForward();
                }

                break;
            case PlayerMovementType.LookWalk:
                // If our camera is within the movement angle threshold then we move forward.
                if (vrCamera.transform.eulerAngles.x >= minimumMovementAngle && vrCamera.transform.eulerAngles.x < 90.0f)
                {
                    MoveForward();
                }

                break;
            case PlayerMovementType.Teleport:
                float rightTriggerAxis = Input.GetAxis("RightTrigger");
                if (rightTriggerAxis >= 0.7f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(vrCamera.transform.position, vrCamera.transform.forward, out hit))
                    {
                        Vector3 position = hit.point;

                        // If teleport mode is false then we just entered it.
                        // In this case, we need to execute some initialization logic.
                        if (!isInTeleportMode)
                        {
                            teleportSelectionGameObject = Instantiate(positionSelectionPrefab, position, Quaternion.identity);
                        }
                        else
                        {
                            teleportSelectionGameObject.transform.position = position;
                            teleportSelectionGameObject.transform.rotation = Quaternion.identity;
                        }

                        isInTeleportMode = true;
                    }
                }
                else
                {
                    // If teleport mode is true and we have let go of the trigger
                    // it means that we need to apply the teleport.
                    if (isInTeleportMode)
                    {
                        StartCoroutine("ExecuteTeleportFade");

                        // We only care about the x and z components of the teleport position.
                        // We need the y to stay the same so that the camera doesn't clip into the floor.
                        Vector3 newPosition = new Vector3(teleportSelectionGameObject.transform.position.x, transform.position.y, teleportSelectionGameObject.transform.position.z);
                        transform.position = newPosition;
                    }

                    isInTeleportMode = false;

                    Destroy(teleportSelectionGameObject);
                    teleportSelectionGameObject = null;
                }

                break;
        }
    }

    /// <summary>
    /// Moves the player forward.
    /// </summary>
    private void MoveForward()
    {
        Vector3 direction = vrCamera.transform.TransformDirection(Vector3.forward);
        characterController.SimpleMove(direction * movementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Executes the teleport fade.
    /// </summary>
    private IEnumerator ExecuteTeleportFade()
    {
        fadeCamera.enabled = true;
        yield return new WaitForSeconds(teleportFadeDuration);
        fadeCamera.enabled = false;
    }
}
