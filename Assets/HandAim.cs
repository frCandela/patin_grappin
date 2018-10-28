using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandAim : MonoBehaviour {

    [SerializeField] private GameObject aimPrefab = null;
    private GameObject m_aim = null;
    private Grap grap;
    private PlayerController player;
    private Rigidbody playerRb;
    [SerializeField, Range(0f, 1000f)] private float attractionForce = 50f;
    [SerializeField, Range(0f, 1f)] private float elasticity = 0.5f;

    public bool grappling = false;
    Vector3 targetGrap;

    // Use this for initialization
    void Awake ()
    {
        grap = FindObjectOfType<Grap>();
        player = FindObjectOfType<PlayerController>();
        playerRb = player.GetComponent<Rigidbody>();

        Util.EditorAssert(aimPrefab != null, "Grapple.Awake: ropePrefab not set");
        m_aim = Instantiate(aimPrefab);
        m_aim.GetComponent<MeshRenderer>().enabled = false;
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
        Quaternion rHandRot = InputTracking.GetLocalRotation(XRNode.RightHand);
        Vector3 rHandPos = transform.parent.position + InputTracking.GetLocalPosition(XRNode.RightHand);
        Vector3 rHandRotForward = rHandRot * Vector3.forward;

        float length = 0.1f;

        Ray ray = new Ray(rHandPos, rHandRotForward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            length = hit.distance;
        }

        if (Input.GetButtonDown("Grapple"))
        {
            if (!grappling && length != 0.1f)
            {
                AkSoundEngine.PostEvent("Play_Grab_Impact", gameObject);
                grappling = true;
                targetGrap = hit.point;
                m_aim.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        if (Input.GetButtonUp("Grapple"))
        {
            if (grappling)
            {
                grappling = false;
            }
        }

        if( !grappling)
        {
            m_aim.transform.rotation = rHandRot;
            m_aim.transform.localScale = new Vector3(0.01f, 0.01f, length);
            m_aim.transform.position = rHandPos + 0.5f * length * rHandRotForward;

            if (Input.GetButtonDown("GrappleAim"))
                m_aim.GetComponent<MeshRenderer>().enabled = true;
            if (Input.GetButtonUp("GrappleAim"))
                m_aim.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            Vector3 dir = targetGrap - rHandPos;

            m_aim.transform.localScale = new Vector3(0.05f, 0.05f, dir.magnitude);
            m_aim.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            m_aim.transform.position = rHandPos + 0.5f * dir;
        }
    }
}
