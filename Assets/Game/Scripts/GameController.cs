/*
 * Author: Shon Verch
 * File Name: GameController.cs
 * Project Name: VRMovementResearch
 * Creation Date: 12/27/2017
 * Modified Date: 12/27/2017
 * Description: Controls the state of the game.
 */

using UnityEngine;

/// <summary>
/// Controls the state of the game.
/// </summary>
public class GameController : MonoBehaviour
{
    /// <summary>
    /// The active <see cref="GameController"/> in the scene.
    /// </summary>
    public static GameController Instance { get; private set; }

    /// <summary>
    /// The amount of seconds that are left in this game. 
    /// </summary>
    public float TimeLeft { get; private set; }

    /// <summary>
    /// The score of the user.
    /// </summary>
    public int Score { get; set; }

    [SerializeField]
    private int gameLengthInSeconds = 60;

    /// <summary>
    /// Called on the frame when this <see cref="MonoBehaviour"/> is enabled, 
    /// before any of the Update methods are called.
    /// </summary>
    private void Start()
    {
        TimeLeft = gameLengthInSeconds;
        Instance = this;
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    private void Update()
    {
        TimeLeft -= Time.deltaTime;
        if (!(TimeLeft <= 0)) return;

        Time.timeScale = 0;

        Debug.Log("Gameover");
        Debug.Log(Score + " points.");
    }
}
