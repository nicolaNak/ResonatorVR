using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Last update: 16/10/16
/// Person/People: Nicola
/// GameObject to be on: enemy nav points parent
/// GameObjects to link to: spawnpoint parent, enemies parents
/// Other Scripts linked to: enemyNavigationScript, GameStateScript, SpawnPointPathScript
/// Purpose of Script: to give arrays of points and spawn points to each enemy
/// To Do's: 
/// </summary>

public class EnemyPathBlackboardScript : MonoBehaviour {

    public GameObject spawnPointParent;
    SpawnPointPathScript[] spawnPointsScript;

    public GameObject unwingedEnemyParent;
    GameObject[] unwingedEnemies;
    public GameObject wingedEnemyParent;
    GameObject[] wingedEnemies;
    public GameObject sharkEnemyParent;
    GameObject[] sharkEnemies;
    public GameObject tankEnemyParent;
    GameObject[] tankEnemies;

    int totalPaths;
    int totalEnemies;

    int enemyDeadCount;
    public int maxEnemies;// for debugging

    EnemyNavigationScript[] enemyInfo;
    bool[] activeStatus;

    public float waveTimer; //for setting in editor
    float timeIncrements;

    //GameState - accessed by GameStateScript//
    public bool runningGame;

    //Score - accessed by GameStateScript//
    public float killScore;
    public uint waveCount;

    //UI display
    public Text waveText;
    public Text killScoreText;

    [HideInInspector]
    public enum EnemyWaveState
    {
        STATE_NONE, //in intro/tutorial mode
        WAVE_LOADING,
        WAVE_RUNNING,
        WAVE_FINISHED
    }
    public EnemyWaveState enemyWaveState;

    void Awake()
    {
        //spawn point array init//
        spawnPointsScript = new SpawnPointPathScript[spawnPointParent.transform.childCount];
        for (int i = 0; i < spawnPointsScript.Length; i++)
        {
            spawnPointsScript[i] = spawnPointParent.transform.GetChild(i).GetComponent<SpawnPointPathScript>();
        }
        //enemies arrays init//
        unwingedEnemies = new GameObject[unwingedEnemyParent.transform.childCount];
        for (int i = 0; i < unwingedEnemies.Length; i++)
        {
            unwingedEnemies[i] = unwingedEnemyParent.transform.GetChild(i).gameObject;
        }
        wingedEnemies = new GameObject[wingedEnemyParent.transform.childCount];
        for (int i = 0; i < wingedEnemies.Length; i++)
        {
            wingedEnemies[i] = wingedEnemyParent.transform.GetChild(i).gameObject;
        }
        sharkEnemies = new GameObject[sharkEnemyParent.transform.childCount];
        for (int i = 0; i < sharkEnemies.Length; i++)
        {
            sharkEnemies[i] = sharkEnemyParent.transform.GetChild(i).gameObject;
        }
        tankEnemies = new GameObject[tankEnemyParent.transform.childCount];
        for (int i = 0; i < tankEnemies.Length; i++)
        {
            tankEnemies[i] = tankEnemyParent.transform.GetChild(i).gameObject;
        }

        totalPaths = spawnPointParent.transform.childCount; //used as check for the maxEnemies value so does not go over amount
        totalEnemies = unwingedEnemies.Length + wingedEnemies.Length
           + sharkEnemies.Length + tankEnemies.Length;

        enemyDeadCount = 0;
        maxEnemies = 5; //starting value, will iterate up

        activeStatus = new bool[totalEnemies];
        enemyInfo = new EnemyNavigationScript[totalEnemies];
        int inArrayCount = 0;
        for (int i = 0; i < totalEnemies; i++)
        {
            activeStatus[i] = false;
            if (i < unwingedEnemies.Length) //unwinged first
            {
                enemyInfo[i] = unwingedEnemies[i].GetComponent<EnemyNavigationScript>();
                continue;
            }
            if (i >= unwingedEnemies.Length && i < (unwingedEnemies.Length + wingedEnemies.Length)) //winged second
            {
                inArrayCount = i - unwingedEnemies.Length;
                enemyInfo[i] = wingedEnemies[inArrayCount].GetComponent<EnemyNavigationScript>();  
                continue;
            }
            if (i >= (unwingedEnemies.Length + wingedEnemies.Length) && i < (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length)) //shark third
            {
                inArrayCount = i - (unwingedEnemies.Length + wingedEnemies.Length);
                enemyInfo[i] = sharkEnemies[inArrayCount].GetComponent<EnemyNavigationScript>(); //out of array exception
                continue;
            }
            if (i >= (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length) && i < (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length + tankEnemies.Length)) //tank last
            {
                inArrayCount = i - (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length);
                enemyInfo[i] = tankEnemies[inArrayCount].GetComponent<EnemyNavigationScript>();
                continue;
            }
        }

        waveTimer = 15.0f;
        timeIncrements = 0.02f;

        runningGame = true; //**Changed for FPS scene

        killScore = 0;
        waveCount = 1;

        enemyWaveState = EnemyWaveState.STATE_NONE;
    }

