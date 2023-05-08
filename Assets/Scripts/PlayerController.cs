using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //a float representing the direction the player is moving in.
    private float movementInputDirection;

    //a float representing the amount of time the player has to perform a higher jump by holding down the jump button.
    private float jumpTimer;

    // a float representing the amount of time the player is unable to move or flip directions after changing directions while wall sliding.
    private float turnTimer;

    // a float representing the amount of time the player is unable to wall jump after performing a wall jump.
    private float wallJumpTimer;

    //a float representing the amount of time the player has left to dash.
    private float dashTimeLeft;

    //a float representing the position of the last image created when dashing.
    private float lastImageXpos;

    //a float representing the time the player last dashed.
    private float lastDash = -100f;

    //an integer representing the number of jumps the player has left.
    private int amountOfJumpsLeft;

    //an integer representing the direction the player is facing.
    private int facingDirection = 1;

    //an integer representing the direction the player last wall jumped in.
    private int lastWallJumpDirection;

    //a boolean representing whether the player is facing right.
    private bool isFacingRight = true;

    //a boolean representing whether the player is walking.
    private bool isWalking;

    //a boolean representing whether the player is grounded.
    private bool isGrounded;

    //a boolean representing whether the player is touching a wall.
    private bool isTouchingWall;

    // a boolean representing whether the player is sliding down a wall.
    private bool isWallSliding;

    //a boolean representing whether the player can perform a normal jump.
    private bool canNormalJump;

    //a boolean representing whether the player can perform a wall jump.
    private bool canWallJump;

    //a boolean representing whether the player
    private bool isAttemptingToJump;

    //check if the player is able to jump higher than usual
    private bool checkJumpMultiplier;

    // determine if the player is currently able to move
    private bool canMove;

    // determine if the player is currently able to flip their character sprite
    private bool canFlip;

    //keep track of whether the player has already performed a wall jump.
    private bool hasWallJumped;

    //determine if the player is currently dashing
    private bool isDashing;


    //Rigidbody2D component for physics simulation and an Animator component for animations.
    private Rigidbody2D rb; 
    private Animator anim;

    //an integer representing the number of jumps the player can perform.
    public int amountOfJumps = 2;

    //a float representing the player's movement speed.
    public float movementSpeed = 10.0f;

    // a float representing the force applied to the player when jumping.
    public float jumpForce = 16.0f;

   // a float representing the radius of a circle around the player used to check if the player is grounded.
    public float groundCheckRadius;

    // a float representing the distance the player can check for walls.
    public float wallCheckDistance;

    //a float representing the speed at which the player slides down walls.
    public float wallSlideSpeed;

    // a float representing the force applied to the player when moving in the air.
    public float movementForceInAir;

    //a float representing the drag applied to the player when moving in the air.
    public float airDragMultiplier = 0.95f;

    //a float representing the amount the player's jump height is multiplied by if the jump button is released early.
    public float variableJumpHeightMultiplier = 0.5f;

    // a float representing the force applied to the player when wall hopping.
    public float wallHopForce;

    // a float representing the force applied to the player when wall jumping.
    public float wallJumpForce;

    // a float representing the amount of time the player has to press the jump button to perform a higher jump.
    public float jumpTimerSet = 0.15f;

    // a float representing the amount of time the player is unable to move or flip directions after changing directions while wall sliding.
    public float turnTimerSet = 0.1f;

    // a float representing the amount of time the player is unable to wall jump after performing a wall jump.
    public float wallJumpTimerSet = 0.5f;

    //a float representing the amount of time the player can dash for.
    public float dashTime;

    //a float representing the speed at which the player dashes.
    public float dashSpeed;

    // a float representing the distance between images created when dashing.
    public float distanceBetweenImages;

    //a float representing the amount of time the player has to wait before being able to dash again.
    public float dashCoolDown;

    // store the direction in which the player should move when performing a wall hop or wall jump. 
    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;


    //detect the presence of ground and walls, respectively.
    //They are typically positioned at the bottom of the player's sprite for ground detection and on either side of the sprite for wall detection.

    public Transform groundCheck;
    public Transform wallCheck;

    //define which layers are considered "ground" for the purposes of ground detection.
    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        //This line normalizes the wallHopDirection (wallJumpDirection) vector , which makes it a unit vector (i.e., a vector with a magnitude of 1) pointing in the same direction as before.
        //This is useful when calculating movement vectors, as it simplifies the calculations.

        wallHopDirection.Normalize(); 
        wallJumpDirection.Normalize();
            
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
        CheckDash();
    }

    //method that is called at fixed intervals and is used for physics-related calculations and updates. In this code, it is used to apply movement to the player character and check the surroundings for any collisions or overlaps with other game objects.
    // This is done in FixedUpdate() instead of Update() to ensure
    // that the physics calculations are done at a fixed interval and not affected by the frame rate.

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }


   // The CheckIfWallSliding() method checks if the character is currently touching a wall and moving towards it,
   // and if the character's vertical velocity is negative (i.e. the character is falling down).
    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection && rb.velocity.y < 0 )
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }



    //CheckSurroundings() method checks for any overlaps between the player's collider and the ground or walls
    //using the Physics2D.OverlapCircle() method. This is used to determine whether the player is currently grounded
    //or against a wall, which is necessary for certain actions like jumping or wall-sliding.

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
       
    }



    // method is responsible for updating the variables related to the player's ability to jump.
    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (isTouchingWall)
        {
            canWallJump = true;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }

    }


    // method is responsible for checking and updating the player's movement direction.
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x)>= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }




    //method updates the relevant animation parameters based on the current state of the player.
    //It sets the "isWalking" boolean parameter based on the player's horizontal velocity, the "isGrounded" boolean parameter
    //based on whether the player is on the ground or not, and the "yVelocity" float parameter based on the player's vertical velocity.
    //It also sets the "isWallSliding" boolean parameter based on whether the player is currently sliding on a wall.

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }









    // CheckInput()   method checks for player input by checking various inputs such as movementInputDirection, jump input, horizontal input, and dash input.

    //    If the player presses the jump button, it checks whether the player is grounded or not,
    //    or if they have any jumps left while touching a wall.
    //    If the player is not grounded and doesn't have any jumps left,
    //    it sets a jumpTimer and isAttemptingToJump to true,
    //    which are used to perform a wall jump or double jump.

    //If the player presses the horizontal input while touching a wall, it checks whether they are in mid-air and not facing the wall,
    //and if so, it sets the canMove and canFlip variables to false and initiates a turnTimer which, upon expiry,
    //sets canMove and canFlip to true. This prevents the player from moving or flipping while turning around on the wall.

    //The checkJumpMultiplier variable is set to true upon jumping and if the player releases the jump button before reaching the peak of the jump,
    //it sets the checkJumpMultiplier to false, and applies a variable jump height multiplier to the player's vertical velocity.

    //Finally, if the player presses the dash button and the dash is not on cooldown,
    //the AttemptToDash method is called, which attempts to perform a dash.

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            //Jump();
            if(isGrounded || (amountOfJumpsLeft > 0 && isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if(Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if(!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (!canMove)
        {
            turnTimer -= Time.deltaTime;

            if(turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if ( checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }


        if(Input.GetButtonDown("Dash"))
        {
            if(Time.time >= (lastDash + dashCoolDown))
            AttemptToDash();
        }

    }





   // method is called when the player attempts to dash by pressing the "Dash" button.It sets the isDashing boolean flag to true,
   // which indicates that the player is currently dashing.It also sets the dashTimeLeft variable to the value of dashTime,
   // which determines how long the dash will last.

   // The method also sets the lastDash variable to the current time,
   // which is used to determine when the dash can be used again after a cooldown period.

   // Finally, the method calls the GetFromPool() method of a PlayerAfterImagePool singleton instance,
   // which is responsible for creating and managing afterimages that are left behind by the player during the dash.
   // It also stores the current position of the player in the lastImageXpos variable, which is used to determine the position of the afterimages.



    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }



    public int GetFacingDirection()
    {
        return facingDirection;
    }

   // This function checks if the player is currently dashing and updates their movement and dash time accordingly.
   // If isDashing is true, the player's movement and flipping abilities are disabled, and they move horizontally at dashSpeed in the direction they are facing.
   // During the dash, the player's position is checked to create an afterimage effect,
   // and the dashTimeLeft is decremented by Time.deltaTime.If the dashTimeLeft runs out or the player touches a wall,
   // isDashing is set to false, and the player's movement and flipping abilities are restored.

    private void CheckDash()
    {
        if (isDashing)
        {
            if(dashTimeLeft > 0) { 
            canMove = false;
            canFlip = false;
            rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);
            dashTimeLeft -= Time.deltaTime;

            if(Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
            {
                PlayerAfterImagePool.Instance.GetFromPool();
                lastImageXpos = transform.position.x;
            }
          }
            if(dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
        }
    }


    // method is used to handle different types of jumps: normal jumps and wall jumps.

   // If the jumpTimer is greater than 0, it checks if the player is touching a wall, if they are not grounded,
   // if they are moving in a direction other than the wall they are touching, and if they are pressing the jump button.
   // If all of these conditions are met, a wall jump is executed using the WallJump() method.

  // If the player is grounded, a normal jump is executed using the NormalJump() method.

  // The method also handles the wallJumpTimer. If the player has wall jumped and is currently in the air, the timer starts to decrease.
  // If the timer reaches 0 or the player touches the ground, the hasWallJumped variable is reset to false.


    private void CheckJump()
    {
      
        if(jumpTimer > 0)
        {
            //WallJump
            if(!isGrounded && isTouchingWall && movementInputDirection !=0 && movementInputDirection != facingDirection)
            {
                WallJump();
            }
            else if(isGrounded)
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump)
            {
                jumpTimer -= Time.deltaTime;
            }
        
        if(wallJumpTimer > 0)
        {
            if(hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;

            }
            else if(wallJumpTimer<= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }

        //else if (isWallSliding && movementInputDirection == 0 && canJump) //Wall hop
        //{
        //    isWallSliding = false;
        //    amountOfJumpsLeft--;
        //    Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
        //    rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        //}
      
    }



//The NormalJump method is responsible for executing a regular jump if the player is not wall-sliding and still has jumps left. It does the following:
//It checks if the player can perform a normal jump(canNormalJump) and is not wall-sliding(!isWallSliding).
//If the conditions are met, it sets the player's vertical velocity to jumpForce.
//It decrements the amount of jumps left(amountOfJumpsLeft).
//It sets jumpTimer to 0 and isAttemptingToJump to false.
//It sets checkJumpMultiplier to true, which will later be used to multiply the jump height if the player holds down the jump button.
//In summary, NormalJump handles the logic of a regular jump in the game.
    private void NormalJump()
    {
        if (canNormalJump && !isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;  
        }
    }

    //The WallJump() method is called when the player is in contact with a wall and tries to jump.
    //It first checks if wall jumping is currently allowed (canWallJump). If it is,
    //the player's velocity in the y-direction is set to 0 and isWallSliding is set to false to
    //indicate that the player is no longer wall sliding. The number of remaining jumps is then updated
    //(amountOfJumpsLeft) and decreased by 1.

   // Next, a force is added to the player's rigid body in the direction of the wall jump.
   // The force is calculated by multiplying wallJumpForce by wallJumpDirection,
   // which is set based on which wall the player is currently touching, and movementInputDirection,
   // which is the direction the player is currently trying to move. The force is applied using rb.AddForce()
   // with the ForceMode2D.Impulse argument, which adds an instantaneous force to the object.

    // The jump timer is reset to 0, isAttemptingToJump is set to false,
    // and checkJumpMultiplier is set to true to indicate that the jump height should be multiplied by a variable jump
    // height multiplier.turnTimer is set to 0 and canMove and canFlip are set to true to ensure that the player can
    // move and flip normally after the wall jump.hasWallJumped is set to true to indicate that the player has wall jumped,
    // and wallJumpTimer is set to wallJumpTimerSet to determine how long the wall jump can be used for. Finally,
    // lastWallJumpDirection is set to the opposite of facingDirection, which is used later to determine if the player
    // has successfully executed a wall jump.
    private void WallJump()
    {
           if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            isWallSliding = false;
            amountOfJumpsLeft = amountOfJumps;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }
    }

    // ApplyMovement() method applies the current movement direction and speed to the player's rigid body
    // using the rb.velocity property. 


    private void ApplyMovement()
    {
         if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }

      else if(canMove) //if (isGrounded)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
       
        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }
    public void DisableFlip()
        {
        canFlip = false;
        }

    public void EnableFlip()
    {
        canFlip = true;
    }


    private void Flip()
    {
        if (!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }
    // The OnDrawGizmos method is used to draw debug information in the Unity Editor scene view.
    // In this specific code, it is used to draw a wire sphere and a line to visualize the ground and wall
    // check positions respectively. The Gizmos class provides various functions for drawing shapes and lines
    // in the scene view, and the DrawWireSphere and DrawLine functions are used here to draw the sphere and line.
    // The groundCheck and wallCheck objects are transforms that are used to determine whether the player is touching the ground or a wall,
    // and the groundCheckRadius and wallCheckDistance variables determine the size of the sphere and length of the line respectively.
    // By using this method, the developer can visualize the check positions and adjust them as needed to ensure accurate collision detection.
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
