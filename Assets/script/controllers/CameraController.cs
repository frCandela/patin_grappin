using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;


[RequireComponent( typeof(  Camera ))]
public class CameraController : MonoBehaviour
{
    [Header("Head rotation:")]
    [SerializeField, Range(0f, 1f)] private float headRotLerpX = 0.1f;
    [SerializeField, Range(0f, 1f)] private float headRotLerpY = 0.1f;
    [SerializeField, Range(0f, 1f)] private float headRotLerpZ = 0.1f;

    [SerializeField, Range(0f, 5f)] private float headRotMultiX = 0.5f;
    [SerializeField, Range(0f, 5f)] private float headRotMultiY = 0.5f;
    [SerializeField, Range(0f, 5f)] private float headRotMultiZ = 0f;

    [Header("Camera movement:")]
    [SerializeField, Range(0f, 1f)] private float lerpPosition = 0.2f;
    [SerializeField, Range(0f, 1f)] private float lerpRotationY = 0.5f;
    [SerializeField, Range(0f, 1f)] private float lerpRotationXZ = 0.1f;

    [Header("Y correction:")]
    [SerializeField] private bool m_YCorrection = false;
    [SerializeField, Range(10f, 40f)] private float m_lowerAngleCorrection = 30;



    //References
    private Rigidbody targetRb = null;
    private Transform targetRagdoll = null;
    private Track m_track = null;
    RagdollController m_ragdollController = null;

    //Initial camera position and rotation parameters
    Vector3 m_baseTranslation;
    Quaternion m_baseRotationSelf;

    //Backup data for head rotation lerp
    private float m_prevPoseX = 0;
    private float m_prevPoseY = 0;
    private float m_prevPoseZ = 0;
    private Quaternion m_prevRot = Quaternion.identity;
    private float m_yOffset = 0f;

    //other
    public bool activateMusic = true;

    // Use this for initialization
    void Awake ()
    {
        //Get references
        m_track = FindObjectOfType<Track>();
        m_ragdollController = FindObjectOfType<RagdollController>();
        targetRb = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();
        targetRagdoll = FindObjectOfType<PlayerController>().GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Spine);

        Util.EditorAssert(m_track != null, "HeadCameraController.Awake(): no track in the level");
        Util.EditorAssert(targetRb != null, "HeadCameraController.Awake(): no playerRb set");
    }

    private void Start()
    {
        InitCamera();
        UpdateCameraTransform();

        //AkSoundEngine.PostEvent("Mute_FX_Mix", gameObject);
        AkSoundEngine.PostEvent("Play_Speed_RTPC", gameObject);

        //AkSoundEngine.PostEvent("Stop_Music_Placeholder", gameObject);
        //if (activateMusic)
        //AkSoundEngine.PostEvent("Play_Music_Placeholder", gameObject);
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        UpdateCameraTransform();
    }

    private void InitCamera()
    {
        m_baseTranslation = targetRb.transform.localPosition - transform.localPosition;

        Vector3 euler = transform.rotation.eulerAngles;
        m_baseRotationSelf = Quaternion.Euler(euler.x,0, euler.z);
    }

    public void UpdateCameraTransform()
    {
        //Get target position and rotation
        Vector3 targetPosition;
        Quaternion targetRotation;
        if (m_ragdollController.ragdollActivated)
        {
            targetPosition = targetRagdoll.transform.position;
            targetRotation = Quaternion.LookRotation(m_ragdollController.averageVelocity);
        }
        else
        {
            targetPosition = targetRb.transform.position;

            if (targetRb.velocity != Vector3.zero)
                targetRotation = Quaternion.LookRotation(targetRb.velocity);
            else
                targetRotation = Quaternion.LookRotation(targetRb.transform.forward);
        }

        //Apply position
        transform.position = Vector3.Lerp(transform.position, targetPosition - targetRotation *  m_baseTranslation, lerpPosition) ;
        transform.position += new Vector3(0, m_yOffset, 0);


        //Apply rotation with two lerp values for Y and XZ rotations
        Quaternion newRot = (targetRotation * m_baseRotationSelf);
        transform.rotation = m_prevRot;

        Vector3 eulerY = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        Vector3 newEulerY = new Vector3(0, newRot.eulerAngles.y, 0);

        Vector3 eulerXZ = new Vector3(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
        Vector3 newEulerXZ = new Vector3(newRot.eulerAngles.x, 0, newRot.eulerAngles.z);

        Quaternion rotY = Quaternion.Lerp(Quaternion.Euler(eulerY), Quaternion.Euler(newEulerY), lerpRotationY);
        Quaternion rotXZ = Quaternion.Lerp(Quaternion.Euler(eulerXZ), Quaternion.Euler(newEulerXZ), lerpRotationXZ);

        transform.rotation = rotY * rotXZ;
        m_prevRot = transform.rotation;

        //character below frame y correction
        if(m_YCorrection)
        {
            float angle = Vector3.Angle(transform.forward, targetPosition - transform.position);
            float correctionDelta = angle / m_lowerAngleCorrection / 100;

            if (angle > 90)
                m_yOffset = 0;
            if (angle > m_lowerAngleCorrection)
                m_yOffset -= correctionDelta;
            else if (angle < m_lowerAngleCorrection - 5)
                m_yOffset = 0f;
            else
                m_yOffset += correctionDelta;
        }


        //Head rotation 
        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();
        if (pose.IsValid)
        {
            m_prevPoseX = Mathf.LerpAngle(m_prevPoseX, pose.Rotation.eulerAngles.x, headRotLerpX);
            transform.RotateAround(transform.position, transform.right, headRotMultiX * m_prevPoseX);

            m_prevPoseY = Mathf.LerpAngle(m_prevPoseY, pose.Rotation.eulerAngles.y, headRotLerpY);
            transform.RotateAround(transform.position, transform.up, headRotMultiY * m_prevPoseY);

            m_prevPoseZ = Mathf.LerpAngle(m_prevPoseZ, pose.Rotation.eulerAngles.z, headRotLerpZ);
            transform.RotateAround(transform.position, transform.forward, headRotMultiZ * m_prevPoseZ);
            
            transform.rotation = transform.rotation;
        }
    }
}
