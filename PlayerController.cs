using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//need to decrement a proper HUNGER object, then send data to HUD


public static class playerGlobals
{
    public const float full_hun = 512f; // Unmodifiable
    public const float full_sleep = 512f; // Unmodifiable

}


public class PlayerController : MonoBehaviour
{
    public HealthBar healthBar;
    public float movementSpeed;
    private bool isMoving;


    public GameObject playerMovePoint;
    private Transform pmr;
    private Transform prev_pmr;

    private bool pmrSpawned;
    private GameObject triggeringPMR;
    //bite playing sound
    public AudioSource biteSound;

    //grounded handleres
    float playerGravity = 8.0f;
    float verticalSpeed = 0.0f;
    Vector3 movement = Vector3.zero;



    float hungerTimer = playerGlobals.full_hun;
    float sleepTimer = playerGlobals.full_sleep;

    // HUD manager
    //HudManager hudManager;
    CharacterController c;

    RaycastHit hit;
    //followers
    List<PrideController> prideList = new List<PrideController>();

    void Start()
    {
        //get character controller
        c = GetComponent<CharacterController>();
      //  hudManager = FindObjectOfType<HudManager>();

         

    }

    void Update()
    {

        hungerTimer -= 1 * Time.deltaTime;
      //  hudManager.hungerLabel.text = "Hunger: " + hungerTimer;
        sleepTimer -= 1 * Time.deltaTime;
      //  hudManager.sleepLabel.text = "Energy: " + sleepTimer;


        //sends out raycasts from mouse position on camera
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitDistance = 0.0f;

        //if raycast sent out and colidess 
        /*if (playerPlane.Raycast(ray, out hitDistance))
        {
            Vector3 mousePosition = ray.GetPoint(hitDistance);
            if (Input.GetMouseButtonDown(0))
            {
                isMoving = true;

                if (pmrSpawned)
                {
                    Destroy(prev_pmr.gameObject);
                    pmr = Instantiate(playerMovePoint.transform, mousePosition, Quaternion.identity);
                    prev_pmr = pmr;
                }
                else
                {
                    pmr = Instantiate(playerMovePoint.transform, mousePosition, Quaternion.identity);
                    prev_pmr = pmr;
                }
            }
        }*/
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 mousePosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            //Vector3 mousePosition = ray.GetPoint(hitDistance);
            if (Input.GetKey(KeyCode.Mouse0))
            {
                isMoving = true;

                if (pmrSpawned)
                {
                    Destroy(prev_pmr.gameObject);
                    pmr = Instantiate(playerMovePoint.transform, mousePosition, Quaternion.identity);
                    prev_pmr = pmr;
                }
                else
                {
                    pmr = Instantiate(playerMovePoint.transform, mousePosition, Quaternion.identity);
                    prev_pmr = pmr;
                }
              

            }

        }
        if (pmr)
        {
            pmrSpawned = true;
        }
        else
        {
            pmrSpawned = false;
        }

        if (isMoving)
        {
            Move(pmr);
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
        movement = movement + (m* Time.deltaTime * 2);
        c.Move(movement);

    }

    public void Move(Transform pmr)
    {
        //need to add rotation
        if (pmr != null)
        {
            Vector3 pmrDir = pmr.transform.position - transform.position;

            transform.LookAt(pmr.transform.position);
            c.Move(pmrDir.normalized * movementSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PMR")
        {
            //  print("Entered PMR");
            Destroy(other.gameObject);
        }


    }
    private void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.gameObject.CompareTag("Prey"))
        {
            {
                biteSound.Play();
                EatPrey(other);
            }
        }
        /*if (other.gameObject.CompareTag("Prey"))
        {
            {
                //Sleep();
            }
        }*/

    }

   /* private void Sleep()
    {
        //when collide with tree sleep and disable movement
        // set timmer for length to disable movement
        currEnergy = playerGlobals.full_energy;
    }*/

    private void EatPrey(ControllerColliderHit o)
    {
        //play grass eat sound
        //set timer for grass eat
        //turn off AWARENESS function
        //sets timer to eat and remove grass object
        GameManager.instance.IncreaseScore(1);
        Destroy(o.gameObject);
        hungerTimer = playerGlobals.full_hun;
        healthBar.SetBarSize();
        
    }

    public void AddFollower(PrideController lion)
    {
        prideList.Add(lion);
        print(lion.name);
    }

    public void RemoveFollower()
    {
        prideList.Clear();
    }
    private void Die(){
        
    }
}
