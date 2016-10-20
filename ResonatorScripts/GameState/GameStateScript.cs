using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Last update: 17/10/16
/// Person/People: Nicola
/// GameObject to be on: camera (eyes)
/// GameObjects to link to: tutorial Menus, left hand menu pieces, many game objects
/// Other Scripts linked to: menu scripts
/// Purpose of Script: no win, only lose.
/// To Do's:
/// </summary>

public class GameStateScript : MonoBehaviour {

    ////for tutorial state////
    public GameObject tutorialObj;
    TutorialScript tutorialScript;

    ////for start/end state////
    public GameObject logoObj;
    public Text finalScoreText;
    public GameObject startFloorObj;
    public GameObject blackConeObj2;
    public GameObject blackConeObj;
    ConeActivationScript blackConeScript;
    public GameObject gestureObj;
    bool gestureChanged;
    Edwon.VR.Gesture.GestureEventScript gestureScript;
    public GameObject overdriveInfoObj;
    public GameObject finalScoreObj;
    public Text restartText;
    public Text quitText;
    //to start the music once the gun has finished starting up
    public GameObject gun;
    AudioSource gunStartupSound;
    //hiding/showing the scene itself
    public GameObject scenePartsObj;
    //get controller states
    public GameObject controllerA;
    public GameObject controllerB;
    HandControllerScript controllerAScript;
    HandControllerScript controllerBScript;
        
    ////for running state////
    public GameObject damageTakenConeObj;
    ConeActivationScript damageTakenConeScript;
    //for suiciding out of the game
    public GameObject Suiciding;
    SuicideExitScript suicidingScript;
    bool hit;
    //for th gun ammo selector script
    public GameObject gunScriptObj;
    ammoSelector gunScript;
    //for UI on wrist - being moved to the deck
    public GameObject healthBarUI;
    WristUIScript healthBarScript;
    public Text killScoreText;
    public Text waveText;
    //for enemy waves
    public GameObject enemyBlackboardObj;
    EnemyPathBlackboardScript enemyBlackboardScript;      
    //audio activation <-- this will need to be changed according to Liam's setup
    public GameObject modulateObj;
    public GameObject hypersawObj;
    public GameObject overdriveObj;
    AudioSource modulateAudio;
    AudioSource hypersawAudio;
    AudioSource overdriveAudio;

    public enum GameState
    {
        START, //pick up the gun in this state
        TUTORIAL, //learn the mechanics in this state
        RUN, //the game itself
        END //game over, either by health hitting zero or exiting by suicide
    }
    public GameState gameState;

    //accessed by the exit script
    public float playerHealth;
    public float damage;

    public bool enemyAtPlayer; //accessed by the player point
    float enemyTimer;
    float timeIncrement;

    public bool tutorialFinished; //accessed by the start buttons
    public bool endOfGame;

    void Awake()
    {
        tutorialScript = tutorialObj.GetComponent<TutorialScript>();

        blackConeScript = blackConeObj.GetComponent<ConeActivationScript>();

        gestureScript = gestureObj.GetComponent<Edwon.VR.Gesture.GestureEventScript>();

        controllerAScript = controllerA.GetComponent<HandControllerScript>();

        controllerBScript = controllerB.GetComponent<HandControllerScript>();

        damageTakenConeScript = damageTakenConeObj.GetComponent<ConeActivationScript>();

        suicidingScript = Suiciding.GetComponent<SuicideExitScript>();
        hit = false;

        gunScript = gunScriptObj.GetComponent<ammoSelector>();

        healthBarScript = healthBarUI.GetComponent<WristUIScript>();

        enemyBlackboardScript = enemyBlackboardObj.GetComponent<EnemyPathBlackboardScript>();

        gameState = GameState.START;
        playerHealth = 100.0f;
        damage = 35.0f;

        enemyAtPlayer = false;
        enemyTimer = 2.5f;
        timeIncrement = 0.02f;

        tutorialFinished = false;
        endOfGame = false;
        gestureChanged = false;

        gunStartupSound = gun.GetComponent<AudioSource>();

        modulateAudio = modulateObj.GetComponent<AudioSource>();
        hypersawAudio = hypersawObj.GetComponent<AudioSource>();
        overdriveAudio = overdriveObj.GetComponent<AudioSource>();
         
    }

    void Update()
    {
        CheckGameState();
    }

