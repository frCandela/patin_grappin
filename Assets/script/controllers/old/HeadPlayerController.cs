using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadPlayerController : MonoBehaviour
{
    [Header("Linked Instances:")]
    [SerializeField]private Track track = null;

    [Header("Movement:")]
    [SerializeField] private float velocity = 2f;
    [SerializeField] private float boostForce = 15f;
    [SerializeField] private float turnForce = 150f;
    [SerializeField] private float maxTurnForce = 0.5f;

    [Header("Other:")]
    [SerializeField] private float gravity = -20;

    //Components references
    private Rigidbody m_rb;
    private Grap m_grapple;

    private Vector3 m_previousPosition;

    // Use this for initialization
    void Awake ()
    {
        m_rb = GetComponent<Rigidbody>();
        m_grapple = GetComponent<Grap>();

        Physics.gravity = new Vector3(0, gravity, 0);

        Util.EditorAssert(track != null, "BetterHeadPlayerController.Awake(): no track set");

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
        m_previousPosition = transform.position - transform.forward;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetButtonDown("Grapple"))
            m_grapple.Throw( transform );
        if (Input.GetButtonUp("Grapple"))
            m_grapple.Cancel();
    }

    private void StartBoost()
    {
        m_rb.AddForce(boostForce * m_rb.velocity.normalized, ForceMode.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //rotates the model depending on his speed
        transform.rotation = Quaternion.LookRotation(transform.position - m_previousPosition);
        m_previousPosition = transform.position;

        Quaternion trackRot = Quaternion.LookRotation(track.trackSection.trackDirection);
        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();

        //Eye tracker control

        //forward speed
        m_rb.AddForce(track.trackSection.trackDirection * velocity, ForceMode.Acceleration);

        //Calculates right speed
        Vector3 right = (trackRot * Vector3.right).normalized;
        float rightMagnitude = Vector3.Dot(m_rb.velocity, right);

        //Lerp the player velocity yo align it with the track direction
        if (!m_grapple.grappling)
            m_rb.velocity -= 0.3f * right * rightMagnitude;

        //Tobii  control
        if (pose.IsValid)
        {
            //Calculates head input
            float headAxis = pose.Rotation.eulerAngles.y;
            if (headAxis > 180)
                headAxis -= 360;
            headAxis /= 90;
            headAxis = Mathf.Clamp(headAxis, -maxTurnForce, maxTurnForce);

            // Turn right and left
            m_rb.transform.position = m_rb.transform.position + headAxis * turnForce * right;
        }
        //else
        {
            float horizontal = 0.5f * Input.GetAxis("Horizontal");
            horizontal = Mathf.Clamp(horizontal, -maxTurnForce, maxTurnForce);

            // Turn right and left
            m_rb.transform.position = m_rb.transform.position + horizontal * turnForce * right;
        }
    }


    private void OnDrawGizmos()
    {



        /*if (track && track.GetCurrentTrackSection().trackDirection != Vector3.zero)
        {
            Quaternion trackRot = Quaternion.LookRotation(track.GetCurrentTrackSection().trackDirection);
            Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();
            Quaternion headRot = Quaternion.Euler(0, pose.Rotation.eulerAngles.y, 0);
            Debug.DrawLine(transform.position, transform.position + headRot * trackRot * Vector3.right * turnForce);

            Debug.DrawLine(transform.position, transform.position + 20 * track.GetCurrentTrackSection().trackDirection);
        }*/
    }
}
