using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSection : MonoBehaviour
{
    [Header("Key positions")]
    [SerializeField] private Transform m_targetGrap;
    [SerializeField] private Transform m_targetCamera;
    [SerializeField] private Transform m_positionCamera;
    [SerializeField] private Transform m_popupObjects;

    [Header("Lerp")]
    [SerializeField] private float m_cameraLerpPosition = 0.1f;
    [SerializeField] private float m_cameraLerpRotation = 0.1f;
    //References
    private PlayerController m_playerController;
    private RagdollController m_ragdollController;
    private Grap m_grap;
    private CameraController m_cameraController;

    //Private members
    private bool m_endTriggered = false;
    private Quaternion m_endCameraRotation;

    private void Awake()
    {
        m_cameraController = FindObjectOfType<CameraController>();
        m_playerController = FindObjectOfType<PlayerController>();
        m_ragdollController = FindObjectOfType<RagdollController>();
        m_grap = FindObjectOfType<Grap>();

        m_popupObjects.gameObject.SetActive(false);

        m_endCameraRotation = Quaternion.LookRotation(m_targetCamera.transform.position- m_positionCamera.transform.position);



    }

    // Use this for initialization
    void Start ()
    {
		
	}

    private void ShowEnd()
    {
        m_endTriggered = true;

        //Disable/enable required scripts
        m_popupObjects.gameObject.SetActive(true);
        m_playerController.enabled = false;
        m_cameraController.enabled = false;

        //Set the player
        m_grap.Cancel();
        m_grap.Throw(m_targetGrap.position, m_targetGrap.gameObject);
        m_ragdollController.SetRagdoll(true);

        AkSoundEngine.PostEvent("Play_Victory", gameObject);
    }
	

	void LateUpdate ()
    {
		if(m_endTriggered)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, m_positionCamera.position, m_cameraLerpPosition);
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, m_endCameraRotation, m_cameraLerpRotation  );
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(!m_endTriggered)
            ShowEnd();
    }
}
