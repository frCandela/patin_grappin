using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grap : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject ropePrefab = null;
    [SerializeField] private GameObject targetPrefab = null;
    [SerializeField] private GameObject fxPrefab = null;

    [Header("Parameters")]
    [SerializeField, Range(0f, 1000f)] private float attractionForce = 50f;
    [SerializeField, Range(0f, 2000f)] private float attractionForceRagdoll = 1000;
    [SerializeField, Range(0f, 1000f)] private float maxDistance = 150;
    [SerializeField, Range(0f, 1000f)] private float minDistance = 20;
    [SerializeField, Range(0f, 1f)] private float elasticity = 1f;  

    //Public properties
    public bool grappling { get; private set; }

    //Instances
    public GameObject grappleTarget { get; private set; }
    private GameObject m_aimTarget;
    private GameObject m_grabFX;

    //References
    private Rigidbody m_rigidbody;
    private ParticleSystem m_particleSystem;
    private Rope m_rope = null;
    private Vector3 m_target;
    private RagdollController m_ragdollController;
    private AnimationController m_animationController;

    bool m_rightHandUsed = true;

    private void Awake()
    {
        //Instances
        grappleTarget = GameObject.Instantiate(targetPrefab);
        m_aimTarget = new GameObject();
        m_grabFX = GameObject.Instantiate(fxPrefab);
        //m_grabFX.transform.parent = grappleTarget.transform;
        //m_grabFX.transform.localPosition = Vector3.zero;
        
        //Get components
        m_rigidbody = GetComponent<Rigidbody>();
        m_ragdollController = FindObjectOfType<RagdollController>();
        m_animationController = FindObjectOfType<AnimationController>();
        m_particleSystem = m_grabFX.GetComponent<ParticleSystem>();

        //Set Rope
        Util.EditorAssert(ropePrefab != null, "Grapple.Awake: ropePrefab not set");
        m_rope = Instantiate(ropePrefab).GetComponent<Rope>();

        grappling = false;
    }

    public bool Throw()
    {
        GazeManager.GazeInfo result = GazeManager.GetGazeWorldPoint();
        if (!grappling && result != null)
        {
            m_target = result.position;

            float sqrDist = Vector3.SqrMagnitude(m_target - transform.position);

            //Launch the grapple if the target is valid
            if (m_target != Vector3.zero && sqrDist < maxDistance * maxDistance)
            {
                m_rightHandUsed = m_animationController.rightHandUsed;
                AkSoundEngine.PostEvent("Play_Grab_Impact", gameObject);
                m_rope.SetRope(m_animationController.grappleHandTransform, grappleTarget.transform);
                grappling = true;


                //Set hook
                Vector3 direction = m_aimTarget.transform.position - m_animationController.grappleHandTransform.position;
                grappleTarget.transform.position = m_target - 1.5f * direction.normalized;
                grappleTarget.transform.LookAt(grappleTarget.transform.position - direction);

                //Particle system
                m_particleSystem.transform.position = m_aimTarget.transform.position;
                m_particleSystem.Emit(1);

                //Cloud
                if (result.gameobject.tag == "cloud")
                {
                    Animator cloudAnimator = result.gameobject.GetComponent<Animator>();
                    //cloudAnimator.Play("cloud_take",-1,0f);
                }
                return true;
            }
        }
        else
            m_aimTarget.transform.position = Vector3.zero;
        return false;
    }

    public bool Cancel()
    {
        //Cancel the grapple
        if (grappling)
        {
            grappling = false;
            m_rope.ResetRope();
            return true;
        }
        return false;
    }

    private void Update()
    {
        //Update the grapple target position
        GazeManager.GazeInfo result = GazeManager.GetGazeWorldPoint();

        if (result != null)
            m_aimTarget.transform.position = result.position;
        else
            m_aimTarget.transform.position = Vector3.zero;

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

                if(m_ragdollController.ragdollActivated)
                    targetRb.AddForce(attractionForceRagdoll * direction, ForceMode.Acceleration);
                else
                    targetRb.AddForce(attractionForce * direction, ForceMode.Acceleration);

                //Remove velocity in the wrong direction
                float wrongVelocity = Vector3.Dot(targetRb.velocity, direction);
                if (wrongVelocity < 0)
                    targetRb.velocity = targetRb.velocity - elasticity * wrongVelocity * direction;
            }
        }
    }
}
