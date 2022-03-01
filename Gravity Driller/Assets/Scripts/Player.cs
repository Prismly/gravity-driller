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
    [SerializeField] private float maxBoost; //The time, in seconds, it takes for the player to start being affected by high gravity after jumping with the key held.
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

    [Space(1)]
    [Header("Drill Dash")]
    [SerializeField]
    private float drillSpeed;
    [SerializeField]
    private float drillMaxBoost;
    [SerializeField]
    private float drillBoost;
    private Vector2 drillDest;
    private bool isDrillDashing = false;
    //Used to remember the initial direction / magnitude of the most recent drill dash, to make slowing the player during it more efficient.
    private Vector2 initialDrillVel;
    [SerializeField]
    private bool inPlanet = false;
    [SerializeField]
    private bool canDrillDash = true;

    //A unit vector indicating the direction in which gravity is currently pulling the player; the player's 'down'.
    private Vector2 gravDir = Vector2.down;
    //The player's velocity, relative to the gravity well they're currently centered around.
    private Vector2 relativeVel = Vector2.zero;

    [Space(1)]
    [Header("Misc.")]
    [SerializeField]
    private GameObject sceneCam;
    [SerializeField]
    private GameObject planetDetector;
    [SerializeField]
    private GameObject groundDetector;
    private Rigidbody2D myRigidbody;

    KeyCode moveLeft = KeyCode.A;
    KeyCode moveRight = KeyCode.D;
    KeyCode jump = KeyCode.W;

    
    
    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        sceneCam.GetComponent<CameraScript>().MoveToPosition(currentGravCenter.transform.position);
    }

    private void Update()
    {
        gravDir = getGravDir();

        //If the player is currently drilling, all player input should be ignored until the drill move concludes.
        if(!isDrillDashing)
        {
            //~~~~~~~~~~~~~~~
            //PLAYER MOVEMENT
            //~~~~~~~~~~~~~~~
            ProcessHorizontalInput();
            ProcessVerticalInput();
            myRigidbody.velocity = RelativeToActual(relativeVel);
        }
        else if(drillBoost > 0)
        {
            //~~~~~~~~~~~~~~
            //DRILL MOVEMENT
            //~~~~~~~~~~~~~~

            jumpLock = false;

            //The player is currently drilling at top speed.
            if (!inPlanet)
            {
                //We don't want the player to run out of drill boost halfway through a planet, so only deplete boost if they are not in a planet.
                drillBoost -= Time.deltaTime;
            }
            myRigidbody.velocity = initialDrillVel;
        }
        else
        {
            //The player has stopped drilling, normal inputs can resume.
            SetIsDrillDashing(false);
        }


        //Rotate the relative velocity defined by relativeVel to match the player's orientation (relative to the gravity source) this frame
        if (!isDrillDashing)
        {
            //Only do this, however, if the player is not currently drill dashing!
            transform.rotation = Quaternion.Euler(Vector3.forward * GetAngleFromDown() * (180 / Mathf.PI));
        }

        DeterminePlayerColor();
    }

    private void DeterminePlayerColor()
    {
        if (drillBoost > 0)
        {
            //Drill dash in progress
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else if (canDrillDash)
        {
            //Drill dash ready
            GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        else
        {
            //Cooldown
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private Vector2 getGravDir()
    {
        return (transform.position - currentGravCenter.transform.position) / Vector2.Distance(currentGravCenter.transform.position, transform.position) * -1;
    }

    private void ProcessHorizontalInput()
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

            if (isGrounded)
            {
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
    }

    private void ProcessVerticalInput()
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
            boost -= Time.deltaTime;
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

        //Tell the camera to start moving such that it is centered on the new well.
        sceneCam.GetComponent<CameraScript>().MoveToPosition(newWell.transform.position);

        //Now, assign the player's relative velocity the value it should have given the player's current actual velocity.
        //This requires converting from actual to relative, whereas in Update it's the other way around.
        relativeVel = actualToRelative(myRigidbody.velocity);
        //Debug.Log("Gravity Switch: " + actualVel + " to " + relativeVel);
    }

    private Vector2 RelativeToActual(Vector2 relative)
    {
        Vector2 actual = Vector2.zero;
        float angle = GetAngleFromDown();
        actual.x = (Mathf.Cos(angle) * relativeVel.x) - (Mathf.Sin(angle) * relativeVel.y);
        actual.y = (Mathf.Sin(angle) * relativeVel.x) + (Mathf.Cos(angle) * relativeVel.y);
        return actual;
    }

    private float GetAngleFromDown()
    {
        float angle = Mathf.Acos(-gravDir.y);
        if (transform.position.x > currentGravCenter.transform.position.x)
        {
            angle = -angle;
        }
        return angle;
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

    public void SetJumpBoostPercent(float newBoostPercent)
    {
        boost = maxBoost * (newBoostPercent / 100);
    }

    public void SetJumpLock(bool newJumpLock)
    {
        jumpLock = newJumpLock;
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

    public void SetIsDrillDashing(bool newIsDrilling, Vector2? dest = null)
    {
        if(newIsDrilling && dest != null)
        {
            drillDest = (Vector2) dest;
            //Set the player's velocity to be of magnitude drillSpeed, in the direction of drillDest.
            //This is rather expensive, which is why we only want to do it once (and then scale back the velocity with multiplication later).
            Vector2 diff = drillDest - (Vector2)transform.position;
            Vector2 unitDiff = diff / diff.magnitude;
            initialDrillVel = unitDiff * drillSpeed;
            myRigidbody.velocity = initialDrillVel;

            jumpLock = false;
            isDrillDashing = true;
            canDrillDash = false;
            drillBoost = drillMaxBoost;
            groundDetector.SetActive(false);
            planetDetector.SetActive(true);
            GetComponent<CircleCollider2D>().enabled = false;

            //Debug.Log(initialDrillVel);
        }
        else if(!newIsDrilling)
        {
            //Ensure that the player does not stop in their tracks when the boost ends (drill boost momentum must be conserved).
            relativeVel = actualToRelative(myRigidbody.velocity);

            isDrillDashing = false;
            planetDetector.SetActive(false);
            groundDetector.SetActive(true);
            GetComponent<CircleCollider2D>().enabled = true;
        }
    }

    public void SetInPlanet(bool newInPlanet)
    {
        inPlanet = newInPlanet;
    }

    public void SetCanDrillDash(bool newCanDrillDash)
    {
        canDrillDash = newCanDrillDash;
    }

    public bool GetCanDrillDash()
    {
        return canDrillDash;
    }

    public void SetDrillBoostPercent(float newBoostPercent)
    {
        drillBoost = drillMaxBoost * (newBoostPercent / 100);
    }

    public GameObject GetGroundDetector()
    {
        return groundDetector;
    }
}
