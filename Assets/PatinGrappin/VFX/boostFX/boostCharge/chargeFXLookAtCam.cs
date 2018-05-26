using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chargeFXLookAtCam : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.transform.LookAt(Camera.main.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.LookAt(Camera.main.transform.position);
	}
}
