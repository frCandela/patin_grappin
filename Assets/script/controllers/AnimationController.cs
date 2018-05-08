using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [Header("Trigger anims:")]
    [SerializeField] private float landingVelocityChangeThreshold = 15f;

    //private references
    private PlayerController m_playerController = null;
    private Rigidbody m_playerRb = null;
    private Animator m_animator = null;

    //privates members
   private float prevYVelocity = 0f;

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
    }

    private void Update()
    {
        //Launch landing animation
        float velocityChange = m_playerRb.velocity.y - prevYVelocity;
        if ( velocityChange > 0f && velocityChange > landingVelocityChangeThreshold)
            m_animator.SetTrigger("landing");   
        prevYVelocity = m_playerRb.velocity.y;
    }

    private void LaunchGrapple()
    {
        m_animator.SetTrigger("launchGrappin");
        m_animator.SetBool("isGrounded", false);
    }


    private void ResetGrapple()
    {
        m_animator.SetBool("isGrounded", true);
    }

}
