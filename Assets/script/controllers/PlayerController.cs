using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{ 
    [Header("Movement:")]
    [SerializeField] private float initialVelocity = 2f;
    [SerializeField] private float forwardVelocity = 30f;
    [SerializeField] private float turnFactor = 0.1f;
    [SerializeField] private float maxSpeedRtpc = 150f;
    [SerializeField] private float heightPlayer = 3.5f;

    
    [Header("Other:")]

    [SerializeField] private float gravity = -30;

    [Header("Events:")]
    public UnityEvent onGrappleLaunch;
    public UnityEvent onGrappleReset;
    
    bool grounded = false;

    //Components references
    private Rigidbody m_rb;
    private Grap m_grapple;
    private Track m_track = null;
    private ParticleSystem m_particleSystemPofPof = null;


    float boostEnd = -1f;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] float impulseForce = 100f;
    [SerializeField] float accelerationForce = 100f;

    void Boost()
    {
        Vector3 impulseDir = Camera.main.transform.forward;
        m_rb.AddForce(impulseForce * impulseDir, ForceMode.VelocityChange);

        boostEnd = Time.time + boostDuration;
    }

    // Use this for initialization
    void Awake ()
    {
        //Get references
        m_track = FindObjectOfType<Track>();
        m_rb = GetComponent<Rigidbody>();
        m_grapple = GetComponent<Grap>();

        Physics.gravity = new Vector3(0, gravity, 0);
        m_rb.velocity = initialVelocity * m_track.trackSection.trackDirection;

        //Set events
        onGrappleLaunch = new UnityEvent();
        onGrappleReset = new UnityEvent();

        m_particleSystemPofPof = GetComponentInChildren<ParticleSystem>();

        foreach( boostEyeFX befx in FindObjectsOfType<boostEyeFX>())
        {
            befx.onBoost.AddListener(Boost);
        }

    }

    private void Update()
    {
        float rtpc = Mathf.Clamp((m_rb.velocity.magnitude) / maxSpeedRtpc, 0f, 1f);
        AkSoundEngine.SetRTPCValue("Speed_RTPC", rtpc);

        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, heightPlayer, LayerMask.GetMask("Track")))
        {
            if( !grounded )
            {
                grounded = true;
                AkSoundEngine.PostEvent("Play_Ice_Skate_Reception", gameObject);
                m_particleSystemPofPof.Emit(6);
            }
        }
        else
        {
            if (grounded)
            {
                grounded = false;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if( boostEnd > Time.time)
        {
            //Vector3 impulseDir = m_rb.velocity.normalized;
            Vector3 impulseDir = Camera.main.transform.forward;
            // continuous force stay horizontal
            impulseDir.y = 0f;
            m_rb.AddForce(accelerationForce * impulseDir, ForceMode.Acceleration);
        }

        Quaternion rotation = Quaternion.Euler(0, InputTracking.GetLocalRotation(XRNode.Head).eulerAngles.y, 0);
        m_rb.rotation = rotation;
        Vector3 forward = InputTracking.GetLocalRotation(XRNode.Head) * Vector3.forward;
        m_rb.AddForce(forwardVelocity * forward.normalized, ForceMode.Acceleration);

        Vector3 dir = rotation * Vector3.forward;

        float yVel = m_rb.velocity.y;
        m_rb.velocity = new Vector3(m_rb.velocity.x, 0, m_rb.velocity.z);

        float goodVelocity = Vector3.Dot(m_rb.velocity, dir);
        m_rb.velocity = turnFactor * ( m_rb.velocity - goodVelocity * dir) + goodVelocity*dir;

        m_rb.velocity = new Vector3(m_rb.velocity.x, yVel, m_rb.velocity.z);
    }


}
