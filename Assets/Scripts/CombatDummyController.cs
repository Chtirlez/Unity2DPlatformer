using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDummyController : MonoBehaviour
{
    //    maxHealth: a float representing the maximum health of the combat dummy.
     //   knockbackSpeedX: a float representing the speed at which the combat dummy is knocked back horizontally.
    //    knockbackSpeedY: a float representing the speed at which the combat dummy is knocked back vertically.
     //   knockbackDuration: a float representing the duration of the knockback effect.
    //    knockbackDeathSpeedX: a float representing the speed at which the combat dummy is knocked back horizontally when it dies.
     //   knockbackDeathSpeedY: a float representing the speed at which the combat dummy is knocked back vertically when it dies.
      //  deathTorque: a float representing the amount of torque applied to the combat dummy when it dies.

    [SerializeField]
    private float maxHealth, knockbackSpeedX, knockbackSpeedY, knockbackDuration, knockbackDeathSpeedX, knockbackDeathSpeedY, deathTorque;


    // indicating whether the combat dummy is affected by knockback.
    [SerializeField]
    private bool applyKnockback;

    //float representing the current health of the combat dummy.
    // float representing the time when the knockback effect starts.
    private float currentHealth, knockbackStart;

    //int indicating the facing direction of the player.
    private int playerFacingDirection;

    //indicating whether the player is on the left side of the combat dummy.
    private bool playerOnLeft, knockback;

    //reference to the PlayerController script attached to the player game object.
    private PlayerController pc;
    //reference to the game object representing the alive,  top broken, broken state of the combat dummy.
    private GameObject aliveGO, brokenTopGO, brokenBotGO;

    //reference to the rigidbody 
    private Rigidbody2D rbAlive, rbBrokenTop, rbBrokenBot;
    private Animator aliveAnim;



    //This method is called once at the start of the game.
    //It initializes several variables including currentHealth,
    //which is set to the maximum health of the game object,
    //pc, which is set to the PlayerController component of the game object named "Player",
    //and aliveGO, brokenTopGO, and brokenBotGO, which are set to the corresponding child game objects
    //of the current game object. aliveAnim, rbAlive, rbBrokenTop, and rbBrokenBot are set to the corresponding
    //Animator and Rigidbody2D components of these game objects. Finally, the aliveGO is set to be active while
    //the other two game objects are set to be inactive.

    private void Start()
    {
        currentHealth = maxHealth;
        pc = GameObject.Find("Player").GetComponent<PlayerController>();

        aliveGO = transform.Find("Alive").gameObject;
        brokenTopGO = transform.Find("Broken Top").gameObject;
        brokenBotGO = transform.Find("Broken Bottom").gameObject;

        aliveAnim = aliveGO.GetComponent<Animator>();
        rbAlive = aliveGO.GetComponent<Rigidbody2D>();
        rbBrokenTop = brokenTopGO.GetComponent<Rigidbody2D>();
        rbBrokenBot = brokenBotGO.GetComponent<Rigidbody2D>();

        aliveGO.SetActive(true);
        brokenTopGO.SetActive(false);
        brokenBotGO.SetActive(false);
    }



    private void Update()
    {
        CheckKnockback();
    }



    //method is called to damage the combat dummy. It takes in a float parameter amount which represents the amount of damage to be applied to the dummy.
    //The method first reduces the currentHealth of the dummy by the amount of damage inflicted.

   // It then sets the playerFacingDirection variable to the value returned by the GetFacingDirection() method
   // of the PlayerController script.If playerFacingDirection is 1, playerOnLeft is set to true; otherwise, it is set to false.
   // These variables are used to determine the direction of the knockback and the animation of the dummy.

  // The method then sets the playerOnLeft boolean as a parameter for the "playerOnLeft" boolean of the aliveAnim animator component,
  // and triggers the "damage" animation of the aliveAnim component.

   //If the applyKnockback boolean is true and currentHealth is greater than 0,
   //the Knockback() method is called.If currentHealth is less than or equal to 0, the Die() method is called.
    private void Damage(float amount)
    {
        currentHealth -= amount;
        playerFacingDirection = pc.GetFacingDirection();

        if (playerFacingDirection == 1)
        {
            playerOnLeft = true;
        }
        else
        {
            playerOnLeft = false;
        }
        aliveAnim.SetBool("playerOnLeft", playerOnLeft);
        aliveAnim.SetTrigger("damage");

        if (applyKnockback && currentHealth > 0.0f)
        {
            //Knockback
            Knockback();
        }
        if (currentHealth <=0.0f)
        {
            //Die
            Die();
        }
    }




    //This method is responsible for applying a knockback effect on the object.
    //It sets the "knockback" flag to true, which is used in the CheckKnockback() method.
    //It also sets the knockbackStart variable to the current time using Time.time,
    //which is used to calculate when the knockback effect should end. Finally,
    //it applies a force to the Rigidbody2D component of the aliveGO object,
    //with a velocity determined by the knockbackSpeedX and knockbackSpeedY variables and the player's facing direction.
    private void Knockback()
    {
        knockback = true;
        knockbackStart = Time.time;
        rbAlive.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbackSpeedY);
    }




    //The CheckKnockback method checks if the knockback duration has elapsed since the knockback started,
    //and if so, it sets knockback to false and stops the horizontal knockback by setting the X velocity
    //of the rbAlive rigidbody to 0.0f while keeping its Y velocity unchanged.

   // This method ensures that the enemy does not continue to be knocked back indefinitely and
   // returns to its normal state after a set amount of time.
    private void CheckKnockback()
    {
        if (Time.time >= knockbackStart + knockbackDuration && knockback)
        {
            knockback = false;
            rbAlive.velocity = new Vector2(0.0f, rbAlive.velocity.y);
        }
    }



     // The Die() method seems to be responsible for handling the player's death in the game.
     // It sets the active status of the aliveGO object to false,
     // which presumably represents the player's alive state in the game.
     // It also sets the active status of the brokenTopGO and brokenBotGO objects to true,
     // which presumably represent the player's broken state in the game after death.
     // The method then sets the position of the brokenTopGO and brokenBotGO objects to the position of the aliveGO object,
     // which presumably is the position of the player at the time of their death.
     // The rbBrokenBot and rbBrokenTop objects appear to be Rigidbody2D components attached to the brokenBotGO and brokenTopGO objects,
     // respectively.The method sets the velocity of rbBrokenBot and rbBrokenTop to a Vector2 representing the knockback speed in the x and y directions, respectively,
     // multiplied by the player's facing direction. This presumably causes the broken objects to move away from the player's death position in a specific direction.
     // Finally, the method adds torque to rbBrokenTop in the z direction(out of the 2D plane) based on the deathTorque value and the player's facing direction.
     // This likely causes the broken object to spin as it moves away from the player's death position.


    private void Die()
    {
        aliveGO.SetActive(false);
        brokenTopGO.SetActive(true);
        brokenBotGO.SetActive(true);

        brokenTopGO.transform.position = aliveGO.transform.position;
        brokenBotGO.transform.position = aliveGO.transform.position;

        rbBrokenBot.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbackSpeedY);
        rbBrokenTop.velocity = new Vector2(knockbackDeathSpeedX * playerFacingDirection, knockbackDeathSpeedY);
        rbBrokenTop.AddTorque(deathTorque * -playerFacingDirection, ForceMode2D.Impulse);
        
    }
}
