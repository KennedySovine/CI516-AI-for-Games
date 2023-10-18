// ----------------------------------------------------------------------
// --------------------  AI Game Manager
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DD_GameManager : MonoBehaviour
{
    // ---------------------------------------------------------------------
    // Game Data
    public GameObject[,] playArea;
    // objects
    public GameObject unitPrefab;
    public GameObject boxPrefab;
    public GameObject objectParent;

    // ---------------------------------------------------------------------
    private void Awake()// Runs before start on other objects
    {
        // Create Array for play area
        playArea = new GameObject[100, 100]; // rows z, cols x

        // Populate the play area
        AddObjects(500, 1000);
        AddUnits(50);


    }//------

    // ---------------------------------------------------------------------
    void AddUnits(int amountOfUnits)
    {
        int[] units = new int[amountOfUnits];

        foreach (int unit in units)
        {
            int newZ = Random.Range(0, playArea.GetLength(0)); // Limit to array length
            int newX = Random.Range(0, playArea.GetLength(1));

            // check if space is empty
            if (playArea[newZ, newX] == null)
            {
                // Add unit to playArea array and spawn
                playArea[newZ, newX] = (GameObject)Instantiate(unitPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);
            }
        }
    }//----

    // ---------------------------------------------------------------------
    private void AddObjects(int min, int max)
    {
        //Add a Random number of Box Objects
        int[] crates = new int[Random.Range(min, max)];

        for (int i = 0; i < crates.Length; i++)
        {
            int newZ = Random.Range(0, playArea.GetLength(0));
            int newX = Random.Range(0, playArea.GetLength(1));

            // check if space is empty
            if (playArea[newZ, newX] == null)
            {
                // Add block object to array
                playArea[newZ, newX] = (GameObject)Instantiate(boxPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                // child to Objects to keep Hierachy Organised
                playArea[newZ, newX].transform.parent = objectParent.transform;
            }
        }
    }//------   

}//==========