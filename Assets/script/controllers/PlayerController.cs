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

    [Header("Other:")]
    [SerializeField] private float gravity = -20;

    //Components references
    private Rigidbody m_rb;
    private Grapple m_grapple;


    // Use this for initialization
    void Awake ()
    {
        Util.EditorAssert(track != null, "BetterHeadPlayerController.Awake(): no track set");

        m_rb = GetComponent<Rigidbody>();
        m_grapple = GetComponent<Grapple>();

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

        if (Input.GetButtonDown("Grapple") || Input.GetButtonUp("Grapple"))
            m_grapple.Toogle();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_rb.AddForce(forwardForce * track.trackSection.trackDirection);
        m_rb.transform.rotation = Quaternion.LookRotation(m_rb.velocity);

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
            headAxis = Input.GetAxis("Horizontal");
            headAxis = Mathf.Clamp(headAxis, -maxheadYAngle / 90, maxheadYAngle / 90);
        }

        //Turn right
        Vector3 right = Vector3.Cross(Vector3.up, track.trackSection.trackDirection).normalized;
        m_rb.AddForce(headAxis * turnForce * right);

    }
}
