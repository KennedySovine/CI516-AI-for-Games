// Simple NPC
// DD 2023
using UnityEngine;

public class DD_NPC_01 : MonoBehaviour
{
    // ---------------------------------------------------------------------
    public float speed = 1;

    // ---------------------------------------------------------------------
    void Start()
    {
        transform.Rotate(0, Random.Range(1, 360), 0);
    }//---

    // ---------------------------------------------------------------------
    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }//---

    // ---------------------------------------------------------------------
    private void OnCollisionEnter(Collision collision)
    {
        print("hit something");      
        transform.Rotate(0, Random.Range(100, 260), 0);
    }//---

}//=====

