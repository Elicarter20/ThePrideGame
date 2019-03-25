using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        int hitDist = 1;
        Ray groundRay = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(groundRay, out hit, hitDist))
        {
            transform.up = hit.normal;

        }
    }
    /*
    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        if (hit.normal != Vector3.zero)
        {
            transform.up = hit.normal;
        }

    }*/

}
