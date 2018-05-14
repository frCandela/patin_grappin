using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;




[RequireComponent( typeof(  Camera ))]
public class RagdollCameraController : MonoBehaviour
{
    private PlayerController m_playerController;
    private Rigidbody m_targetRb;
    CameraController m_cameraController;

    bool used = false;

    private void Awake()
    {
        m_playerController = FindObjectOfType<PlayerController>();
        m_targetRb = m_playerController.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Head).GetComponent<Rigidbody>();
        m_cameraController = GetComponent<CameraController>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            used = true;
            m_cameraController.enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {//SetRagdoll(false);
            used = false;
            m_cameraController.enabled = true;
        }

        //if (false)
        {
            transform.position = m_targetRb.transform.position - 15 * Vector3.forward + 5 * Vector3.up;
            transform.LookAt(m_targetRb.transform.position);
        }

    }



}
