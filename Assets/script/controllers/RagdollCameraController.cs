using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;




[RequireComponent( typeof(  Camera ))]
public class RagdollCameraController : MonoBehaviour
{
    [SerializeField] private float lerpPosition = 0.1f;
    [SerializeField] private float lerpRotation = 0.1f;

    private PlayerController m_playerController;
    private Transform m_targetTransform;

    private Vector3 m_translation;

    private void Awake()
    {
        m_playerController = FindObjectOfType<PlayerController>();
        m_targetTransform = m_playerController.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Spine);
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        m_translation = transform.position - m_targetTransform.transform.position;
    }

    private void LateUpdate()
    {
        //Lerp position
        Vector3 newPos = m_targetTransform.position + m_translation;
        transform.position = Vector3.Lerp(transform.position, newPos, lerpPosition);

        //Lerp rotation
        Quaternion prevRot = transform.rotation;
        transform.LookAt(m_targetTransform.position);
        transform.rotation = Quaternion.Lerp(prevRot, transform.rotation, lerpRotation);

    }
}
