// ----------------------------------------------------------------------
// --------------------  AI: Hit Text
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using UnityEngine;

public class DD_HitText : MonoBehaviour
{

    public float fl_life_time = 1;
    public float fl_speed = 5;


    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, fl_life_time);
     //   transform.LookAt(GameObject.Find("Main Camera").transform.position);

    }//-----

    // Update is called once per frame
    void Update()
    {
        transform.position = new (transform.position.x, transform.position.y + fl_speed * Time.deltaTime, transform.position.z);
    }//-----

}//==========