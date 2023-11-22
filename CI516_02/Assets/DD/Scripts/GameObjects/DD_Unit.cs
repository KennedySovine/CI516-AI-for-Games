// ----------------------------------------------------------------------
// --------------------  AI: Unit Object Class
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

// Enums
public enum States { idle, roam, wander, chase, attack, flee, harvest, deposit }
public enum Heading { north, east, south, west }

public class DD_Unit : DD_BaseObject
{
    // ---------------------------------------------------------------------    
    public DD_Team team;
    public DD_GameManager gameManager;
    public int unitID = -1;
    public States unitState = States.idle;
    public bool isPlayerControlled = false;

    // Position Variables
    [Header("Unit Positions")]
    public Vector3 basePosition = Vector3.zero;
    public Vector3 targetPosition = Vector3.zero;
    public Vector3 currentPosition = Vector3.zero;
    public Vector3 startPosition = Vector3.zero;
    public Vector3 nextPosition = Vector3.zero;

    [Header("Movement")]
    public float speed = 1;
    public bool isMoving = false;
    public float fleeRange = 20;
    public float stopRange = 2;
    // Wander vars
    public bool obstacleAhead = false;
    public Heading unitHeading;

    [Header("Resources")]
    public float resourceRange = 18;
    public float resourceCarrying = 0;
    public float resourceLimit = 10;
    public float resourceHarvestSpeed = 5;
    public bool isDepositing = false;
    public Vector3 nearestResourcePosition = new(-50, 50, -50);
    private GameObject nearestResource = null;

    [Header("Combat")]
    public int ammo = 1000;
    public Vector3 nearestEnemyPosition = new(-50, 50, -50);
    private GameObject nearestEnemy = null;
    public float enemyChaseRange = 20;
    public float attackRange = 10;
    public float attackCoolDown = 1;
    public GameObject attackPF = null;
    private float nextAttackTime = 0;

    public List<int> friendsIDs = new();

    // ---------------------------------------------------------------------
    private void Start()
    {
        // The game manager will be use to access the game board
        gameManager = GameObject.Find("GameManager").GetComponent<DD_GameManager>();

        // Round off postion to nearest int ands store the current position
        xPos = (int)Mathf.Round(transform.position.x);
        zPos = (int)Mathf.Round(transform.position.z);
        transform.position = new Vector3(xPos, 0, zPos);
        currentPosition = transform.position;

        // Set initial states
        unitHeading = (Heading)Random.Range(0, 4);
      
        if (!isPlayerControlled)
            unitState = States.wander;
        else 
            unitState = States.idle;


        // Set Teams to ignore for Attacks
        friendsIDs = new() { team.teamID }; // Start with the team the unit belong to
        foreach (int team in team.friendlyTeams) // Add the friends teams
        {
            friendsIDs.Add(team);
        }

    }//----

    // ---------------------------------------------------------------------
    private void FixedUpdate()
    {
        if (isAlive)
        {
            if (!isMoving)
            {
                FindNearestResource();
                FindNearestEnemy();
                StateManager();
                UnitActions();
            }
            MoveUnit();
        }
    }//---

    // ---------------------------------------------------------------------
    private void UnitActions()
    {
        if (unitState == States.roam) Roam();
        if (unitState == States.wander) Wander();
        if (unitState == States.chase) ChaseDirect(false);
        if (unitState == States.flee) ChaseDirect(true);
        if (unitState == States.harvest) HarvestResource();
        if (unitState == States.deposit) DepositResource();
        if (unitState == States.attack) AttackEnemy();

    }//------


    // ---------------------------------------------------------------------
    private void StateManager()
    {
        // Enemy Target may need resetting if not targets exist

        if (isPlayerControlled) return;
        if (isMoving) return;
        if (isDepositing) return;

        // Check if enemy is close
        if (Vector3.Distance(nearestEnemyPosition, currentPosition) < enemyChaseRange)
        {
            AttackEnemy();
        }
        else
        {
            // Check Resource in Range 
            if (Vector3.Distance(currentPosition, nearestResourcePosition) < resourceRange)
            {
                targetPosition = nearestResourcePosition;
                unitState = States.chase;
            }

            if (Vector3.Distance(currentPosition, nearestResourcePosition) < stopRange)
                unitState = States.idle;

            // wander if out of range 
            if (Vector3.Distance(currentPosition, nearestResourcePosition) > resourceRange)
                unitState = States.wander;


            // Harvest Resource if close
            if (Vector3.Distance(nearestResourcePosition, currentPosition) <= stopRange)
                unitState = States.harvest;

            // Depoit Resource when full
            if (resourceCarrying > resourceLimit - 0.1F)
                unitState = States.deposit;

        }

        // is the path blocked and not wandering
        if (unitState != States.wander)
        {
            if (obstacleAhead)
            {
                unitState = States.roam;
                obstacleAhead = false;
            }
        }
    }//----



    //                   ****************   COMBAT ***************************
    // ---------------------------------------------------------------------
    public void AttackEnemy()
    {
        if (!isAlive) return;

        if (!nearestEnemy)
        {
            nearestEnemyPosition = new(-50, -50, -50); // out of range
            unitState = States.wander;
            return; // no enemy found
        }
        //Enemy in Chase Range
        if (Vector3.Distance(nearestEnemyPosition, currentPosition) < enemyChaseRange && Vector3.Distance(nearestEnemyPosition, currentPosition) > attackRange)
        {
            targetPosition = nearestEnemyPosition;
            unitState = States.chase;
        }
        else if (Vector3.Distance(nearestEnemyPosition, currentPosition) < attackRange) // in Attack range
        {
            targetPosition = nearestEnemyPosition;
            unitState = States.idle;
            SendAttack();
        }
    }//-----


