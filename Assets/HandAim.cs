using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandAim : MonoBehaviour {

    [Header("Prefabs")]
    [SerializeField] private GameObject fxPrefab = null;
    [SerializeField] private GameObject aimPrefab = null;
    [SerializeField] private GameObject ropePrefab = null;
    [SerializeField] private GameObject targetPrefab = null;

    [Header("Parameters")]

    private Grap grap;
    private PlayerController player;
    private Rigidbody playerRb;
    [SerializeField, Range(0f, 1000f)] private float attractionForce = 50f;
    [SerializeField, Range(0f, 1f)] private float elasticity = 0.5f;
    [SerializeField] private string grapName = "GrappleR";
    [SerializeField] private string grapAxisName = "GripR";

    [SerializeField] XRNode hand = XRNode.RightHand;

    public GameObject grappleTarget { get; private set; }
    private GameObject m_grabFX;
    private GameObject m_aim = null;
    private Material m_aimMat = null;


    private ParticleSystem m_particleSystem;

    public bool grappling = false;
    Vector3 targetGrap;
    private Rope m_rope = null;

    // Use this for initialization
    void Awake ()
    {
        grap = FindObjectOfType<Grap>();
        player = FindObjectOfType<PlayerController>();
        playerRb = player.GetComponent<Rigidbody>();

        Util.EditorAssert(aimPrefab != null, "Grapple.Awake: ropePrefab not set");
        m_aim = Instantiate(aimPrefab);
        m_aim.GetComponent<MeshRenderer>().enabled = false;
        m_aimMat = m_aim.GetComponent<MeshRenderer>().material;


     m_grabFX = GameObject.Instantiate(fxPrefab);
        m_particleSystem = m_grabFX.GetComponent<ParticleSystem>();

        m_rope = Instantiate(ropePrefab).GetComponent<Rope>();

        grappleTarget = GameObject.Instantiate(targetPrefab);
    }

    private void FixedUpdate()
    {
        if( grappling)
        {          
            Vector3 rHandPos = transform.parent.position + InputTracking.GetLocalPosition(XRNode.RightHand);
            Vector3 dir = (targetGrap - rHandPos).normalized;

            playerRb.AddForce(attractionForce * dir, ForceMode.Acceleration);

            //Remove velocity in the wrong direction
            float wrongVelocity = Vector3.Dot(playerRb.velocity, dir);
            if (wrongVelocity < 0)
                playerRb.velocity = playerRb.velocity - elasticity * wrongVelocity * dir;


            //playerRb.AddForce(grapForce * dir, ForceMode.Acceleration);
        }
    }

    // Update is called once per frame
    void Update ()
    {

        Quaternion rHandRot = transform.rotation;
        Vector3 rHandPos = transform.parent.position + InputTracking.GetLocalPosition(hand);
        Vector3 rHandRotForward = transform.forward;

        // Raycast to aim with the gapple
        float length = 0.1f;
        Ray ray = new Ray(rHandPos, rHandRotForward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            length = hit.distance;
        }

        if (Input.GetButtonDown(grapName) || Input.GetAxis(grapAxisName) != 0f)
        {
            if (!grappling && length != 0.1f)
            {

                grappling = true;
                targetGrap = hit.point;
                m_aim.GetComponent<MeshRenderer>().enabled = true;

                //Cloud anim
                if (hit.collider.gameObject.tag == "cloud")
                {
                    Animator cloudAnimator = hit.collider.gameObject.GetComponent<Animator>();
                    cloudAnimator.Play("cloud_take", -1, 0f);
                }

                //grab fx
                m_particleSystem.transform.position = targetGrap;
                m_particleSystem.Emit(1);
                AkSoundEngine.PostEvent("Play_Grab_Impact", gameObject);

                //Set hook  
                Vector3 direction = targetGrap - rHandPos;
                grappleTarget.transform.position = targetGrap - 1.5f * direction.normalized;
                grappleTarget.transform.LookAt(grappleTarget.transform.position - direction);

                grappleTarget.SetActive(true);
                m_rope.SetRope(gameObject.transform, grappleTarget.transform);
            }
        }

        if ( (Input.GetAxis(grapAxisName) == 0f &&  ! Input.GetButton(grapName)))
        {
            if (grappling)
            {
                grappling = false;
                m_rope.ResetRope();
                grappleTarget.SetActive(false);
            }
        }

        if( !grappling)
        {
            m_aim.transform.rotation = rHandRot;
            m_aim.transform.localScale = new Vector3(0.01f, 0.01f, 1000f);
            m_aim.transform.position = rHandPos + 0.5f * 1000f * rHandRotForward;
            m_aim.GetComponent<MeshRenderer>().enabled = true;

            // color aim 
            if( length != 0.1f)
                m_aimMat.color = new Color(1f,0.2f,0.2f, 1f);
            else
                m_aimMat.color = new Color(0f, 0f, 0f, 5f);

        }
        else
        {
            m_aim.GetComponent<MeshRenderer>().enabled = false;
            /*Vector3 dir = targetGrap - rHandPos;

            m_aim.transform.localScale = new Vector3(0.05f, 0.05f, dir.magnitude);
            m_aim.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            m_aim.transform.position = rHandPos + 0.5f * dir;*/
        }
    }
}
