using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;




[RequireComponent( typeof(  Camera ))]
public class RagdollCameraController : MonoBehaviour
{
    private PlayerController m_playerController;
    private Rigidbody m_targetRb;

    private Vector3 m_translation;

    private void Awake()
    {
        m_playerController = FindObjectOfType<PlayerController>();
        m_targetRb = m_playerController.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Head).GetComponent<Rigidbody>();;
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        m_translation = transform.position - m_targetRb.transform.position;
    }

    private void LateUpdate()
    {
        transform.position = m_targetRb.transform.position + m_translation ;
        transform.LookAt(m_targetRb.transform.position);
    }
}
