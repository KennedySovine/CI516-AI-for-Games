// ----------------------------------------------------------------------
// --------------------  AI: Unit Player Control
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

public class DD_UnitPlayerControl : MonoBehaviour
{
    // ---------------------------------------------------------------------
    private DD_GameManager gameManager;
    private DD_Unit unitScript = null;
    public Vector3 playerSetTarget = Vector3.zero;
    public DD_Team parentTeam;

    // squad
    [Header("Squad Stuff")]
    public int squadID = -1;
    public int squadPattern;
    public bool isSquadLeader;

    private GameObject highlight;
    public bool isSelected = false;

    // check if squad active
    // Check unit is part of squad

    // ---------------------------------------------------------------------
    private void Start()
    {
        // The game manager will be use to access the game board
        gameManager = GameObject.Find("GameManager").GetComponent<DD_GameManager>();
        parentTeam = GetComponentInParent<DD_Team>();
        // Unit Control script
        unitScript = GetComponent<DD_Unit>();
        CreateHighlight();
    }//-----


    // ---------------------------------------------------------------------
    private void FixedUpdate()
    {
        UpdateHighlight();
        StateManagerPlayerControl();
        ManageSquad();
    }//-----



    // ---------------------------------------------------------------------
    public GameObject squadLeader;

    private void ManageSquad()
    {
        if (unitScript.isMoving) return;

        if (!parentTeam.squadActive)
        {
            isSquadLeader = false;
        }
        else
        {// check pos in squad
            squadID = -1;
            int index = 1;
            foreach (GameObject unit in parentTeam.squad)
            {
                if (index == 1) squadLeader = unit;

                if (unit == this.gameObject)
                {
                    squadID = index;
                }
                index++;
            }
        }

        if (squadID == 1)
            isSquadLeader = true;
        else
            isSquadLeader = false;

        if (squadID < 1) squadLeader = null;
    }//-----


    public bool useArrayFormation = false;

    // ---------------------------------------------------------------------
    private void SetSquadPosition()
    {
        int xOffset = 0, zOffset = 0;
        if (useArrayFormation)
        {
            int leaderOffsetX = 0, leaderOffsetZ = 0;

            for (int row = 0; row < parentTeam.squadShape.GetLength(0); row++)
            {
                for (int col = 0; col < parentTeam.squadShape.GetLength(1); col++)
                {
                    if (parentTeam.squadShape[row, col] == 1)
                    {
                        leaderOffsetX = col;
                        leaderOffsetZ = row;
                    }
                    if (parentTeam.squadShape[row, col] == squadID)
                    {
                        xOffset = col; zOffset = row;
                    }
                }
            }
            xOffset -= leaderOffsetX; zOffset -= leaderOffsetZ;
        }
        else// use Algorithm 
        {
            xOffset = 0;
            zOffset = squadID ;
        }

        unitScript.targetPosition = new(squadLeader.transform.position.x + xOffset, 0, +squadLeader.transform.position.z + zOffset);

        // Check if enemy is close
        if (Vector3.Distance(unitScript.nearestEnemyPosition, unitScript.currentPosition) < unitScript.enemyChaseRange)
        {
            unitScript.AttackEnemy();
        }

        // Move to squad Postion
        if (Vector3.Distance(unitScript.targetPosition, unitScript.currentPosition) > unitScript.stopRange)
            unitScript.ChaseDirect(false);

        // Avoid Obstacle
        if (unitScript.obstacleAhead)
        {
            unitScript.unitState = States.roam;
            unitScript.obstacleAhead = false;
        }

    }//-----


    // ---------------------------------------------------------------------
    private void StateManagerPlayerControl()
    {
        if (unitScript.isPlayerControlled == false) return;
        if (unitScript.isMoving) return;
        if (unitScript.isDepositing) return;


        if (squadID > 1) // part of squad but not leader
        {
            SetSquadPosition();
            if (Vector3.Distance(unitScript.nearestEnemyPosition, unitScript.currentPosition) < unitScript.enemyChaseRange)
            {
                unitScript.AttackEnemy();
            }
            return;
        }


        if (isSelected)
        {
            unitScript.WaypointMoveToTarget(gameManager.playerSetTargetPos);
        }
        else // Wander, gather and fight
        {
            unitScript.unitState = States.wander;

            // Check Resource in Range 
            if (Vector3.Distance(unitScript.currentPosition, unitScript.nearestResourcePosition) < unitScript.resourceRange)
            {
                // Check Resource is in Line of sight before moving towards it
                if (gameManager.ai.CheckTargetInLineOfSight(unitScript.currentPosition, unitScript.nearestResourcePosition))
                {
                    unitScript.targetPosition = unitScript.nearestResourcePosition;
                    unitScript.unitState = States.chase;
                }

            }

            if (Vector3.Distance(unitScript.currentPosition, unitScript.nearestResourcePosition) < unitScript.stopRange)
                unitScript.unitState = States.idle;

            // wander if out of range 
            if (Vector3.Distance(unitScript.currentPosition, unitScript.nearestResourcePosition) > unitScript.resourceRange)
                unitScript.unitState = States.wander;

            // Harvest Resource if close
            if (Vector3.Distance(unitScript.nearestResourcePosition, unitScript.currentPosition) <= unitScript.stopRange)
                unitScript.unitState = States.harvest;

            // Depoit Resource when full
            if (unitScript.resourceCarrying > unitScript.resourceLimit - 0.1F)
                unitScript.unitState = States.deposit;

            // Check if enemy is close
            if (Vector3.Distance(unitScript.nearestEnemyPosition, unitScript.currentPosition) < unitScript.enemyChaseRange)
            {
                unitScript.AttackEnemy();
            }
        }

        // is the path blocked and not wandering
        if (unitScript.unitState != States.wander)
        {
            if (unitScript.obstacleAhead)
            {
                unitScript.unitState = States.roam;
                unitScript.obstacleAhead = false;
            }
        }
    }//-----

    // ---------------------------------------------------------------------
    private void UpdateHighlight()
    {
        if (isSelected)
            highlight.SetActive(true);
        else
            highlight.SetActive(false);
    }//-----


    // ---------------------------------------------------------------------
    private void CreateHighlight()
    {
        highlight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        highlight.transform.position = new(transform.position.x, 0.7f, transform.position.z);
        highlight.transform.localScale = new(0.8f, 0.01f, 0.8f);
        highlight.transform.SetParent(transform);
        highlight.SetActive(false);
    }//----



}//===============================

