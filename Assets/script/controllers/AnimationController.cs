using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Trigger anims:")]
    [SerializeField] private float landingVelocityChangeThreshold = 15f;

    [Header("Velocity anims:")]
    [SerializeField] private float velocityDelta = 5f;
    [SerializeField] private float MinXYVelocityIdle = 50;
    [SerializeField] private float MaxXYVelocityIdle = 60;

    //Public properties
    public Transform leftHand { get; private set; }
    public Transform rightHand { get; private set; }

    //private references
    private PlayerController m_playerController = null;
    private Rigidbody m_playerRb = null;
    private Animator m_animator = null;
    private armIK m_armIK = null;

    //privates members
    private float prevYVelocity = 0f;
    private int m_currentState = 2;

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
        prevYVelocity = m_playerRb.velocity.y;
        leftHand = m_animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHand = m_animator.GetBoneTransform(HumanBodyBones.RightHand);

        //Set IKanim scipts
        m_armIK = GetComponentInChildren<armIK>();
        m_armIK.targetIK = GetComponent<Grapple>().grappleTarget.transform;
        m_armIK.isIK = false;
    }

    private void Update()
    {
        //Launch landing animation
        float velocityChange = m_playerRb.velocity.y - prevYVelocity;
        if ( velocityChange > 0f && velocityChange > landingVelocityChangeThreshold)
            m_animator.SetTrigger("landing");   
        prevYVelocity = m_playerRb.velocity.y;

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
    }

    private void SetAnim(int state)
    {
        m_animator.SetInteger("animationControl", state);
        m_currentState = state;
    }

    private void LaunchGrapple()
    {
        m_animator.SetTrigger("launchGrappin");
        m_animator.SetBool("isGrounded", false);
        m_armIK.isIK = true;
    }


    private void ResetGrapple()
    {
        m_animator.SetBool("isGrounded", true);
        m_armIK.isIK = false;
    }

}
