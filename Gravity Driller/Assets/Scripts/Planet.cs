using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private Vector2 driftVelocity;
    [SerializeField]
    private float driftDecel; //The amount by which the drift velocity's magnitude decelerates every second.
    [SerializeField]
    private float topDriftSpeed;
    [SerializeField]
    private float minDriftSpeed;

    private Rigidbody2D myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (driftVelocity.magnitude > minDriftSpeed)
        {
            Vector2 unitVelocity = driftVelocity / driftVelocity.magnitude;
            driftVelocity -= driftDecel * unitVelocity * Time.deltaTime;
        }
        else if(driftVelocity.magnitude < -minDriftSpeed)
        {
            Vector2 unitVelocity = driftVelocity / driftVelocity.magnitude;
            driftVelocity += driftDecel * unitVelocity * Time.deltaTime;
        }
        else
        {
            driftVelocity = Vector2.zero;
        }

        myRigidbody.velocity = driftVelocity;
    }

    public void AddDriftForce(Vector2 newForce)
    {
        driftVelocity += newForce;
        if (driftVelocity.magnitude > topDriftSpeed)
        {
            //The driftVelocity's magnitude exceeds the allowed top speed; clamp it while preserving the direction.
            driftVelocity *= (topDriftSpeed / driftVelocity.magnitude);
        }
    }
}
