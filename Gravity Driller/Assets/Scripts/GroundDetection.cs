using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [SerializeField]
    Player myPlayerScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            //Player has landed on some ground.
            myPlayerScript.SetIsGrounded(true);
            myPlayerScript.SetJumpBoostPercent(100);
            myPlayerScript.SetCanDrillDash(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            Debug.Log("GD trigger exit");
            //Player has left the ground they were previously on.
            myPlayerScript.SetIsGrounded(false);
            myPlayerScript.SetJumpBoostPercent(100);
            myPlayerScript.SetJumpLock(true);
        }
    }
}
