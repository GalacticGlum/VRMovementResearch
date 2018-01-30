/*
 * Author: Shon Verch
 * File Name: DropArea.cs
 * Project Name: VRMovementResearch
 * Creation Date: 12/27/2017
 * Modified Date: 12/27/2017
 * Description: The area which the player can drop boxes in.
 */

using UnityEngine;

/// <summary>
/// The area which the player can drop boxes in.
/// </summary>
public class DropArea : MonoBehaviour
{
    /// <summary>
    /// Called when a collider enters the trigger.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" || other.gameObject.layer != LayerMask.NameToLayer("Pickup")) return;
        GameController.Instance.Score += 1;
    }
}
