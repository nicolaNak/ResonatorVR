using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Last update: 6/10/16
/// Person/People: Nicola
/// GameObject to be on: End Game Scene Parts
/// GameObjects to link to: 
/// Other Scripts linked to: 
/// Purpose of Script: to allow player to choose if want to restart or exit the game at end
/// To Do's:
/// </summary>

public class EndGameScript : MonoBehaviour {

    [HideInInspector]
    public enum EndGameState
    {
        NONE, 
        RESTART, //draw a circle
        EXIT //draw a cross
    }
    [HideInInspector]
    public EndGameState endGameState; //accessed by GameStateScript

    void Awake ()
    {
        endGameState = EndGameState.NONE;
	}
	
    void OnEnable()
    {
        endGameState = EndGameState.NONE;
    }

	void Update ()
    {
        switch (endGameState)
        {
            case EndGameState.NONE:
                //this is before a choice has been made
                //check what choice has been made with gestures
                //healthbar on wrist needs to be set to empty in GameStateScript
                break;
            case EndGameState.RESTART:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case EndGameState.EXIT:
                Debug.Log("Quit the game - not able to quit in editor (this is normal)");
                Application.Quit();
                break;
            default:
                endGameState = EndGameState.NONE;
                break;
        }
	}
}
