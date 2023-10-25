// ----------------------------------------------------------------------
// --------------------  AI: Unit Object Class
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DD_Unit : DD_BaseObject
{
        public DD_GameManager gameManager;

    // Movement Variables
    public Vector3 targetPosition = Vector3.zero;
    public Vector3 currentPostion = Vector3.zero;
    public Vector3 startPosition = Vector3.zero;
    public Vector3 nextPosition = Vector3.zero;
    public GameObject nextSlotObject = null;
    public float speed = 1;
    bool isMoving = false;

    // Wander vars
    public bool obstacleAhead = false;
    public Heading unitHeading;
    public States unitState = States.idle;

    // ---------------------------------------------------------------------
    private void Start()
    {
        // The game manager will be use to access the game board
        gameManager = GameObject.Find("GameManager").GetComponent<DD_GameManager>();

        // Round the positions to nearest whole number
        xPos = (int)transform.position.x;
        zPos = (int)transform.position.z;

        // Round off postion to nearest int ands store the current position
        transform.position = new Vector3(xPos, 0, zPos);
        currentPostion = transform.position;

        // Set initial states
        unitHeading = (Heading)Random.Range(0, 4);
        unitState = States.roam;
    }//----

    // ---------------------------------------------------------------------
    private void FixedUpdate()
    {
        CheckState();
    }//---


    // ---------------------------------------------------------------------
    private void CheckState()
    {

        if (unitState == States.roam) Roam();
        if (unitState == States.wander) Wander();
        if (unitState == States.chase) Chase();
        MoveUnit();
    }//------


    // ---------------------------------------------------------------------
    private void Chase()
    {
       
    }//---

    // ---------------------------------------------------------------------
    private void Wander()
    {
        if (!isMoving)
        {
            if (obstacleAhead) // path blocked - find a new heading
            {
                unitHeading = (Heading)Random.Range(0, 4); // pick a new direction from the heading enum
                isMoving = true;
                obstacleAhead = false;
            }
            else
            {
                // Find Next Postion based on heading
                if (unitHeading == Heading.north) nextPosition = new Vector3(currentPostion.x, 0, currentPostion.z + 1);
                if (unitHeading == Heading.south) nextPosition = new Vector3(currentPostion.x, 0, currentPostion.z - 1);
                if (unitHeading == Heading.east) nextPosition = new Vector3(currentPostion.x + 1, 0, currentPostion.z);
                if (unitHeading == Heading.west) nextPosition = new Vector3(currentPostion.x - 1, 0, currentPostion.z);

                // Round off next Pos
                nextPosition = new Vector3((int)nextPosition.x, 0, (int)nextPosition.z);
                int newX = (int)nextPosition.x;
                int newZ = (int)nextPosition.z;

                // Check if the new postion is on the board
                if (newX >= 0 && newX < gameManager.playArea.GetLength(1) && newZ >= 0 && newZ < gameManager.playArea.GetLength(0))
                {
                    if (gameManager.playArea[newZ, newX] == null) //check new pos is empty
                    {
                        gameManager.playArea[zPos, xPos] = null; // clear old slot in array
                        gameManager.playArea[newZ, newX] = gameObject; //set new slot in array                 
                        xPos = newX; zPos = newZ; // store position 
                        currentPostion = nextPosition;
                        isMoving = true;
                    }
                    else // next slot in array is not empty
                    {
                        // print("new slot occupied by " + gameManager.playArea[newZ, newX]);
                        isMoving = false;
                        obstacleAhead = true;
                    }
                }
                else // new position not on the board
                {
                    isMoving = false;
                    obstacleAhead = true;
                }
            }
        }
    }//-----



    // ---------------------------------------------------------------------
    private void Roam()
    {
        // generate random adjacent tile to move to     
        if (!isMoving) // Set a new direction if unit is not moving
        {
            int direction = (int)UnityEngine.Random.Range(0, 4);
            if (direction == 0) nextPosition = new Vector3(currentPostion.x - 1, 0, currentPostion.z);
            if (direction == 1) nextPosition = new Vector3(currentPostion.x + 1, 0, currentPostion.z);
            if (direction == 2) nextPosition = new Vector3(currentPostion.x, 0, currentPostion.z - 1);
            if (direction == 3) nextPosition = new Vector3(currentPostion.x, 0, currentPostion.z + 1);

            // Round off next Pos
            nextPosition = new Vector3((int)nextPosition.x, 0, (int)nextPosition.z);
            int newX = (int)nextPosition.x;
            int newZ = (int)nextPosition.z;

            // Check if the new postion is on the board
            if (newX >= 0 && newX < gameManager.playArea.GetLength(1) && newZ >= 0 && newZ < gameManager.playArea.GetLength(0))
            {
                if (gameManager.playArea[newZ, newX] == null) // slot is empty
                {
                    gameManager.playArea[zPos, xPos] = null; // clear old slot
                    gameManager.playArea[newZ, newX] = gameObject; //set new slot                 
                    xPos = newX; zPos = newZ;
                    currentPostion = nextPosition;
                    isMoving = true;
                }
                else
                {
                    // print("new slot occupied by " + gameManager.playArea[newZ, newX]);
                    isMoving = false;
                }
            }
        }
    }//-----

    // ---------------------------------------------------------------------
    private void MoveUnit()
    {
        if (isMoving)
        {
            // check distance to new current position
            Vector2 movePos = new(currentPostion.x, currentPostion.z);
            Vector2 curentRealPos = new(transform.position.x, transform.position.z);

            if (Vector2.Distance(movePos, curentRealPos) > 0.01F) // Move Unit to new slot
            {
                transform.LookAt(currentPostion);
                transform.Translate(0, 0, speed * Time.deltaTime);
            }
            else
            {
                //  print("at target, stopping Moving");
                isMoving = false; // stop moving            
            }
        }
    }//----

}//==========

public enum States { idle, roam, wander, chase, attack, flee }
public enum Heading { north, east, south, west }
