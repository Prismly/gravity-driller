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
    Player myPlayer;

    bool inDeadzone = false;
    bool keyHeld = false;

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
        mySpriteObject.GetComponent<LineRenderer>().SetPosition(0, myPlayer.transform.position);
        mySpriteObject.GetComponent<LineRenderer>().SetPosition(1, transform.position);
    }

    private void ProcessClicks()
    {
        if (Input.GetKey(leftClick) && myPlayer.GetCanDrillDash())
        {
            //The player has started aiming the drill move; slow down time and enable the guide objects.
            GamespeedManager.GamespeedToSlow();
            keyHeld = true;
        }
        else if (Input.GetKeyUp(leftClick) && myPlayer.GetCanDrillDash())
        {
            //The player has released the aim button; time returns to normal speed, activate drill move if cursor was not in the deadzone.
            GamespeedManager.GamespeedToNormal();
            keyHeld = false;
            if (!inDeadzone)
            {
                //Valid fire location; activate Player drilling routine
                myPlayer.SetIsDrillDashing(true, transform.position);
            }
        }

        if (keyHeld && !inDeadzone)
        {
            mySpriteObject.GetComponent<LineRenderer>().enabled = true;
        }
        else
        {
            mySpriteObject.GetComponent<LineRenderer>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inDeadzone = collision.tag == "Deadzone";
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inDeadzone = collision.tag != "Deadzone";
    }
}
