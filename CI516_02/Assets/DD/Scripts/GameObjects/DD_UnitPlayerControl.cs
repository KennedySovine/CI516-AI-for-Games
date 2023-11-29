// ----------------------------------------------------------------------
// --------------------  AI: Unit Player Control
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_UnitPlayerControl : MonoBehaviour
{
    // ---------------------------------------------------------------------
    private DD_GameManager gameManager;
    private DD_Unit unitScript = null;
    public Vector3 playerSetTarget = Vector3.zero;

    // squad
    [Header("Squad Stuff")]
    public int squadID = -1;
    public int squadPattern;
    public bool isSquadLeader;

    private GameObject highlight;
    public bool isSelected = false;


    // ---------------------------------------------------------------------
    private void Start()
    {
        // The game manager will be use to access the game board
        gameManager = GameObject.Find("GameManager").GetComponent<DD_GameManager>();
        // Unit Control script
        unitScript = GetComponent<DD_Unit>();

        CreateHighlight();
    }//-----


    // ---------------------------------------------------------------------
    private void FixedUpdate()
    {
        UpdateHighlight();
        StateManagerPlayerControl();
    }//-----



    // ---------------------------------------------------------------------
    private void StateManagerPlayerControl()
    {
        if (unitScript.isPlayerControlled == false) return;
        if (unitScript.isMoving) return;
        if (unitScript.isDepositing) return;

        if (isSelected)
        {
            unitScript.WaypointMoveToTarget( gameManager.playerSetTargetPos);
          
        }
        else // Wander, gather and fight
        {
            //unitScript.unitState = States.wander;
            //Vector3 nearestWP = gameManager.ai.GetNearestWaypointPos(unitScript.currentPosition);
            Vector3[] nearestWPs = gameManager.ai.GetNearestWaypointstoPos(unitScript.currentPosition, 2);
            unitScript.targetPosition = nearestWPs[Random.Range(0, 1)];
            unitScript.unitState = States.chase;
            StartCoroutine(WPtoWander());
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
            //if (Vector3.Distance(unitScript.currentPosition, unitScript.nearestResourcePosition) > unitScript.resourceRange)
                //unitScript.unitState = States.wander;

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

    IEnumerator WPtoWander()
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }



}//===============================

