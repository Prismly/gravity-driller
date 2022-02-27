using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : MonoBehaviour
{
    [SerializeField]
    private int priority;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            //The player has just entered this gravity well.
            if (this == collision.GetComponent<Player>().GetCurrentGravCenter())
            {
                //The player is already linked to this gravity well.
                collision.GetComponent<Player>().SetInCurrentGravField(true);
            }
            else if (priority <= collision.GetComponent<Player>().GetCurrentGravCenter().GetPriority() 
                || !collision.GetComponent<Player>().GetInCurrentGravField())
            {
                //The player is NOT already linked to this gravity well, but EITHER...
                //This gravity well is prioritized over the player's current one, OR...
                //The player has left their linked well's AOE, so any well is fair game, regardless of priority.
                collision.GetComponent<Player>().SetCurrentGravCenter(this);
                collision.GetComponent<Player>().SetInCurrentGravField(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && this == collision.GetComponent<Player>().GetCurrentGravCenter())
        {
            //The player is both linked to this well, and has left its area of effect.
            //The player will still be pulled to this well, but can 
            collision.GetComponent<Player>().SetInCurrentGravField(false);
        }
    }

    public int GetPriority()
    {
        return priority;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, GetComponent<CircleCollider2D>().radius);
    }
}
