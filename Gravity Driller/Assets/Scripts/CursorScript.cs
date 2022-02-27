using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    [SerializeField]
    Camera sceneCam;
    [SerializeField]
    GameObject mySpriteObject;
    [SerializeField]
    GameObject playerObject;

    KeyCode leftClick = KeyCode.Mouse0;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        UpdatePosition();
        ProcessClicks();
    }

    private void UpdatePosition()
    {
        Vector3 newPos = sceneCam.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0;
        transform.position = newPos;
        mySpriteObject.GetComponent<LineRenderer>().SetPosition(0, playerObject.transform.position);
        mySpriteObject.GetComponent<LineRenderer>().SetPosition(1, transform.position);
    }

    private void ActivateDrillGUI()
    {
        GamespeedManager.GamespeedToSlow();
        mySpriteObject.GetComponent<LineRenderer>().enabled = true;
    }

    private void DeactivateDrillGUI()
    {
        GamespeedManager.GamespeedToNormal();
        mySpriteObject.GetComponent<LineRenderer>().enabled = false;
    }

    private void ProcessClicks()
    {
        if (Input.GetKeyDown(leftClick))
        {
            //The player has started aiming the drill move; slow down time and enable the guide objects.
            ActivateDrillGUI();
        }
        else if (Input.GetKeyUp(leftClick))
        {
            //The player has released the aim button; time returns to normal speed, activate drill move if cursor was not in the deadzone.
            DeactivateDrillGUI();
        }
        
        if (Input.GetKey(leftClick))
        {

        }
    }
}
