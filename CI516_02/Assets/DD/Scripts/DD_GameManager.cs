// ----------------------------------------------------------------------
// --------------------  AI Game Manager
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DD_GameManager : MonoBehaviour
{
    // ---------------------------------------------------------------------
    // Game Data
    public GameObject[,] playArea = { };
    private DD_Player_Input playerInputManager;

    [Header("Unit Targets")]
    public Vector3 mainTargetPos = new(-50,-50,-50);
    public Vector3 secondaryTargetPos = new(-50,50,-50);

    [Header("Game Objects")]
    // Teams
    public GameObject[] Teams = new GameObject[2];

    // objects
    public GameObject markerPrefab;
    public GameObject redMarkerPrefab;
    public GameObject unitPrefab;
    public GameObject obstaclePrefab;
    public GameObject objectParent;
    public GameObject unitParent;
    private GameObject targetMarker;
    private GameObject redTargetMarker;
    public GameObject resourcePF;
    public int resourcesToSpawn = 100;
    public Vector2 objectsToSpawn = new(500, 1000);
    public int unitsToSpawn = 1000;
    private readonly List<GameObject> activeTeams = new();
    private readonly List<GameObject> activeUnits = new();

    // UI
    [Header("Game UI")]
    public Text LeftTextWindow;

    // ---------------------------------------------------------------------
    private void Awake()// Runs before start on other objects
    {
        // Create Array for play area
        playArea = new GameObject[100, 100]; // rows z, cols x
    }//------

    // ---------------------------------------------------------------------
    private void Start()
    {
        playerInputManager = GetComponent<DD_Player_Input>();
        AddInitialGameObjects();
    }//---

    // ---------------------------------------------------------------------
    private void FixedUpdate() // Capped at 50 FPS
    {
        DisplayGameData();
    }//---

    // ---------------------------------------------------------------------
    private void Update() // variable FPS based on frame render time
    {
      //  ChangeStateOnKeyPress();
    }//---


    // ---------------------------------------------------------------------
    private void AddInitialGameObjects()
    {    
        AddTeams();
        AddObstacles((int)objectsToSpawn.x, (int)objectsToSpawn.y);
        AddInitialResourceObjects(resourcesToSpawn);   
    }//------


    // ---------------------------------------------------------------------
    private void AddTeams()
    {
        int newZ = -5;
        int newX = -5;
        int teamIndex = 0;

        foreach (GameObject team in Teams)
        {
            GameObject newTeam = Instantiate(Teams[teamIndex], new Vector3((float)newX, 0, (float)newZ), transform.rotation);

            newX = (int)newTeam.GetComponent<DD_Team>().teamPosition.x;
            newZ = (int)newTeam.GetComponent<DD_Team>().teamPosition.y;

            if (playArea[newZ, newX] == null)
            {
                // Add team object to array
                playArea[newZ, newX] = newTeam;
                newTeam.transform.position = new(newX, 0, newZ);
                newTeam.GetComponent<DD_Team>().teamID = teamIndex;
            }
            activeTeams.Add(newTeam);
            teamIndex++;
        }
    }//-----

    // ---------------------------------------------------------------------
    private void AddInitialResourceObjects(int resourceAmount)
    {
        for (int i = 0; i < resourceAmount; i++)
        {
            int newZ = Random.Range(0, playArea.GetLength(0));
            int newX = Random.Range(0, playArea.GetLength(1));
            // check if space is empty
            if (playArea[newZ, newX] == null)
            {
                // Add block object to array
                playArea[newZ, newX] = (GameObject)Instantiate(resourcePF, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                // child to Objects to keep Hierachy Organised
                playArea[newZ, newX].transform.parent = objectParent.transform;
            }
        }
    }//------   

   

    // ---------------------------------------------------------------------
    private void AddObstacles(int min, int max)
    {
        //Add a Random number of Box Objects
        int spawnAmount = Random.Range(min, max);

        for (int i = 0; i < spawnAmount; i++)
        {
            int newZ = Random.Range(0, playArea.GetLength(0));
            int newX = Random.Range(0, playArea.GetLength(1));
            // check if space is empty
            if (playArea[newZ, newX] == null)
            {
                // Add block object to array
                playArea[newZ, newX] = (GameObject)Instantiate(obstaclePrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                // child to Objects to keep Hierachy Organised
                playArea[newZ, newX].transform.parent = objectParent.transform;
            }
        }
    }//------   


    // ---------------------------------------------------------------------
    private void DisplayGameData()
    {
        // Left Hand Text Window
        LeftTextWindow.text = "Game States \n ==================";

       // LeftTextWindow.text += "L:  " + playerInputManager.leftClickPostion;
       // LeftTextWindow.text += "  R:  " + playerInputManager.rightClickPostion;

        // Display Team Stats
        foreach (GameObject team in activeTeams)
        {
            DD_Team teamScript = team.GetComponent<DD_Team>();
            LeftTextWindow.text += "\n\nTeam: " + teamScript.teamName;
            LeftTextWindow.text += "\nUnits: " + teamScript.activeTeamMembers + " / " + teamScript.teamMembersTotal;
            LeftTextWindow.text += "\nResources: " + (int)teamScript.teamResources;
        }
    }//-----






    //  ************************** Older Functions Not Currently Used ****************************

    // ---------------------------------------------------------------------
    private void SetTargets()
    {
        // Yellow Marker
        mainTargetPos = new(playerInputManager.leftClickPostion.x, 0, playerInputManager.leftClickPostion.y);

        //place marker gameobject on position where mouse was clicked
        if (targetMarker == null)
        {// if there is no marker already in the scene
            targetMarker = (GameObject)Instantiate(markerPrefab, new(playerInputManager.leftClickPostion.x, -0.9F, playerInputManager.leftClickPostion.y), transform.rotation);
        }
        else
        {
            targetMarker.transform.position = new(playerInputManager.leftClickPostion.x, -0.9F, playerInputManager.leftClickPostion.y);
        }

        // Red Marker
        secondaryTargetPos = new(playerInputManager.rightClickPostion.x, 0, playerInputManager.rightClickPostion.y);

        //place marker gameobject on position where mouse was clicked
        if (redTargetMarker == null)
        {// if there is no marker already in the scene
            redTargetMarker = (GameObject)Instantiate(redMarkerPrefab, new(playerInputManager.rightClickPostion.x, -0.9F, playerInputManager.rightClickPostion.y), transform.rotation);
        }
        else
        {
            redTargetMarker.transform.position = new(playerInputManager.rightClickPostion.x, -0.9F, playerInputManager.rightClickPostion.y);
        }

    }//-----


    // ---------------------------------------------------------------------
    private void ChangeStateOnKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (GameObject go in activeUnits)
            {
                go.GetComponent<DD_Unit>().unitState = States.idle;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (GameObject go in activeUnits)
            {
                go.GetComponent<DD_Unit>().unitState = States.roam;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (GameObject go in activeUnits)
            {
                go.GetComponent<DD_Unit>().unitState = States.wander;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            foreach (GameObject go in activeUnits)
            {
                go.GetComponent<DD_Unit>().unitState = States.chase;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            foreach (GameObject go in activeUnits)
            {
                go.GetComponent<DD_Unit>().unitState = States.flee;
            }
        }

    }//---



    // ---------------------------------------------------------------------
    void AddTestUnit()
    {
        // Specific Test Start Positions
        int newZ = 5; int newX = 5;
        if (playArea[newZ, newX] == null) playArea[newZ, newX] = (GameObject)Instantiate(unitPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

    }//----


    // ---------------------------------------------------------------------
    void AddUnits(int amountOfUnits)
    {
        for (int i = 0; i < amountOfUnits; i++)
        {
            int newZ = Random.Range(0, playArea.GetLength(0)); // Limit to array length
            int newX = Random.Range(0, playArea.GetLength(1));

            // check if space is empty
            if (playArea[newZ, newX] == null) // Add unit to playArea array and spawn
            {
                playArea[newZ, newX] = (GameObject)Instantiate(unitPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);
                // child to Objects to keep Hierachy Organised
                playArea[newZ, newX].transform.parent = unitParent.transform;

                activeUnits.Add(playArea[newZ, newX]);
            }
        }

        print("\nActive units in list: " + activeUnits.Count);

    }//----

    // ---------------------------------------------------------------------
    private void AddRectangle(int xStart, int zStart, int width, int height, bool isFilled)
    {
        if (xStart + width < playArea.GetLength(1) && zStart + height < playArea.GetLength(0))
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int newZ = zStart + row;
                    int newX = xStart + col;

                    if (isFilled)
                    {
                        // check if space is empty
                        if (playArea[newZ, newX] == null)
                        {
                            // Add block object to array
                            playArea[newZ, newX] = (GameObject)Instantiate(obstaclePrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                            // child to Objects to keep Hierachy Organised
                            playArea[newZ, newX].transform.parent = objectParent.transform;
                        }
                    }
                    else
                    {
                        if (row == 0 || row == height - 1 || col == 0 || col == width - 1)
                        {
                            // Add block object to array
                            playArea[newZ, newX] = (GameObject)Instantiate(obstaclePrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                            // child to Objects to keep Hierachy Organised
                            playArea[newZ, newX].transform.parent = objectParent.transform;
                        }
                    }
                }
            }
        }
        else
        {
            print("Square is off the board");
        }
    }//-----


}//==========