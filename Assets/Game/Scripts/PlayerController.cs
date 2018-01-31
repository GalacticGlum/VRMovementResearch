/*
 * Author: Shon Verch
 * File Name: PlayerController.cs
 * Project Name: VRMovementResearch
 * Creation Date: 12/20/2017
 * Modified Date: 12/27/2017
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
    /// <summary>
    /// The active <see cref="PlayerController"/> in the scene.
    /// </summary>
    public static PlayerController Instance { get; private set; }

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
    [SerializeField]
    private float teleportRange = 6;

    [Header("Object Pickup")]
    [SerializeField]
    private float pickupObjectOffset = 3;
    [SerializeField]
    private float pickupRadius = 5;
    [SerializeField]
    private float pickupMovementSmoothening = 4;
    [SerializeField]
    private float pickupRotationSpeed = 30;
    [SerializeField]
    private float objectDropThrowVelocity = 5;

    private GameObject objectBeingCarried;
    private bool isCarryingObject;

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
        movementType = GameSettings.PlayerMovementType;

        Instance = this;
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    private void Update()
    {
        // If our settings have changed, let's update our player data.
        if (GameSettings.PlayerMovementType != movementType)
        {
            movementType = GameSettings.PlayerMovementType;
        }

        DPadInput.Update();

        if (isCarryingObject && objectBeingCarried != null)
        {
            Vector3 carryDestination = GetCarryObjectDestination();
            objectBeingCarried.transform.position = Vector3.Lerp(objectBeingCarried.transform.position, carryDestination, Time.deltaTime * pickupMovementSmoothening);

            float rightJoystickHorizontalAxis = Input.GetAxis("RightJoystickHorizontal");
            objectBeingCarried.transform.Rotate(Vector3.up * pickupRotationSpeed * Time.deltaTime * -rightJoystickHorizontalAxis);

            // If we aren't pressing the pickup key, let's bail!
            // If we are pressing it at this point, we want to drop the item that we are holding.
            if (!DPadInput.Down) return;

            isCarryingObject = false;

            Rigidbody objectBeingCarriedRigidbody = objectBeingCarried.GetComponent<Rigidbody>();
            objectBeingCarriedRigidbody.isKinematic = false;
            objectBeingCarriedRigidbody.velocity = vrCamera.transform.forward * objectDropThrowVelocity;

            // Reset the colour of the material just in case it was changed anywhere else.
            objectBeingCarried.GetComponent<MeshRenderer>().material.color = Color.white;

            objectBeingCarried = null;
        }
        else
        {
            // We only want to pickup an item when we press up on the Dpad. When it is NOT down,
            // let's bail out of this method.
            if (!DPadInput.Up) return;

            RaycastHit hit;
            if (!Physics.Raycast(vrCamera.transform.position, vrCamera.transform.forward, out hit)) return;

            // If the object we hit has no rigidbody or isn't on the pickup layer then there is nothing to do.
            if (hit.collider.attachedRigidbody == null ||
                hit.transform.gameObject.layer != LayerMask.NameToLayer("Pickup")) return;

            // If the object we hit is out of range let's not pick it up.
            float distanceFromHit = Vector3.Distance(transform.position, hit.point);
            if (distanceFromHit > pickupRadius) return;

            isCarryingObject = true;
            objectBeingCarried = hit.transform.gameObject;
            hit.collider.attachedRigidbody.isKinematic = true;
        }
    }

    /// <summary>
    /// Called every fixed framerate frame.
    /// </summary>
    private void FixedUpdate()
    {
        switch (movementType)
        {
            case PlayerMovementType.FreeWalk:
                float joystickHorizontalAxis = Input.GetAxis("LeftJoystickHorizontal");
                float joystickVerticalAxis = Input.GetAxis("LeftJoystickVertical");

                Move(transform.forward * -joystickVerticalAxis);
                Move(transform.right * joystickHorizontalAxis);

                break;
            case PlayerMovementType.LookWalk:
                // If our camera is within the movement angle threshold then we move forward.
                if (vrCamera.transform.eulerAngles.x >= minimumMovementAngle && vrCamera.transform.eulerAngles.x < 90.0f)
                {
                    Move(transform.forward);
                }

                break;
            case PlayerMovementType.Teleport:
                HandleTeleport();
                break;
        }
    }

    /// <summary>
    /// Hadnles the teleport logic.
    /// </summary>
    private void HandleTeleport()
    {
        float rightTriggerAxis = Input.GetAxis("RightTrigger");
        if (rightTriggerAxis >= 0.7f)
        {
            RaycastHit hit;

            // Do the raycast on every layer but the pickable object one.
            LayerMask raycastLayerMask = ~(1 << LayerMask.NameToLayer("Pickup"));

            // If we don't hit anything in our raycast then we can bail.
            if (!Physics.Raycast(vrCamera.transform.position, vrCamera.transform.forward, out hit, Mathf.Infinity, raycastLayerMask)) return;

            if (isCarryingObject)
            {
                // We make our carried object translucent so we can see where we want to teleport (beyond the carried object).
                objectBeingCarried.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0.2f);
            }

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

            float distanceFromHit = Vector3.Distance(hit.point, transform.position);

            Color teleportSelectionColour = distanceFromHit > teleportRange ? new Color(1, 0, 0, 0.4f) : new Color(0, 1, 0, 0.4f);
            teleportSelectionGameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = teleportSelectionColour;

            isInTeleportMode = true;
        }
        else
        {
            // If teleport mode is true and we have let go of the trigger
            // it means that we need to apply the teleport.
            if (isInTeleportMode)
            {
                // We only care about the x and z components of the teleport position.
                // We need the y to stay the same so that the camera doesn't clip into the floor.
                Vector3 newPosition = new Vector3(teleportSelectionGameObject.transform.position.x, transform.position.y, teleportSelectionGameObject.transform.position.z);

                float distanceFromNewPosition = Vector3.Distance(newPosition, transform.position);

                // Only execute the teleport if it is within range
                if (distanceFromNewPosition <= teleportRange)
                {
                    StartCoroutine("ExecuteTeleportFade");
                    transform.position = newPosition;

                    if (isCarryingObject)
                    {
                        objectBeingCarried.transform.position = GetCarryObjectDestination();
                    }
                }
            }

            isInTeleportMode = false;

            Destroy(teleportSelectionGameObject);
            teleportSelectionGameObject = null;
        }
    }

    /// <summary>
    /// Moves the player forward.
    /// </summary>
    /// <param name="direction">A unit vector representing the direction to move in.</param>
    private void Move(Vector3 direction)
    {
        Vector3 transformDirection = vrCamera.transform.TransformDirection(direction);
        characterController.SimpleMove(transformDirection * movementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Executes the teleport fade.
    /// </summary>
    private IEnumerator ExecuteTeleportFade()
    {
        fadeCamera.enabled = true;
        yield return new WaitForSeconds(teleportFadeDuration);
        fadeCamera.enabled = false;

        if (isCarryingObject)
        {
            // Reset our carried object color back to white (full alpha).
            objectBeingCarried.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
    
    /// <summary>
    /// Gets the destination position of the carry object with the offset applied.
    /// </summary>
    private Vector3 GetCarryObjectDestination()
    {
        return vrCamera.transform.position + vrCamera.transform.forward * pickupObjectOffset;
    }
}
