using UnityEngine;
using System.Collections;

/// <summary>
/// Last update: 16/10/2016
/// Person/People: Liam/Nicola (
/// GameObject to be on: gun itself
/// Other Scripts linked to: controller script, gesture script, used by many other scripts
/// Purpose of Script: to manage which ammo is activated and can be shot out of the gun
/// To Do's: 
/// </summary>

public class ammoSelector : MonoBehaviour
{
    public GameObject gestureHandParts;
    public GameObject handA;
    public GameObject handB;
    HandControllerScript gestureHandScript;

    //for detecting gestures
    public GameObject gestureObj;
    Edwon.VR.Gesture.GestureEventScript gestureScript;

    public trackShooter[] trackScripts;
    public AudioSource [] sources;
    public gunAnim anim;
    public int currentAmmunition; //use this is a new way, used in material change on gun script

    [HideInInspector]
    public enum TrackState //states based on colour do have actual names
    {
        NONE,
        GREEN,  //Modulate
        PURPLE, //HyperSaw
        RED     //OverDrive
    }
    //[HideInInspector]
    public TrackState trackState;

    void Awake()
    {
        gestureScript = gestureObj.GetComponent<Edwon.VR.Gesture.GestureEventScript>();

        currentAmmunition = -1;

        for(int i = 0; i < trackScripts.Length; i++)
        {
            trackScripts[i].enabled = false;
        }

        trackState = TrackState.NONE; 
    }

    void OnEnable()
    {
        if(gestureHandScript == null)
        {
            //find the controller the gesture hand is attached to
            if (gestureHandParts.transform.parent.tag == handA.tag)
            {
                //get access to the trigger state in the script
                gestureHandScript = handA.GetComponent<HandControllerScript>();
            }
            if(gestureHandParts.transform.parent.tag == handB.tag)
            {
                //get access to the trigger state in the script
                gestureHandScript = handB.GetComponent<HandControllerScript>();
            }
        }

        //put here as safety catch
        gestureScript = gestureObj.GetComponent<Edwon.VR.Gesture.GestureEventScript>();

        currentAmmunition = -1;

        for (int i = 0; i < trackScripts.Length; i++)
        {
            trackScripts[i].enabled = false;
        }

        trackState = TrackState.NONE;
    }

    void Update()
    {
        
        CheckGesture(); //set trackState in here

        switch (trackState)
        {
            case TrackState.NONE: //at start of process will be like this
                break;
            case TrackState.GREEN:
               
                    trackScripts[0].enabled = true;
                    trackScripts[1].enabled = false;
                    trackScripts[2].enabled = false;
                    sources[0].mute = false;
                    sources[1].mute = true;
                    sources[2].mute = true;
                
                currentAmmunition = 0;
                break;
            case TrackState.PURPLE:
                if (!trackScripts[1].enabled)
                {
                    trackScripts[0].enabled = false;
                    trackScripts[1].enabled = true;
                    trackScripts[2].enabled = false;
                    sources[0].mute = true;
                    sources[1].mute = false;
                    sources[2].mute = true;
                }
                currentAmmunition = 1;
                break;
            case TrackState.RED:
                if (!trackScripts[2].enabled)
                {
                    trackScripts[0].enabled = false;
                    trackScripts[1].enabled = false;
                    trackScripts[2].enabled = true;
                    sources[0].mute = true;
                    sources[1].mute = true;
                    sources[2].mute = false;
                }
                currentAmmunition = 2;
                break; 
            default:
                trackState = TrackState.NONE;
                break;
        }
        switch (currentAmmunition) //resets the track scripts back
        {
            case -1:
                for (int i = 0; i < trackScripts.Length; i++)
                {
                    trackScripts[i].enabled = false;
                }
                break;
            case 0:
                trackScripts[1].enabled = false;
                trackScripts[2].enabled = false;
                break;
            case 1:
                trackScripts[0].enabled = false;
                trackScripts[2].enabled = false;
                break;
            case 2:
                trackScripts[0].enabled = false;
                trackScripts[1].enabled = false;
                break;
            default:
                currentAmmunition = -1;
                break;
        }
    }

    void CheckGesture()
    {
        switch (gestureScript.gestureState)
        {
            case Edwon.VR.Gesture.GestureEventScript.GestureState.NONE:
                trackState = TrackState.NONE;
                break;
            case Edwon.VR.Gesture.GestureEventScript.GestureState.CIRCLE:
                trackState = TrackState.GREEN;
                break;
            case Edwon.VR.Gesture.GestureEventScript.GestureState.ARC:
                trackState = TrackState.RED;
                break;
            case Edwon.VR.Gesture.GestureEventScript.GestureState.SQUIGGLE_Z:
                trackState = TrackState.PURPLE;
                break;
            default:
                trackState = TrackState.NONE;
                break;
        }
    }
}
