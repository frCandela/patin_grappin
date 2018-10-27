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

    [Header("Corrections:")]
    [SerializeField, Range(-30f,30f)] private float m_xAngleHeadCorrection = 0;
    [SerializeField] private bool m_DownYCorrection = true;
    [SerializeField] private bool m_UpYCorrection = true;
    [SerializeField] private bool m_lerpLagCorrection = true;
    [SerializeField, Range(0,10f)] private float m_downOffset = 2f;
    [SerializeField, Range(0, 10f)] private float m_upOffset = 5f;
    private float m_baseDistance = 0f;

    //References
    private Camera m_camera = null;
    private Rigidbody targetRb = null;
    private Transform targetRagdoll = null;
    private Track m_track = null;

    //Initial camera position and rotation parameters
    Vector3 m_baseTranslation;
    Quaternion m_baseRotationSelf;

    //Backup data for head rotation lerp
    private float m_prevPoseX = 0;
    private float m_prevPoseY = 0;
    private float m_prevPoseZ = 0;
    private Quaternion m_prevRot = Quaternion.identity;
    private float m_yOffset = 0f;

    // Use this for initialization
    void Awake ()
    {
        //Get references
        m_track = FindObjectOfType<Track>();
        targetRb = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();
        targetRagdoll = FindObjectOfType<PlayerController>().GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Spine);
        m_camera = GetComponent<Camera>();

        Util.EditorAssert(m_track != null, "HeadCameraController.Awake(): no track in the level");
        Util.EditorAssert(targetRb != null, "HeadCameraController.Awake(): no playerRb set");

        m_baseDistance = Vector3.Distance(targetRb.transform.position, transform.position);
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
        m_baseTranslation = targetRb.transform.localPosition - transform.localPosition;

        Vector3 euler = transform.rotation.eulerAngles;
        m_baseRotationSelf = Quaternion.Euler(euler.x,0, euler.z);
    }

    public void UpdateCameraTransform()
    {
        //Get target position and rotation
        Vector3 targetPosition;
        Quaternion targetRotation;

        /*if (m_ragdollController.ragdollActivated)
        {
            targetPosition = targetRagdoll.transform.position;
            targetRotation = Quaternion.LookRotation(m_ragdollController.averageVelocity);
        }
        else*/
        {
            targetPosition = targetRb.transform.position;

            if (targetRb.velocity != Vector3.zero)
                targetRotation = Quaternion.LookRotation(targetRb.velocity);
            else
                targetRotation = Quaternion.LookRotation(targetRb.transform.forward);
        }

        //Caalculates the ratio between the target distance (base distance) and the current one
        float distanceRatio = Vector3.Distance(targetRb.transform.position, transform.position) / m_baseDistance;
        float lerpcorrection = 1f;
        if (m_lerpLagCorrection)
            lerpcorrection = distanceRatio;

        //Apply position
        transform.position = Vector3.Lerp(transform.position, targetPosition - targetRotation *  m_baseTranslation, lerpcorrection * lerpPosition) ;
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


        float scaledDistanceRatio = Mathf.Clamp(distanceRatio, 0f, 1f);
        m_yOffset = 0;
        //down Y correction
        if (m_DownYCorrection)
        {
            Vector3 newTarget = new Vector3(targetPosition.x, targetPosition.y - scaledDistanceRatio * m_downOffset, targetPosition.z);
            if (newTarget.y < transform.position.y)
            {
                Vector3 lowForward = Quaternion.AngleAxis(m_camera.fieldOfView / 2, transform.right) * transform.forward;
                Vector3 planeUp = Vector3.Cross(lowForward, transform.right);
                Plane lowFtrustrumPlane = new Plane(planeUp, transform.position);
                float enter;
                Ray ray = new Ray(newTarget, Vector3.up);
                if (lowFtrustrumPlane.Raycast(ray, out enter))
                    m_yOffset -= enter;
            }
        }
        if( m_UpYCorrection )
        {
            Vector3 newTarget = new Vector3(targetPosition.x, targetPosition.y + scaledDistanceRatio * m_upOffset, targetPosition.z);
            if (newTarget.y > transform.position.y)
            {
                Vector3 upForward = Quaternion.AngleAxis( - m_camera.fieldOfView / 2, transform.right) * transform.forward;
                Vector3 planeUp = Vector3.Cross(upForward, transform.right); 
                
                 Plane lowFtrustrumPlane = new Plane(planeUp, transform.position);
                float enter;
                Ray ray = new Ray(newTarget, - Vector3.up);
                if (lowFtrustrumPlane.Raycast(ray, out enter))
                    m_yOffset += enter;
            }
        }

        //Head rotation 
        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();
        if (pose.IsValid)
        {
            m_prevPoseX = Mathf.LerpAngle(m_prevPoseX, pose.Rotation.eulerAngles.x + m_xAngleHeadCorrection , headRotLerpX);
            transform.RotateAround(transform.position, transform.right, headRotMultiX * m_prevPoseX);

            m_prevPoseY = Mathf.LerpAngle(m_prevPoseY, pose.Rotation.eulerAngles.y, headRotLerpY);
            transform.RotateAround(transform.position, transform.up, headRotMultiY * m_prevPoseY);

            m_prevPoseZ = Mathf.LerpAngle(m_prevPoseZ, pose.Rotation.eulerAngles.z, headRotLerpZ);
            transform.RotateAround(transform.position, transform.forward, headRotMultiZ * m_prevPoseZ);
            
            transform.rotation = transform.rotation;
        }
    }
}
