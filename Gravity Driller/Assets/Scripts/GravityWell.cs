using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : MonoBehaviour
{
    [SerializeField]
    //Terminal velocity for this gravity source.
    private float gravStrength;
    [SerializeField]
    //The time, in seconds, it takes to reach terminal velocity.
    private float gravAccel;

    public float getGravStrength()
    {
        return gravStrength;
    }

    public float getGravAccel()
    {
        return gravAccel;
    }
}
