using UnityEngine;
using System.Collections;

/// <summary>
/// Last update: 6/10/16
/// Person/People: Nicola
/// GameObject to be on: the cone of damage and start/end
/// GameObjects to link to: 
/// Other Scripts linked to: GameStateScript
/// Purpose of Script: turn the cone on, then fade it away when called
/// To Do's:
/// </summary>

public class ConeActivationScript : MonoBehaviour {

    [HideInInspector]
    public enum ConeState
    {
        NONE,
        FADE_IN,
        VISIBLE,
        FADE_OUT,
        INVISIBLE
    }
    //[HideInInspector]
    public ConeState coneState;  //accessed by GameStateScript

    public bool damageCone; //changed in the inspector

    public GameObject darkFloorObj;

    public GameObject cone;

    Material coneMat;
    Color coneFadeColour;
    float fadeAmount;
    float timeIncrement;

	void Awake ()
    {
        if (damageCone)
        {
            coneState = ConeState.INVISIBLE;
        }
        else
        {
            coneState = ConeState.NONE;
        }
        coneMat = cone.GetComponent<Renderer>().material;
        coneFadeColour = coneMat.color;
        fadeAmount = 0.5f;
        timeIncrement = 0.02f;
    }
	
	void Update ()
    {
        switch (coneState)
        {
            case ConeState.NONE:
                coneState = ConeState.INVISIBLE;
                break;
            case ConeState.FADE_IN:
                if (!cone.activeSelf) //make visible
                {
                    cone.SetActive(true);
                    darkFloorObj.SetActive(true);
                }
                //fade in the material
                coneFadeColour.a += fadeAmount * timeIncrement;
                coneMat.color = coneFadeColour;
                if (coneMat.color.a >= 1.0f)
                {
                    coneState = ConeState.VISIBLE;
                }
                break;
            case ConeState.VISIBLE:
                if (!cone.activeSelf) //make visible if skipped FADE_IN state
                {
                    cone.SetActive(true);
                    if (!damageCone)
                    {
                        darkFloorObj.SetActive(true);
                    } 
                }
                //check if can fade yet
                if (damageCone)
                {
                    coneState = ConeState.FADE_OUT;
                }
                //the start/end cone is changed to FADE state in GameStateScript
                break;
            case ConeState.FADE_OUT:
                //countdown the opactiy to 0
                coneFadeColour.a -= fadeAmount * timeIncrement;
                coneMat.color = coneFadeColour;
                if(coneMat.color.a <= 0)
                {
                    coneState = ConeState.INVISIBLE;
                }
                //once at zero change to invisible state
                break;
            case ConeState.INVISIBLE:
                if (damageCone)
                {
                    coneFadeColour.a = 1.0f;
                    coneMat.color = coneFadeColour; //reset the colour to show for the next time
                }
                cone.SetActive(false); //hide the cone
                darkFloorObj.SetActive(false);
                //now can start the next game state
                break;
            default:
                coneState = ConeState.NONE;
                break;
        }
	}
}