    void OnEnable()
    {
        //spawn point array init//
        spawnPointsScript = new SpawnPointPathScript[spawnPointParent.transform.childCount];
        for (int i = 0; i < spawnPointsScript.Length; i++)
        {
            spawnPointsScript[i] = spawnPointParent.transform.GetChild(i).GetComponent<SpawnPointPathScript>();
        }
        //enemies arrays init//
        unwingedEnemies = new GameObject[unwingedEnemyParent.transform.childCount];
        for (int i = 0; i < unwingedEnemies.Length; i++)
        {
            unwingedEnemies[i] = unwingedEnemyParent.transform.GetChild(i).gameObject;
        }
        wingedEnemies = new GameObject[wingedEnemyParent.transform.childCount];
        for (int i = 0; i < wingedEnemies.Length; i++)
        {
            wingedEnemies[i] = wingedEnemyParent.transform.GetChild(i).gameObject;
        }
        sharkEnemies = new GameObject[sharkEnemyParent.transform.childCount];
        for (int i = 0; i < sharkEnemies.Length; i++)
        {
            sharkEnemies[i] = sharkEnemyParent.transform.GetChild(i).gameObject;
        }
        tankEnemies = new GameObject[tankEnemyParent.transform.childCount];
        for (int i = 0; i < tankEnemies.Length; i++)
        {
            tankEnemies[i] = tankEnemyParent.transform.GetChild(i).gameObject;
        }

        totalPaths = spawnPointParent.transform.childCount; //used as check for the maxEnemies value so does not go over amount
        totalEnemies = unwingedEnemies.Length + wingedEnemies.Length
           + sharkEnemies.Length + tankEnemies.Length;

        enemyDeadCount = 0;
        maxEnemies = 5; //starting value, will iterate up

        activeStatus = new bool[totalEnemies];
        enemyInfo = new EnemyNavigationScript[totalEnemies];
        int inArrayCount = 0;
        for (int i = 0; i < totalEnemies; i++)
        {
            activeStatus[i] = false;
            if (i < unwingedEnemies.Length) //unwinged first
            {
                enemyInfo[i] = unwingedEnemies[i].GetComponent<EnemyNavigationScript>();
                continue;
            }
            if (i >= unwingedEnemies.Length && i < (unwingedEnemies.Length + wingedEnemies.Length)) //winged second
            {
                inArrayCount = i - unwingedEnemies.Length;
                enemyInfo[i] = wingedEnemies[inArrayCount].GetComponent<EnemyNavigationScript>();
                continue;
            }
            if (i >= (unwingedEnemies.Length + wingedEnemies.Length) && i < (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length)) //shark third
            {
                inArrayCount = i - (unwingedEnemies.Length + wingedEnemies.Length);
                enemyInfo[i] = sharkEnemies[inArrayCount].GetComponent<EnemyNavigationScript>(); //out of array exception
                continue;
            }
            if (i >= (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length) && i < (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length + tankEnemies.Length)) //tank last
            {
                inArrayCount = i - (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length);
                enemyInfo[i] = tankEnemies[inArrayCount].GetComponent<EnemyNavigationScript>();
                continue;
            }
        }

        waveTimer = 15.0f;
        timeIncrements = 0.02f;

        runningGame = true; //**changed for FPS scene

        killScore = 0;
        waveCount = 1;

        enemyWaveState = EnemyWaveState.STATE_NONE;
    }


