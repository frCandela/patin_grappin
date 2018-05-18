using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;


[RequireComponent( typeof(  Camera ))]
public class CameraController : MonoBehaviour
{
    [Header("References:")]
    private Rigidbody targetRb = null;
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

    [Header("Camera switch:")]
    [SerializeField] private float lerpPositionSwitch = 0.05f;
    [SerializeField] private float lerpRotationSwitch = 0.05f;
    [SerializeField] private float switchDuration = 1f;
    public float m_switchState = 0f; //Between 0 and 1

    //Initial camera position and rotation parameters
    Vector3 m_baseTranslation;
    Quaternion m_baseRotationSelf;

    //Backup data for head rotation lerp
    private float m_prevPoseX = 0;
    private float m_prevPoseY = 0;
    private float m_prevPoseZ = 0;
    private Quaternion m_prevRot = Quaternion.identity;

    //other
   

    // Use this for initialization
    void Awake ()
    {
        m_track = FindObjectOfType<Track>();

        targetRb = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();

        Util.EditorAssert(m_track != null, "HeadCameraController.Awake(): no track in the level");
        Util.EditorAssert(targetRb != null, "HeadCameraController.Awake(): no playerRb set");
    }

    private void Start()
    {
        InitCamera();
        UpdateCameraTransform();

        AkSoundEngine.PostEvent("Play_Music_Placeholder", gameObject);
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        UpdateCameraTransform();
    }

    private void OnEnable()
    {
        StartCoroutine(SwitchCamera());
    }

    private void InitCamera()
    {
        m_baseTranslation = targetRb.transform.localPosition - transform.localPosition;

        Vector3 euler = transform.rotation.eulerAngles;
        m_baseRotationSelf = Quaternion.Euler(euler.x,0, euler.z);
    }

    public void UpdateCameraTransform()
    {
        //Calculate effective lerp values
        float effectiveLerpPosition = Mathf.Lerp(lerpPosition, lerpPositionSwitch, m_switchState);
        float effectiveLerpRotationY = Mathf.Lerp(lerpRotationY, lerpRotationSwitch, m_switchState);
        float effectiveLerpRotationXZ = Mathf.Lerp(lerpRotationXZ, lerpRotationSwitch, m_switchState);

        //Calculate effective target rotations and positions



        //Apply position
        transform.position = Vector3.Lerp(transform.position, targetRb.transform.position - targetRb.transform.rotation *  m_baseTranslation, effectiveLerpPosition) ;

        //Apply rotation with two lerp values for Y and XZ rotations
        Quaternion newRot = (targetRb.transform.rotation * m_baseRotationSelf);
        transform.rotation = m_prevRot;

        Vector3 eulerY = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        Vector3 newEulerY = new Vector3(0, newRot.eulerAngles.y, 0);

        Vector3 eulerXZ = new Vector3(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
        Vector3 newEulerXZ = new Vector3(newRot.eulerAngles.x, 0, newRot.eulerAngles.z);


        Quaternion rotY = Quaternion.Lerp(Quaternion.Euler(eulerY), Quaternion.Euler(newEulerY), effectiveLerpRotationY);
        Quaternion rotXZ = Quaternion.Lerp(Quaternion.Euler(eulerXZ), Quaternion.Euler(newEulerXZ), effectiveLerpRotationXZ);

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

    IEnumerator SwitchCamera()
    {
        m_switchState = 1;
        float delta = switchDuration / 20f;
        while (m_switchState > 0f)
        {
            m_switchState -= 0.05f;
            yield return new WaitForSeconds(delta);
        }
        m_switchState = 0f;
    }
}
