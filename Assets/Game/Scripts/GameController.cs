/*
 * Author: Shon Verch
 * File Name: GameController.cs
 * Project Name: VRMovementResearch
 * Creation Date: 12/27/2017
 * Modified Date: 12/27/2017
 * Description: Controls the state of the game.
 */

using UnityEngine;
using UnityEngine.SceneManagement;

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
        Invoke("EndGame", gameLengthInSeconds);
        Time.timeScale = 1;

        Instance = this;
    }

    /// <summary>
    /// Handle the end game logic.
    /// </summary>
    private void EndGame()
    {
        Time.timeScale = 0;

        // There is no actual gameover screen, rather we just output the results to the console as this is
        // a demo for research purposes.
        Debug.Log("Gameover");
        Debug.Log(Score + " points.");

        // Go back to the main menu scene.
        SceneManager.LoadScene(0);
    }
}
