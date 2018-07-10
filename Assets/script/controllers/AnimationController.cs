using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationController : MonoBehaviour
{
    [Header("Trigger anims:")]
    [SerializeField] private float heightNotGrounded = 0.2f;

    [Header("Velocity anims:")]
    [SerializeField] private float velocityDelta = 5f;
    [SerializeField] private float MinXYVelocityIdle = 50;
    [SerializeField] private float MaxXYVelocityIdle = 60;

    [Header("Air Pose Manager:")]
    [SerializeField] private float minAirVelocity = -10f;
    [SerializeField] private float maxAirVelocity = 10f;

    [Header("Events")]
    public UnityEvent onLanding;

    //private references
    private PlayerController m_playerController = null;
    private ModifiersController m_modifiersController = null;
    private Rigidbody m_playerRb = null;
    private spineOrientationIK m_spineOrientationIK = null;
    private RagdollController m_ragdollController = null;
    private Grap m_grap = null;
    private armIK m_armIK = null;
    private Transform m_leftFoot;
    private Transform m_rightFoot;

    //privates members
    private int m_currentState = 2;

    //Public properties
    public Animator animator { get; private set; }
    public bool grounded { get; private set; }
    public Transform leftHand { get; private set; }
    public Transform rightHand { get; private set; }

    // Use this for initialization
    void Awake ()
    {
        //Get and set references
        animator = GetComponentInChildren<Animator>();
        m_modifiersController = FindObjectOfType<ModifiersController>();
        m_playerController = FindObjectOfType<PlayerController>();
        m_ragdollController = FindObjectOfType<RagdollController>();
        m_playerRb = m_playerController.GetComponent<Rigidbody>();
        m_grap = m_playerController.GetComponent<Grap>();
        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
    }

    private void Start()
    {
        //Connect events
        m_playerController.onGrappleLaunch.AddListener(LaunchGrapple);
        m_playerController.onGrappleReset.AddListener(ResetGrapple);
        m_ragdollController.onRagdollStop.AddListener(RagdollStop);
        m_modifiersController.onBoostStart.AddListener(StartBoost);
        m_modifiersController.onBoostStop.AddListener(StopBoost);

        //Init values
        grounded = true;
        m_leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        m_rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);

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

        if( m_modifiersController.boosting && grounded)
        {
            SetAnim(3);//Go to Accelerate
        }
        else if(m_currentState == 1)//Idle
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
        animator.SetFloat("airVelocity", (airLerpParam * 2) - 1);

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
                animator.SetTrigger("landing");
                animator.SetBool("isGrounded", true);
                onLanding.Invoke();
                SetMusic();
            }
        }
        else if( grounded)
        {
            grounded = false;
            animator.SetBool("isGrounded", false);
            SetMusic();
        }
    }

    private void SetMusic()
    {
        
        if (LevelManager.s_verticalMusicEnabled)
        {
            print("SetMusic");
            if (grounded)
            {
                switch (m_currentState)
                {
                    case 1://idle
                        AkSoundEngine.SetState("Interactive", "Normal");
                        break;
                    case 2://speed
                        AkSoundEngine.SetState("Interactive", "Fast");
                        break;
                    case 3://accelerate
                        AkSoundEngine.SetState("Interactive", "Slow");
                        break;

                }
            }
            else
                AkSoundEngine.SetState("Interactive", "Air");
        }
    }

    private void SetAnim(int state)
    { 
        animator.SetInteger("animationControl", state);
        m_currentState = state;
        SetMusic();
    }

    private void LaunchGrapple()
    {
        animator.SetTrigger("launchGrappin");

        if (m_grap.m_rightHandUsed)
            m_armIK.whichHand = armIK.Hands.right;  
        else
            m_armIK.whichHand = armIK.Hands.left;
            
        m_armIK.isIK = true;
    }

    private void ResetGrapple()
    {
        m_armIK.isIK = false;
    }

    private void RagdollStop()
    {
        if(grounded)
            animator.Play("Armature|ReceptionL_P");
    }

    private void StartBoost()
    {
    }

    private void StopBoost()
    {
       
    }
    
}
