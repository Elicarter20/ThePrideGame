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
 *      When lion in SMELL DISTANCE, run away                        - 
 *          When lion gone a certain distance, stop running     - works, but with rigid body need to fix
 * 
 *      Random movement system                          - have basic thing
 *     
 * 
 *      Herd following movement system              - implement LEADER state into switch statement that others will follow their WANDEr function
 *      Leader randomly assigned and fluctuates    - randomizer
 * 
 */

    //current issues: need to set TARGET to be ignored if HUNGRY/EATING
    // random limits withion the plane, fixed by invisible walls?
   // needs to dynamically assign player


    // for next demo: menu, music, eat grass, eat zebra

public enum herdState
{
    IDLE,
    FOLLOWING,
    HERDING,
    FLEEING,
    EATING,
    DEAD
}
public static class Globals
{
    public const int full_hun = 512; // Unmodifiable
}



public class HerdController : MonoBehaviour
{
    // public ThirdPersonCharacter Herd { get; private set; } // the character we are controlling
    herdState nherdState;
    int hunger = Globals.full_hun;
    int eating = 1000;
    private Transform target;                                    // target to aim for
    public Player player;
    public float movementSpeed;
    public float seekSpeed;

    float fleeSpeed = 0.5F;

    //rigidbody component
    Rigidbody rb;

    //collider component
    Collider col;

        public float rotSpeed = 100f;

    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;


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
      //  print(distance);

        if (isWandering == false)
        {
            StartCoroutine(Wander());
        }
        if (isRotatingRight == true)
        {
           transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
        }
        if (isRotatingLeft == true)
        {
            transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
        }
        if (isWalking == true)
        {
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
        }


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
                        transform.LookAt(obj.transform.position, Vector3.zero);
                        transform.position = Vector3.MoveTowards(transform.position, obj.transform.position, seekSpeed);
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
               transform.position += transform.forward * movementSpeed * Time.deltaTime;
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

    IEnumerator Wander()
    {
        int rotTime = Random.Range(1, 3);
        int rotateWait = Random.Range(0, 1);
        int rotateLorR = Random.Range(0, 3);
        int walkWait = Random.Range(0, 1);
        int walkTime = Random.Range(1, 5);

        isWandering = true;

        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateWait);
        if(rotateLorR == 1)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
        }
        if (rotateLorR == 2)
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
        }
        isWandering = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grass"))
        {
            EatGrass(other);  
        }
        if (other.tag == "Predator")
        {
            //print("--------------------Got ate!!-----------------------");
               Destroy(this.gameObject);
        }
    }
    private void EatGrass(Collider o)
    {
        //play grass eat sound
        //set timer for grass eat
        //turn off AWARENESS function
        //sets timer to eat and remove grass object
        Destroy(o.gameObject);
        hunger = Globals.full_hun;
    }
  }
