using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class armIK : MonoBehaviour {

    public enum Hands { right, left };
	public Hands whichHand = Hands.left;
	Animator animator;
	public Transform targetIK;
	Vector3 targetPos;
	public bool isIK;
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
			targetPos = targetIK.position;

			/*Vector3 toTarget = targetPos - transform.position;
			Vector3 rightVec = transform.parent.transform.right;
			float tDotF = Vector3.Dot(rightVec, toTarget);

			if(tDotF <= 0)
			{
				whichHand = Hands.left;
			}
			else if(tDotF > 0)
			{
				whichHand = Hands.right;
			}*/

			if(whichHand == Hands.left)
			{
				animator.SetIKPositionWeight(leftHand, 1f);
				animator.SetIKPosition(leftHand, targetPos);
			}
			else if(whichHand == Hands.right)
			{
				animator.SetIKPositionWeight(rightHand, 1f);
				animator.SetIKPosition(rightHand, targetPos);
			}
		}
		else
		{
			animator.SetIKPositionWeight(leftHand, 0f);
			animator.SetIKPositionWeight(rightHand, 0f);
		}	
	}
}
