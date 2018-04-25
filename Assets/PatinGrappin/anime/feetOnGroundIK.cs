using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))] 

public class feetOnGroundIK : MonoBehaviour {
    
    protected Animator animator;
    
    public bool ikActive = false;
    public Transform target = null;

    Transform lFoot, rFoot;
    Quaternion lFootBaseRotation, rFootBaseRotation;
    float lFootWeightIK, rFootWeightIK;
    float lFootRotWeight, rFootRotWeight;
    GameObject Player;

    public float rayCastOffset, raycastDistance, groundOffset;
    public bool leftFootAtGround = false;

    void Start () 
    {
        animator = GetComponent<Animator>();

    }
    
    //a callback for calculating IK
    void OnAnimatorIK()
    {
    	RaycastHit leftHit;
        RaycastHit rightHit;
            
            if(ikActive) {
                lFootWeightIK = animator.GetFloat("leftIK");
                lFootRotWeight = animator.GetFloat("leftRotation");
                rFootWeightIK = animator.GetFloat("rightIK");
                rFootRotWeight = animator.GetFloat("rightRotation");

                if(lFootWeightIK != 0f){

                    lFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                    lFootBaseRotation = lFoot.rotation;

                    if (Physics.Raycast(
                        (Vector3.up * rayCastOffset) + lFoot.position,
                        Vector3.down,out leftHit, raycastDistance))
                    {
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, lFootRotWeight);
                        animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.Euler(leftHit.normal));

                        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, lFootWeightIK);
                        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftHit.point + new Vector3(0f, groundOffset, 0f));

                            
                    }
                } else animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);

                if(rFootWeightIK != 0f){

                    rFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                    rFootBaseRotation = rFoot.rotation;

                    if (Physics.Raycast(
                        (Vector3.up * rayCastOffset) + rFoot.position,
                        Vector3.down,out rightHit, raycastDistance))
                    {
                       animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rFootRotWeight);
                       animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.Euler(rightHit.normal));

                        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rFootWeightIK);
                        animator.SetIKPosition(AvatarIKGoal.RightFoot, rightHit.point + new Vector3(0f, groundOffset, 0f));
                    }
                } else animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
            }
            else {          
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,0); 
            }
    }    
}