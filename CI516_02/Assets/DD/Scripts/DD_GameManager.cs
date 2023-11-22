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
    private DD_PlayerInputManager playerInputManager;

    [Header("Unit Targets")]
    public Vector3 playerSetTargetPos = new(-50, 50, -50);

    [Header("Game Objects")]
    public GameObject[] Teams = new GameObject[2];
    public GameObject markerPrefab;
    public GameObject redMarkerPrefab;
    private GameObject selectionMarker;
    private GameObject redTargetMarker;
    public GameObject obstaclePrefab;
    public GameObject objectParent;
    public GameObject resourcePF;
    public int resourcesToSpawn = 100;
    public Vector2 objectsToSpawn = new(500, 1000);

    // Lists of Active Objects
    private readonly List<GameObject> activeTeams = new();
    public List<GameObject> activeUnits = new();
    public List<GameObject> activeResources = new();
    public List<GameObject> selectedPlayerUnits = new();
    private bool playerMarkerActive = false;

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
        playerInputManager = GetComponent<DD_PlayerInputManager>();
        AddInitialGameObjects();
    }//---

    // ---------------------------------------------------------------------
    private void FixedUpdate() // Capped at 50 FPS
    {
        DisplayGameData();
        SetSelectionMarkers();
    }//---



    // ---------------------------------------------------------------------
    private void AddInitialGameObjects()
    {
        AddTeams();
        AddObstacles((int)objectsToSpawn.x, (int)objectsToSpawn.y);
        AddInitialResourceObjects(resourcesToSpawn);
    }//------


    // ---------------------------------------------------------------------
    private void SelectUnits()
    {
        selectedPlayerUnits.Clear(); // clear last selection

        if (selectedPlayerUnits.Count == 0) // Deselect units
            foreach (GameObject unit in activeUnits)
                if (unit.GetComponent<DD_UnitPlayerControl>())
                    unit.GetComponent<DD_UnitPlayerControl>().isSelected = false;


        // find units within these position based on mouse clicks

        // Get the player clicks
        int startCol = (int)playerInputManager.leftClickPosition.x;
        int startRow = (int)playerInputManager.leftClickPosition.y;
        int endCol = (int)playerInputManager.leftDownPosition.x;
        int endRow = (int)playerInputManager.leftDownPosition.y;


        // ensure position are on the board
        if (startCol < 0 || startCol > playArea.GetLength(1)) return;
        if (startRow < 0 || startRow > playArea.GetLength(0)) return;
        if (endCol < 0 || endCol > playArea.GetLength(1)) return;
        if (endRow < 0 || endRow > playArea.GetLength(0)) return;


        // Swap start and end if end is lower
        if (endCol < startCol) (startCol, endCol) = (endCol, startCol);
        if (endRow < startRow) (startRow, endRow) = (endRow, startRow);


        // Loop through all selected slots to find a PC unit
        for (int col = startCol; col <= endCol; col++)
        {
            for (int row = startRow; row <= endRow; row++)
            {
                if (playArea[row, col] && playArea[row, col].GetComponent<DD_Unit>()) // Ensure the object is a unit
                {
                    if (playArea[row, col].GetComponent<DD_Unit>().team.teamID == 1) // the Player controlled team
                        selectedPlayerUnits.Add(playArea[row, col]);
                }
            }
        }
        // print(selectedPlayerUnits.Count + " units selected");

        // Select the units
        if (selectedPlayerUnits.Count > 0)
            foreach (GameObject unit in selectedPlayerUnits)
                unit.GetComponent<DD_UnitPlayerControl>().isSelected = true;
    }//-----



    // ---------------------------------------------------------------------
    private void SetSelectionMarkers()
    {
        //place marker gameobject 
        if (selectionMarker == null) // if there is no marker already in the scene add one        
            selectionMarker = (GameObject)Instantiate(markerPrefab, new(playerInputManager.leftDownPosition.x, -0.9F, playerInputManager.leftDownPosition.y), transform.rotation);

        if (playerInputManager.mouseLDown)
        {
            selectedPlayerUnits.Clear();

            playerMarkerActive = true;

            int startX = (int)Mathf.Round(playerInputManager.leftClickPosition.x);
            int startZ = (int)Mathf.Round(playerInputManager.leftClickPosition.y);
            int endX = (int)Mathf.Round(playerInputManager.leftDownPosition.x);
            int endZ = (int)Mathf.Round(playerInputManager.leftDownPosition.y);

            float width = startX - endX;
            float height = startZ - endZ;

            // Scale Marker with mouse drag
            selectionMarker.transform.localScale = new(width, 0.01f, height);
            selectionMarker.transform.position = new(endX + width / 2, 0.1f, endZ + height / 2);
            playerSetTargetPos = selectionMarker.transform.position;
        }
        if (playerInputManager.mouseLUP && playerMarkerActive) // move off the board when mouse button up
        {
            SelectUnits();
            playerMarkerActive = false;
        }

        if (!playerMarkerActive)
        {
            selectionMarker.transform.position = new(-2, 0, 0);
            selectionMarker.transform.localScale = new(1, 0.01F, 1);
        }


        //---------------------------------------------------------------
        // Red Marker for Target Setting

        if (playerInputManager.mouseRDown)
        {
            playerSetTargetPos = new(playerInputManager.rightClickPosition.x, 0, playerInputManager.rightClickPosition.y);

            //place marker gameobject on position where mouse was clicked
            if (redTargetMarker == null)
            {// if there is no marker already in the scene
                redTargetMarker = (GameObject)Instantiate(redMarkerPrefab, new(playerInputManager.rightClickPosition.x, -0.9F, playerInputManager.rightClickPosition.y), transform.rotation);
            }
            else
            {
                redTargetMarker.transform.position = new(playerInputManager.rightClickPosition.x, -0.9F, playerInputManager.rightClickPosition.y);
            }
        }

    }//-----





    // ---------------------------------------------------------------------
    private void AddTeams()
    {
        int newZ = -5;
        int newX = -5;
        int teamIndex = 0;

        foreach (GameObject team in Teams)
        {
            GameObject newTeam = Instantiate(Teams[teamIndex], new Vector3((float)newX, 0, (float)newZ), transform.rotation);

            newX = (int)Mathf.Round(newTeam.GetComponent<DD_Team>().teamPosition.x);
            newZ = (int)Mathf.Round(newTeam.GetComponent<DD_Team>().teamPosition.y);

            if (playArea[newZ, newX] == null)
            {
                // Add team object to array
                playArea[newZ, newX] = newTeam;
                newTeam.transform.position = new(newX, 0, newZ);
                newTeam.GetComponent<DD_Team>().teamID = teamIndex + 1;
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

                GameObject newResource = (GameObject)Instantiate(resourcePF, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                playArea[newZ, newX] = newResource;

                // child to Objects to keep Hierachy Organised
                newResource.transform.parent = objectParent.transform;

                activeResources.Add(newResource);
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