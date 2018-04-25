using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;


[RequireComponent( typeof(  Camera ))]
public class HeadCameraController : MonoBehaviour
{
    [Header("Linked Instances:")]
    [SerializeField] private GameObject playerGameObject = null;

    [Header("Parameters:")]
    [SerializeField] private float lerpSpeedRotation = 0.2f;
    [SerializeField] private float lerpSpeedPosition = 0.2f;
    [SerializeField] private bool headRotation = true;
    [SerializeField] private float headRotationMultiplier = 2f;

    private Vector3 previousTranslation;
    private Quaternion previousRotation;

    // Use this for initialization
    void Awake ()
    {
        previousTranslation = transform.position - playerGameObject.transform.position;
        previousRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();
        if (pose.IsValid)
        {
            //Saves current position and rotation
            Quaternion rotation = transform.rotation;
            Vector3 position = transform.position;

            //Reset position and rotation
            transform.position = playerGameObject.transform.position + previousTranslation;
            transform.rotation = previousRotation;

            Quaternion trackRot = Quaternion.LookRotation(TrackSection.trackDir);
            //Apply wanted transformation
            /* if (headRotation)
                 transform.RotateAround(playerGameObject.transform.position, Vector3.up, pose.Rotation.eulerAngles.y);*/
            if (headRotation)
                transform.RotateAround(playerGameObject.transform.position, Vector3.up, trackRot.eulerAngles.y + headRotationMultiplier * pose.Rotation.eulerAngles.y);


            //Lerp between current position/rotation and the wanted position/rotation
            transform.position = Vector3.Lerp(position, transform.position, lerpSpeedPosition);
            transform.rotation = Quaternion.Lerp(rotation, transform.rotation, lerpSpeedRotation);
        }
    }
}
