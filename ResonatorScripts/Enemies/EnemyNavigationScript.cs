using UnityEngine;
using System.Collections;

/// <summary>
/// Last update: 17/10/16
/// Person/People: Nicola
/// GameObject to be on: each enemy
/// Other Scripts linked to: tutorial script, gamestate script
/// Other GameObjects linked to: 
/// Purpose of Script: the enemy can follow a set path of points along the navmesh
/// from a set spawning point and controls enemy health
/// To Do's: pull out getcomponent on the animation call
/// </summary>

public class EnemyNavigationScript : MonoBehaviour {

    //health/ game state stuff//
    //values given in the scene
    public float health;
    public float modulateDamage;
    public float hypersawDamage;
    public float overdriveDamage;

    float storedHealth;

    [HideInInspector]
    public GameObject[] points;

	Vector3 origin;
	int destPoint;
	NavMeshAgent agent;
    public GameObject enemyMeshObj;
    SkinnedMeshRenderer enemyMesh;

	float pointTimer;
	float maxPointTime;
    float timeIncrement;

    //animator
    Animator enemyAnim; //on the same game object
    int attackHash;
    int dyingHash;
    public GameObject ghostObj;
    public GameObject ghostAnimObj;
    Animator ghostAnim;
    bool dying;
    bool ghosting;

    [HideInInspector]
    public bool inTutorial;

    [HideInInspector]
    public enum EnemyState
    {
        NONE,
        ALIVE,
        DYING,
        DEAD
    }
    //[HideInInspector]
    public EnemyState enemyState;

    [HideInInspector]
    public enum EnemyNavigationState
    {
        NO_PATH,
        NEW_TRAVERSING,
        TRAVERSING,
        AT_POINT,
        AT_DESTINATION
    }
    //[HideInInspector]
    public EnemyNavigationState enemyNavState;

    void Awake()
    {
        points = new GameObject[0]; //set this to nothing to start so if never assigned can be caught

        origin = transform.position;

        destPoint = 0;

        agent = GetComponent<NavMeshAgent>();

        enemyMesh = enemyMeshObj.GetComponent<SkinnedMeshRenderer>();
        enemyMesh.enabled = false;

        pointTimer = 0.0f;
        maxPointTime = 5.0f;
        timeIncrement = 0.02f; //this is set because normally using fixed update, not sure need it but left it in

        enemyAnim = GetComponent<Animator>();
        attackHash = Animator.StringToHash("AttackBeak");
        dyingHash = Animator.StringToHash("DeathFallEnd");
        ghostAnim = ghostAnimObj.GetComponent<Animator>();
        dying = false;
        ghosting = false;

        enemyState = EnemyState.NONE;
        storedHealth = health; //store this as lost when disabled/reenabled
        inTutorial = false;

        enemyNavState = EnemyNavigationState.NO_PATH;
    }

    void OnEnable ()
	{
        points = new GameObject[0]; //set this to nothing to start so if never assigned can be caught

		origin = transform.position;

		destPoint = 0;

		agent = GetComponent<NavMeshAgent>();

		pointTimer = 0.0f;
		maxPointTime = 5.0f;
        timeIncrement = 0.02f; //this is set because normally using fixed update, not sure need it but left it in

        enemyState = EnemyState.ALIVE;
        health = storedHealth;
        inTutorial = false;

        enemyAnim = GetComponent<Animator>();
        attackHash = Animator.StringToHash("AttackBeak");
        dyingHash = Animator.StringToHash("DeathFallEnd");
        ghostAnim = ghostAnimObj.GetComponent<Animator>();
        ghostAnimObj.SetActive(false);
        dying = false;
        ghosting = false;

        enemyNavState = EnemyNavigationState.NO_PATH;
	}

