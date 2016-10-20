using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Last update: 6/10/16
/// Person/People: Nicola
/// GameObject to be on: health bar object on hand not holding gun
/// GameObjects to link to: image sprites
/// Other Scripts linked to: GameStateScript
/// Purpose of Script: manage the images on the wrist
/// To Do's:
/// </summary>

public class WristUIScript : MonoBehaviour {

    [HideInInspector]
    public enum HealthBarState
    {
        NONE,
        FULL,
        HALF,
        QUARTER,
        EMPTY
    }
    [HideInInspector]
    public HealthBarState healthBarState;

    public GameObject[] healthBarImages; //4 of these

    void Awake ()
    {
        healthBarState = HealthBarState.FULL;
	}
	
	void Update ()
    {
        switch (healthBarState)
        {
            case HealthBarState.NONE:
                healthBarState = HealthBarState.FULL;
                break;
            case HealthBarState.FULL:
                if (!healthBarImages[0].activeSelf)
                {
                    healthBarImages[0].SetActive(true);
                }
                break;
            case HealthBarState.HALF:
                if (!healthBarImages[1].activeSelf)
                {
                    healthBarImages[1].SetActive(true);
                    healthBarImages[0].SetActive(false);
                }
                break;
            case HealthBarState.QUARTER:
                if (!healthBarImages[2].activeSelf)
                {
                    healthBarImages[2].SetActive(true);
                    healthBarImages[1].SetActive(false);
                }
                break;
            case HealthBarState.EMPTY:
                if (!healthBarImages[3].activeSelf)
                {
                    healthBarImages[3].SetActive(true);
                    healthBarImages[2].SetActive(false);
                }
                break;
            default:
                healthBarState = HealthBarState.NONE;
                break;
        }
	}
}
