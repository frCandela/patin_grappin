using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;


[RequireComponent( typeof(  Camera ))]
public class HeadCameraController1 : MonoBehaviour
{
    [Header("Linked Instances:")]
    [SerializeField] private Rigidbody targetRb = null;
    [SerializeField] private Track track = null;

    [Header("Parameters:")]
    [SerializeField, Range(0f, 1f)] private float headRotLerpX = 0.1f;
    [SerializeField, Range(0f, 1f)] private float headRotLerpY = 0.1f;
    [SerializeField, Range(0f, 1f)] private float headRotLerpZ = 0.1f;

    [SerializeField, Range(0f, 2f)] private float headRotMultiX = 1f;
    [SerializeField, Range(0f, 2f)] private float headRotMultiY = 1f;
    [SerializeField, Range(0f, 2f)] private float headRotMultiZ = 1f;

    private Vector3 m_initialTranslation;
    private float m_initialDistance;

    private float m_prevPoseX = 0;
    private float m_prevPoseY = 0;
    private float m_prevPoseZ = 0;

    // Use this for initialization
    void Awake ()
    {
        Util.EditorAssert(track != null, "HeadCameraController.Awake(): no track set");
        Util.EditorAssert(targetRb != null, "HeadCameraController.Awake(): no playerRb set");

        //Backup camera position relative to the player
        m_initialTranslation = transform.position - targetRb.transform.position;
        m_initialDistance = m_initialTranslation.magnitude;
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        //Backup
        Quaternion prevRot = transform.rotation;
        Vector3 prevPos = transform.position;

        //Rotation
        Vector3 direction = new Vector3(targetRb.velocity.x, 0, targetRb.velocity.z).normalized;
        transform.position = targetRb.transform.position - m_initialDistance * direction + 0 * Vector3.up;
        transform.LookAt(targetRb.transform.position);

        transform.RotateAround(targetRb.transform.position, transform.right, 30);
        transform.RotateAround(transform.position, transform.right, -20);

        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();
        //Tobii  control
        if (pose.IsValid)
        {
            m_prevPoseX = Mathf.LerpAngle(m_prevPoseX, pose.Rotation.eulerAngles.x, headRotLerpX);
            transform.RotateAround(transform.position, transform.right, headRotMultiX * m_prevPoseX);

            m_prevPoseY = Mathf.LerpAngle(m_prevPoseY, pose.Rotation.eulerAngles.y, headRotLerpY);
            transform.RotateAround(transform.position, transform.up, headRotMultiY * m_prevPoseY);

            m_prevPoseZ = Mathf.LerpAngle(m_prevPoseZ, pose.Rotation.eulerAngles.z, headRotLerpZ);
            transform.RotateAround(transform.position, transform.forward, headRotMultiZ * m_prevPoseZ);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(track.trackSection.trackPosition, 1);
        Gizmos.DrawLine(track.trackSection.trackPosition, track.trackSection.trackPosition + track.trackSection.trackDirection);
    }
}
