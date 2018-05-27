using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RagdollController : MonoBehaviour
{
    [Header("Triggers")]
    [SerializeField] private float m_ragdollDuration = 2f;

    [Header("Triggers")]
    [SerializeField] private float m_minVelocity = 30f;

    [SerializeField, Range(0f,1f )] private float m_VelocityRatioTrigger = 0.5f;

    //Events
    public UnityEvent onRagdollStart;
    public UnityEvent onRagdollStop;

    //References
    private Animator m_animator;
    private Rigidbody m_mainRb;
    private Collider m_mainCollider;
    private Rigidbody[] m_ragdollRbs;
    private Collider[] m_ragdollColliders;
    private PlayerController m_playerController;

    //Properties
    public bool ragdollActivated { get; private set; }
    public Vector3 averageVelocity { get; private set; }
    public Rigidbody rightArmRb { get; private set; }
    public Rigidbody leftArmRb { get; private set; }
    public Rigidbody mainRb { get; private set; }
    private Transform spineTransform;

    private float startTime = 0;

    //private 
    float prevXZvelocity = 1000f;

    private void Awake()
    {
        //Set events
        onRagdollStart = new UnityEvent();
        onRagdollStop = new UnityEvent();

        //Get references
        m_animator = GetComponentInChildren<Animator>();
        m_mainRb = GetComponent<Rigidbody>();
        m_mainCollider = GetComponent<Collider>();
        m_ragdollRbs = GetComponentsInChildren<Rigidbody>();
        m_ragdollColliders = GetComponentsInChildren<Collider>();
        m_playerController = GetComponent<PlayerController>();

        //Set properties
        spineTransform = m_playerController.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Spine);
        leftArmRb = m_playerController.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftLowerArm).GetComponent<Rigidbody>();
        rightArmRb = m_playerController.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightLowerArm).GetComponent<Rigidbody>();
        mainRb =  m_playerController.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
    }


    // Use this for initialization
    void Start ()
    {
        SetRagdoll(false, true);
        startTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        //Ragdoll input
        if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(StartRagdoll());

        //Ragdoll updates
        if (ragdollActivated)
        {
            m_mainRb.position = spineTransform.position;
            UpdateAverageRagdollVelocity();
        }
        else
        {
            //Impact triggers ragdoll
            Vector3 XZVelocityVec = new Vector3(m_mainRb.velocity.x, 0, m_mainRb.velocity.z);
            float XZVelocity = XZVelocityVec.magnitude;

            if (XZVelocity < prevXZvelocity)
                if (XZVelocity / prevXZvelocity < 1f - m_VelocityRatioTrigger && Time.realtimeSinceStartup > startTime + 3)                
                    StartCoroutine(StartRagdoll());                

            prevXZvelocity = XZVelocity;
        }
    }

    public void SetRagdoll(bool state, bool force = false)
    {
        //Return if we are already in the same state
        if (ragdollActivated == state && !force)
            return;

        ragdollActivated = state;
        if (state)//Activates ragdoll
        {
            //Activates ragdoll colliders and rigidbodies
            foreach (Rigidbody rb in m_ragdollRbs)
                if (rb != m_mainRb)
                {
                    rb.isKinematic = false;
                    rb.velocity = m_mainRb.velocity;
                }
            foreach (Collider col in m_ragdollColliders)
                col.enabled = true;

            //Desactivate main collider, main rigidbody and animator
            m_mainCollider.enabled = false;
            m_mainRb.isKinematic = true;
           // m_animator.playableGraph
            m_animator.enabled = false;

            onRagdollStart.Invoke();
        }
        else//Desactivate ragdoll
        {
            //Desactivate ragdoll colliders and rigidbodies
            foreach (Rigidbody rb in m_ragdollRbs)
                if (rb != m_mainRb)
                    rb.isKinematic = true;
            foreach (Collider col in m_ragdollColliders)
                col.enabled = false;

            //Activate main collider, main rigidbody and animator
            m_mainCollider.enabled = true;
            m_mainRb.isKinematic = false;
            m_animator.enabled = true;
            m_mainRb.velocity = averageVelocity;

            onRagdollStop.Invoke();
        }
    }

    //Calculates the average velocity of all the ragdoll rigidbodies
   public void UpdateAverageRagdollVelocity()
    {
        averageVelocity = Vector3.zero;
        foreach (Rigidbody rb in m_ragdollRbs)
            if (rb != m_mainRb)
                averageVelocity += rb.velocity;

        averageVelocity /= m_ragdollRbs.Length - 1;
    }

    IEnumerator StartRagdoll()
    {
        SetRagdoll(true);
        yield return new WaitForSeconds(m_ragdollDuration);

        while (averageVelocity.sqrMagnitude < m_minVelocity * m_minVelocity)
        {
            yield return new WaitForSeconds(m_ragdollDuration / 2f);

        }
        SetRagdoll(false);
    }
}
