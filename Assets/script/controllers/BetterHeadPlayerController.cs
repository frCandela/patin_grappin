using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BetterHeadPlayerController : MonoBehaviour
{
    [Header("Movement parameters:")]
    [SerializeField] private float velocity = 2f;
    [SerializeField] private float boostForce = 15f;
    [SerializeField] private float turnForce = 150f;


    //Components references
    private Rigidbody m_rb;
    private BestGrapple m_grapple;

    private float m_boostMultiplier = 1f;

    // Use this for initialization
    void Awake ()
    {
        m_rb = GetComponent<Rigidbody>();
        m_grapple = GetComponent<BestGrapple>();

        Physics.gravity = new Vector3(0, -20, 0);
    }

    private void Start()
    {
        //Looks for every boosts and connects the events
        object[] objArray = GameObject.FindObjectsOfType(typeof(GOChangeColor));
        foreach (object obj in objArray)
        {
            GOChangeColor boost = (GOChangeColor)obj;
            boost.onColorChanged.AddListener(StartBoost);
        }

        //Initial parameters
        m_rb.velocity = 5f * Vector3.forward;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetButtonDown("Grapple"))
        {
            m_grapple.Toogle();
        }
    }

    private void StartBoost()
    {
        m_rb.AddForce(boostForce * m_rb.velocity.normalized, ForceMode.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        Quaternion trackRot = Quaternion.LookRotation(TrackSection.trackDir);
        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();
        if (pose.IsValid)// && ! m_grapple.m_grappling)
        {
            m_rb.AddForce(trackRot *  Vector3.right * turnForce * pose.Rotation.y, ForceMode.Acceleration);
        }

        Quaternion headRot = Quaternion.Euler(0, pose.Rotation.eulerAngles.y, 0);

        m_rb.AddForce(headRot * trackRot * Vector3.forward * m_boostMultiplier * velocity, ForceMode.Acceleration);

        //rotates the model depending on his speed
        if( m_rb.velocity.magnitude > 1f )
            transform.rotation = Quaternion.LookRotation(m_rb.velocity);
    }
}
