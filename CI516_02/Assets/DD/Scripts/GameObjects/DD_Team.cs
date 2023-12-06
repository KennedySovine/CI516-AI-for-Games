// ----------------------------------------------------------------------
// --------------------  AI: Team Class
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------
using System.Collections.Generic;

using UnityEngine;

public class DD_Team : MonoBehaviour
{
    // ---------------------------------------------------------------------

    [Header("Team Stats")]
    public Color teamColor = Color.blue;
    public int teamID = 0;
    public string teamName = "Team X";
    public int teamMembersTotal = 50;
    public int activeTeamMembers = 0;
    public float teamResources = 100;
    public Vector2 teamPosition = Vector2.zero;
    public Heading spawnDirection = Heading.east;
    public int unitSpawnDistance = 3;

    public int[] friendlyTeams = { };

    [Header("Team Objects")]
    public GameObject unit = null;
    public float unitCost = 5;
    public List<GameObject> teamUnits = new();

    public float spawnCoolDownTime = 5.0f;
    private float nextSpawnTime = 0.0f;
    private DD_GameManager gameManager;

    [Header("Squad Stuff")]
    public List<GameObject> squad = new();
    public bool squadActive = false;


    // ---------------------------------------------------------------------
    private void Start()
    {
        // The game manager will be use to access the game board
        gameManager = GameObject.Find("GameManager").GetComponent<DD_GameManager>();
        gameObject.GetComponent<Renderer>().material.color = teamColor;
        CreateUnits();


    }//-----

    // ---------------------------------------------------------------------
    private void FixedUpdate()
    {
        SpawnUnit();
    }//-----


    private void Update()
    {
        SetSquadFormation();
    }

    // ---------------------------------------------------------------------
    private void SpawnUnit()
    {
        if (nextSpawnTime > Time.time) return;    // wait until coolDown over   
        if (teamResources < unitCost) return;  // Check resources are available

        //  print("resources available to create new life");
        GameObject newUnit = null;
        DD_Unit unitScript;

        // Find first Available Unit 
        foreach (GameObject unit in teamUnits)
        {
            unitScript = unit.GetComponent<DD_Unit>();
            if (unitScript.isAlive == false)
            {
                newUnit = unit;
                break; // stop searching for inactive units
            }
        }

        if (!newUnit) return; // exit as no available units 

        // Unit found so Set unit Position and add to playArea Array
        int newX = (int)Mathf.Round(teamPosition.x);
        int newZ = (int)Mathf.Round(teamPosition.y); // y is z in this case as V2

        if (spawnDirection == Heading.east) newX += unitSpawnDistance;
        if (spawnDirection == Heading.west) newX -= unitSpawnDistance;
        if (spawnDirection == Heading.north) newZ += unitSpawnDistance;
        if (spawnDirection == Heading.south) newZ -= unitSpawnDistance;

        if (gameManager.playArea[newZ, newX] == null)
        {
            // Add resource object to array
            gameManager.playArea[newZ, newX] = newUnit;
            newUnit.transform.position = new(newX, 0, newZ); // position unit

            // Active unit and add to the totals
            unitScript = newUnit.GetComponent<DD_Unit>();
            unitScript.currentPosition = new(newX, 0, newZ); // reset current position
            unitScript.xPos = newX;
            unitScript.zPos = newZ;
            unitScript.isAlive = true;
            unitScript.health = newUnit.GetComponent<DD_UnitHealth>().maxHealth;
            activeTeamMembers++;
            gameManager.activeUnits.Add(newUnit);

            // reduce resesource & set next spawn time
            teamResources -= unitCost;
        }

        nextSpawnTime = Time.time + spawnCoolDownTime;
    }//-----



    // ---------------------------------------------------------------------
    private void CreateUnits()
    {
        for (int i = 0; i < teamMembersTotal; i++)
        {
            int newX = i;
            int newZ = -3 - teamID; // position inactive units off the board in rows of teams

            GameObject newUnit = Instantiate(unit, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);
            newUnit.transform.SetParent(transform); // keep the hierarchy tidy
            newUnit.GetComponent<Renderer>().material.color = teamColor;
            DD_Unit unitScript = newUnit.GetComponent<DD_Unit>();
            unitScript.unitID = i;
            unitScript.basePosition = new(teamPosition.x, 0, teamPosition.y); // tell each unit where home is
            unitScript.team = GetComponent<DD_Team>();

            // Add to list of units
            teamUnits.Add(newUnit);
        }
    }//----



    // ---------------------------------------------------------------------
    public void DepositResource(float DepositRate) // receiver for unit resources
    {
        teamResources += DepositRate;
    }//-----


    // ===========================================================================


    public int[,] squadShape =  {   { 1, 0, 0, 2, 0, 0, 3 },
                                    { 0, 0, 0, 0, 0, 0, 0 },
                                    { 4, 0, 0, 5, 0, 0, 6 },
                                    { 0, 0, 0, 0, 0, 0, 0 },
                                    { 0, 0, 0, 0, 0, 0, 0 },
                                    { 0, 0, 0, 0, 0, 0, 0 },
                                    { 7, 0, 0, 8, 0, 0, 9 }      };


    // ---------------------------------------------------
    void SetSquadFormation()
    {  
        if (Input.GetKeyDown(KeyCode.Y))
        {
            squadShape = new int[7, 7]
            {
                { 1, 0, 2, 0, 3, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 4, 0, 5, 0, 6, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 7, 0, 8, 0, 9, 0, 0 }
            };
        }
        // Formation 2  --------------------	
        if (Input.GetKeyDown(KeyCode.U))
        {
            squadShape = new int[7, 7]
            {
                { 0, 0, 1, 0, 0 , 0, 0 },
                { 0, 0, 0, 0, 0 , 0, 0 },
                { 0, 2, 0, 3, 0 , 0, 0 },
                { 0, 0, 6, 0, 0 , 0, 0 },
                { 4, 7, 0, 8, 5 , 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0 }
            };
        }
        // Formation 3	--------------------			
        if (Input.GetKeyDown(KeyCode.I))
        {
            squadShape = new int[5, 5]
            {
                { 5, 2, 1, 3, 4 },
                { 6, 7, 8, 9, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 }
            };
        }
        // Formation 4	--------------------			
        if (Input.GetKeyDown(KeyCode.O))
        {
            squadShape = new int[5, 5]
            {
                { 2, 0, 0, 0, 3 },
                { 0, 5, 0, 6, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 7, 0, 8, 0 },
                { 4, 0, 0, 0, 5 }
            };
        }
    }//-----


}//==========