	//this must be update, not fixed update as fixed update causes if statement to be accessed
	//3 times before actioning
	void Update () 
	{
        switch (enemyState)
        {
            case EnemyState.NONE:
                break;
            case EnemyState.ALIVE:
                if (health <= 0.0f)
                {
                    enemyState = EnemyState.DYING;
                    //enemyAnim.Stop();
                    break;
                }
                else
                {
                    Navigation();
                    break;
                }
            case EnemyState.DYING:
                enemyAnim.SetTrigger(dyingHash);
                if (enemyAnim.GetCurrentAnimatorStateInfo(0).shortNameHash == dyingHash && !dying)
                {
                    dying = true;
                    //if (!ghostAnimObj.activeSelf)
                    //{
                    //    ghostAnimObj.SetActive(true);
                    //    Debug.Log("Set ghost obj to be active");
                    //}
                }
                if (dying && !ghosting)
                {
                    if(enemyAnim.GetCurrentAnimatorStateInfo(0).length > 1.25f)
                    {
                        //ended the dying animation, can ghost up now
                        enemyMesh.enabled = false;
                        ghosting = true;
                    }
                }
                if (ghosting)
                {
                    //if (ghostAnim.GetCurrentAnimatorStateInfo(0).length > 1.25f)
                    //{
                        //finished the ghost anim
                        dying = false;
                        ghosting = false;
                        enemyState = EnemyState.DEAD;
                    //}
                }
                break;
            case EnemyState.DEAD:
                agent.Warp(origin);
                destPoint = 0;
                agent.Stop();
                break;
            }     
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Modulate")
        {
            other.GetComponent<AudioSource>().Play();
            health -= modulateDamage;
            other.gameObject.GetComponent<Projectile>().projectileState = Projectile.ProjectileState.DEAD;
        }
        if (other.tag == "Hypersaw")
        {
            other.GetComponent<AudioSource>().Play();
            health -= hypersawDamage;
            other.gameObject.GetComponent<Projectile>().projectileState = Projectile.ProjectileState.DEAD;
        }
        if (other.tag == "Overdrive")
        {
            other.GetComponent<AudioSource>().Play();
            health -= overdriveDamage;
            other.gameObject.GetComponent<Projectile>().projectileState = Projectile.ProjectileState.DEAD;
        }
    }

    void Navigation()
    {
        switch (enemyNavState)
        {
            case EnemyNavigationState.NO_PATH:
                //no path assigned yet - this is fine
                break;
            case EnemyNavigationState.NEW_TRAVERSING:
                if(destPoint == 0) //first point
                {
                    //warp to the spawn point
                    agent.Warp(points[destPoint].transform.position);
                    enemyMesh.enabled = true;
                    //go find the next point to begin the path traversal
                    enemyNavState = EnemyNavigationState.AT_POINT;
                }
               else
                {
                    enemyNavState = EnemyNavigationState.TRAVERSING;
                }
                break;
            case EnemyNavigationState.TRAVERSING:
                if (agent.remainingDistance <= 0.5f)
                {
                    enemyNavState = EnemyNavigationState.AT_POINT;
                }
                break;
            case EnemyNavigationState.AT_POINT:
                if (points.Length == 0) //catch if no path assigned
                {
                    enemyNavState = EnemyNavigationState.NO_PATH;
                }
                if (destPoint >= (points.Length - 1)) //have reached player
                {
                    enemyNavState = EnemyNavigationState.AT_DESTINATION;
                }
                else
                {
                    GotoNextPoint();
                    enemyNavState = EnemyNavigationState.NEW_TRAVERSING;
                }
                break;
            case EnemyNavigationState.AT_DESTINATION:
                if (!inTutorial)
                {
                    //attacking time!
                    agent.Stop();
                    enemyAnim.SetTrigger(attackHash);
                    Debug.Log("Playing the attack sound");
                    gameObject.GetComponent<AudioSource>().Play();
                }
                else //in tutorial so won't attack the player
                {
                    agent.Stop();
                    Debug.Log("Playing the attack sound");
                    gameObject.GetComponent<AudioSource>().Play();
                }
                break;
            default:
                enemyNavState = EnemyNavigationState.NO_PATH;
                break;
        }
    }

	void GotoNextPoint()
	{
		destPoint++;

        if(destPoint >= points.Length) //to stop out of array bounds error and wrong travel
        {
            Debug.Log("Enemy path points array out of bounds catch called");
            enemyNavState = EnemyNavigationState.AT_DESTINATION;
            return;
        }

		// Set the agent to go to the currently selected destination.
		agent.SetDestination(points[destPoint].transform.position);		
	}
}
