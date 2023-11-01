// ----------------------------------------------------------------------
// --------------------  AI: Team Class
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class DD_Team : MonoBehaviour
{

    public int team_id = 0;
    public string team_name = "Team X";
    public int team_members = 50;
    public int activeTeamMembers = 0;
    public Vector2 team_position = Vector2.zero;
    public Heading spawnDirection = Heading.east;
  
 

    public GameObject baseObjectPrefab = null;
    public GameObject baseObject = null;

    public GameObject unit = null;
    private List<GameObject> teamUnits = new();

    public float teamResources = 100;

   
    private void Start()
    {

        // Spawn Base at start Pos


        // Create list of units or Array?

        // Insantiate off screen and set inactive

        // Creat Units  - OR sub contract to unit spawner / base 




    }//-----




}//==========
