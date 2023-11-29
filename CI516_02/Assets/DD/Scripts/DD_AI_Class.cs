// ----------------------------------------------------------------------
// --------------------  AI: AI Class - Pathfinding
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class DD_AI_Class : MonoBehaviour
{
    // ---------------------------------------------------------------------
    public Vector3[] wayPointPositions = new Vector3[25];
    DD_GameManager gameManager;
 //  public GameObject wpMarker = new();

    // ---------------------------------------------------------------------
    private void Start()
    {
        gameManager = GetComponent<DD_GameManager>();
        CreateWaypoints();
    }//----

    // ---------------------------------------------------------------------
    private void CreateWaypoints()
    {
        // Loop through all map tiles

        for (int row = 0; row < gameManager.levelData.level1.GetLength(0); row++)
        {
            for (int col = 0; col < gameManager.levelData.level1.GetLength(1); col++)
            {
                int newX = col;
                int newZ = gameManager.levelData.level1.GetLength(0) - 1 - row;
              // int newZ = row;

                if (gameManager.levelData.level1[row, col] > 900) 
                {               

                    int currentWaypoint = gameManager.levelData.level1[row, col] - 900; // convert to array index
                    wayPointPositions[currentWaypoint].x = newX;
                    wayPointPositions[currentWaypoint].z = newZ;

                  // GameObject newWP = Instantiate(wpMarker, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);
                  //newWP.GetComponent<TextMesh>().text = currentWaypoint.ToString();
                }
            }
        }
      //  print(wayPointPositions);
    }//---



    // ---------------------------------------------------------------------
    public List<int> GetListOfWaypointsToTarget(Vector3 pStartPos, Vector3 pTargetPos)  // returns a list 
    {
        List<int> wayPointsFound = new();

        // Find Nearest WP to start and Target
        int startWP = -1, targetWP = -1;
        float distanceToStart = 1000, distanceToTarget = 1000;

        // Loop through all WPs    
        for (int i = 1; i < wayPointPositions.GetLength(0); i++)
        {
            // Start Position                 
            float distance = Vector3.Distance(pStartPos, wayPointPositions[i]);

            if (CheckTargetInLineOfSight(pStartPos, wayPointPositions[i]))
            {
                if (distance < distanceToStart)
                {
                    distanceToStart = distance;
                    startWP = i;
                }
            }
            // Target Position
            distance = Vector3.Distance(pTargetPos, wayPointPositions[i]);

            if (CheckTargetInLineOfSight(pTargetPos, wayPointPositions[i]))
            {
                if (distance < distanceToTarget)
                {
                    distanceToTarget = distance;
                    targetWP = i;
                }
            }
        }

        if (distanceToTarget > 100 || distanceToStart > 100)
        {
            print ( "\n position not in sight of a waypoint");
            return wayPointsFound;
        }

        // find Path of Waypoints
        int currentWP = startWP;
        int nextWP = 0;   

        for (int i = 0; i < wayPointPositions.GetLength(0); i++)
        {
            // Add current WP to List
            wayPointsFound.Add(currentWP);

            // Get Next WP from Table
            nextWP = gameManager.levelData.lookUpTable[currentWP - 1,targetWP - 1];

            if (currentWP == targetWP) // found the target
            {          
                break; // stop looping
            }
            else
            {
                currentWP = nextWP;
            }
        }
        print(" WPs in Path: " + wayPointsFound.Count ) ;

        return wayPointsFound;
    }//---




    // ---------------------------------------------------------------------
    public Vector3 GetNearestWaypointPos(Vector3 currentPosition)
    {
        int nearestWP = 0;
        float distanceToNearestWP = 1000; //  a large number off the board initially

        // Loop through all wps
        for (int i = 1; i < wayPointPositions.GetLength(0); i++)
        {
            float distance = Vector3.Distance(currentPosition, wayPointPositions[i]);

            // is waypoint in line of sight
            if (CheckTargetInLineOfSight(currentPosition, wayPointPositions[i]))
            {
                if (distance < distanceToNearestWP)
                {
                    distanceToNearestWP = distance;
                    nearestWP = i;
                }
            }
        }
        Vector3 wayPointPos = new(wayPointPositions[nearestWP].x, 0, wayPointPositions[nearestWP].z);
        return wayPointPos;
    }//---




    // ---------------------------------------------------------------------

    public bool CheckTargetInLineOfSight(Vector3 pStartPos, Vector3 pTargtetPos)
    {
        Vector3 nextPos = Vector3.zero, currentPos = pStartPos;
        bool canSeeTarget = false;
        bool searching = true;

        while (searching)   // Loop until target  == currentPos or tile is not empty
        {
            // Calculate angle to target
            int dX = (int)Mathf.Round(pTargtetPos.x) - (int)Mathf.Round(currentPos.x);
            int dZ = (int)Mathf.Round(pTargtetPos.z) - (int)Mathf.Round(currentPos.z);
            float angle = Mathf.Atan2(dX, dZ);

            // Calculate next Position
            nextPos.x = currentPos.x + Mathf.Round(1.4F * Mathf.Sin(angle));
            nextPos.z = currentPos.z + Mathf.Round(1.4F * Mathf.Cos(angle));

            // Is the next Pos the target?
            if (nextPos.x == pTargtetPos.x && nextPos.z == pTargtetPos.z)
            {
                canSeeTarget = true;
                searching = false; // exit loop                                
            }

            // is the next Pos empty
            if (gameManager.playArea[(int)Mathf.Round(nextPos.z), (int)Mathf.Round(nextPos.x)] != null)
            {
                searching = false; // exit loop                                   
            }
            currentPos = nextPos;
        }
        return canSeeTarget;
    }

}//==========
