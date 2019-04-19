using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum prideState
{
    IDLE,   //waiting, not much wandering distance
    FOLLOWING,// follows player
    HUNGRY, // sent  after prey? is that realistic?
    HOMEWARD, //telling to return home base
    DEAD
}

public class newPride : MonoBehaviour
{

    prideState nprideState;
    int currHunger = Globals.full_hun;
    private Transform target;                                    // target to aim for
    public newPlayer player;
    public float movementSpeed = 15f;
    public float seekSpeed = 0.5f;
    public float rotSpeed = 100f;
    float followSpeed = 2f;

    //bite playing sound
    public AudioSource biteSound;

    //grounded handleres
    float playerGravity = 8.0f;
    float verticalSpeed = 0.0f;
    Vector3 movement = Vector3.zero;

    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;
    private bool touchingPlayer = false;
    private bool isFollowing = false;
    private bool isAlive = true;
    private bool isHunting = false;
    public PrideHealthBar health_bar;
    CharacterController c;
    Animator PrideAnim;
    int animState = 1; //1-idle, 
    // Start is called before the first frame update
    void Start()
    {
        //get character controller
        c = GetComponent<CharacterController>();
        this.name = GenerateName();
        PrideAnim = GetComponent<Animator>();

    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }



    // Update is called once per frame
    void Update()
    {

  
            currHunger--;
            if (health_bar.health <= 0)
            {
                isAlive = false;
                animState = 10;
                this.gameObject.tag = "Untagged";
                 nprideState = prideState.DEAD;
            }
            //gets current target position and distance 
            Vector3 playerDir = player.transform.position - transform.position;
            float playerAngle = Vector3.Angle(playerDir, transform.forward);
            float playerDistance = Vector3.Distance(player.transform.position, transform.position);

        //Performs wandering routines
        if (isAlive)
        {
            if (!isWalking) { animState = 1; }
            if (isWandering == false)
            {
                StartCoroutine(Wander());
            }
            if (isRotatingRight == true)
            {
                transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
                animState = 3;
            }
            if (isRotatingLeft == true)
            {
                transform.Rotate(0, -rotSpeed * Time.deltaTime, 0);
                animState = 4;
            }

            if (!isFollowing)
            {
                if (isWalking == true)
                {
                    Vector3 f = transform.TransformDirection(Vector3.forward);
                    c.SimpleMove(f * movementSpeed);
                    animState = 5;
                }
            }
        }
      


            //Performs stateful routines
            switch (nprideState)
            {
                case prideState.IDLE:
                    IdleRoutine(playerDistance);
                    break;

                case prideState.HUNGRY:
                    HungryRoutine(playerDistance);
                    break;
                case prideState.HOMEWARD:
                    HomewardRoutine();
                    break;
                case prideState.DEAD:
                    // DeadRoutine();
                    break;
                case prideState.FOLLOWING:
                    FollowingRoutine(playerDir, playerDistance);
                    break;
            }

        
        AnimationHandler(animState);

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
    // switches to follow state if player recruits
    //switches to hungry state if hunger low
    //switches to homeward state if player calls
    //else Wanders
    void IdleRoutine(float playerDistance)
    {
        if (playerDistance < 20 && Input.GetKey(KeyCode.F))// | if (player_angle <= 50.0F)
        {
            SetTarget(target);
            nprideState = prideState.FOLLOWING;
            player.AddFollower(this);
            print("pride started following");
        }
        else if (Input.GetKey(KeyCode.H))
        {
            SetTarget(target);
           // nprideState = prideState.HOMEWARD;
            print("pride headed home");
        }
        else if (currHunger <= 0)
        {
          //  nprideState = prideState.HUNGRY;   // sent itself after food
        }
    }

    //Following Routine follows player until input says stop or return home
    //input can also switch to hungry state
    void FollowingRoutine(Vector3 playerDir, float playerDistance)
    {
         isFollowing = true;
        transform.LookAt(player.transform.position);
        Vector3 f = transform.TransformDirection(playerDir);
        if (!isHunting)
        {
            animState = 2;
            if (playerDistance > 25) { touchingPlayer = false; }
            if (touchingPlayer)
            {
                c.SimpleMove(Vector3.zero * 0);
                animState = 1;
            }
            else
            {
                c.Move(playerDir * followSpeed * Time.deltaTime);
                //animState = 2;
            }
        }
        if (Input.GetKey(KeyCode.F))
        {
            isHunting = false;
        }
        if (Input.GetKey(KeyCode.G))
        {
            print("pride stopped following");
            player.RemoveFollower();
            nprideState = prideState.IDLE;
            isFollowing = false;
        }
       else if (Input.GetKey(KeyCode.H))
        {
            print("pride headed home");
            nprideState = prideState.HOMEWARD;
            isFollowing = false;
        }
        else if (Input.GetKey(KeyCode.J))
        {
            touchingPlayer = false;
            print("pride went hunting");
            nprideState = prideState.HUNGRY;
            isFollowing = false;
        }
        else if (Input.GetKey(KeyCode.K))//makes pause
        {
            c.SimpleMove(Vector3.zero * 0);
            animState = 1;
            isHunting = true;
        }
    }
    void HungryRoutine(float playerDistance)
    {
        //gets all prey tag objects in world
    
        var eatObjects = GameObject.FindGameObjectsWithTag("Prey");
        var eatCount = eatObjects.Length;
        print(eatCount);
        if (eatCount > 0)
        {
            //sets prey obj target
            var eatTarget = eatObjects[0];
            Vector3 eatDir = eatTarget.transform.position - transform.position;

            float eatAngle = Vector3.Angle(eatDir, transform.forward);
            if (eatAngle < 150.0F)
            {
                transform.LookAt(eatTarget.transform.position);
                c.Move(eatDir.normalized * movementSpeed * Time.deltaTime);
                animState = 2;

            }
            else
            {
                print("Cannot find prey");
                //should wander to find it
            }
        }
        if (currHunger <= -Globals.full_hun)
        {
            //  nherdState = herdState.DEAD;
        }
        if (playerDistance < 20 && Input.GetKey(KeyCode.F))// | if (player_angle <= 50.0F)
        {
            SetTarget(target);
            nprideState = prideState.FOLLOWING;
            player.AddFollower(this);
            print("pride started following");
        }
        else if (Input.GetKey(KeyCode.H))
        {
            SetTarget(target);
            nprideState = prideState.HOMEWARD;
            print("pride headed home");
        }

    }
    //Homeward Routine sends character home then switches to idle
    void HomewardRoutine()
    {
        GameObject[] homeObj;
        homeObj = GameObject.FindGameObjectsWithTag("Home");
        //gets current target position and distance 
        Vector3 homeDir = homeObj[0].transform.position - transform.position;
        float homeDistance = Vector3.Distance(homeObj[0].transform.position, transform.position);
        transform.LookAt(homeObj[0].transform.position);


        if (homeObj.Length == 0)
        {
            Debug.Log("No game objects are tagged with 'Home'");
        }

        Vector3 f = transform.TransformDirection(homeDir);
        c.SimpleMove(f * seekSpeed);
        //c.Move(f * movementSpeed * Time.deltaTime);
        if (homeDistance < 20)
        {
            c.SimpleMove(Vector3.zero * 0);
            nprideState = prideState.IDLE;
        }


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("VisionCone"))
        {
            c.SimpleMove(Vector3.zero * 0);
            print("pleases stop");
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.gameObject.CompareTag("Prey"))
        {
            {
                biteSound.Play();
                //Destroy(other.gameObject);
                currHunger = Globals.full_hun;
                nprideState = prideState.IDLE;
                Debug.Log("hit prey");
                health_bar.ResetHunger();


            }
        }
        if (other.gameObject.CompareTag("Player"))
        {
            {
                touchingPlayer = true;

                //print("Follow that player");     
            }
        }
        if (other.gameObject.CompareTag("Home"))
        {
            {
                Debug.Log("Home");

                health_bar.ResetEnergy();
               
            }
        }

    }
    private void AnimationHandler(int animState)
    {
        if (animState == 1) { PrideAnim.Play("IdleBreathe"); }
        if (animState == 2) { PrideAnim.Play("Run"); }
        if (animState == 3) { PrideAnim.Play("WalkTurnR"); }
        if (animState == 4) { PrideAnim.Play("WalkTurnL"); }
        if (animState == 5) { PrideAnim.Play("Walk"); }
        if (animState == 6)
        {
            PrideAnim.speed = 0.5f;
            PrideAnim.Play("Walk");
        }
        if (animState == 10) { PrideAnim.Play("Death"); }

        else { PrideAnim.speed = 1.0f; }


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
    public string GenerateName()
    {
        int i = Random.Range(0, 5);
        switch (i)
        {
            case 0:
                return "Simba";
                break;
            case 1:
                return "Nala";
                break;
            case 2:
                return "Mufasa";
                break;
            case 3:
                return "Puumba";
                break;
            case 4:
                return "Timon";
                break;
            case 5:
                return "Safara";
                break;
            default:
                return "Lion";
        }
    }

}
