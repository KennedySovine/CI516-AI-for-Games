// ----------------------------------------------------------------------
// --------------------  AI: Unit Object Class
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------
using System;
using UnityEngine;

public class DD_Unit : DD_BaseObject
{
    public States unitState = States.idle;
    public DD_GameManager gameManager;

    // Movement Variables
    public Vector3 targetPosition = Vector3.zero;
    public Vector3 currentPostion = Vector3.zero;
    public Vector3 startPosition = Vector3.zero;
    public Vector3 nextPosition = Vector3.zero;
    public GameObject nextSlotObject = null;
    public float speed = 1;
    bool isMoving = false;

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
              
        // Set the initial state
        unitState = States.roam;
    }

    // ---------------------------------------------------------------------
    private void FixedUpdate()
    {
        if (unitState == States.roam)
        {
            Roam();
            MoveUnit();
        }
    }//---

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
          //  print("NextPos " + nextPosition);              

            //check next Postion tile is empty     
            int newX = (int)nextPosition.x;
            int newZ = (int)nextPosition.z;

            if (newX >= 0 && newX < 100 && newZ > 0 && newZ < 100) // is on the board
            {
                nextSlotObject = gameManager.playArea[newZ, newX];

                if (gameManager.playArea[newZ, newX] == null) // slot is empty
                {                  
                    gameManager.playArea[zPos, xPos] = null; // clear old slot
                    gameManager.playArea[newZ, newX] = gameObject; //set new slot                 
                    xPos = newX;
                    zPos = newZ;
                    currentPostion = nextPosition;
                    isMoving = true;
                }
                else
                {
                    print("new slot occupied by " + gameManager.playArea[newZ, newX]);
                    isMoving = false;
                }
            }
        }

    }//-----

    private void MoveUnit()
    {
        if (isMoving)
        {
            // check distance to next position
            Vector2 nextPos = new(nextPosition.x, nextPosition.z);
            Vector2 curentRealPos = new(transform.position.x, transform.position.z);

            if (Vector2.Distance(nextPos, curentRealPos) > 0.01F) // Move Unit to new slot
            {
                transform.LookAt(nextPosition);
                transform.Translate(0, 0, speed * Time.deltaTime);
            }
            else
            {
                print("at target, stopping Moving");
                isMoving = false; // stop moving            
            }
        }
    }//----

}//==========

public enum States { idle, roam, explore, attack, flee }
