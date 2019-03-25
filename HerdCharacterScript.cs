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
//ray casting and rotation dont mix
// need grass tags in real environment


// for next demo: menu, music, eat grass, eat zebra


public enum herdState
{
    IDLE,
    FOLLOWING,
    HERDING,
    FLEEING,
    HUNGRY,
    DEAD
}
public static class Globals
{
    public const int full_hun = 512; // Unmodifiable
}


public class HerdCharacterScript : MonoBehaviour
{
    // public ThirdPersonCharacter Herd { get; private set; } // the character we are controlling
    herdState nherdState;
    int currHunger = Globals.full_hun;
    private Transform target;                                    // target to aim for
    public PlayerController player;
    public float movementSpeed = 15f;
    public float fleeSpeed = 25f;
    public float seekSpeed = 0.5f;
    public float rotSpeed = 100f;


    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;
    float eatTimer = 10;


    //grounded handleres
    float playerGravity = 8.0f;
    float verticalSpeed = 0.0f;
    Vector3 movement = Vector3.zero;


    CharacterController c;
    void Start()
    {
        //get character controller
        c = GetComponent<CharacterController>();

    }

    //Sets Herd animal's target to run from

    public void SetTarget(Transform target)
    {
        this.target = target;
    }


    void Update()
    {
        //decrements current hunger
        currHunger--;
        //gets current target position and distance 
        Vector3 playerDir = player.transform.position - transform.position;
        float playerAngle = Vector3.Angle(playerDir, transform.forward);
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);

        //Performs wandering routines
        if (isWandering == false)
        {
            StartCoroutine(Wander());
        }
        if (isRotatingRight == true)
        {
           transform.Rotate(0,  rotSpeed * Time.deltaTime, 0);
        }
        if (isRotatingLeft == true)
        {
            transform.Rotate(0, -rotSpeed * Time.deltaTime, 0);
        }
        if (isWalking == true)
        {
            Vector3 f = transform.TransformDirection(Vector3.forward);
            c.SimpleMove(f * movementSpeed);
        }


        //Performs stateful routines
        switch (nherdState)
        {
            case herdState.IDLE:
                IdleRoutine(playerDistance);
                break;
            case herdState.FLEEING:
                FleeRoutine(playerDistance);
                break;
            case herdState.HUNGRY:
                HungryRoutine();
                break;
            case herdState.DEAD:
                DeadRoutine();
                break;
        }

        //grounded handler
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

    //Performs IDLE state routines
    // switches to flee state if found player
    //switches to eating state if hungry
    //else Wanders
    void IdleRoutine(float playerDistance)
    {
        if (playerDistance < 20)// | if (player_angle <= 50.0F)
        {
            SetTarget(target);
            nherdState = herdState.FLEEING;
        }
        else if (currHunger <= 0)
        {
           // nherdState = herdState.HUNGRY;   // temporary since no grass yet
        }


    }

    //Hungry Routine seeks out grass objects and moves to eat them
    // not able to flee while eating
    // if cannot find food, enters DEAD state
    void HungryRoutine()
    {
        //gets all grass tag objects in world
        var eatObjects = GameObject.FindGameObjectsWithTag("Grass");
        var eatCount = eatObjects.Length;
        if (eatCount > 0)
        {
            //sets grass obj target
            var eatTarget = eatObjects[0];
            Vector3 eatDir = eatTarget.transform.position - transform.position;

            //gets angle to grass obj
            float eatAngle = Vector3.Angle(eatDir, transform.forward);
            if (eatAngle < 350.0F)
            {
                transform.LookAt(eatTarget.transform.position);
                c.Move(eatDir.normalized * seekSpeed * Time.deltaTime);
                nherdState = herdState.IDLE;
            }
            else
            {
                print("Cannot find grass");
                //should wander to find it
            }
        }
        if (currHunger <= -Globals.full_hun)
        {
            //  nherdState = herdState.DEAD;
        }
      
    }
    //Flee Routine runs from player or target
    //nneds to rotate away and run away quickly
    void FleeRoutine(float playerDistance)
    {
        Vector3 f = transform.TransformDirection(Vector3.forward);
        c.SimpleMove(f * fleeSpeed);
        //c.Move(transform.forward * fleeSpeed * Time.deltaTime);
        if (playerDistance > 40)
        {
            SetTarget(null);
            nherdState = herdState.IDLE;
        }
    }

    //Dead Routine makes corpse of character
    void DeadRoutine()
    {
        //kill character
        //Destroy(this);
    }

    //Wander script randomly performs random movement
    IEnumerator Wander()
    {
        int rotTime = Random.Range(1, 3);
        int rotateWait = Random.Range(0, 3);
        int rotateLorR = Random.Range(0, 3);
        int walkWait = Random.Range(0, 4);
        int walkTime = Random.Range(1, 5);

        isWandering = true;

        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateWait);
        if (rotateLorR == 1)
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
    }

    private void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.gameObject.CompareTag("Predator"))
        {
            {
                print("Hit by predator");
                Destroy(this.gameObject);
            }
        }
    }


    //Function to eat grass
    // plays sound effect
    // pauses character
    // must send message to create new grass object
    private void EatGrass(Collider o)
    {

        StartCoroutine(BlockWait());
        Destroy(o.gameObject);
        currHunger = Globals.full_hun;
    }

    IEnumerator BlockWait()
    {
        yield return new WaitForSeconds(5.5f);
    }

}
