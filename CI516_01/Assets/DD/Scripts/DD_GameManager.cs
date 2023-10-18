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

    public double minPercentage = .40;
    public double maxPercentage = .60;

    public int boxAmount= 10;
    public bool filled;

    // ---------------------------------------------------------------------
    private void Awake()// Runs before start on other objects
    {
        // Create Array for play area
        playArea = new GameObject[100, 100]; // rows z, cols x

        // Populate the play area
        AddObjects(500, 1000);
        //StraightLine(boxAmount);
        //RectangleBox(boxAmount, filled);
        AddUnits(100, false, 1);
        AddUnits(100, true, 3);



    }//------

    // ---------------------------------------------------------------------
    void AddUnits(int amountOfUnits, bool colorChange, int corner)
    {
        int[] units = new int[amountOfUnits];

        int halfZ = playArea.GetLength(0) / 2;
        int halfX = playArea.GetLength(1) / 2;
        int newZ = 0;
        int newX = 0;

        foreach (int unit in units)
        {
            switch (corner){
                case 0:
                    newZ = Random.Range(0, playArea.GetLength(0)); // Limit to array length
                    newX = Random.Range(0, playArea.GetLength(1));//Full Board Spawning
                    break;
                case 1:
                    newZ = Random.Range(halfZ, playArea.GetLength(0)); //Top Left
                    newX = Random.Range(0, halfX);
                    break;
                case 2:
                    newZ = Random.Range(halfZ, playArea.GetLength(0)); //Top Right
                    newX = Random.Range(halfX, playArea.GetLength(1));
                    break;
                case 3:
                    newZ = Random.Range(0, halfZ); //Bottom Right
                    newX = Random.Range(halfX, playArea.GetLength(1));
                    break;
                case 4:
                    newZ = Random.Range(0, halfZ); //Bottom left
                    newX = Random.Range(0, halfX);
                    break;
                
            }

            /*int newZ = Random.Range(0, playArea.GetLength(0)); // Limit to array length
            int newX = Random.Range(0, playArea.GetLength(1));//Full Board Spawning*/

            // check if space is empty
            if (playArea[newZ, newX] == null)
            {
                GameObject newUnit = (GameObject)Instantiate(unitPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);
                // Add unit to playArea array and spawn
                if (colorChange){
                    newUnit.GetComponent<Renderer>().material.color = Color.red;
                }
                playArea[newZ, newX] = newUnit;
            }
        }
    }//----

    // ---------------------------------------------------------------------
    private void AddObjects(int min, int max)
    {
        //Add a Random number of Box Objects
        int[] crates = new int[Random.Range(min, max)];

        int halfZ = playArea.GetLength(0) / 2;
        int halfX = playArea.GetLength(1) / 2;

        for (int i = 0; i < crates.Length; i++)
        {
            /*int newZ = Random.Range(0, playArea.GetLength(0));
            int newX = Random.Range(0, playArea.GetLength(1));*/

            //Spawns in middle or somewhere i guess
            int newZ = Random.Range((int)(playArea.GetLength(0) * minPercentage), (int)(playArea.GetLength(0) * maxPercentage));
            int newX = Random.Range((int)(playArea.GetLength(1) * minPercentage), (int)(playArea.GetLength(1) * maxPercentage));

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

    private void StraightLine(int numBoxes){
        int[] crates = new int[numBoxes];

        int randomNumGen = Random.Range(0, 2);
        bool vertical;
        int newZ;
        int newX;

        if (randomNumGen == 1) {
            newZ = Random.Range(0, playArea.GetLength(0));
            newX = Random.Range(0, playArea.GetLength(1) - numBoxes);
            vertical = false;
        }
        else{
            newZ = Random.Range(0, playArea.GetLength(0)- numBoxes);
            newX = Random.Range(0, playArea.GetLength(1));
            vertical = true;
        }

        for (int i = 0; i < crates.Length; i++)
        {
            // check if space is empty
            if (playArea[newZ, newX] == null)
            {
                // Add block object to array
                playArea[newZ, newX] = (GameObject)Instantiate(boxPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                // child to Objects to keep Hierachy Organised
                playArea[newZ, newX].transform.parent = objectParent.transform;
            }

            if (vertical)
                newZ++;
            else
                newX++;
        }
    }

    public void RectangleBox(int sideLength, bool fill){
        int thing = 0;

        int newZ = Random.Range(0, playArea.GetLength(0) - sideLength);
        int newX = Random.Range(0, playArea.GetLength(1) - sideLength);
        int regX = newX;

        if (fill){
            for (int i = 0; i < sideLength; i++){
                newX = regX;
                for (int j = 0; j < sideLength; j++){
                    // check if space is empty
                    if (playArea[newZ, newX] == null){
                        // Add block object to array
                        playArea[newZ, newX] = (GameObject)Instantiate(boxPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                        // child to Objects to keep Hierachy Organised
                        playArea[newZ, newX].transform.parent = objectParent.transform;
                    }
                    newX++;
                }
                newZ++;
            }
        }
        else{
            for (int i = 0; i < 4 ; i++){
                switch (i){
                    case 1:
                        thing = 1;
                        break;
                    case 2:
                        thing = 0;
                        break;
                    case 3:
                        thing = 1;
                        break;
            }
            for (int j = 0; j < sideLength - thing; j++){
                // check if space is empty
                if (playArea[newZ, newX] == null)
                {
                    // Add block object to array
                    playArea[newZ, newX] = (GameObject)Instantiate(boxPrefab, new Vector3((float)newX, -0.5f, (float)newZ), transform.rotation);

                    // child to Objects to keep Hierachy Organised
                    playArea[newZ, newX].transform.parent = objectParent.transform;
                }
                switch (i){
                    case 0:
                        newX++;
                        break;
                    case 1:
                        newZ++;
                        break;
                    case 2:
                        newX--;
                        break;
                    case 3:
                        newZ--;
                        break;
                }
            }
        }

            }
    }//==========
}