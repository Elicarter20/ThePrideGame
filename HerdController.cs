using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

//USER STORY -> Requirements (how + methodlogy) -> FEATURE  (traceability matrix shows how features meet requirements)
//suggestion: combine Tech Screen with High Level Design


/* 
 * Zebra's have internal hunger meter
 *     Hunger decrements on timer or i                   
 *      Seek out any grass TAG when hunger is empty              - works
 *          Replete hunger
 *          
 *      When lion in vision cone, run away                        - instead of vision cone, is smell distance...
 *          When lion gone a certain distance, stop running     - works, but with rigid body need to fix
 * 
 *      Random movement system                          - have YouTube video
 *     
 * 
 *      Herd following movement system              - implement LEADER state into switch statement that others will follow their WANDEr function
 *      Leader randomly assigned and fluctuates    - easy randomizer
 * 
 */

    //current issues: stacking RIGID BODY BAD, instead need to trigger an actual 3rd person navmesh movement function, transforms are busted...
    // random limits withion the plane

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
    int hunger = 60;
    int eating = 1000;
    private Transform target;                                    // target to aim for
    public Player player;
    public float movementSpeed;
    float fleeSpeed = 0.5F;

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

    void Update()
    {
        hunger--;
        Vector3 forward = transform.forward;
        Vector3 playerDir = player.transform.position - transform.position;
        float player_angle = Vector3.Angle(playerDir, forward);
        var distance = Vector3.Distance(player.transform.position, transform.position);
        print(distance);
        switch (nherdState)
        {
            case herdState.IDLE:
                //Look for player  
                eating = 1000; //restore eating timer
                if (distance < 20)//if (player_angle <= 50.0F)
                {
                    //if player is found
                    SetTarget(target);
                    nherdState = herdState.FLEEING;
                   
                    print("! --------------- ZEBRA FLEE");
                }

                //Check hunger
                //if hunger low, look for grass
                else if (hunger <= 0)
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
                          //  nherdState = herdState.EATING;
                            print("! --------------- ZEBRA EAT");

                    }
                    else
                    {
                        print("Cannot find grass");
                    }
                }
             break;

            case herdState.FLEEING:

                //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, movementSpeed); //move away not towards
                Vector2 velocity = new Vector2((transform.position.x - player.transform.position.x) * fleeSpeed, 0);
                rb.velocity = velocity;

                if (distance>40)
                {
                    SetTarget(null);
                    nherdState = herdState.IDLE;
                    print("! --------------- ZEBRA IDLE");
                }
            break;

          /*  case herdState.EATING:
                eating--;
                if (eating <= 0)
                {
                    nherdState = herdState.IDLE;
                    print("! --------------- ZEBRA IDLE");
                }
                break;
                //etc.*/

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
