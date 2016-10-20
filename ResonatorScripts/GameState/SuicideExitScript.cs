using UnityEngine;
using System.Collections;

/// <summary>
/// Last update: 9/10/16
/// Person/People: Nicola
/// GameObject to be on: head on camera rig which needs a sphere collider 
/// GameObjects to link to: Gun End, textUI for end of gun
/// Other Scripts linked to: GameStateScript
/// Purpose of Script: tell if projectile has hit head, go to end game state
/// To Do's: 
/// </summary>

public class SuicideExitScript : MonoBehaviour {

    [HideInInspector]
    public enum SuicideState
    {
        NONE,
        FIRST_HIT,
        SECOND_HIT,
    }
    //[HideInInspector]
    public SuicideState suicideState; //accessed by GameStateScript

    public GameObject exitUI; //these both start as inactive for the start state of the game
    public GameObject areYouSureUI;

    bool hitWait;
    float hitTimer;
    float noResponseTimer;
    float timeIncrements;

    void Awake ()
    {
        suicideState = SuicideState.NONE;

        hitWait = false;
        hitTimer = 1.5f;
        noResponseTimer = 10.0f;
        timeIncrements = 0.02f;
	}
	
	void Update ()
    {
        switch (suicideState)
        {
            case SuicideState.NONE:
                //show the text "Time to end it all? Shoot Yourself"
                if (!exitUI.activeSelf)
                {
                    exitUI.SetActive(true);
                }
                break;
            case SuicideState.FIRST_HIT:
                //show the text "Are You Sure? Shoot Again"
                if (exitUI.activeSelf || !areYouSureUI.activeSelf)
                {
                    exitUI.SetActive(false);
                    areYouSureUI.SetActive(true);
                }
                //run timer, if do not shoot in time revert to NONE state
                noResponseTimer -= timeIncrements;
                if(noResponseTimer <= 0.0f)
                {
                    noResponseTimer = 10.0f;
                    exitUI.SetActive(true);
                    areYouSureUI.SetActive(false);
                    suicideState = SuicideState.NONE;
                }
                if (hitWait) //make sure don't get multiple hits from the one shot
                {
                    hitTimer -= timeIncrements;
                    if(hitTimer <= 0.0f)
                    {
                        hitTimer = 1.5f;
                        hitWait = false;
                    }
                }
                break;
            case SuicideState.SECOND_HIT:
                //go to end game - this is checked for in the game running state
                if(exitUI.activeSelf || areYouSureUI.activeSelf)
                {
                    exitUI.SetActive(false);
                    areYouSureUI.SetActive(false);
                }
                break;
        }
	}

    void OnTriggerEnter(Collider trig)
    {
        if (!hitWait)
        {
            if (trig.tag == "Modulate" || trig.tag == "Overdrive" || trig.tag == "Hypersaw")
            {
                if(suicideState == SuicideState.NONE)
                {
                    suicideState = SuicideState.FIRST_HIT;
                    hitWait = true;
                    return;
                }
                if(suicideState == SuicideState.FIRST_HIT)
                {
                    suicideState = SuicideState.SECOND_HIT;
                    return;
                }
            }
        }
    }
}
