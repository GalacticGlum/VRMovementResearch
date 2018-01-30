/*
 * Author: Shon Verch
 * File Name: SpawningArea.cs
 * Project Name: VRMovementResearch
 * Creation Date: 12/26/2017
 * Modified Date: 12/26/2017
 * Description: Spawns a prefab randomly throughout the volume of a Transform.
 */

using UnityEngine;

/// <summary>
/// Spawns a prefab randomly throughout the volume of a <see cref="Transform"/>.
/// </summary>
public class SpawningArea : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPrefab;

    [Tooltip("The amount of objects to spawn.")]
    [SerializeField]
    private float objectCount = 100;

    /// <summary>
    /// Called on the frame when this <see cref="MonoBehaviour"/> is enabled, 
    /// before any of the Update methods are called.
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            position = transform.TransformPoint(position * 0.5f);

            Instantiate(spawnPrefab, position, Quaternion.identity);
        }
    }
}
