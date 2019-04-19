using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//need to decrement a proper HUNGER object, then send data to HUD


public static class newplayerGlobals
{
    public const float full_hun = 512f; // Unmodifiable
    public const float full_sleep = 512f; // Unmodifiable

}

public class newPlayer : MonoBehaviour
{
    public HealthBar statusPanel;


    float runSpeed = 0.3f;
    float walkSpeed = 0.08f;
    float backupSpeed = 0.1f;
    float turnSpeed = 74.0f;

    private bool isMoving;

    //bite playing sound
    public AudioSource biteSound;

    //grounded handleres
    float playerGravity = 8.0f;
    float verticalSpeed = 0.0f;
    Vector3 movement = Vector3.zero;


    float hungerTimer = newplayerGlobals.full_hun;
    float sleepTimer = newplayerGlobals.full_sleep;
    private bool isAlive = true;

    CharacterController c;

    //followers
    List<newPride> prideList = new List<newPride>();
    Animator LionAnim;
    private int animState = 1; //1- healthy
    private bool isRoaring = false;
    void Start()
    {
        //get character controller
        c = GetComponent<CharacterController>();
        LionAnim = GetComponent<Animator>();

        if (Manager.isLoadedData)
        {
            Vector3 pos = transform.position;
            pos.x = PlayerPrefs.GetFloat("PlayerX");
            pos.y = PlayerPrefs.GetFloat("PlayerY");
            pos.z = PlayerPrefs.GetFloat("PlayerZ");
            transform.position = pos;

            float rotation = PlayerPrefs.GetFloat("PlayerRotation");
            transform.rotation = Quaternion.Euler(0, rotation, 0);

        }

    }

    void Update()
    {

        AnimationHandler(animState);

        hungerTimer -= 1 * Time.deltaTime;
        sleepTimer -= 1 * Time.deltaTime;

        if (statusPanel.energy <= 0 || statusPanel.hunger <= 0)
        {
            isAlive = false;
            animState = 10;
            Debug.Log("I am dead techincally");
            statusPanel.energy = 0;
            statusPanel.hunger = 0;
        }
        if (isAlive) { 
            wasdMove();
        }
       
        //Gravity movements
        if (c.isGrounded)
        {
            verticalSpeed = -c.stepOffset / Time.deltaTime;
        }
        else
        {
            verticalSpeed -= playerGravity * Time.deltaTime;
        }
        Vector3 m = new Vector3(0, verticalSpeed, 0);
        movement = movement + (m * Time.deltaTime * 2);
        c.Move(movement);

    }

    private void wasdMove()
    {
        var frontObj = GameObject.FindGameObjectsWithTag("VisionCone");//gets front of character
        Vector3 frontDir = frontObj[0].transform.position - transform.position;

        if(animState != 7) {


        if (Input.GetKey(KeyCode.W))
        {

            if (Input.GetKey(KeyCode.LeftShift))
            {
                turnSpeed = 155;
                c.Move(frontDir * runSpeed);
                animState = 2;
            }
            else
            {
                turnSpeed = 74.0f;
                c.Move(frontDir * walkSpeed);
                animState = 5;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
            }
            c.Move(-frontDir * backupSpeed);
            animState = 6;
        }
        else if (Input.GetKey(KeyCode.D))
        {

            //   c.Move(Vector3.right);
            transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
            animState = 3;
        }

        else if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
            animState = 4;
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            animState = 7;
            statusPanel.ResetBarEnergy();

        }
        else if (Input.GetKey(KeyCode.R))
        {
                isRoaring = true;
                if (isRoaring)
                {
                    biteSound.Play();
                    isRoaring = false;
                }

                animState = 9;

            }
            else { animState = 1; }
        }
        if(Input.GetKey(KeyCode.X))
        {
            animState = 8;
        }


    }
    private void AnimationHandler(int animState)
    {
        if (animState == 1) { LionAnim.Play("IdleBreathe"); }
        if (animState == 2) { LionAnim.Play("Run"); }
        if (animState == 3) { LionAnim.Play("WalkTurnR"); }
        if (animState == 4) { LionAnim.Play("WalkTurnL"); }
        if (animState == 5) { LionAnim.Play("Walk"); }
        if (animState == 6)
        {
            LionAnim.speed = 0.5f;
            LionAnim.Play("Walk");
        }
        if (animState == 7) { LionAnim.Play("GoToRest"); }
        if (animState == 8) { LionAnim.Play("GoBackUp"); }
        if (animState == 9) { LionAnim.Play("Roar"); }
        if (animState == 10) { LionAnim.Play("Death"); }
        else { LionAnim.speed = 1.0f; }
       // if (animState == 8) { LionAnim.Play("ClawsAttack"); }

    }

    private void OnTriggerEnter(Collider other)
    {
        /* if (other.tag == "PMR")
         {
             //  print("Entered PMR");
             animState = 1;
             isMoving = false;
             Destroy(other.gameObject);
         }*/


    }
    private void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.gameObject.CompareTag("Prey"))
        {
            {              
               // LionAnim.Play("ClawsAttack");
                EatPrey(other);
                LionAnim.Play("Bite"); 

            }
        }
   

    }



    private void EatPrey(ControllerColliderHit o)
    {
           // animState = 8;
      //  Destroy(o.gameObject);
        hungerTimer = playerGlobals.full_hun;
       // biteSound.Play();

        statusPanel.ResetBarHunger();
    }

    public void AddFollower(newPride lion)
    {
        prideList.Add(lion);
        print(lion.name);
    }

    public void RemoveFollower()
    {
        prideList.Clear();
    }
    private void Die()
    {

    }
}
