using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class headLookAtIK : MonoBehaviour {

	public GameObject lookTarget;
	Animator animator;

	public bool isLookAt;


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void OnAnimatorIK () {
		
		if(isLookAt && lookTarget != null)
		{
			animator.SetLookAtWeight(1);
			animator.SetLookAtPosition(lookTarget.transform.position);
		} else animator.SetLookAtWeight(0);

	}
}
