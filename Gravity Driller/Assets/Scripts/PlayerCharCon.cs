using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharCon : MonoBehaviour
{
    CharacterController myController;
    [SerializeField]
    private float moveSpeed; //The player's top walking speed, in units.
    [SerializeField]
    private float accelFrames; //The number of frames it takes the player to reach top speed, assuming 60fps.
    [SerializeField]
    private float decelFrames; //The number of frames it takes the player to slow to a stop, assuming 60fps.
    [SerializeField]
    private float jumpSpeed; //The speed at which the player leaves the ground when they jump.
    [SerializeField]
    private float lowGrav; //The gravity applied to the player while they are jumping up and are holding the jump key.
    [SerializeField]
    private float highGrav; //The gravity applied to the player while they are falling down.

    [SerializeField]
    private GravityWell currentGravCenter;

    Vector2 cartDisp = Vector2.zero;

    KeyCode moveLeft = KeyCode.LeftArrow;
    KeyCode moveRight = KeyCode.RightArrow;
    KeyCode jump = KeyCode.UpArrow;

    private void Start()
    {
        myController = GetComponent<CharacterController>();
    }

    void Update()
    {
        processHorizontalInput();
        processVerticalInput();

        Debug.Log(myController.isGrounded);
        myController.Move(cartDisp * Time.deltaTime);
    }

    void processHorizontalInput()
    {
        if (Input.GetKey(moveLeft) && !Input.GetKey(moveRight))
        {
            //~~~~~~~~~~
            //LEFT INPUT
            //~~~~~~~~~~

            if (myController.velocity.x < -moveSpeed)
            {
                //The player is currently moving left faster than their top speed; decelerate them towards the TOP SPEED, rather than 0.
                cartDisp = new Vector2(cartDisp.x + (moveSpeed / decelFrames), cartDisp.y);
                if (cartDisp.x > -moveSpeed)
                {
                    //The player should not be able to decelerate past their top speed WHILE MOVING, so clamp at top speed.
                    cartDisp = new Vector2(-moveSpeed, cartDisp.y);
                }
            }
            else
            {
                //Accelerate the player left.
                cartDisp = new Vector2(cartDisp.x - (moveSpeed / accelFrames), cartDisp.y);
                if (cartDisp.x < -moveSpeed)
                {
                    //The player should not be able to accelerate past their top speed without momentum, so clamp at top speed.
                    cartDisp = new Vector2(-moveSpeed, cartDisp.y);
                }
            }
        }
        else if (Input.GetKey(moveRight) && !Input.GetKey(moveLeft))
        {
            //~~~~~~~~~~~
            //RIGHT INPUT
            //~~~~~~~~~~~

            if (myController.velocity.x > moveSpeed)
            {
                //The player is currently moving right faster than their top speed; decelerate them towards the TOP SPEED, rather than 0.
                cartDisp = new Vector2(cartDisp.x - (moveSpeed / decelFrames), cartDisp.y);
                if (cartDisp.x < moveSpeed)
                {
                    //The player should not be able to decelerate past their top speed WHILE MOVING, so clamp at top speed.
                    cartDisp = new Vector2(moveSpeed, cartDisp.y);
                }
            }
            else
            {
                //Accelerate the player left.
                cartDisp = new Vector2(cartDisp.x + (moveSpeed / accelFrames), cartDisp.y);
                if (cartDisp.x > moveSpeed)
                {
                    //The player should not be able to accelerate past their top speed without momentum, so clamp at top speed.
                    cartDisp = new Vector2(moveSpeed, cartDisp.y);
                }
            }
        }
        else
        {
            //~~~~~~~~
            //NO INPUT
            //~~~~~~~~

            if (myController.velocity.x > 0)
            {
                cartDisp = new Vector2(cartDisp.x - (moveSpeed / decelFrames), cartDisp.y);
                if (cartDisp.x < 0)
                {
                    cartDisp = new Vector2(0, cartDisp.y);
                }
            }
            else if (myController.velocity.x < 0)
            {
                cartDisp = new Vector2(cartDisp.x + (moveSpeed / decelFrames), cartDisp.y);
                if (cartDisp.x > 0)
                {
                    cartDisp = new Vector2(0, cartDisp.y);
                }
            }
        }
    }

    void processVerticalInput()
    {
        //~~~~~~~
        //GRAVITY
        //~~~~~~~
        if (Input.GetKey(jump) && cartDisp.y > 0)
        {
            //The player is moving up (presumably from having jumped) and is extending their air time by holding the jump key.
            //Use low gravity.
            cartDisp = new Vector2(cartDisp.x, cartDisp.y - lowGrav);
            if (cartDisp.y < -lowGrav)
            {
                cartDisp = new Vector2(cartDisp.x, -lowGrav);
            }
        }
        else
        {
            //Otherwise, use high gravity.
            cartDisp = new Vector2(cartDisp.x, cartDisp.y - highGrav);
            if (cartDisp.y < -highGrav)
            {
                cartDisp = new Vector2(cartDisp.x, -highGrav);
            }
        }

        if (myController.isGrounded)
        {
            cartDisp = new Vector2(cartDisp.x, 0);
        }

        //~~~~~~~~~~
        //JUMP INPUT
        //~~~~~~~~~~
        if (Input.GetKeyDown(jump) && myController.isGrounded)
        {
            //The player tries to jump and is grounded; jump.
            cartDisp = new Vector2(cartDisp.x, jumpSpeed);
        }
    }
}
