using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Trigger anims:")]
    [SerializeField] private float heightNotGrounded = 0.2f;

    [Header("Velocity anims:")]
    [SerializeField] private float velocityDelta = 5f;
    [SerializeField] private float MinXYVelocityIdle = 50;
    [SerializeField] private float MaxXYVelocityIdle = 60;

    //RAOUL qui fout sa merde
    [Header("Air Pose Manager:")]
    [SerializeField] private float minAirVelocity = -5f;
    [SerializeField] private float maxAirVelocity = 5f;



    //Public properties
    public bool rightHandUsed
    {
        get{return m_animator.GetBool("side");}
        private set{ }
    }

    public Transform grappleHandTransform {
        get
        {
            if (m_animator.GetBool("side"))
                return m_animator.GetBoneTransform(HumanBodyBones.RightHand);
            else
                return m_animator.GetBoneTransform(HumanBodyBones.LeftHand);
        }
        private set
        { }
    }

    //private references
    private PlayerController m_playerController = null;
    private Rigidbody m_playerRb = null;
    private Animator m_animator = null;
    private spineOrientationIK m_spineOrientationIK = null;
    private armIK m_armIK = null;
    private Transform m_leftFoot;
    private Transform m_rightFoot;

    //private properties
    public bool grounded { get; private set; }

    //privates members
    private int m_currentState = 2  ;

    // Use this for initialization
    void Awake ()
    {
        //Get and set references
        m_animator = GetComponentInChildren<Animator>();
        m_playerController = FindObjectOfType<PlayerController>();
        m_playerRb = m_playerController.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Connect events
        m_playerController.onGrappleLaunch.AddListener(LaunchGrapple);
        m_playerController.onGrappleReset.AddListener(ResetGrapple);

        //Init values
        grounded = true;
        grappleHandTransform = m_animator.GetBoneTransform(HumanBodyBones.RightHand);
        m_leftFoot = m_animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        m_rightFoot = m_animator.GetBoneTransform(HumanBodyBones.RightFoot);

        //Set IKanim scripts
        m_spineOrientationIK = GetComponentInChildren<spineOrientationIK>();
        m_spineOrientationIK.spineTarget = GetComponent<Grap>().grappleTarget;
        m_spineOrientationIK.isOriented = false;

        m_armIK = GetComponentInChildren<armIK>();
        m_armIK.targetIK = GetComponent<Grap>().grappleTarget.transform;
        m_armIK.isIK = false;
    }

    private void Update()
    {
        //Speed animations
        Vector3 XZVelocity = new Vector3(m_playerRb.velocity.x, 0, m_playerRb.velocity.z);
        float speed = XZVelocity.magnitude;

        if(m_currentState == 1)//Idle
        {
            if (speed < MinXYVelocityIdle - velocityDelta)
                SetAnim(3);//Go to Accelerate
            else if (speed > MaxXYVelocityIdle + velocityDelta)
                SetAnim(2);//Go to In speed
        }
        else if (m_currentState == 2)//In speed
        {
            if(speed < MaxXYVelocityIdle - velocityDelta)
                SetAnim(1);//Go to idle
        }
        else if (m_currentState == 3)//Accelerate
        {
            if (speed > MinXYVelocityIdle + velocityDelta) 
                SetAnim(1);//Go to idle
        }

        // Update in-air velocity and animator's float "airVelocity" [RAOUL qui fout sa merde]
        float playerVerticalVelocity = Mathf.Clamp(m_playerRb.velocity.y, minAirVelocity, maxAirVelocity);
        float airLerpParam = Mathf.InverseLerp(minAirVelocity, maxAirVelocity, playerVerticalVelocity);

        // set airVelocity entre -1 et 1 selon la velocité verticale du joueur entre la min et max
        m_animator.SetFloat("airVelocity", (airLerpParam * 2) - 1);

        //Set grounded parameter
        Ray rayLeft = new Ray(m_leftFoot.position, Vector3.down);
        Ray rayRight = new Ray(m_rightFoot.position, Vector3.down);
        RaycastHit raycastHit;
        if ( Physics.Raycast(rayLeft,  out raycastHit, heightNotGrounded, LayerMask.GetMask("Track"))
             || Physics.Raycast(rayRight, out raycastHit, heightNotGrounded, LayerMask.GetMask("Track")))
        {
            if( !grounded)
            {
                grounded = true;
                m_animator.SetBool("isGrounded", true);
                m_animator.SetTrigger("landing");
            }
        }
        else if( grounded)
        {
            grounded = false;
            m_animator.SetBool("isGrounded", false);
        }
    }

    private void SetAnim(int state)
    {
        m_animator.SetInteger("animationControl", state);
        m_currentState = state;
    }

    private void LaunchGrapple()
    {
        m_animator.SetTrigger("launchGrappin");

        if (m_animator.GetBool("side"))
            m_armIK.whichHand = armIK.Hands.right;  
        else
            m_armIK.whichHand = armIK.Hands.left;
            

        m_spineOrientationIK.isOriented = false;
        m_armIK.isIK = true;
    }


    private void ResetGrapple()
    {
        m_spineOrientationIK.isOriented = false;
        m_armIK.isIK = false;
    }
}
