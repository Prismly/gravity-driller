using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Vector3 targetPos;
    private float initialZPos;

    public void Start()
    {
        targetPos = gameObject.transform.position;
        initialZPos = gameObject.transform.position.z;
    }

    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
    }

    //Smoothly moves the camera from its current location to the given location.
    public void MoveToPosition(Vector3 newTargetPos)
    {
        targetPos = newTargetPos;
        targetPos.z = initialZPos;
    }
}
