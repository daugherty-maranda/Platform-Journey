using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//This is the Game Controller. It handles the Menu and Scenes.

public class GameController : MonoBehaviour
{
    //This is the player
    private Player player;

    //This is the start of a win scene
    private Player playerWon;

    //This is the start of a lose scene
    private Player playerLose;

    //This will load the 'Win' Scene when the player wins the level
    void winGame()
    {
        playerWon.gameObject.GetComponent<Text>().text = ("YOU WIN!");
    }

    //This waits for 5 seconds before it loads a scene
    private IEnumerator WaitForScene()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //This is for when the player loses
    public void gameOver()
    {
        Debug.Log("GAME OVER");
        SceneManager.LoadScene("Lose");
        //playerWon.gameObject.GetComponent<Text>().text = ("You Lose...");
    }

    //This is will pause the game and bring up the menu
    void pauseGame()
    {

    }

    //This will resume the current actions
    void resumeGame()
    {

    }

    //This brings up the game controls
    void gameControls()
    {

    }

    //This quits the game
    void quitGame()
    {

    }

    //This Starts the game
    void startGame()
    {

    }
}