    private void SendAttack()
    {
        if (!attackPF) return; // no bullet object referenced

        if (nextAttackTime < Time.time)
        {
            transform.LookAt(nearestEnemyPosition);
            GameObject unitAttack = Instantiate(attackPF, gameObject.transform);
            unitAttack.transform.SetParent(null);
            unitAttack.GetComponent<DD_Attack>().teamID = team.teamID;
            nextAttackTime = Time.time + attackCoolDown;
        }
    }//-----


    // ---------------------------------------------------------------------
    private void FindNearestEnemy()
    {
        // Use the list of active resoures in the Game Manager if any units are in it
        if (gameManager.activeUnits.Count > 0)
        {
            // nearest Enemy 
            GameObject closestEnemy = null;
            float distanceToEnemy = Mathf.Infinity;

            foreach (GameObject foundActiveUnit in gameManager.activeUnits) // Loop through list of units
            {
                bool friendFound = false;

                // ignore own team members and Friends
                foreach (int friendID in friendsIDs)
                {
                    if (foundActiveUnit.GetComponent<DD_Unit>().team.teamID == friendID) friendFound = true;
                }

                if (friendFound) continue; // skip forward to next unit in Acive List

                float currentNearestDistance = Vector3.Distance(foundActiveUnit.transform.position, currentPosition);

                if (currentNearestDistance < distanceToEnemy)
                {
                    closestEnemy = foundActiveUnit;
                    distanceToEnemy = currentNearestDistance;
                }
            }

            // Set Target of Enemy Unit
            if (closestEnemy != null)
            {     
                nearestEnemyPosition = new((int)Mathf.Round(closestEnemy.transform.position.x), 0, (int)Mathf.Round(closestEnemy.transform.position.z));
                nearestEnemy = closestEnemy;
            }
            else
            {
                nearestEnemyPosition = new(-100, -100, -100);
            }
        }
    }//-----




    private void FindNearestAmmo()
    {


    }

    private void FindHealth()
    {

    }



    //                      ****************   RESOURCES   ***************************

    // ---------------------------------------------------------------------
    private void FindNearestResource()
    {
        if (unitState == States.wander)
        {
            // Use the list of active resoures in the Game Manager
            if (gameManager.activeResources.Count > 0)
            {
                // Find the nearest
                GameObject closestResource = null;
                float distanceToResource = Mathf.Infinity;

                foreach (GameObject resource in gameManager.activeResources) // Loop though list of resources
                {
                    float currentNearestDistance = Vector3.Distance(resource.transform.position, currentPosition);

                    if (currentNearestDistance < distanceToResource)
                    {
                        closestResource = resource;
                        distanceToResource = currentNearestDistance;
                    }
                }
                // Set Target of resourse
                if (closestResource != null)
                {
                    nearestResourcePosition = new((int)Mathf.Round(closestResource.transform.position.x), 0, (int)Mathf.Round(closestResource.transform.position.z));
                    nearestResource = closestResource;
                    targetPosition = nearestResourcePosition;
                }
            }
        }
    }//----


    // ---------------------------------------------------------------------
    private void HarvestResource()
    {
        if (!nearestResource)
        {
            nearestResourcePosition = new(-50, -50, -50); // out of range
            unitState = States.wander;
            return; // no nearest resource found
        }

        if (Vector3.Distance(nearestResourcePosition, currentPosition) <= resourceRange) // in range
        {
            if (nearestResource)
            {
                if (nearestResource.GetComponent<DD_Resource>().resourceHeld > resourceHarvestSpeed * Time.deltaTime)
                {
                    nearestResource.GetComponent<DD_Resource>().GetResource(resourceHarvestSpeed * Time.deltaTime);
                    resourceCarrying += resourceHarvestSpeed * Time.deltaTime;
                }
            }
            else
            {
                nearestResourcePosition = new(-50, -50, -50); // out of range
                unitState = States.wander;
            }
        }
        else
        {
            unitState = States.wander;
        }
    }//-----

    // ---------------------------------------------------------------------
    private void DepositResource()
    {
        // Move home
        if (Vector3.Distance(basePosition, currentPosition) > stopRange)
        {
            targetPosition = basePosition;
            ChaseDirect(false);
        }
        else // Unit is Close to home
        {
            // Stop depositing when unit resource almost empty
            if (resourceCarrying > 0 && resourceCarrying < 0.2F)
            {
                isDepositing = false;
                unitState = States.wander;
            }
            else
            {
                isDepositing = true;
            }
            if (resourceCarrying > resourceHarvestSpeed * Time.deltaTime)
            {
                team.DepositResource(resourceHarvestSpeed * Time.deltaTime);
                resourceCarrying -= (resourceHarvestSpeed * Time.deltaTime);
            }
        }
    }//-----




    //                      ****************   MOVEMENT   ***************************

    // ---------------------------------------------------------------------
    private void ChaseDirect(bool reverse)
    {
        if (!isMoving) // Move to target if unit is not moving
        {

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
            nextPosition = new Vector3((int)Mathf.Round(nextPosition.x), 0, (int)Mathf.Round(nextPosition.z));
            int newX = (int)Mathf.Round(nextPosition.x);
            int newZ = (int)Mathf.Round(nextPosition.z);

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
                nextPosition = new Vector3((int)Mathf.Round(nextPosition.x), 0, (int)Mathf.Round(nextPosition.z));
                int newX = (int)Mathf.Round(nextPosition.x);
                int newZ = (int)Mathf.Round(nextPosition.z);

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
            nextPosition = new Vector3((int)Mathf.Round(nextPosition.x), 0, (int)Mathf.Round(nextPosition.z));
            int newX = (int)Mathf.Round(nextPosition.x);
            int newZ = (int)Mathf.Round(nextPosition.z);

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


