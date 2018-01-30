/*
 * Author: Shon Verch
 * File Name: MenuController.cs
 * Project Name: VRMovementResearch
 * Creation Date: 12/27/2017
 * Modified Date: 12/27/2017
 * Description: Controls all menu functionality.
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls all menu functionality.
/// </summary>
public class MenuController : MonoBehaviour
{
    [SerializeField]
    private Dropdown movementTypeDropdown;

    /// <summary>
    /// Called on the frame when this <see cref="MonoBehaviour"/> is enabled, 
    /// before any of the Update methods are called.
    /// </summary>
    private void Start()
    {
        movementTypeDropdown.onValueChanged.AddListener(MovementTypeDropdownValueChanged);
    }

    /// <summary>
    /// Called when the movement type dropdown value changes.
    /// </summary>
    private static void MovementTypeDropdownValueChanged(int value)
    {
        GameSettings.PlayerMovementType = (PlayerMovementType) value;
    }

    /// <summary>
    /// Loads a scene by index.
    /// </summary>
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
