using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class spineOrientationIK : MonoBehaviour {

Animator animator;
private Transform boneTransform;

public GameObject spineTarget;
public bool isOriented;

	void Start () {
		animator = GetComponent<Animator>();
	}
	
	void OnAnimatorIK () {
		
		if(isOriented && spineTarget != null)
		{
			boneTransform = animator.GetBoneTransform(HumanBodyBones.Spine);

			Quaternion boneLocalRotation = boneTransform.localRotation;
			Quaternion boneRotation = boneTransform.rotation;

			Vector3 targetPos = spineTarget.transform.position;
			Vector3 bonePos = boneTransform.position;

			Quaternion rotationOffset = Quaternion.LookRotation(targetPos - bonePos, transform.up) * Quaternion.Euler(0,180,0);
			rotationOffset = Quaternion.Inverse(boneRotation) * rotationOffset;

			animator.SetBoneLocalRotation(HumanBodyBones.Spine, boneLocalRotation * rotationOffset);


		}

	}
}
