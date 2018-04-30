using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadPlayerController1 : MonoBehaviour
{
    [Header("Linked Instances:")]
    [SerializeField]private Track track = null;

    [Header("Movement:")]
    [SerializeField] private float velocity = 50f;
    [SerializeField] private float boostForce = 15f;
    [SerializeField] private float turnForce = 2.5f;
    [SerializeField] private float maxTurnForce = 0.5f;
    [SerializeField] private float powerTurn = 1f;
    [Header("Other:")]
    [SerializeField] private float gravity = -20;

    //Components references
    private Rigidbody m_rb;
    private BestGrapple m_grapple;

    // Use this for initialization
    void Awake ()
    {
        Util.EditorAssert(track != null, "BetterHeadPlayerController.Awake(): no track set");

        m_rb = GetComponent<Rigidbody>();
        m_grapple = GetComponent<BestGrapple>();

        Physics.gravity = new Vector3(0, gravity, 0);
    }

    private void Start()
    {
        //Initial parameters
        m_rb.velocity = 1f * Vector3.forward;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetButtonDown("Grapple"))
        {
            m_grapple.Toogle();
            AkSoundEngine.PostEvent("Play_Grab_Impact", gameObject);
        }
        else if (Input.GetButtonUp("Grapple"))
            m_grapple.Toogle();
    }

    void FixedUpdate()
    {
        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();

        //Tobii  control
        if (pose.IsValid)
        {
            //Calculates head input
            float headAxis = pose.Rotation.eulerAngles.y;
            if (headAxis > 180)
                headAxis -= 360;
            headAxis /= 90;
            float sign = headAxis > 0 ? 1: -1;
            headAxis = sign * Mathf.Abs( Mathf.Pow(Mathf.Abs(headAxis), powerTurn));
            headAxis = Mathf.Clamp(headAxis, -maxTurnForce, maxTurnForce);

            //turn
            transform.Rotate(Vector3.up, turnForce * headAxis);

            //Change thhe x,z velocity to go in the right direction
            float yVel = m_rb.velocity.y;
            m_rb.velocity = velocity * transform.forward;
            m_rb.velocity = new Vector3(m_rb.velocity.x, yVel, m_rb.velocity.z);
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
