using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class headLookAtIK : MonoBehaviour {

	public GameObject lookTarget;
	Animator animator;

	public bool isLookAt;
	public float lookAtWeight;


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void OnAnimatorIK () {
		
		if(isLookAt && lookTarget != null && lookAtWeight != 0f)
			{
				animator.SetLookAtWeight(lookAtWeight);
				animator.SetLookAtPosition(lookTarget.transform.position);
			}

	}
}
