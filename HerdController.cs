using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

//USER STORY -> Requirements (how + methodlogy) -> FEATURE  (traceability matrix shows how features meet requirements)
//suggestion: combine Tech Screen with High Level Design


/* 
 * Zebra's have internal hunger meter
 *     Hunger decrements on timer or i
 *      Seek out any grass TAG when hunger is empty
 *          Replete hunger
 *          
 *      When lion in vision cone, run away
 *          When lion gone a certain distance, stop running
 * 
 *      Random movement system
 *     
 * 
 *      Herd following movement system
 *      Leader randomly assigned and fluctuates
 * 
 */



public enum herdState
{
    IDLE,
    FOLLOWING,
    HERDING,
    FLEEING,
    EATING,
    DEAD
}


public class HerdController : MonoBehaviour
{
    // public ThirdPersonCharacter Herd { get; private set; } // the character we are controlling
    herdState nherdState;
    int test = 0;
    int hunger = 60;
    private Transform target;                                    // target to aim for
    public Player player;
    public float movementSpeed;


    //rigidbody component
    Rigidbody rb;

    //collider component
    Collider col;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Start()
    {
        //gets rigidbody
        rb = GetComponent<Rigidbody>();
        //gets collider
        col = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        hunger--;
        Vector3 forward = transform.forward;
        Vector3 playerDir = player.transform.position - transform.position;
        float player_angle = Vector3.Angle(playerDir, forward);
        switch (nherdState)
        {
            case herdState.IDLE:
            //Look for player  
            if (player_angle < 50.0F)
            {
                //if player is found
                SetTarget(target);
                nherdState = herdState.FLEEING;
                print("Saw player - " + test);
            }

            //Check hunger
            //if hunger low, look for grass
            if (hunger <= 0)
            {
                //print("Hungry");
                var objects = GameObject.FindGameObjectsWithTag("Grass");
                var objectCount = objects.Length;
                var obj = objects[0];
                Vector3 grassDir = obj.transform.position - transform.position;
                float grass_angle = Vector3.Angle(grassDir, forward);
                if (grass_angle < 300.0F)
                {
                        transform.position = Vector3.MoveTowards(transform.position, obj.transform.position, movementSpeed);
                }
                else
                {
                    print("Cannot find grass");
                }
                //{nherdState = herdState.EATING}
            }
            break;

            case herdState.FLEEING:
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, movementSpeed); //move away not towards
                if (player_angle>50.0F)
                {
                    SetTarget(null);
                    nherdState = herdState.IDLE;
                    print("Zebra flee");
                }
            break;
                    //etc.

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grass"))
        {
            EatGrass(other);  
        }
    }
    private void EatGrass(Collider o)
    {
        //play grass eat sound
        //set timer for grass eat
        //turn off AWARENESS function
        //sets timer to eat and remove grass object
        Destroy(o.gameObject);
        hunger = 100;
    }
  }
