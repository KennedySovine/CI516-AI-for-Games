// ----------------------------------------------------------------------
// --------------------  AI: Base Object Class
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using UnityEngine;

public abstract class DD_BaseObject : MonoBehaviour
{
    [Header("Unit States")]
    // This will Appear on Derived Class Ojects
    public bool isAlive = false;
    public float health = 100;
    public int xPos, zPos; // position in array


}//===========
