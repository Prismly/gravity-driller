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

    [SerializeField]
    private float drillCooldownMax;
    [SerializeField]
    private float drillCooldown;
    [SerializeField]
    private bool drillCooldownIsTicking = false;

    KeyCode leftClick = KeyCode.Mouse0;

    private void Start()
    {
        Cursor.visible = false;
        drillCooldown = drillCooldownMax;
    }

    private void Update()
    {
        UpdatePosition();

        if (drillCooldown <= 0)
        {
            PauseDrillDashCooldown();
        }
        else
        {
            drillCooldown -= Time.deltaTime;
        }

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

    public void ResetDrillDashCooldown()
    {
        drillCooldownIsTicking = true;
        drillCooldown = drillCooldownMax;
    }

    public void PauseDrillDashCooldown()
    {
        drillCooldownIsTicking = false;
    }

    public void ResumeDrillDashCooldown()
    {
        drillCooldownIsTicking = true;
    }

    public bool IsDrillDashCooldownOver()
    {
        return drillCooldown <= 0;
    }

    private void ProcessClicks()
    {
        if (Input.GetKeyDown(leftClick))
        {
            //The player has started aiming the drill move; slow down time and enable the guide objects.
            GamespeedManager.GamespeedToSlow();
            keyHeld = true;
        }
        else if (Input.GetKeyUp(leftClick))
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
