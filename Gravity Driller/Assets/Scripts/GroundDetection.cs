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
            //Debug.Log("Ground Detected");
            myPlayerScript.SetIsGrounded(true);
            myPlayerScript.SetJumpBoostPercent(100);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            myPlayerScript.SetIsGrounded(false);
            myPlayerScript.SetJumpBoostPercent(100);
            myPlayerScript.SetJumpLock(true);
        }
    }
}
