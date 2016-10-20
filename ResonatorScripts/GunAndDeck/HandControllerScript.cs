using UnityEngine;
using System.Collections;

/// <summary>
/// Last update: 17/10/16
/// Person/People: Nicola
/// GameObject to be on: both left and right controllers
/// GameObjects to link to: the gun and hand object parts
/// Other Scripts linked to: gun scripts, left hand script
/// Purpose of Script: see which had has the gun and send trigger values to scripts
/// To Do's: work out how to get the vibrations working again
/// </summary>

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class HandControllerScript : MonoBehaviour {

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;

    public GameObject handObj;

    public GameObject gunParts;
    public GameObject gun;
    public GameObject leftHandParts;

    AudioSource gunStartUpAudio;
    gunAnim gunAnimationScript;
    GunLevitationScript gunLevitation;

    [HideInInspector]
    public enum ControllerState
    {
        NONE,
        ASSIGNED
    }
    [HideInInspector]
    public ControllerState controllerState;

    [HideInInspector]
    public enum TriggerState
    {
        NONE,
        PRESSED,
        RELEASED
    }
    public TriggerState triggerState; //accessed by gun and left hand scripts

    void Awake ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        controllerState = ControllerState.NONE;
        triggerState = TriggerState.NONE;

        gunStartUpAudio = gun.GetComponent<AudioSource>();
        gunAnimationScript = gun.GetComponent<gunAnim>();
        gunLevitation = gunParts.GetComponent<GunLevitationScript>();
	}
	

	void FixedUpdate ()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);

        switch (controllerState)
        {
            case ControllerState.NONE:
                TriggerControls();
                if (gunParts.transform.parent.tag == "Hand A" && handObj.tag == "Hand B")
                {
                    //the gun has been picked up by the right hand
                    //assign the left hand parts to the left controller
                    leftHandParts.transform.parent = gameObject.transform;
                    leftHandParts.transform.localPosition = new Vector3(0, 0, 0);
                    leftHandParts.SetActive(true);
                    //align left hand health bar to controller
                    leftHandParts.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    controllerState = ControllerState.ASSIGNED;
                }
                break;
            case ControllerState.ASSIGNED:
                TriggerControls();
                break;
            default:
                controllerState = ControllerState.NONE;
                break;
        }
	   
	}

    void OnTriggerStay(Collider trig)
    {
        if(trig.tag == gunParts.tag)
        {
            if(gunParts.transform.parent == null)
            {
                gunLevitation.enabled = false;
                //can pick up the gun - have it so don't have the hold down the trigger to avoid confusion with shooting the gun using the trigger
                trig.transform.parent = gameObject.transform;
                //reset the local position to be at 0 so aligned to the controller in it's world position
                trig.transform.localPosition = new Vector3(0, 0, 0);
                //need to align the rotation to that of the controller as well
                trig.transform.localRotation = new Quaternion(0, 0, 0, 0);
                //activate gun animations
                gunAnimationScript.enabled = true;
                //hide the hand
                handObj.SetActive(false);
                controllerState = ControllerState.ASSIGNED;
                //activate the gun loading sound
                gunStartUpAudio.Play();
            }
        }
    }

    void TriggerControls()
    {
        switch (triggerState)
        {
            case TriggerState.NONE: //not being pressed or released state
                if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    triggerState = TriggerState.PRESSED;
                }
                break;
            case TriggerState.PRESSED:
                device.TriggerHapticPulse(600); //TODO: work out how to change the value to be either gun or left hand & vibrate to the bass
                if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
                {
                    triggerState = TriggerState.RELEASED;
                }
                break;
            case TriggerState.RELEASED:
                if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    triggerState = TriggerState.PRESSED;
                }
                else
                {
                    triggerState = TriggerState.NONE;
                }
                break;
            default:
                triggerState = TriggerState.NONE;
                break;
        }
    }
}
