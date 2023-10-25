// ----------------------------------------------------------------------
// --------------------  Simple NPC Movement
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------
using UnityEngine;

// Ensure there is a Character Controller Attached
[RequireComponent(typeof(CharacterController))]

public class DD_NPC_CC_Move : MonoBehaviour
{
    // ---------------------------------------------------------------------
    private CharacterController cc_atached;
    public Vector3 v3_direction = new(0, 0, 0);
    public float fl_speed = 1;
    public bool bl_moving = true;

    // ---------------------------------------------------------------------
    void Start()
    {
        cc_atached = GetComponent<CharacterController>();
    }//----

    // ---------------------------------------------------------------------
    void Update()
    {
        if (bl_moving) MoveNPC();
    }//---

    // ---------------------------------------------------------------------
    private void MoveNPC()
    {
        cc_atached.SimpleMove(fl_speed * transform.TransformDirection(Vector3.forward));
    }//----

    // ---------------------------------------------------------------------
    private void ChangeDirection()
    {
        print("Changing Direction");
        transform.Rotate(0, Random.Range(100, 260), 0);
        bl_moving = true;
    }//---

    // ---------------------------------------------------------------------
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.gameObject.CompareTag("Ground"))
        {
            print("hit Something");
            bl_moving = false;
            ChangeDirection();
        }
    }//---

}//==========