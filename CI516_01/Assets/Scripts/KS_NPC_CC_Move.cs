using UnityEngine;
using System.Collections;

//CC Move

[RequireComponent(typeof(CharacterController))]

public class KS_NPC_CC_Move : MonoBehaviour
{

    private CharacterController cc_attached;
    public Vector3 v3_direction = new(0, 0, 0);
    public float fl_speed = 1;
    public bool bl_moving = true;

    // Use this for initialization
    void Start()
    {
        cc_attached = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bl_moving) MoveNPC();
    }

    private void MoveNPC()
    {
        cc_attached.SimpleMove(fl_speed * transform.TransformDirection(Vector3.forward));
    }

    private void ChangeDirection()
    {
        print("Changing Direction");
        transform.Rotate(0, Random.Range(100, 260), 0);
        bl_moving = true;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.gameObject.CompareTag("Ground"))
        {
            print("hit Something");
            bl_moving = false;
            ChangeDirection();
        }
    }
}
