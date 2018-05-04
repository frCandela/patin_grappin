using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Linked Instances:")]
    [SerializeField]private Track track = null;

    [Header("Movement:")]
    [SerializeField] private float initialVelocity = 2f;
    [SerializeField] private float forwardForce = 30f;
    [SerializeField] private float turnForce = 300f;

    [SerializeField] private float maxheadYAngle = 20f;
    /* [SerializeField] private float boostForce = 15f;
     
     
     [SerializeField] private float maxRightSpeed = 20f;*/

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
        m_rb.velocity = initialVelocity * track.trackSection.trackDirection;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetButtonDown("Grapple"))
            m_grapple.Toogle();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_rb.AddForce(forwardForce * track.trackSection.trackDirection);
        m_rb.transform.rotation = Quaternion.LookRotation(m_rb.velocity);

        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();
        if (pose.IsValid)
        {
            //Calculates head input
            float headAxis = pose.Rotation.eulerAngles.y;
            if (headAxis > 180)
                headAxis -= 360;
            headAxis = Mathf.Clamp(headAxis, -maxheadYAngle, maxheadYAngle);
            headAxis /= 90;
            
            //Turn right
            Vector3 right = Vector3.Cross(Vector3.up, track.trackSection.trackDirection).normalized;
            m_rb.AddForce(headAxis * turnForce * right);
        }

        //rotates the model depending on his speed
        /*  transform.rotation = Quaternion.LookRotation(transform.position - m_previousPosition);
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
          if (!m_grapple.isGrappling)
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
          }*/
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
