using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetDetection : MonoBehaviour
{
    [SerializeField]
    Player myPlayer;
    [SerializeField]
    private float drillBoostPercentAfterDig;

    //During a drill dash, we want the player object to be able to move through planets, and we want to be able to detect
    //when the player leaves a planet, so we can stop the dash.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            //Debug.Log("ENTER planet");
            //The player is entering a planet. While they are inside of it, drill boost should not deplete and the player should not be able to dash again.
            myPlayer.SetInPlanet(true);
            myPlayer.SetCanDrillDash(false);
            myPlayer.SetJumpBoostPercent(0);
        }
        if (collision.tag == "Core")
        {
            //The player has come into contact with a planet's core while drilling. The planet should start moving slightly, and the player should rebound off the core, back the way they came.

            //First, calculate the unit vector direction in which the planet should be moved, from the player's current drill dash
            collision.transform.parent.GetComponent<Planet>().AddDriftForce(myPlayer.GetPlanetPushForce() * myPlayer.GetInitDrillUnitDir());
            myPlayer.SetInitDrillUnitDir(-myPlayer.GetInitDrillUnitDir());
            Debug.Log("core collide");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            //Debug.Log("LEAVE planet");
            //The player is leaving a planet; their drill boost should immediately deplete and this object should deactivate.
            myPlayer.SetInPlanet(false);
            myPlayer.SetDrillBoostPercent(drillBoostPercentAfterDig);
            //myPlayer.SetJumpLock(false);
            //myPlayer.SetCanDrillDash(true);

            //myPlayer.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            //myPlayer.GetGroundDetector().SetActive(true);
            //gameObject.SetActive(false);
        }
    }
}
