using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grapple : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject ropePrefab;
    [SerializeField] private GameObject targetPrefab;

    [Header("Parameters")]
    [SerializeField, Range(0f, 1000f)] private float attractionForce = 50f;
    [SerializeField, Range(0f, 1000f)] private float minDistance = 1;
    [SerializeField, Range(0f, 1000f)] private float maxDistance = float.MaxValue;
    [SerializeField, Range(0f, 1f)] private float elasticity = 1f;

    public bool isGrappling
    {
        get         { return m_grappling;  }
        private set { m_grappling = value; }
    }

    private Rigidbody m_rigidbody;

    private GameObject m_grappleTarget;
    private GameObject m_aimTarget;

    private Rope m_rope = null;


    //private float m_distance = -1f;
    private bool m_grappling = false;
    private Vector3 m_target;

    private void Awake()
    {
        //Get components
        m_rigidbody = GetComponent<Rigidbody>();

        //Target world point
        m_grappleTarget = GameObject.Instantiate(targetPrefab);
        m_aimTarget = GameObject.Instantiate(targetPrefab);

        //Rope
        Util.EditorAssert(ropePrefab != null, "Grapple.Awake: ropePrefab not set");
        GameObject tmpRope = Instantiate(ropePrefab);
        m_rope = tmpRope.GetComponent<Rope>();
        m_rope.SetRope(gameObject, m_grappleTarget);
        m_rope.enabled = false;
    }

    public bool Throw()
    {
        if (!m_grappling)
        {
            //Get the grapple target : first with the GetGazeWorldPoint else with the mouse position
            m_target = GazeManager.GetGazeWorldPoint();
            if (m_target == Vector3.zero)
            {
                if (Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width && Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit raycastHit;
                    if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity))
                        m_target = raycastHit.point;
                }
            }

            //Launche the grapple if the target is valid
            if (m_target != Vector3.zero)
            {
                AkSoundEngine.PostEvent("Play_Grab_Impact", gameObject);

                m_grappling = true;
                m_rope.enabled = true;
                m_grappleTarget.transform.position = m_target;
                return true;
            }
        }
        return false;
    }

    public bool Cancel()
    {
        if (m_grappling)
        {
            m_grappling = false;
            m_rope.enabled = false;
            return true;
        }
        return false;
    }

    private void Update()
    {
        m_aimTarget.transform.position = GazeManager.GetGazeWorldPoint();
    }

    void FixedUpdate ()
    {
        if(m_grappling)
        {
            float sqrDist = Vector3.SqrMagnitude(m_target - transform.position);
            if(sqrDist > minDistance * minDistance && sqrDist < maxDistance*maxDistance)
            {
                //Force in the good direction
                Vector3 direction = (m_target - transform.position).normalized;
                m_rigidbody.AddForce(attractionForce * direction, ForceMode.Acceleration);

                //Remove velocity in the wrong direction
                float wrongVelocity = Vector3.Dot(m_rigidbody.velocity, direction);
                if (wrongVelocity < 0)
                    m_rigidbody.velocity = m_rigidbody.velocity - elasticity * wrongVelocity * direction;
            }
        }
	}
}
