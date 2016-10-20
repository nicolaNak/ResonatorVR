using UnityEngine;
using System.Collections;

/// <summary>
/// Last update: 16/10/16
/// Person/People: Liam/Nicola
/// GameObject to be on: projectile
/// GameObjects to link to: gun end and parent empty
/// Other Scripts linked to:
/// Purpose of Script: move projectile at given speed and curve
/// To Do's: 
/// </summary>

public class Projectile : MonoBehaviour
{

    //have variables set up outside of on enable otherwise will keep resetting them(?)
    public float bulletspeed;
    public float threshold;
    public GameObject gunEnd;
    public GameObject parent;
    public TrailRenderer trail;

    float timer = 0.0f;
    float timeResolution = 0.02f;
    float maxTime = 20.0f;

    Vector3 origin; // = new Vector3(0, 0, 0);
    Vector3 rotOrigin = new Vector3(0, 0, 0);
    Vector3 originScale = new Vector3 (0.05f, 0.05f, 0.05f);

    Vector3 velocity;
    Vector3 currentPos;

    Vector3 gravityVec = new Vector3(0.0f, -0.07f, 0.0f);

    //apperance
    //public Color colourTint;
    //public Color colourSphere;
    //public Material matSphere;
    //public Material matCube;

    //audio
    public AudioSource audioSource;

    //size
    public float maxSize = 1;
    public float minSize = 0.1f;
    public float rotateSpeed = 100;
    public float attack = 5;
    public float decay = 3;
    Vector3 newSize;
    Vector3 attckVec;
    Vector3 decayVec;

    //both of these on the game object
    private Effect effect;
    private VUThreshold vu;

    float offset;
    float changedOffset;
    bool hasOffset;

    //Movement0
    [Header("Movement type 0")]
    public float magnitude0 = 0.4f;
    public float frequancy0 = 25;
    Vector3 axis0;
    Vector3 pos0;


    //Movement1
    [Header("Movement type 1")]
    public float magnitude1 = 0.5f;
    public float frequancy1 = 8;
    Vector3 axis1;
    Vector3 pos1;

    //Movement2
    [Header("Movement type 2")]
    Vector3 pos2;
    public GameObject empty; //meant to be emtpy
    GameObject rotatePoint;

    //projectile type
    [HideInInspector]
    public enum ProjectileType
    {
        GREEN,
        PURPLE,
        RED
    }
    public ProjectileType projectileType; //set in inspector

    [HideInInspector]
    public enum ProjectileState
    {
        NONE,
        ALIVE,
        DEAD
    }
    //[HideInInspector]
    public ProjectileState projectileState;

    void Awake()
    {
        origin = transform.localPosition;

        hasOffset = false;

        vu = gameObject.GetComponent<VUThreshold>();


        projectileState = ProjectileState.NONE;

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        trail.time = 0.5f;
        transform.localScale = originScale;
        projectileState = ProjectileState.ALIVE;

        newSize = transform.localScale;
        attckVec = new Vector3(attack, attack, attack);
        decayVec = new Vector3(decay, decay, decay);

        offset = Random.Range(-3, 3);
        changedOffset = offset;
        hasOffset = false;

        vu = gameObject.GetComponent<VUThreshold>(); //safety catch
        effect = GetComponent<Effect>();

        audioSource.mute = false;

        //Movment0
        axis0 = transform.right;
        pos0 = transform.position;

        //Movement1       
        axis1 = transform.up;
        pos1 = transform.position;

        //Movement2
        pos2 = transform.position;
        rotatePoint = Instantiate(empty);

        transform.parent = null;
        
        currentPos = transform.position;
        transform.position = gunEnd.transform.position;
        transform.rotation = gunEnd.transform.rotation;

        velocity = transform.forward * bulletspeed;

        switch (projectileType)
        {
            case ProjectileType.GREEN:
                vu.threshold = -9.5f;
                break;
            case ProjectileType.PURPLE:
                vu.threshold = -9.5f;
                break;
            case ProjectileType.RED:
                vu.threshold = -9.5f;
                break;
        }
    }

    void FixedUpdate()
    {
        switch (projectileState)
        {
            case ProjectileState.NONE: //if reaches this and has been enabled then is alive
                projectileState = ProjectileState.ALIVE;
                break;
            case ProjectileState.ALIVE:
                currentPos = transform.position;

                transform.position += velocity * timeResolution;

                transform.Rotate(Vector3.right * 5);
                transform.Rotate(Vector3.up * 5);

                //velocity += gravityVec; //leaving this in so does fall in an arc

                //Specific Movment
                switch (projectileType)
                {
                    case ProjectileType.GREEN:
                      
                            pos0 += velocity * Time.deltaTime;
                            transform.position = pos0 + axis0 * Mathf.Sin((Time.time * frequancy0) + changedOffset) * magnitude0 * 0.5f;
                        
                    
                            //pos0 += velocity * Time.deltaTime;
                            //transform.position = pos0;
                        


                        break;
                    case ProjectileType.PURPLE:
                     
                            pos1 += velocity * Time.deltaTime;
                            transform.position = pos1 + axis1 * Mathf.Sin((Time.time * frequancy1) + changedOffset) * magnitude1;
                        
                       
                            //pos1 += velocity * Time.deltaTime;
                            //transform.position = pos1;
                        
                        break;
                    case ProjectileType.RED:
                        //rotatePoint.transform.position = pos2;              
                        transform.RotateAround(rotatePoint.transform.position, new Vector3(0, 0, 1), 2 * timeResolution);
                        //pos2 += velocity * Time.deltaTime;
                        //pos2 = new Vector3(transform.position.x, transform.position.y, pos2.z);
                        //transform.position = pos2;
                        break;

                }

                //Size
                transform.Rotate(rotateSpeed * Time.deltaTime, rotateSpeed * Time.deltaTime, rotateSpeed * Time.deltaTime);

                newSize = transform.localScale;

                if (vu.IsPlaying() && transform.localScale.x < maxSize)
                {
                    newSize += attckVec * Time.deltaTime;
                    
                }
                else if (!vu.IsPlaying() && transform.localScale.x > minSize)
                {
                    newSize -= decayVec * Time.deltaTime;
                  
                }

                if (transform.localScale.x > maxSize)
                {
                    transform.localScale = new Vector3(maxSize, maxSize, maxSize);
                }
                if (projectileType == ProjectileType.GREEN)
                {
                    trail.startWidth = newSize.x * 2;
                }

                if (projectileType == ProjectileType.PURPLE)
                {
                    trail.startWidth = newSize.x * 10;
                }
                

                transform.localScale = newSize;

                timer += timeResolution;
                if (timer >= maxTime) //has been alive for 20 seconds and not hit anything, safety catch
                {
                    projectileState = ProjectileState.DEAD; 
                }
                break;
            case ProjectileState.DEAD:
                transform.parent = parent.transform;
                transform.localPosition = Vector3.zero;
                transform.rotation = Quaternion.Euler(rotOrigin);
                transform.localScale = originScale;
                timer = 0.0f;
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
        
    }

    void OnCollisionEnter(Collision ground)
    {
        if (ground.collider.tag == "Environment")
        {
            projectileState = ProjectileState.DEAD;   
        }
    }
}
