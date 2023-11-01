// ----------------------------------------------------------------------
// --------------------  AI: Unit Object Class
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------
//using System;
using UnityEngine;

// Enums
public enum States { idle, roam, wander, chase, attack, flee }
public enum Heading { north, east, south, west }

public class DD_Unit : DD_BaseObject
{
    // ---------------------------------------------------------------------    
    [Header("Unit State")]
    public States unitState = States.idle;
    private DD_GameManager gameManager;

    // Movement Variables
    [Header("Unit Positions")]
    public Vector3 targetPosition = Vector3.zero;
    public Vector3 currentPosition = Vector3.zero;
    public Vector3 startPosition = Vector3.zero;
    public Vector3 nextPosition = Vector3.zero;


    [Header("Movement")]
    public float speed = 1;
    bool isMoving = false;
    public float chaseRange = 20;
    // Wander vars
    public bool obstacleAhead = false;
    public Heading unitHeading;


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
        currentPosition = transform.position;

        // Set initial states
        unitHeading = (Heading)Random.Range(0, 4);
        unitState = States.wander;
    }//----

    // ---------------------------------------------------------------------
    private void FixedUpdate()
    {
        StateManager();
        CheckState();
    }//---

    //****************************************************************************
    // ************            Add to the State Manager below 
    //****************************************************************************

    // ---------------------------------------------------------------------
    private void StateManager()
    {
        



    }//----


 


    // ---------------------------------------------------------------------
    private void CheckState()
    {
        if (unitState == States.roam) Roam();
        if (unitState == States.wander) Wander();
        if (unitState == States.chase) ChaseDirect(false);
        if (unitState == States.flee) ChaseDirect(true);
        MoveUnit();
    }//------


    // ---------------------------------------------------------------------
    private void ChaseDirect(bool reverse)
    {
        if (!isMoving) // Set a new direction if unit is not moving
        {
            if (reverse)
                targetPosition = gameManager.secondaryTargetPos;
            else
                targetPosition = gameManager.mainTargetPos;


            // Find Straight Line to target  ---------------------------
            float dx = (targetPosition.x - currentPosition.x);
            float dz = (targetPosition.z - currentPosition.z);
            float angle = Mathf.Atan2(dx, dz);

            // use Trig to work out which slot is closest to a straight line to target
            if (!reverse)
            {
                if (Mathf.Abs(dx) > 0.1F) nextPosition.x = currentPosition.x + Mathf.Round(1.4F * Mathf.Sin(angle));
                if (Mathf.Abs(dz) > 0.1F) nextPosition.z = currentPosition.z + Mathf.Round(1.4F * Mathf.Cos(angle));
            }
            else
            {
                if (Mathf.Abs(dx) > 0.1F) nextPosition.x = currentPosition.x - Mathf.Round(1.4F * Mathf.Sin(angle));
                if (Mathf.Abs(dz) > 0.1F) nextPosition.z = currentPosition.z - Mathf.Round(1.4F * Mathf.Cos(angle));
            }

            // Round off next Pos
            nextPosition = new Vector3((int)nextPosition.x, 0, (int)nextPosition.z);
            int newX = (int)nextPosition.x;
            int newZ = (int)nextPosition.z;

            // Check if the new postion is on the board and free
            if (CheckNewPositionisFree(newX, newZ)) SetNewPosition(newX, newZ);
        }
    }//---




    // ---------------------------------------------------------------------
    private void ChaseSimple(bool reverse)
    {
        if (!isMoving) // Set a new direction if unit is not moving
        {
            if (reverse)
                targetPosition = gameManager.secondaryTargetPos;
            else
                targetPosition = gameManager.mainTargetPos;

            Vector3 moveDirection = Vector3.zero;

            if (targetPosition.x < currentPosition.x) moveDirection += Vector3.left;
            if (targetPosition.x > currentPosition.x) moveDirection += Vector3.right;
            if (targetPosition.z < currentPosition.z) moveDirection += Vector3.back;
            if (targetPosition.z > currentPosition.z) moveDirection += Vector3.forward;

            // chase or flee
            if (reverse) nextPosition = currentPosition - moveDirection;
            else nextPosition = currentPosition + moveDirection;

            // Round off next Pos
            nextPosition = new Vector3((int)nextPosition.x, 0, (int)nextPosition.z);
            int newX = (int)nextPosition.x;
            int newZ = (int)nextPosition.z;

            // Check if the new postion is on the board and free
            if (CheckNewPositionisFree(newX, newZ)) SetNewPosition(newX, newZ);
        }
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
                if (unitHeading == Heading.north) nextPosition = new Vector3(currentPosition.x, 0, currentPosition.z + 1);
                if (unitHeading == Heading.south) nextPosition = new Vector3(currentPosition.x, 0, currentPosition.z - 1);
                if (unitHeading == Heading.east) nextPosition = new Vector3(currentPosition.x + 1, 0, currentPosition.z);
                if (unitHeading == Heading.west) nextPosition = new Vector3(currentPosition.x - 1, 0, currentPosition.z);

                // Round off next Pos
                nextPosition = new Vector3((int)nextPosition.x, 0, (int)nextPosition.z);
                int newX = (int)nextPosition.x;
                int newZ = (int)nextPosition.z;

                // Check if the new postion is on the board and free
                if (CheckNewPositionisFree(newX, newZ)) SetNewPosition(newX, newZ);
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
            if (direction == 0) nextPosition = new Vector3(currentPosition.x - 1, 0, currentPosition.z);
            if (direction == 1) nextPosition = new Vector3(currentPosition.x + 1, 0, currentPosition.z);
            if (direction == 2) nextPosition = new Vector3(currentPosition.x, 0, currentPosition.z - 1);
            if (direction == 3) nextPosition = new Vector3(currentPosition.x, 0, currentPosition.z + 1);

            // Round off next Pos
            nextPosition = new Vector3((int)nextPosition.x, 0, (int)nextPosition.z);
            int newX = (int)nextPosition.x;
            int newZ = (int)nextPosition.z;

            // Check if the new postion is on the board and free
            if (CheckNewPositionisFree(newX, newZ)) SetNewPosition(newX, newZ);
        }
    }//-----


    // ---------------------------------------------------------------------
    private bool CheckNewPositionisFree(int newXPos, int newZPos)
    {
        if (newXPos >= 0 && newXPos < gameManager.playArea.GetLength(1) && newZPos >= 0 && newZPos < gameManager.playArea.GetLength(0))
        {
            if (gameManager.playArea[newZPos, newXPos] == null)
            {
                obstacleAhead = false;
                return true;
            }
            else
            {
                obstacleAhead = true;
                return false;
            }
        }
        obstacleAhead = true;
        return false;
    }//---


    // ---------------------------------------------------------------------
    private void SetNewPosition(int newXPos, int newZPos)
    {
        if (gameManager.playArea[newZPos, newXPos] == null) // slot is empty
        {
            gameManager.playArea[zPos, xPos] = null; // clear old slot
            gameManager.playArea[newZPos, newXPos] = gameObject; //set new slot                 
            xPos = newXPos; zPos = newZPos;
            currentPosition = nextPosition;
            isMoving = true;
        }
        else
        {          
            isMoving = false;
        }

    }//----

    // ---------------------------------------------------------------------
    private void MoveUnit()
    {
        if (isMoving)
        {
            // check distance to new position from  current position
            Vector2 movePos = new(currentPosition.x, currentPosition.z);
            Vector2 curentRealPos = new(transform.position.x, transform.position.z);

            if (Vector2.Distance(movePos, curentRealPos) > 0.1F) // Move Unit to new slot 10cm from centre
            {
                transform.LookAt(currentPosition);
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


