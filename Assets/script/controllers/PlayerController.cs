using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{ 
    [Header("Movement:")]
    [SerializeField] private float initialVelocity = 2f;
    [SerializeField] private float forwardForce = 30f;
    [SerializeField] private float forwardForceGrapple = 30f;  
    [SerializeField] private float turnForce = 300f;
    [SerializeField] private float maxheadYAngle = 20f;

    [Header("Ragdoll Movement:")]
    [SerializeField] private float forwardForceRagdoll = 50f;
    [SerializeField] private float turnForceRagdoll = 1500;

    [Header("Other:")]
    [SerializeField] private bool trackForceWhenGrappling = false;
    [SerializeField] private float gravity = -30;

    [Header("Events:")]
    public UnityEvent onGrappleLaunch;
    public UnityEvent onGrappleReset;

    [HideInInspector] public float boostMultiplier = 1f;

    //Components references
    private Rigidbody m_rb;
    private Grap m_grapple;
    private Track m_track = null;
    RagdollController m_ragdollController = null;

    // Use this for initialization
    void Awake ()
    {
        //Get references
        m_track = FindObjectOfType<Track>();
        m_ragdollController = FindObjectOfType<RagdollController>();
        m_rb = GetComponent<Rigidbody>();
        m_grapple = GetComponent<Grap>();

        Physics.gravity = new Vector3(0, gravity, 0);
        m_rb.velocity = initialVelocity * m_track.trackSection.trackDirection;

        //Set events
        onGrappleLaunch = new UnityEvent();
        onGrappleReset = new UnityEvent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //Launch or reset grapple
        if (Input.GetButtonDown("Grapple") && m_grapple.Throw())
            onGrappleLaunch.Invoke();
        if (Input.GetButtonUp("Grapple") && m_grapple.Cancel())
            onGrappleReset.Invoke();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody targetRB;
        if (m_ragdollController.ragdollActivated)
            targetRB = m_ragdollController.mainRb;
        else
            targetRB = m_rb;            

        //Forward force in the track direction
        if (trackForceWhenGrappling || !m_grapple.grappling)
        {
            if( m_ragdollController.ragdollActivated)
                targetRB.AddForce(boostMultiplier * forwardForceRagdoll * m_track.trackSection.trackDirection, ForceMode.Acceleration);
            else
                targetRB.AddForce(boostMultiplier * forwardForce * m_track.trackSection.trackDirection, ForceMode.Acceleration);
        }

        //forward force when grappling
        if (m_grapple.grappling)
        {
            Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            targetRB.AddForce(forwardForceGrapple * forwardXZ, ForceMode.Acceleration);
        }

        //Orientation towards the player speed
        if (  ! m_ragdollController.ragdollActivated && targetRB.velocity != Vector3.zero)
            targetRB.transform.rotation = Quaternion.LookRotation(targetRB.velocity);
        
        float headAxis = 0;
        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();
        if (pose.IsValid)
        {
            //Calculates head input
            headAxis = pose.Rotation.eulerAngles.y;
            if (headAxis > 180)
                headAxis -= 360;
            headAxis = Mathf.Clamp(headAxis, -maxheadYAngle, maxheadYAngle);
            headAxis /= 90;
        }
        else
        {
            //Calculates keyboard input if no head connected
            headAxis = Input.GetAxis("Horizontal");
            headAxis = Mathf.Clamp(headAxis, -maxheadYAngle / 90, maxheadYAngle / 90);
        }

        //Turn right
        Vector3 right = Vector3.Cross(Vector3.up, m_track.trackSection.trackDirection).normalized;

        if (m_ragdollController.ragdollActivated)
            targetRB.AddForce(boostMultiplier * headAxis * turnForceRagdoll * right, ForceMode.Acceleration);
        else
            targetRB.AddForce(boostMultiplier * headAxis * turnForce * right, ForceMode.Acceleration);
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;

        Vector3 XZVelocity;
        if( m_ragdollController.ragdollActivated)
            XZVelocity = new Vector3(m_ragdollController.averageVelocity.x,0, m_ragdollController.averageVelocity.z);
        else
            XZVelocity = new Vector3(m_rb.velocity.x, 0, m_rb.velocity.z);

        GUI.Label(new Rect(0, 20, 100, 10), "XZ velocity: " + XZVelocity.magnitude, style);
    }
}
