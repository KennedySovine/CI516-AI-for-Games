// ----------------------------------------------------------------------
// --------------------  AI: Unit Health
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using UnityEngine;

public class DD_UnitHealth : MonoBehaviour
{
    // ---------------------------------------------------------------------
    public DD_Unit unitScript = null;
    private GameObject hpBar = null;
    // private GameObject hitText = null;
    public float maxHealth = 100;

    // ---------------------------------------------------------------------
    void Start()
    {
        unitScript = GetComponentInParent<DD_Unit>();
        CreateHPBar();
    }
    //----  


    // ---------------------------------------------------------------------
    void FixedUpdate()
    {
        UpdateHPBar();
        CheckHealthLevel();
    }//-----



    // ---------------------------------------------------------------------
    private void CheckHealthLevel()
    {

        if (unitScript.health < 0)
        {
            // Move the unit off the board
            int newX = unitScript.unitID;
            int newZ = -3 - unitScript.team.teamID;
            unitScript.transform.position = new(newX, 0, newZ);

            // Clear the array
            unitScript.gameManager.playArea[(int)unitScript.currentPosition.z, (int)unitScript.currentPosition.x] = null;

            // Reduce Team active members
            unitScript.team.activeTeamMembers--;
            unitScript.gameManager.activeUnits.Remove(unitScript.gameObject);
            unitScript.health = maxHealth;

            // Deactivate unit
            unitScript.isAlive = false;
        }
    }//-----



    // ---------------------------------------------------------------------
    public void Damage(float damage) // Damage Receiver
    {
        unitScript.health -= damage;
        // GameObject.Instantiate(hitText, Vector3.zero, Quaternion.identity);

        print("taking damage");

    }//-----


    private void UpdateHPBar()
    {
        float currentHealth = unitScript.health;

        if (currentHealth < 0) return;

        // Resize and colour the bar based on current HP
        hpBar.transform.localScale = new Vector3((currentHealth / maxHealth), 0.1F, 0.1F);

        if (currentHealth > maxHealth / 2)
            hpBar.GetComponent<Renderer>().material.color = Color.green;

        if (currentHealth > maxHealth / 4 && currentHealth < maxHealth / 2)
            hpBar.GetComponent<Renderer>().material.color = Color.yellow;

        if (currentHealth < maxHealth / 4)
            hpBar.GetComponent<Renderer>().material.color = Color.red;

    }//-----


    // ---------------------------------------------------------------------
    private void CreateHPBar()
    {
        hpBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hpBar.transform.position = new(transform.position.x, 1.5f, transform.position.z);
        hpBar.transform.localScale = new(1, 0.1f, 0.1f);
        hpBar.transform.SetParent(transform);
    }//----


}//==========
