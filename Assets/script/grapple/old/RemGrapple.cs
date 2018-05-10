using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpringJoint))]
public class RemGrapple : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject ropePrefab;


    [Header("Editor")]
    [SerializeField] private bool showTarget = true;

    //Usefull components
    private GameObject m_grappleTarget = null;
    private Rigidbody m_grappleTargetRb = null;
    private SpringJoint m_springJoint = null;

    //Graphic instances
    private GameObject m_grappleWorldPointSphere = null;
    private Rope m_rope = null;


    public bool m_grappling = false;

    private void Awake()
    {
        //Target world point
        m_grappleWorldPointSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_grappleWorldPointSphere.GetComponent<Collider>().isTrigger = true;
        m_grappleWorldPointSphere.name = "grappleWorldPointSphere";
        m_grappleWorldPointSphere.transform.localScale = 0.5f * m_grappleWorldPointSphere.transform.localScale;
        m_grappleWorldPointSphere.layer = LayerMask.NameToLayer("Ignore Raycast");

        //Target for the spring join
        m_grappleTarget = new GameObject();
        m_grappleTarget.name = "GrappleTarget";
        m_grappleTargetRb = m_grappleTarget.AddComponent<Rigidbody>();
        m_grappleTargetRb.mass = 0;

        //Rope
        Util.EditorAssert(ropePrefab != null, "Grapple.Awake: ropePrefab not set");
        GameObject tmpRope = Instantiate(ropePrefab);
        m_rope = tmpRope.GetComponent<Rope>();
        m_rope.SetRope(transform, m_grappleTarget.transform);
        m_rope.enabled = false;

        //Spring joint
        m_springJoint = GetComponent<SpringJoint>();
        m_springJoint.autoConfigureConnectedAnchor = false;
        m_springJoint.connectedBody = m_grappleTargetRb;
        m_springJoint.anchor = Vector3.zero;
        m_springJoint.connectedAnchor = Vector3.zero;
        m_springJoint.maxDistance = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Target world point position set
        if (showTarget)
        {
            if (m_grappling)
                m_grappleWorldPointSphere.transform.position = m_grappleTargetRb.transform.position;
            else
                m_grappleWorldPointSphere.transform.position = GazeManager.GetGazeWorldPoint();
        }
            
        else
            m_grappleWorldPointSphere.transform.position = Vector3.zero;
      

        if (Input.GetButtonDown("Grapple"))
        {
            if (m_grappling)
                ResetGrapple();
            else
                TryGrappleTarget();
        }
    }

    void TryGrappleTarget(  )
    {
        Vector3 targetPosition = GazeManager.GetGazeWorldPoint();
        if(targetPosition != Vector3.zero)
        {
            m_grappleTargetRb.isKinematic = true;
            m_grappleTargetRb.position = targetPosition;
            m_rope.enabled = true;
            m_grappling = true;
        }
    }


    void ResetGrapple()
    {
        m_grappleTargetRb.isKinematic = false;
        m_rope.enabled = false;
        m_grappling = false;
    }
}
