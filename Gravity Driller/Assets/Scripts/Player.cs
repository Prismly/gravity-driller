using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Horizontal")]
    [SerializeField]
    private float moveSpeed; //The player's top walking speed, in units.
    [SerializeField]
    private float accelTime; //The number of seconds it takes the player to reach top speed.
    [SerializeField]
    private float decelTime; //The number of seconds it takes the player to slow to a stop.

    [Space(1)]
    [Header("Vertical")]
    [SerializeField]
    private float jumpSpeed; //The speed at which the player leaves the ground when they jump.
    [SerializeField] private float maxBoost;
    [SerializeField] private float boost;
    [SerializeField]
    private float lowGrav; //The gravity applied to the player while they are jumping up and are holding the jump key.
    [SerializeField]
    private float highGrav; //The gravity applied to the player while they are falling down.
    [SerializeField]
    private float maxFallSpeed; 
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool jumpLock;

    [Space(1)]
    [Header("Gravity")]
    [SerializeField]
    private GravityWell currentGravCenter;
    private bool inCurrentGravField;

    //A unit vector indicating the direction in which gravity is currently pulling the player; the player's 'down'.
    private Vector2 gravDir = Vector2.down;
    //The player's velocity, relative to the gravity well they're currently centered around.
    private Vector2 relativeVel = Vector2.zero;

    [Space(1)]
    [Header("Misc.")]
    KeyCode moveLeft = KeyCode.A;
    KeyCode moveRight = KeyCode.D;
    KeyCode jump = KeyCode.W;

    private Rigidbody2D myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        gravDir = getGravDir();

        ProcessHorizontalInput();
        ProcessVerticalInput();

        //Debug.Log(relativeVel);

        //Rotate the relative velocity defined by relativeVel to match the player's orientation (relative to the gravity source) this frame
        float relativeToActualAngle = 0;
        myRigidbody.velocity = relativeToActual(relativeVel, ref relativeToActualAngle);
        transform.rotation = Quaternion.Euler(Vector3.forward * relativeToActualAngle);
    }

    private Vector2 getGravDir()
    {
        return (transform.position - currentGravCenter.transform.position) / Vector2.Distance(currentGravCenter.transform.position, transform.position) * -1;
    }

    void ProcessHorizontalInput()
    {
        if (Input.GetKey(moveLeft) && !Input.GetKey(moveRight))
        {
            //~~~~~~~~~~
            //LEFT INPUT
            //~~~~~~~~~~

            if (myRigidbody.velocity.x < -moveSpeed)
            {
                //The player is currently moving left faster than their top speed; decelerate them towards the TOP SPEED, rather than 0.
                relativeVel = new Vector2(relativeVel.x + (moveSpeed / decelTime) * Time.deltaTime, relativeVel.y);
                if (relativeVel.x > -moveSpeed)
                {
                    //The player should not be able to decelerate past their top speed WHILE MOVING, so clamp at top speed.
                    relativeVel = new Vector2(-moveSpeed, relativeVel.y);
                }
            }
            else
            {
                //Accelerate the player left.
                relativeVel = new Vector2(relativeVel.x - (moveSpeed / accelTime) * Time.deltaTime, relativeVel.y);
                if (relativeVel.x < -moveSpeed)
                {
                    //The player should not be able to accelerate past their top speed without momentum, so clamp at top speed.
                    relativeVel = new Vector2(-moveSpeed, relativeVel.y);
                }
            }
        }
        else if (Input.GetKey(moveRight) && !Input.GetKey(moveLeft))
        {
            //~~~~~~~~~~~
            //RIGHT INPUT
            //~~~~~~~~~~~

            if (myRigidbody.velocity.x > moveSpeed)
            {
                //The player is currently moving right faster than their top speed; decelerate them towards the TOP SPEED, rather than 0.
                relativeVel = new Vector2(relativeVel.x - (moveSpeed / decelTime) * Time.deltaTime, relativeVel.y);
                if (relativeVel.x < moveSpeed)
                {
                    //The player should not be able to decelerate past their top speed WHILE MOVING, so clamp at top speed.
                    relativeVel = new Vector2(moveSpeed, relativeVel.y);
                }
            }
            else
            {
                //Accelerate the player right.
                relativeVel = new Vector2(relativeVel.x + (moveSpeed / accelTime) * Time.deltaTime, relativeVel.y);
                if (relativeVel.x > moveSpeed)
                {
                    //The player should not be able to accelerate past their top speed without momentum, so clamp at top speed.
                    relativeVel = new Vector2(moveSpeed, relativeVel.y);
                }
            }
        }
        else
        {
            //~~~~~~~~
            //NO INPUT
            //~~~~~~~~

            if (myRigidbody.velocity.x > 0)
            {
                relativeVel = new Vector2(relativeVel.x - (moveSpeed / decelTime) * Time.deltaTime, relativeVel.y);
                if (relativeVel.x < 0)
                {
                    relativeVel = new Vector2(0, relativeVel.y);
                }
            }
            else if (myRigidbody.velocity.x < 0)
            {
                relativeVel = new Vector2(relativeVel.x + (moveSpeed / decelTime) * Time.deltaTime, relativeVel.y);
                if (relativeVel.x > 0)
                {
                    relativeVel = new Vector2(0, relativeVel.y);
                }
            }
        }
    }

    void ProcessVerticalInput()
    {
        //~~~~~~~~~~
        //JUMP INPUT
        //~~~~~~~~~~
        if (Input.GetKeyDown(jump) && isGrounded)
        {
            //The jump key is currently held down.
            isGrounded = false;
            jumpLock = true;
            relativeVel.y = jumpSpeed;
        }
        if (Input.GetKeyUp(jump))
        {
            jumpLock = false;
        }

        //~~~~~~~
        //GRAVITY
        //~~~~~~~
        if (Input.GetKey(jump) && boost > 0 && !isGrounded && jumpLock)
        {
            //The player is boosting upwards. Use low gravity.
            boost -= Time.timeScale;
            relativeVel.y -= lowGrav * Time.deltaTime;
        }
        else
        {
            //Otherwise, use high gravity.
            relativeVel.y -= highGrav * Time.deltaTime;
        }

        if (relativeVel.y < -maxFallSpeed)
        {
            relativeVel.y = -maxFallSpeed;
        }

        //if (isGrounded)
        //{
        //    cartVel.y = 0;
        //}
    }

    public void GravitySwitch(GravityWell newWell)
    {
        //Link the player to the new gravity well they should move around.
        currentGravCenter = newWell;
        gravDir = getGravDir();

        //Now, assign the player's relative velocity the value it should have given the player's current actual velocity.
        //This requires converting from actual to relative, whereas in Update it's the other way around.
        relativeVel = actualToRelative(myRigidbody.velocity);
        //Debug.Log("Gravity Switch: " + actualVel + " to " + relativeVel);
    }

    private Vector2 relativeToActual(Vector2 relative, ref float angle)
    {
        Vector2 actual = Vector2.zero;
        angle = Mathf.Acos(-gravDir.y);
        if (transform.position.x > currentGravCenter.transform.position.x)
        {
            angle = -angle;
        }
        actual.x = (Mathf.Cos(angle) * relativeVel.x) - (Mathf.Sin(angle) * relativeVel.y);
        actual.y = (Mathf.Sin(angle) * relativeVel.x) + (Mathf.Cos(angle) * relativeVel.y);
        angle = angle * (180 / Mathf.PI);
        return actual;
    }

    private Vector2 actualToRelative(Vector2 actual)
    {
        Vector2 relative = Vector2.zero;
        float angle = Mathf.Acos(-gravDir.y);
        if (transform.position.x < currentGravCenter.transform.position.x)
        {
            angle = -angle;
        }
        relative.x = (Mathf.Cos(angle) * actual.x) - (Mathf.Sin(angle) * actual.y);
        relative.y = (Mathf.Sin(angle) * actual.x) + (Mathf.Cos(angle) * actual.y);
        return relative;
    }

    public void SetIsGrounded(bool newIsGrounded)
    {
        isGrounded = newIsGrounded;
    }

    public void SetBoostPercent(float newBoostPercent)
    {
        boost = (int)(maxBoost * newBoostPercent / 100);
    }

    public bool GetInCurrentGravField()
    {
        return inCurrentGravField;
    }

    public void SetInCurrentGravField(bool newInCurrentGravField)
    {
        inCurrentGravField = newInCurrentGravField;
    }

    public void SetCurrentGravCenter(GravityWell newCurrentGravCenter)
    {
        currentGravCenter = newCurrentGravCenter;
    }

    public GravityWell GetCurrentGravCenter()
    {
        return currentGravCenter;
    }
}
