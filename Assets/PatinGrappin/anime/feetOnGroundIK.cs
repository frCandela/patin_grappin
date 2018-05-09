using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))] 

public class feetOnGroundIK : MonoBehaviour {
    
    protected Animator animator;
    
    public bool ikActive = true;
    public Transform target = null;

    Transform lFoot, rFoot;
    float lFootWeightIK, rFootWeightIK;
    float lFootRotWeight, rFootRotWeight;
    GameObject player;

    public float rayCastOffset = 1f;
    public float raycastDistance = 3f;
    public float groundOffset = 0.45f;
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

                    if (Physics.Raycast(
                        (Vector3.up * rayCastOffset) + lFoot.position,
                        Vector3.down,out leftHit, raycastDistance))
                    {
                        Quaternion leftNormalOffset = Quaternion.FromToRotation(transform.up, leftHit.normal);
                        Quaternion leftIKrotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);

                        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, lFootRotWeight);
                        // rotation depends on : base animation of the IK + animation keys, offset of ground normals
                        animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftIKrotation * leftNormalOffset);

                        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, lFootWeightIK);
                        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftHit.point + new Vector3(0f, groundOffset, 0f));

                            
                    }
                } else animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);

                if(rFootWeightIK != 0f){

                    rFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

                    if (Physics.Raycast(
                        (Vector3.up * rayCastOffset) + rFoot.position,
                        Vector3.down,out rightHit, raycastDistance))
                    {
                        Quaternion rightNormalOffset = Quaternion.FromToRotation(transform.up, rightHit.normal);
                        Quaternion rightIKrotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);

                        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rFootRotWeight);
                        animator.SetIKRotation(AvatarIKGoal.RightFoot, rightIKrotation * rightNormalOffset);

                        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rFootWeightIK);
                        animator.SetIKPosition(AvatarIKGoal.RightFoot, rightHit.point + new Vector3(0f, groundOffset, 0f));
                    }
                } else animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
            }
            else {          
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,0); 
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot,0); 
            }
    }    
}