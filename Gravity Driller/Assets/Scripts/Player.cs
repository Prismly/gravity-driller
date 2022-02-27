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
    [SerializeField] private int maxBoost;
    [SerializeField] private int boost;
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
    //The player's horizontal and vertical velocities on a traditional coordinate grid, before being converted to polar.
    private Vector2 cartVel = Vector2.zero;

    [Space(1)]
    [Header("Misc.")]
    KeyCode moveLeft = KeyCode.LeftArrow;
    KeyCode moveRight = KeyCode.RightArrow;
    KeyCode jump = KeyCode.UpArrow;

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

        Debug.Log(cartVel);

        //Rotate the cartesian velocity defined by relVelocity to match the player's orientation (relative to the gravity source) this frame
        float cartesianToPolarAngle = Mathf.Acos((gravDir.x * Vector2.down.x) + (gravDir.y * Vector2.down.y)); //First, we find the angle between gravDir and the absolute down direction (formula shortened as both vectors have length 1).
        if (transform.position.x > currentGravCenter.transform.position.x)
        {
            cartesianToPolarAngle = -cartesianToPolarAngle;
        }
        float polarVelX = (Mathf.Cos(cartesianToPolarAngle) * cartVel.x) - (Mathf.Sin(cartesianToPolarAngle) * cartVel.y);
        float polarVelY = (Mathf.Sin(cartesianToPolarAngle) * cartVel.x) + (Mathf.Cos(cartesianToPolarAngle) * cartVel.y);
        myRigidbody.velocity = new Vector2(polarVelX, polarVelY);

        float cTPAngleDegree = cartesianToPolarAngle * (180 / Mathf.PI);
        transform.rotation = Quaternion.Euler(Vector3.forward * cTPAngleDegree);

        myRigidbody.velocity = new Vector2(polarVelX, polarVelY);
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
                cartVel = new Vector2(cartVel.x + (moveSpeed / decelTime) * Time.deltaTime, cartVel.y);
                if (cartVel.x > -moveSpeed)
                {
                    //The player should not be able to decelerate past their top speed WHILE MOVING, so clamp at top speed.
                    cartVel = new Vector2(-moveSpeed, cartVel.y);
                }
            }
            else
            {
                //Accelerate the player left.
                cartVel = new Vector2(cartVel.x - (moveSpeed / accelTime) * Time.deltaTime, cartVel.y);
                if (cartVel.x < -moveSpeed)
                {
                    //The player should not be able to accelerate past their top speed without momentum, so clamp at top speed.
                    cartVel = new Vector2(-moveSpeed, cartVel.y);
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
                cartVel = new Vector2(cartVel.x - (moveSpeed / decelTime) * Time.deltaTime, cartVel.y);
                if (cartVel.x < moveSpeed)
                {
                    //The player should not be able to decelerate past their top speed WHILE MOVING, so clamp at top speed.
                    cartVel = new Vector2(moveSpeed, cartVel.y);
                }
            }
            else
            {
                //Accelerate the player right.
                cartVel = new Vector2(cartVel.x + (moveSpeed / accelTime) * Time.deltaTime, cartVel.y);
                if (cartVel.x > moveSpeed)
                {
                    //The player should not be able to accelerate past their top speed without momentum, so clamp at top speed.
                    cartVel = new Vector2(moveSpeed, cartVel.y);
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
                cartVel = new Vector2(cartVel.x - (moveSpeed / decelTime) * Time.deltaTime, cartVel.y);
                if (cartVel.x < 0)
                {
                    cartVel = new Vector2(0, cartVel.y);
                }
            }
            else if (myRigidbody.velocity.x < 0)
            {
                cartVel = new Vector2(cartVel.x + (moveSpeed / decelTime) * Time.deltaTime, cartVel.y);
                if (cartVel.x > 0)
                {
                    cartVel = new Vector2(0, cartVel.y);
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
            cartVel = new Vector2(cartVel.x, jumpSpeed);
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
            boost--;
            cartVel = new Vector2(cartVel.x, cartVel.y - lowGrav * Time.deltaTime);
        }
        else
        {
            //Otherwise, use high gravity.
            cartVel = new Vector2(cartVel.x, cartVel.y - highGrav * Time.deltaTime);
        }

        if (cartVel.y < -maxFallSpeed)
        {
            cartVel = new Vector2(cartVel.x, -maxFallSpeed);
        }

        //if (isGrounded)
        //{
        //    cartVel = new Vector2(cartVel.x, 0);
        //}
    }

    //private void findCartesianVelocity()
    //{
    //    //groundedCheck();
    //    //Debug.Log(isGrounded);

    //    //myRigidbody.velocity = gravDir * currentGravCenter.getGravStrength();
    //    if (cartesianVel.y > -currentGravCenter.getGravStrength() && !isGrounded)
    //    {
    //        //Player is falling, but not at terminal velocity. Accelerate them downwards.
    //        float accelThisFrame = (Time.deltaTime / currentGravCenter.getGravAccel()) * currentGravCenter.getGravStrength();
    //        cartesianVel = new Vector2(cartesianVel.x, cartesianVel.y - accelThisFrame);
    //    }
    //    else if (isGrounded)
    //    {
    //        //Player is grounded; to prevent clipping, set their vertical velocity to a universal reduced value.
    //        cartesianVel = new Vector2(cartesianVel.x, -groundedGravStrength);
    //    }

    //    if (Input.GetKey(walkLeft) && !Input.GetKey(walkRight))
    //    {
    //        //LEFT input

    //        cartesianVel -= new Vector2(walkSpeed, 0);
    //        if (cartesianVel.x < -walkSpeed)
    //        {
    //            cartesianVel = new Vector2(-walkSpeed, cartesianVel.y);
    //        }
    //    }
    //    else if (Input.GetKey(walkRight) && !Input.GetKey(walkLeft))
    //    {
    //        //RIGHT input
    //        if (cartesianVel.x < walkSpeed)
    //        {
    //            cartesianVel += new Vector2(walkSpeed, 0);
    //        }

    //        if (cartesianVel.x > walkSpeed)
    //        {
    //            cartesianVel = new Vector2(walkSpeed, cartesianVel.y);
    //        }
    //    }
    //    else
    //    {
    //        //temporary
    //        cartesianVel = new Vector2(0, cartesianVel.y);
    //    }

    //    if (Input.GetKeyDown(jump) && isGrounded)
    //    {
    //        //Debug.Log("Jumped");
    //        //myRigidbody.velocity += getDirRelToPlayer(Vector2.up) * jumpSpeed;
    //        cartesianVel = new Vector2(cartesianVel.x, jumpSpeed);
    //        isGrounded = false;
    //    }
    //}

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
