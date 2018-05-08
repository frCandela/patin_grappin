using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class armIK : MonoBehaviour {

	Animator animator;

	public GameObject targetIK;
	Vector3 targetPos;
	public bool isIK;
	public string wichHand;

	AvatarIKGoal leftHand, rightHand;

	// Use this for initialization
	void Start () {

		animator = GetComponent<Animator>();
		leftHand = AvatarIKGoal.LeftHand;
		rightHand = AvatarIKGoal.RightHand;
		
	}
	
	// Update is called once per frame
	void OnAnimatorIK ()
    {

		if(isIK && targetIK)
		{
			targetPos = targetIK.transform.position;

			if(wichHand == "left")
			{
				animator.SetIKPositionWeight(leftHand, 1f);
				animator.SetIKPosition(leftHand, targetPos);
			}
			else if(wichHand == "right")
			{
				animator.SetIKPositionWeight(rightHand, 1f);
				animator.SetIKPosition(rightHand, targetPos);
			}
			else if(wichHand != "left" && wichHand != "right") Debug.LogError("wichHand (String) is not set to right or left !!!");
			else Debug.LogError("wichHand (String) not set");
		}
		else
		{
			animator.SetIKPositionWeight(leftHand, 0f);
			animator.SetIKPositionWeight(rightHand, 0f);
		}	
	}
}
