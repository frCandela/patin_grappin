using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grap : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject ropePrefab = null;
    [SerializeField] private GameObject targetPrefab = null;

    [Header("Parameters")]
    [SerializeField, Range(0f, 1000f)]
    private float attractionForce = 50f;
    [SerializeField, Range(0f, 1000f)] private float minDistance = 5;
    [SerializeField, Range(0f, 1f)] private float elasticity = 1f;

    //Public properties
    public bool grappling { get; private set; }
    public GameObject grappleTarget { get; private set; }

    //References
    private Rigidbody m_rigidbody;
    private GameObject m_aimTarget;
    private Rope m_rope = null;
    private Vector3 m_target;
    private RagdollController m_ragdollController;
    private AnimationController m_animationController;

    bool m_rightHandUsed = true;

    private void Awake()
    {
        //Get components
        m_rigidbody = GetComponent<Rigidbody>();
        m_ragdollController = FindObjectOfType<RagdollController>();
        m_animationController = FindObjectOfType<AnimationController>();

        //Target world point
        grappleTarget = GameObject.Instantiate(targetPrefab);
        m_aimTarget = GameObject.Instantiate(targetPrefab);

        //Set Rope
        Util.EditorAssert(ropePrefab != null, "Grapple.Awake: ropePrefab not set");
        m_rope = Instantiate(ropePrefab).GetComponent<Rope>();
        m_rope.enabled = false;

        grappling = false;
    }

    public bool Throw()
    {
        if (!grappling)
        {
            m_target = GazeManager.GetGazeWorldPoint();

            //Launch the grapple if the target is valid
            if (m_target != Vector3.zero)
            {
                m_rightHandUsed = m_animationController.rightHandUsed;
                AkSoundEngine.PostEvent("Play_Grab_Impact", gameObject);
                m_rope.SetRope(m_animationController.grappleHandTransform, grappleTarget.transform);
                grappling = true;
                m_rope.enabled = true;
                grappleTarget.transform.position = m_target;
                return true;
            }
        }
        return false;
    }

    public bool Cancel()
    {
        //Cancel the grapple
        if (grappling)
        {
            grappling = false;
            m_rope.enabled = false;
            return true;
        }
        return false;
    }

    private void Update()
    {
        //Update the grapple target position
        m_aimTarget.transform.position = GazeManager.GetGazeWorldPoint();

        //Shader variable
        Shader.SetGlobalVector("_AimTargetPos", m_aimTarget.transform.position);
    }

    void FixedUpdate()
    {
        if (grappling)
        {
            //Change the target rigidbody if the ragdoll is activated
            Rigidbody targetRb;
            if ( ! m_ragdollController.ragdollActivated)
                targetRb = m_rigidbody;
            else
            {
                if(m_rightHandUsed)
                    targetRb = m_ragdollController.rightArmRb;
                else
                    targetRb = m_ragdollController.leftArmRb;
            }
               

            float sqrDist = Vector3.SqrMagnitude(m_target - transform.position);
            if (sqrDist > minDistance * minDistance)
            {
                //Force in the good direction
                Vector3 direction = (m_target - transform.position).normalized;
                targetRb.AddForce(attractionForce * direction, ForceMode.Acceleration);

                //Remove velocity in the wrong direction
                float wrongVelocity = Vector3.Dot(targetRb.velocity, direction);
                if (wrongVelocity < 0)
                    targetRb.velocity = targetRb.velocity - elasticity * wrongVelocity * direction;
            }
        }
    }
}
