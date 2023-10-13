using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple NPC

public class KS_NPC_Move : MonoBehaviour{
    public float speed = 1;
    // Start is called before the first frame update
    void Start(){
        transform.Rotate(0, Random.Range(1, 360), 0);
    }

    // Update is called once per frame
    void Update(){
        transform.Translate(0, 0, speed * Time.deltaTime);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("hit something");
        transform.Rotate(0, Random.Range(100, 260), 0);
    }
}
