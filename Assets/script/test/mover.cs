using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mover : MonoBehaviour {

    public float speed = 0.1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = transform.position + speed * Input.GetAxis("Horizontal") * Vector3.right;
        transform.position = transform.position + speed * Input.GetAxis("Vertical") * Vector3.forward;
    }
}
