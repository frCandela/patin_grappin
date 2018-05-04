using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;


[RequireComponent( typeof(  Camera ))]
public class CameraController : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private Rigidbody targetRb = null;
    private Track m_track = null;

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

    //Initial camera position and rotation parameters
    Vector3 m_baseTranslation;
    Quaternion m_baseRotationSelf;

    //Backup data for head rotation lerp
    private float m_prevPoseX = 0;
    private float m_prevPoseY = 0;
    private float m_prevPoseZ = 0;
    private Quaternion m_prevRot = Quaternion.identity;

    // Use this for initialization
    void Awake ()
    {
        m_track = FindObjectOfType<Track>();

        Util.EditorAssert(m_track != null, "HeadCameraController.Awake(): no track in the level");
        Util.EditorAssert(targetRb != null, "HeadCameraController.Awake(): no playerRb set");
    }

    private void Start()
    {
        InitCamera();
        UpdateCameraTransform();
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        UpdateCameraTransform();
    }

    private void InitCamera()
    {
        m_baseTranslation = targetRb.transform.position - transform.position;
        m_baseRotationSelf = transform.rotation;
    }

    public void UpdateCameraTransform()
    {
        //movement direction
        Vector3 direction = new Vector3(targetRb.velocity.x, 0, targetRb.velocity.z).normalized;
        if (direction.magnitude < 0.1f)
            direction = targetRb.transform.forward;

        //Apply position
        transform.position = Vector3.Lerp(transform.position, targetRb.transform.position - targetRb.transform.rotation *  m_baseTranslation, lerpPosition) ;

        //Apply rotation with two lerp values for Y and XZ rotations
        Quaternion newRot = (targetRb.transform.rotation * m_baseRotationSelf);
        transform.rotation = m_prevRot;

        Vector3 eulerY = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        Vector3 newEulerY = new Vector3(0, newRot.eulerAngles.y, 0);

        Vector3 eulerXZ = new Vector3(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
        Vector3 newEulerXZ = new Vector3(newRot.eulerAngles.x, 0, newRot.eulerAngles.z);

        Quaternion rotY = Quaternion.Lerp(Quaternion.Euler(eulerY), Quaternion.Euler(newEulerY), lerpRotationY);
        Quaternion rotXZ = Quaternion.Lerp(Quaternion.Euler(eulerXZ), Quaternion.Euler(newEulerXZ), lerpRotationXZ);

        transform.rotation = rotY * rotXZ;
        m_prevRot = transform.rotation;

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
