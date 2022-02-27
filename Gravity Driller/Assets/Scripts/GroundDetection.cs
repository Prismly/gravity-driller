using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            //Debug.Log("Ground Detected");
            GetComponentInParent<Player>().SetIsGrounded(true);
            GetComponentInParent<Player>().SetBoostPercent(100);
        }
    }
}