    void CheckGameState()
    {
        switch (gameState)
        {
            case GameState.START:
                //start of game parts activated and black cone on player
                blackConeScript.coneState = ConeActivationScript.ConeState.VISIBLE;
                //have gun hovering and can pick it up with logo
                //once gun picked up (parented to the hand) start the gun load noises & introduction audio
                //fade out
                //check if gun has controller as parent so can start the music
                if(controllerAScript.controllerState == HandControllerScript.ControllerState.ASSIGNED || controllerBScript.controllerState == HandControllerScript.ControllerState.ASSIGNED)
                {
                    scenePartsObj.SetActive(true);
                    blackConeScript.coneState = ConeActivationScript.ConeState.FADE_OUT;
                    blackConeObj2.SetActive(false); //TODO: make this fade as well
                    startFloorObj.SetActive(false);
                    logoObj.SetActive(false);
                    gunStartupSound.Play();
                    //move on to tutorial state
                    gameState = GameState.TUTORIAL;
                }
                break;
            case GameState.TUTORIAL:
                tutorialObj.SetActive(true);
                //if (!gunStartupSound.isPlaying)
                //{
                //    modulateAudio.Play();
                //    hypersawAudio.Play();
                //    overdriveAudio.Play();
                //}
                if (!Suiciding.activeSelf) //turn on exit/end game options
                {
                    Suiciding.SetActive(true);
                }
                if (!gunScript.enabled) //active ammo selector script
                {
                    gunScript.enabled = true;
                }
                //TODO: play tutorial instruction audio
                //check if finished tutorial 
                if(tutorialScript.deadCount >= 4)
                {
                    tutorialObj.SetActive(false); //remove tutorial as finished
                    //start running the enemy waves here
                    enemyBlackboardScript.enabled = true; //enable the script now tutorial finished
                    enemyBlackboardScript.runningGame = true;
                    gameState = GameState.RUN;
                }
                //TODO: check if want to skip tutorial
                //check if shot themselves in the head during tutorial (just in case they want to exit)
                if (suicidingScript.suicideState == SuicideExitScript.SuicideState.FIRST_HIT && !hit)
                {
                    playerHealth -= damage;
                    damageTakenConeScript.coneState = ConeActivationScript.ConeState.VISIBLE;
                    hit = true;
                }
                //check if want to exit through the suicide exit script
                if (suicidingScript.suicideState == SuicideExitScript.SuicideState.SECOND_HIT)
                {
                    gameState = GameState.END;
                }
                //if have timed out after the first hit
                if(suicidingScript.suicideState == SuicideExitScript.SuicideState.NONE && hit)
                {
                    hit = false;
                }
                break;
            case GameState.RUN:
                if (!healthBarUI.activeSelf)
                {
                    healthBarUI.SetActive(true);
                }
                if (playerHealth <= 0.0f)
                {
                    gameState = GameState.END;
                    return;
                }
                if(playerHealth > 30.0f && playerHealth <= 65.0f)
                {
                    healthBarScript.healthBarState = WristUIScript.HealthBarState.QUARTER;
                }
                if(playerHealth == 30.0f)
                {
                    healthBarScript.healthBarState = WristUIScript.HealthBarState.HALF;
                }
                if (enemyAtPlayer)
                {
                    enemyTimer -= timeIncrement;
                    if (enemyTimer <= 0.0f)
                    {
                        playerHealth -= damage;
                        damageTakenConeScript.coneState = ConeActivationScript.ConeState.VISIBLE;
                        enemyTimer = 2.5f;
                        enemyAtPlayer = false;
                        Debug.Log("Enemy attacked player");
                    }
                }
                //check if shot themselves in the head
                if(suicidingScript.suicideState == SuicideExitScript.SuicideState.FIRST_HIT && !hit)
                {
                    playerHealth -= damage;
                    damageTakenConeScript.coneState = ConeActivationScript.ConeState.VISIBLE;
                    hit = true;
                }
                //check if want to exit through the suicide exit script
                if(suicidingScript.suicideState == SuicideExitScript.SuicideState.SECOND_HIT)
                {
                    gameState = GameState.END;
                }
                //if have timed out after the first hit
                if (suicidingScript.suicideState == SuicideExitScript.SuicideState.NONE && hit)
                {
                    hit = false;
                }
                break;
            case GameState.END:
                blackConeScript.coneState = ConeActivationScript.ConeState.FADE_IN;
                blackConeObj2.SetActive(true); //TODO: make fade in
                startFloorObj.SetActive(true);
                logoObj.SetActive(false);
                scenePartsObj.SetActive(false);
                overdriveObj.SetActive(false);
                finalScoreObj.SetActive(true);
                restartText.enabled = true;
                quitText.enabled = true;
                healthBarScript.healthBarState = WristUIScript.HealthBarState.EMPTY;
                waveText.text = (enemyBlackboardScript.waveCount - 1).ToString();
                float finalScore = enemyBlackboardScript.killScore * (float)enemyBlackboardScript.waveCount;
                finalScoreText.enabled = true;
                finalScoreText.text = finalScore.ToString();
                //for enemies so they are removed
                enemyBlackboardScript.runningGame = false;
                enemyBlackboardScript.enemyWaveState = EnemyPathBlackboardScript.EnemyWaveState.STATE_NONE;
                //end state wth gestures to choose if want to restart or exit
                Edwon.VR.Gesture.GestureEventScript.GestureState currentState = gestureScript.gestureState;
                if (!gestureChanged)
                {
                    gestureScript.gestureState = Edwon.VR.Gesture.GestureEventScript.GestureState.ARC; //resets to the gesture not required to end/restart the game
                    gestureChanged = true; 
                }
                if (gestureScript.gestureState == Edwon.VR.Gesture.GestureEventScript.GestureState.CIRCLE && gestureChanged)
                {
                    //restart the game
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                if(gestureScript.gestureState == Edwon.VR.Gesture.GestureEventScript.GestureState.SQUIGGLE_Z && gestureChanged)
                {
                    //end the game completely
                    Application.Quit();
                    Debug.Log("Quit the game - cannot quit in debug this is normal");
                }
                break;
            default: //no default state
                break;
        }
    }

    void OnTriggerStay(Collider trig)
    {
        if (trig.tag == "Enemy" && !enemyAtPlayer)
        {
            enemyAtPlayer = true;
        }
    }

    void OnTriggerExit(Collider trig) //in case enemy killed when touching player
    {
        if (trig.tag == "Enemy" && enemyAtPlayer)
        {
            enemyAtPlayer = false;
        }
    }
}