    void Update()
    {
        switch (enemyWaveState)
        {
            case EnemyWaveState.STATE_NONE:
                if (runningGame)
                {
                    enemyWaveState = EnemyWaveState.WAVE_LOADING;
                }
                else
                {
                    //all enemies are reset to their origins as game no longer running
                    for (int i = 0; i < totalEnemies; i++)
                    {
                        enemyInfo[i].enemyState = EnemyNavigationScript.EnemyState.DEAD;
                        activeStatus[i] = false;
                    }
                }
                break;
            case EnemyWaveState.WAVE_LOADING:
                killScoreText.text = killScore.ToString();
                waveText.text = (waveCount - 1).ToString();
                //check if need to increase maxEnemies from waveCount
                if (waveCount > 1 && waveCount <= 7)
                {
                    if (waveCount % 2 == 1)
                    {
                        if(maxEnemies < 8)
                        {
                            maxEnemies++;
                        }
                    }
                }
                //there are 3 wave counts with the same maxEnemy value, refer to technical doc for table
                if (waveCount > 8 && waveCount < 20) //after wave 20 the maxEnemies does not go up anymore, again refer to tech doc
                {
                    if (waveCount % 2 == 0)
                    {
                        if(maxEnemies < totalPaths)
                        {
                            maxEnemies++;
                        }  
                    }
                }
                SetEnemyPaths(); //iterates through the max enemies and sets each path with an enemy (only backwards in the code for sanity's sake)
                enemyWaveState = EnemyWaveState.WAVE_RUNNING;
                break;
            case EnemyWaveState.WAVE_RUNNING:
                waveText.text = (waveCount - 1).ToString();
                killScoreText.text = killScore.ToString();
                CheckDeadEnemies(); 
                break;
            case EnemyWaveState.WAVE_FINISHED:
                {
                    waveTimer -= timeIncrements;
                    waveText.text = ((int)waveTimer).ToString(); //TODO: needs it's own space?
                    if (waveTimer <= 0.0f)
                    {
                       // Debug.Log("Wave reset");
                        waveTimer = 15.0f;
                        enemyWaveState = EnemyWaveState.WAVE_LOADING;
                    }
                }
                break;
            default:
                enemyWaveState = EnemyWaveState.STATE_NONE;
                break;
        }
    }

    void SetEnemyPaths()
    {
        int unwinged; int winged; int shark; int tank;
        SetEnemyAmounts(out unwinged, out winged, out shark, out tank);
        int unwingedCount = 0; int wingedCount = 0; int sharkCount = 0; int tankCount = 0;
        int enemyChoice = 0;
        bool pathSet = false;
        int totalCount = 0;

        for(int i = 0; i < maxEnemies; i++)
        {
            pathSet = false;
            if (tank > 0) //have tank enemies in this set
            {
                enemyChoice = Random.Range(1, 4);
            }
            else if (tank <= 0)
            {
                enemyChoice = Random.Range(1, 3);
            }
            while (!pathSet)
            {
                //catch here to remove infinite loop
                if((unwingedCount + wingedCount + sharkCount + tankCount) >= maxEnemies)
                {
                    pathSet = true;
                }
                switch (enemyChoice)
                {
                    case 1: //unwinged
                        if (unwingedCount < unwinged)
                        {
                            unwingedEnemies[unwingedCount].SetActive(true); //**getting array index out of error here once reach over 10 enemies (approx)
                            //find where enemy is in enemyInfo/activeStatus
                            enemyInfo[unwingedCount].points = spawnPointsScript[i].path;
                            enemyInfo[unwingedCount].enemyNavState = EnemyNavigationScript.EnemyNavigationState.NEW_TRAVERSING;
                            activeStatus[unwingedCount] = true;
                            unwingedCount++;
                            pathSet = true;
                            break;
                        }
                        else //finished with unwinged enemies
                        {
                            enemyChoice++;
                            break;
                        }
                    case 2: //winged
                        if(wingedCount < winged)
                        {
                            wingedEnemies[wingedCount].SetActive(true);
                            //find where enemy is in enemyInfo/activeStatus
                            totalCount = unwingedEnemies.Length + wingedCount;
                            enemyInfo[totalCount].points = spawnPointsScript[i].path;
                            enemyInfo[totalCount].enemyNavState = EnemyNavigationScript.EnemyNavigationState.NEW_TRAVERSING;
                            activeStatus[totalCount] = true;
                            wingedCount++;
                            pathSet = true;
                            break;
                        }
                        else //finished with winged enemies
                        {
                            enemyChoice++;
                            break;
                        }
                    case 3: //shark
                        if(sharkCount < shark)
                        {
                            sharkEnemies[sharkCount].SetActive(true); //**getting array index out of error here once reach over 10 enemies (approx)
                            //find where enemy is in enemyInfo/activeStatus
                            totalCount = unwingedEnemies.Length + wingedEnemies.Length + sharkCount;
                            enemyInfo[totalCount].points = spawnPointsScript[i].path;
                            enemyInfo[totalCount].enemyNavState = EnemyNavigationScript.EnemyNavigationState.NEW_TRAVERSING;
                            activeStatus[totalCount] = true;
                            sharkCount++;
                            pathSet = true;
                            break;
                        }
                        else
                        {
                            enemyChoice++;
                            break;
                        }
                    case 4: //tank
                        if(tankCount < tank)
                        {
                            tankEnemies[tankCount].SetActive(true);
                            //find where enemy is in enemyInfo/activeStatus
                            totalCount = unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length + tankCount;
                            enemyInfo[totalCount].points = spawnPointsScript[i].path;
                            enemyInfo[totalCount].enemyNavState = EnemyNavigationScript.EnemyNavigationState.NEW_TRAVERSING;
                            activeStatus[totalCount] = true;
                            tankCount++;
                            pathSet = true;
                            break;
                        }
                        else //finished with tank enemies, return to start of switch statement
                        {
                            enemyChoice = 1;
                            break;
                        }
                    default:
                        enemyChoice = 1;
                        break;
                }
            }
        }
    }

