using UnityEngine;
using System.Collections;

/// <summary>
/// Last update: 06/10/16
/// Person/People: Nicola
/// GameObject to be on: tutorial part parent
/// GameObjects to link to: enemies, point children
/// Other Scripts linked to: enemyNavigationScript, GameStateScript
/// Purpose of Script: to create the tutorial and run it
/// To Do's: 
/// </summary>

public class TutorialScript : MonoBehaviour {

    public GameObject[] enemiesObj;
    EnemyNavigationScript[] enemiesScript;

    public GameObject[] pointsObj;
    SpawnPointPathScript[] pointsScript;

    [HideInInspector]
    public int deadCount; //accessed by GameStateScript

    bool wait;
    float timeIncrements;
    float releaseTimer;
    
	void OnEnable ()
    {
        enemiesScript = new EnemyNavigationScript[enemiesObj.Length];
        for(int i = 0; i < enemiesScript.Length; i++)
        {
            enemiesScript[i] = enemiesObj[i].GetComponent<EnemyNavigationScript>();
        }

        pointsScript = new SpawnPointPathScript[pointsObj.Length];
        for(int i = 0; i < pointsScript.Length; i++)
        {
            pointsScript[i] = pointsObj[i].GetComponent<SpawnPointPathScript>();
        }

        deadCount = 0;

        wait = false;
        timeIncrements = 0.02f;
        releaseTimer = 10.0f;
	}
	

	void Update ()
    {
        //if not activated an enemy yet
        if (!enemiesObj[deadCount].activeSelf)
        {
            enemiesObj[deadCount].SetActive(true);
            enemiesScript[deadCount].points = pointsScript[deadCount].path;
            enemiesScript[deadCount].enemyNavState = EnemyNavigationScript.EnemyNavigationState.NEW_TRAVERSING;
            wait = true;
            //TODO: play audio to describe each enemy and way to change ammo
        }

        if (wait)
        {
            releaseTimer -= timeIncrements;
            if(releaseTimer <= 0)
            {
                releaseTimer = 10.0f;
                wait = false;
            }
        }
        else
        {
            //if enemy killed can set count to bring in next enemy
            if (enemiesScript[deadCount].enemyState == EnemyNavigationScript.EnemyState.DEAD)
            {
                if (enemiesObj[deadCount].activeSelf) //still active
                {
                    enemiesObj[deadCount].SetActive(false);
                    deadCount++;
                    return;
                }
            }
        }

        //to play the correct audio
        switch (deadCount)
        {
            case 0:
                //start plays
                //once start finishes first enemy instruction audio plays
                //once correct gesture drawn audio
                break;
            case 1:
                //second enemy instruction audio
                //once correct gesture drawn audio
                break;
            case 2:
                //third enemy instruction audio
                //once correct gesture drawn audio
                break;
            case 3:
                //need audio to explain to shoot with each gestures ammo type <-- no created yet
                //once the tank is dead play end of tutorial/start of game audio
                break;
            default:
                //no audio plays
                break;
        }
	}
}
