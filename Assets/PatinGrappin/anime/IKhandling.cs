using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))] 

public class IKhandling : MonoBehaviour {
    
    protected Animator animator;
    
    public bool ikActive = false;
    public Transform rightHandObj = null;
    public Transform lookObj = null;

    public bool isLookAt = false;

    Transform lFoot, rFoot, spine;
    GameObject Player;

    public float rayCastOffset, raycastDistance;
    public bool leftFootAtGround = false;
    public bool isGrappine = false;
    public Vector3 lookOffset, baseSpineRotation;
    Quaternion spineRotation;

    Quaternion localRotation;

    void Start () 
    {
        animator = GetComponent<Animator>();

        localRotation = animator.GetBoneTransform(HumanBodyBones.Spine).localRotation;
    }
    
    //a callback for calculating IK
    void OnAnimatorIK()
    {
    	RaycastHit leftHit;



        if(animator) {
            
            //if the IK is active, set the position and rotation directly to the goal. 
            if(ikActive) {

                // Set the look target position, if one has been assigned
                if(lookObj != null && isLookAt) {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }    

                // Set the right hand target position and rotation, if one has been assigned
                if(rightHandObj != null) {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
                    animator.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
                }        
                
            }
            
            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else {          
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0); 
                animator.SetLookAtWeight(0);
            }

            if(leftFootAtGround){
            	lFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);

            	if (Physics.Raycast(
                    (Vector3.up * rayCastOffset) + lFoot.position,
                    Vector3.down,out leftHit, raycastDistance))
            	{
            			animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1);
            			animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftHit.point);
            	}
            }
            else animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1);

            if(isGrappine)
            {
                Quaternion rotation = animator.GetBoneTransform(HumanBodyBones.Spine).rotation;
                Vector3 pos = lookObj.transform.position;
                Vector3 bonePos = animator.GetBoneTransform(HumanBodyBones.Spine).position;
                Quaternion objRotation = Quaternion.LookRotation(pos - bonePos) * Quaternion.Euler(0,180,0);
                objRotation = Quaternion.Inverse(rotation) * objRotation;

                animator.SetBoneLocalRotation(HumanBodyBones.Spine, localRotation * objRotation);
            }
        }
    }    
}