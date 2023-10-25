// ----------------------------------------------------------------------
// --------------------  AI Game Manager
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DD_GameManager : MonoBehaviour
{
    // ---------------------------------------------------------------------
    // Game Data
    public GameObject[,] playArea = { };
    private DD_Player_Input playerInputManager;
    public Vector3 mainTargetPos;

    // objects
    public GameObject markerPrefab;
    public GameObject unitPrefab;
    public GameObject boxPrefab;
    public GameObject objectParent;
    public GameObject unitParent;
    private GameObject targetMarker;

    private List<GameObject> activeUnits = new();


    // UI
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
        SetTarget();
       
    }//---

    // ---------------------------------------------------------------------
    private void Update() // variable FPS based on frame render time
    {
        ChangeState();
    }//---

    // ---------------------------------------------------------------------
    private void AddInitialGameObjects()
    {
        // Add some rects and lines
        AddRectange(30, 40, 10, 10, false); //x,z,w,h,filled
        AddRectange(50, 80, 20, 5, false);
        AddRectange(0, 10, 10, 1, false); // a  line
        AddRectange(10, 0, 1, 10, false); // a  line

        AddObjects(100, 200);
        AddUnits(100);

       // AddTestUnit();
    }//------

    // ---------------------------------------------------------------------
    private void SetTarget()
    {
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

    }//-----


    // ---------------------------------------------------------------------
    private void ChangeState()
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

    }//---



  


        // ---------------------------------------------------------------------
        void AddTestUnit()
        {
            // Specific Test Start Positions
            int newZ = 5; int newX = 5;
            if (playArea[newZ, newX] == null) playArea[newZ, newX] = (GameObject)Instantiate(unitPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);
            //    newZ = 2; newX = 2;
            //    if (playArea[newZ, newX] == null) playArea[newZ, newX] = (GameObject)Instantiate(unitPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);
            //     newZ = 8; newX = 8;
            //   if (playArea[newZ, newX] == null) playArea[newZ, newX] = (GameObject)Instantiate(unitPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);
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

            print("unit in list" + activeUnits.Count);

        }//----

        // ---------------------------------------------------------------------
        private void AddObjects(int min, int max)
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
                    playArea[newZ, newX] = (GameObject)Instantiate(boxPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                    // child to Objects to keep Hierachy Organised
                    playArea[newZ, newX].transform.parent = objectParent.transform;
                }
            }
        }//------   

        // ---------------------------------------------------------------------
        private void AddRectange(int xStart, int zStart, int width, int height, bool isFilled)
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
                                playArea[newZ, newX] = (GameObject)Instantiate(boxPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                                // child to Objects to keep Hierachy Organised
                                playArea[newZ, newX].transform.parent = objectParent.transform;
                            }
                        }
                        else
                        {
                            if (row == 0 || row == height - 1 || col == 0 || col == width - 1)
                            {
                                // Add block object to array
                                playArea[newZ, newX] = (GameObject)Instantiate(boxPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

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



        // ---------------------------------------------------------------------
        private void DisplayGameData()
        {
            // Left Hand Text Window
            LeftTextWindow.text = "Game States \n ==================\n";
            LeftTextWindow.text += "\nLeft Mouse:  " + playerInputManager.leftClickPostion;
            LeftTextWindow.text += "\nRight Mouse:  " + playerInputManager.rightClickPostion;
        }//-----


    }//==========