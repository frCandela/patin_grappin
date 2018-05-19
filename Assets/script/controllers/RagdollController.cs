using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
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

    private Transform spineTransform;

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();

        m_mainRb = GetComponent<Rigidbody>();
        m_mainCollider = GetComponent<Collider>();

        m_ragdollRbs = GetComponentsInChildren<Rigidbody>();
        m_ragdollColliders = GetComponentsInChildren<Collider>();

        m_playerController = GetComponent<PlayerController>();

        spineTransform = m_playerController.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Spine);
        leftArmRb = m_playerController.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftLowerArm).GetComponent<Rigidbody>();
        rightArmRb = m_playerController.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightLowerArm).GetComponent<Rigidbody>();
    }


    // Use this for initialization
    void Start ()
    {
        SetRagdoll(false);
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            SetRagdoll(true);
        if (Input.GetKeyUp(KeyCode.A))
            SetRagdoll(false);

        if(ragdollActivated)
        {
            m_mainRb.position = spineTransform.position;
            UpdateAverageRagdollVelocity();
        }
            


    }

    public void SetRagdoll(bool state)
    {
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
            m_animator.enabled = false;
            //m_playerController.enabled = false;
           // m_ragdollCameraController.enabled = true;
            //m_cameraControler.enabled = false;
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
            //m_playerController.enabled = true;
            //m_ragdollCameraController.enabled = false;
            //m_cameraControler.enabled = true;
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

}
