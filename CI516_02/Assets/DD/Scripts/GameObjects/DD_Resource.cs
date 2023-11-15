// ----------------------------------------------------------------------
// --------------------  AI: Resource
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using UnityEngine;

public class DD_Resource : DD_BaseObject
{
    // ---------------------------------------------------------------------
    public float resourceHeld = 10;
    private DD_GameManager gameManager;

    private void Start()
    {  // The game manager will be use to access the game board
        gameManager = GameObject.Find("GameManager").GetComponent<DD_GameManager>();
    }//-----

    // ---------------------------------------------------------------------
    private void FixedUpdate()
    {
        CheckResourceLevel();
    }//-----

    // ---------------------------------------------------------------------
    private void CheckResourceLevel()
    {
        if (resourceHeld <= 0.2F)
        {
            xPos = (int)transform.position.x;
            zPos = (int)transform.position.z;
            gameManager.playArea[zPos, xPos] = null; // clear resource from array
            gameManager.activeResources.Remove(gameObject);
            Destroy(gameObject);
        }
    }//-----

    // ---------------------------------------------------------------------
    // Receiver - units access this to steal the resource
    public void GetResource(float amountToHarvest)
    {
        if (resourceHeld > amountToHarvest) resourceHeld -= amountToHarvest;
    }//-----

}//==========

