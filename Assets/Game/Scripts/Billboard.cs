/*
 * Author: Shon Verch
 * File Name: Billboard.cs
 * Project Name: TheDungeonMaster
 * Creation Date: 12/26/2017
 * Modified Date: 12/26/2017
 * Description: Makes an object always face the camera.
 */

using UnityEngine;

/// <summary>
/// Makes an object always face the camera.
/// </summary>
public class Billboard : MonoBehaviour
{
    /// <summary>
    /// Called every frame.
    /// </summary>
    private void Update()
    {
        Vector3 distance = transform.position - Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(distance);
    }
}