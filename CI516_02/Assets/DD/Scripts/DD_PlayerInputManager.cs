// ----------------------------------------------------------------------
// --------------------  AI: Input Manager
// -------------------- David Dorrington, UoB Games, 2023
// ---------------------------------------------------------------------

using UnityEngine;

public class DD_PlayerInputManager : MonoBehaviour
{
    // ---------------------------------------------------------------------
    // Camera Control
    public GameObject gameCamera;
    public float camMoveSpeed = 10f;

    // Mouse Select
    public Vector2 leftClickPostion = new(99,99);
    public Vector2 rightClickPostion = new(99, 99);
    public Vector2 slotClicked = Vector2.zero;
    
    public int lastKeyPressed = 0;
 

    // ---------------------------------------------------------------------
    void Update()
    {       
        GetPlayerMouseClick();
        CameraControl();
    }//-----

  


    // ---------------------------------------------------------------------
    private void CameraControl()
    {
        // Detect input and update camera postion
        float newCamX = gameCamera.transform.position.x + Input.GetAxis("Horizontal") * camMoveSpeed * Time.deltaTime;
        float newCamZ = gameCamera.transform.position.z + Input.GetAxis("Vertical") * camMoveSpeed * Time.deltaTime;
        float newCamY = gameCamera.transform.position.y;

        gameCamera.transform.position = new(newCamX, newCamY, newCamZ);

        // Zoom by adjusting Field of view with mouse wheel
        if (Input.mouseScrollDelta.y < 0 && gameCamera.GetComponent<Camera>().fieldOfView < 90)
            gameCamera.GetComponent<Camera>().fieldOfView++;

        if (Input.mouseScrollDelta.y > 0 && gameCamera.GetComponent<Camera>().fieldOfView > 10)
            gameCamera.GetComponent<Camera>().fieldOfView--;
    }//---

    // ================================================================================

    private void GetPlayerMouseClick()
    {
        //if the left or right mouse button clicked
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // Create Ray cast source and target 
            Ray rayCamToMousePostion = Camera.main.ScreenPointToRay(Input.mousePosition);

            // cast Ray 100m and store data in _rh_hit)
            if (Physics.Raycast(rayCamToMousePostion, out RaycastHit rayHit, 100F))
            {
                // Set Position LM Clicked id on the gameBoard 
                if (Mathf.Round(rayHit.point.x) > 0 && Mathf.Round(rayHit.point.x) < 100
                    && Mathf.Round(rayHit.point.z) > 0 && Mathf.Round(rayHit.point.z) < 100)
                {

                    if (Input.GetMouseButtonDown(0)) // Left Button
                    {
                        leftClickPostion = new((int)rayHit.point.x, (int)rayHit.point.z);
                      //  print("LM - " + leftClickPostion);

                    }

                    if (Input.GetMouseButtonDown(1)) // Right Button
                    {
                        rightClickPostion = new((int)rayHit.point.x, (int)rayHit.point.z);
                       // print("RM - " + rightClickPostion);
                    }
                }
            }
        }
    }//------


}//==========
