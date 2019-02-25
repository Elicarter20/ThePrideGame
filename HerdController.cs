using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


namespace UnityStandardAssets.Characters.ThirdPerson {
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(HerdController))]
    [RequireComponent(typeof(ThirdPersonCharacter))]


    public class HerdController : MonoBehaviour {


        //walking and run speed
        public float walkSpeed;
        public float runSpeed;

        //rigidbody component
        Rigidbody rb;

        //collider component
        Collider col;

        //stopwatch for eat timer
        public Stopwatch timer;

        int hunger = 100;


        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter herd { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for


        int test = 0;

        // Use this for initialization

        void Start()
        {
            //gets rigidbody
            rb = GetComponent<Rigidbody>();
            //gets collider
            col = GetComponent<Collider>();


            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            herd = GetComponent<ThirdPersonCharacter>();

            agent.updateRotation = false;
            agent.updatePosition = true;
        }


        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        // Update is called once per frame
        void FixedUpdate() {
            CheckView();
            FindGrass();

        }

        void CheckView()
        {
            Vector3 targetDir = target.position - transform.position;
            Vector3 forward = transform.forward;
            float angle = Vector3.Angle(targetDir, forward);
            if (angle < 5.0F)
            {
                test++;
                print("close" + test);
                //RunScared(); //test running away function

            }
            herd.Move(Vector3.forward, false, false);

        }

        void RunScared()
        {
            if (target != null)
                agent.SetDestination(target.position);

            if (agent.remainingDistance > agent.stoppingDistance)
                herd.Move(agent.desiredVelocity, false, false);
            else
                herd.Move(Vector3.zero, false, false);
  
        }

        void FindGrass()
        {
            if (hunger == 100)
            {
                //find grass
            }
            else
            {
                //dont
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Grass"))
            {
                EatGrass(other);
                //play grass eat sound
                //set timer for grass eat
                //turn off AWARENESS function
            }
        }
        //sets timer to eat and remove grass object
        private void EatGrass(Collider o)
        {

            Destroy(o.gameObject);
            /*
            timer = new Stopwatch();
            timer.Start();
            while (timer.Elapsed.Seconds<10)
            {
                print(timer.Elapsed.Seconds);
                if (timer.Elapsed.Seconds == 5)
                {
                    timer.Stop();
                }
            }*/
        }
    }
}