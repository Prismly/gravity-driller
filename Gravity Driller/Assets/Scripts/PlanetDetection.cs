using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetDetection : MonoBehaviour
{
    [SerializeField]
    Player myPlayer;

    //During a drill dash, we want the player object to be able to move through planets, and we want to be able to detect
    //when the player leaves a planet, so we can stop the dash.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            Debug.Log("ENTER planet");
            //The player is entering a planet. While they are inside of it, drill boost should not deplete.
            myPlayer.SetInPlanet(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            Debug.Log("LEAVE planet");
            //The player is leaving a planet; their drill boost should immediately deplete and this object should deactivate.
            myPlayer.SetInPlanet(false);
            myPlayer.SetDrillBoostPercent(0);
            myPlayer.SetJumpLock(false);

            myPlayer.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            gameObject.SetActive(false);
        }
    }
}