    void SetEnemyAmounts(out int unwingedVal, out int wingedVal, out int sharkVal, out int tankVal)
    {
        unwingedVal = 0; wingedVal = 0; sharkVal = 0; tankVal = 0;
        //check waveCount as to what range values can be used for enemies
        //refer to enemy waves table in technical doc for enemy numbers
        if (waveCount <= 2) //1 or 2 values, no tank
        {
            wingedVal = Random.Range(1, 2);
            if (wingedVal == 1)
            {
                unwingedVal = 2;
                sharkVal = 2;
                return;
            }
            if (wingedVal == 2)
            {
                unwingedVal = Random.Range(1, 2);
                if (unwingedVal == 1)
                {
                    sharkVal = 2;
                    return;
                }
                else
                {
                    sharkVal = 1;
                    return;
                }
            }
        }
        if (waveCount > 2 && waveCount <= 12) // 1, 2 or 3 values
        {
            unwingedVal = Random.Range(1, 3); //starting value to base the rest off
            if(waveCount < 6)
            {
                wingedVal = Random.Range(1, 3);
                sharkVal = maxEnemies - (unwingedVal + wingedVal);
                return;
            }
            if (waveCount >= 6 && waveCount < 11) //tank value 0 or 1
            {
                tankVal = Random.Range(0, 1);
               
                if ((maxEnemies - (tankVal + unwingedVal)) % 2 == 0) //remainder can be divided in half
                {
                    wingedVal = (maxEnemies - (tankVal + unwingedVal)) / 2;
                    sharkVal = wingedVal;
                    return;
                }
                else //remainder can't be divided in half, one must be larger than the other
                {
                    wingedVal = ((maxEnemies - (tankVal + unwingedVal)) / 2) + 1;
                    sharkVal = wingedVal - 1;
                    return;
                }
                
            }
            if (waveCount >= 11) //tank value 1 or 2
            {
                tankVal = Random.Range(1, 2);
                if ((maxEnemies - (tankVal + unwingedVal)) % 2 == 0) //remainder can be divided in half
                {
                    wingedVal = (maxEnemies - (tankVal + unwingedVal)) / 2;
                    sharkVal = wingedVal;
                    return;
                }
                else //remainder can't be divided in half, one must be larger than the other
                {
                    wingedVal = ((maxEnemies - (tankVal + unwingedVal)) / 2) + 1;
                    sharkVal = wingedVal - 1;
                    return;
                }
            }
        }
        if (waveCount > 12 && waveCount <= 20) // 2, 3 or 4 values
        {
            wingedVal = Random.Range(2, 4); //swapped this from previous waveCount range to winged, code underneath has swapped as well
            if (waveCount >= 18) //tank value 2 or 3
            {
                tankVal = Random.Range(2, 3);
                if ((maxEnemies - (tankVal + wingedVal)) % 2 == 0) //remainder can be divided in half
                {
                    unwingedVal = (maxEnemies - (tankVal + wingedVal)) / 2;
                    sharkVal = unwingedVal;
                    return;
                }
                else //remainder can't be divided in half, one must be larger than the other
                {
                    unwingedVal = ((maxEnemies - (tankVal + wingedVal)) / 2) + 1;
                    sharkVal = unwingedVal - 1;
                    return;
                }
            }
            tankVal = Random.Range(1, 2); //tank value 1 or 2
            if ((maxEnemies - (tankVal + wingedVal)) % 2 == 0) //remainder can be divided in half
            {
                unwingedVal = (maxEnemies - (tankVal + wingedVal)) / 2;
                sharkVal = unwingedVal;
                return;
            }
            else //remainder can't be divided in half, one must be larger than the other
            {
                unwingedVal = ((maxEnemies - (tankVal + wingedVal)) / 2) + 1;
                sharkVal = unwingedVal - 1;
                return;
            }
        }
        if (waveCount > 20 && waveCount <= 29) //same values, tank increases to 5 - refer to tech doc enemy waves table for info
        {
            unwingedVal = Random.Range(2, 4);
            if(waveCount <= 23 || waveCount == 26)
            {
                tankVal = 3;
            }
            if(waveCount > 23 && waveCount <= 25 || waveCount == 29)
            {
                tankVal = 4;

            }
            if(waveCount > 26 && waveCount <= 28)
            {
                tankVal = 5;
            }
            if ((maxEnemies - (tankVal + unwingedVal)) % 2 == 0) //remainder can be divided in half
            {
                wingedVal = (maxEnemies - (tankVal + unwingedVal)) / 2;
                sharkVal = wingedVal;
                return;
            }
            else //remainder can't be divided in half, one must be larger than the other
            {
                wingedVal = ((maxEnemies - (tankVal + unwingedVal)) / 2) + 1;
                sharkVal = wingedVal - 1;
                return;
            }
        }
        if (waveCount >= 30) //same values, tank always at 6
        {
            tankVal = 6;
            sharkVal = Random.Range(3, 4);
            int enemyVal = maxEnemies - tankVal; //8
            if ((enemyVal - sharkVal) % 2 == 0) //remainder can be divided in half
            {
                wingedVal = (enemyVal - sharkVal) / 2;
                unwingedVal = wingedVal;
                return;
            }
            else //remainder can't be divided in half, one must be larger than the other
            {
                wingedVal = ((enemyVal - sharkVal) / 2) + 1;
                unwingedVal = wingedVal - 1;
                return;
            }
        } 
        //catch if main enemy values aren't allocated
        if((unwingedVal + wingedVal + sharkVal + tankVal) < maxEnemies)
        {
            Debug.Log("not able to set enemy values correctly in SetEnemyAmounts: " + waveCount);
            if(unwingedVal == 0 || wingedVal == 0 || sharkVal == 0)
            {
                unwingedVal = maxEnemies / 3;
                wingedVal = (maxEnemies - unwingedVal) / 2;
                if( waveCount >= 6)
                {
                    tankVal = 1;
                }
                sharkVal = maxEnemies - (unwingedVal + wingedVal + tankVal);
                return;
            }
        }      
    }

