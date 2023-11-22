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
 


        if (isSelected) // The unit has been selected by the player
        {
            // Chase Player Set Target Pos         
         
            // Stop when close                     

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

         
            
            // Harvest

            // Depoosit

            //Combat here?
        }
     
        // Combat here?

        // Obstacle Avoid

       
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

