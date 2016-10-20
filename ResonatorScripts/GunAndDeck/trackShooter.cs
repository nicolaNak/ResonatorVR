using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

/// <summary>
/// Last update: 17/10/2016
/// Person/People: Liam/Nicola
/// GameObject to be on: A child object on the gun
/// Other Scripts linked to: 
/// GameObjects linked to: gun, gun end, right hand script, projectiles, gun reload sound obj, music measurement obj
/// Purpose of Script: Fire and manage projectiles on the beat
/// To Do's: set so can always shoot with the one ammo type/reload with the same type of ammo
/// </summary>

public class trackShooter : MonoBehaviour
{
    //trigger bool variables
    public GameObject handA;
    HandControllerScript gunHandScript;

    [HideInInspector]
    public enum ShootingState
    {
        NONE,
        SHOOTING,
        RELOADING
    }
    //[HideInInspector]
    public ShootingState shootingState;

    //projectile variables
    public List <GameObject> projectiles = new List<GameObject>();
    //public int projectileCount;
    //Liam's variables
    //Vector3 scale;
    //Vector3[] projectileNewSize;
    public GameObject gunEnd;
    //Vector3 multiplierSizeVec;
    //Vector3 maxSizeVec;
    //Vector3 decaySizeVec;
    float timeResolution;

    //materials & music variables
    public float newGlow;
    public Material matPerc;
    public Material matSynth;
    public Material matBass;
    public GameObject VUmeter;
    public GameObject gun;
    private Renderer ren;
    public VUThreshold VUclip;
    public ammoSelector selector;
    //public FPSAmmoSelector FPSselector;
    public Color colour;
    private Color baseColour;
    public bool playingClip;
    //public float multiplierSize;
    //public float decaySize;
    public float multiplierGlow;
    public float decayGlow;
    //public float maxSize;
    //public float minSize;
    public float maxGlow;
    public float minGlow;

    //playing music parts
    public GameObject gunReload;
    AudioSource gunReloadSound;

    void OnEnable()
    {
       
        gunHandScript = handA.GetComponent<HandControllerScript>();
       
        shootingState = ShootingState.NONE;

        //projectileCount = 0;
        //projectileNewSize = new Vector3[projectiles.Count];

        //scale = transform.localScale;
        //multiplierSizeVec = new Vector3(multiplierSize, multiplierSize, multiplierSize);
        //maxSizeVec = new Vector3(maxSize, maxSize, maxSize);
        //decaySizeVec = new Vector3(decaySize, decaySize, decaySize);
        timeResolution = 0.02f;

        VUclip = VUmeter.GetComponent<VUThreshold>();
        baseColour = colour;


        gunReloadSound = gunReload.GetComponent<AudioSource>();
        gunReloadSound.Play(); //start the gun reload sound
    }


    void FixedUpdate()
    {
        switch (shootingState)
        {
            case ShootingState.NONE:
                if (gunHandScript.triggerState == HandControllerScript.TriggerState.PRESSED)
                {
                    shootingState = ShootingState.SHOOTING;
                }
                break;
            case ShootingState.SHOOTING: //shoot the projectiles
                for (int i = 0; i < projectiles.Count; i++)
                {
                    if (!projectiles[i].activeSelf)
                    {
                        projectiles[i].SetActive(true);
                        shootingState = ShootingState.RELOADING;
                        break;
                    }
                    if (i >= (projectiles.Count - 1))
                    {
                        Debug.Log("All projectiles are active");
                        shootingState = ShootingState.RELOADING;
                    }
                }
                break;
            case ShootingState.RELOADING:
                if (gunHandScript.triggerState == HandControllerScript.TriggerState.RELEASED || gunHandScript.triggerState == HandControllerScript.TriggerState.NONE)
                {
                    shootingState = ShootingState.NONE; //resets so can shoot ammo again
                }
                break;
            default:
                shootingState = ShootingState.NONE;
                break;
        }

        // Emmision
        if (playingClip && colour.r < maxGlow)
        {
            colour += baseColour * Mathf.LinearToGammaSpace(multiplierGlow) * timeResolution;
        }
        if (!playingClip && colour.r > minGlow)
        {
            colour -= baseColour * Mathf.LinearToGammaSpace(decayGlow) * timeResolution;
        }
    }
}
