/*
 * Author: Shon Verch
 * File Name: DPad.cs
 * Project Name: VRMovementResearch
 * Creation Date: 12/26/2017
 * Modified Date: 12/26/2017
 * Description: Manages the states of the DPad buttons.
 */

using UnityEngine;

/// <summary>
/// Manages the states of the DPad buttons.
/// </summary>
public static class DPad
{
    public static bool Up { get; private set; }
    public static bool Down { get; private set; }
    public static bool Left { get; private set; }
    public static bool Right { get; private set; }

    private static readonly float lastX;
    private static readonly float lastY;

    static DPad()
    {
        Up = Down = Left = Right = false;
        lastX = Input.GetAxis("DPadHorizontal");
        lastY = Input.GetAxis("DPadVertical");
    }

    /// <summary>
    /// Updates the state of the DPad buttons.
    /// </summary>
    public static void Update()
    {
        Right = Input.GetAxis("DPadHorizontal") == 1 && lastX != 1;
        Left = Input.GetAxis("DPadHorizontal") == -1 && lastX != -1;
        Up = Input.GetAxis("DPadVertical") == 1 && lastY != 1;
        Down = Input.GetAxis("DPadVertical") == -1 && lastY != -1;
    }
}
