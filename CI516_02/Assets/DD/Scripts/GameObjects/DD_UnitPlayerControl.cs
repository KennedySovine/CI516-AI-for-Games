// ----------------------------------------------------------------------
// --------------------  AI: Unit Player Control
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------
using TMPro;
using UnityEditor;
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

    private float targetTime = 10.0f;
    private float timer = 0.0f;


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
        if (unitScript.isPlayerControlled == false)  return;
        if (unitScript.isMoving) return;
        if (unitScript.isDepositing) return;

        if (isSelected) // The unit has been selected by the player
        {
            //have object no longer be under control after 10s.
            if (unitScript.unitState == States.idle)
            {
                if (timer >= targetTime)
                {
                    isSelected = false;
                    unitScript.unitState = States.wander;
                }
                else
                {
                    //timer of 10 seconds
                    timer += Time.deltaTime;
                }
                //Debug.Log(targetTime - timer);
            }
            else if (timer != 0.0f)
            {
                timer = 0.0f;
            }

            //Debug.Log(unitScript.unitState);
            // Chase Player Set Target Pos
            playerSetTarget = gameManager.playerSetTargetPos;
            unitScript.targetPosition = playerSetTarget;
            unitScript.unitState = States.chase;
            
         
            // Stop when close
            if (Vector3.Distance(unitScript.targetPosition, unitScript.currentPosition) < unitScript.stopRange)
            {
                unitScript.unitState = States.idle;
            }

        }
        else // Gather resources
        {
            // Check Resource in Range 
            if (Vector3.Distance(unitScript.currentPosition, unitScript.nearestResourcePosition) < unitScript.resourceRange)
            {
                unitScript.targetPosition = unitScript.nearestResourcePosition;
                unitScript.unitState = States.chase;
            }

            if (Vector3.Distance(unitScript.currentPosition, unitScript.nearestResourcePosition) < unitScript.stopRange)
                unitScript.unitState = States.idle;

            if (Vector3.Distance(unitScript.currentPosition, unitScript.nearestResourcePosition) > unitScript.resourceRange)
                unitScript.unitState = States.wander;

            // Harvest
            if (Vector3.Distance(unitScript.nearestResourcePosition, unitScript.currentPosition) <= unitScript.stopRange)
                unitScript.unitState = unitScript.unitState = States.harvest;

            // Depoosit
            if (unitScript.resourceCarrying > unitScript.resourceLimit - 0.1F)
                unitScript.unitState = States.deposit;

            //Combat here?
            if (Vector3.Distance(unitScript.nearestEnemyPosition, unitScript.currentPosition) < unitScript.enemyChaseRange)
                unitScript.AttackEnemy();
        }

        // Combat here?
        if (Vector3.Distance(unitScript.nearestEnemyPosition, unitScript.currentPosition) < unitScript.enemyChaseRange)
            unitScript.AttackEnemy();

        // Obstacle Avoid
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
        highlight= GameObject.CreatePrimitive(PrimitiveType.Cube);
        highlight.transform.position = new(transform.position.x, 0.7f, transform.position.z);
        highlight.transform.localScale = new(0.8f, 0.01f, 0.8f);
        highlight.transform.SetParent(transform);
        highlight.SetActive(false);
    }//----



}//===============================