    void CheckDeadEnemies()
    {
        for(int i = 0; i < totalEnemies; i++) 
        {
            if (enemyInfo[i].enemyState == EnemyNavigationScript.EnemyState.DEAD && activeStatus[i])
            {
                //find where enemy is in it's type and disable enemy
                int value = 0;
                if(i - unwingedEnemies.Length < 0) //unwinged enemy
                {
                    unwingedEnemies[i].SetActive(false); 
                }
                else if(i - (unwingedEnemies.Length + wingedEnemies.Length) < 0) //winged enemy
                {
                    value = i - unwingedEnemies.Length;
                    wingedEnemies[value].SetActive(false); 
                }
                else if(i - (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length) < 0) //shark enemy
                {
                    value = i - (unwingedEnemies.Length + wingedEnemies.Length);
                    sharkEnemies[value].SetActive(false);
                }
                else if (i - (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length) < 0) //tank enemy
                {
                    value = i - (unwingedEnemies.Length + wingedEnemies.Length + sharkEnemies.Length);
                    tankEnemies[value].SetActive(false);
                }
                else
                {
                    Debug.Log("Value did not match any enemy in the array: " + i); //**this is being hit when i = 12
                }
                activeStatus[i] = false;
                enemyDeadCount++;
                killScore++;         
                if(enemyDeadCount >= maxEnemies)
                {
                    //all enemies are dead, can move on to next wave
                    enemyWaveState = EnemyWaveState.WAVE_FINISHED;
                    waveCount++;
                    enemyDeadCount = 0;
                    return;
                }
            }
        }
    }
}
