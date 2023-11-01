// ----------------------------------------------------------------------
// --------------------  AI: Base Object Class
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using UnityEngine;

public abstract class DD_BaseObject : MonoBehaviour
{
    // This will Appear on Derived Class Ojects
    public bool isAlive = false;
    public float health = 100;
    protected int xPos, zPos; // position in array


}//===========
