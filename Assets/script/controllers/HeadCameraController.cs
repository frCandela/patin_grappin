﻿using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;


[RequireComponent( typeof(  Camera ))]
public class HeadCameraController : MonoBehaviour
{
    [Header("Linked Instances:")]
    [SerializeField] private Rigidbody playerRb = null;
    [SerializeField] private Track track = null;


    [Header("Parameters:")]
    [SerializeField] private float lerpSpeedRotation = 0.2f;
    [SerializeField] private float lerpSpeedPosition = 0.2f;
    [SerializeField] private bool headRotation = true;
    [SerializeField] private float headRotationMultiplier = 2f;

    private Vector3 initialTranslation;
    private Quaternion initialRotation;

    // Use this for initialization
    void Awake ()
    {
        //Backup camera position relative to the player
        initialTranslation = transform.position - playerRb.transform.position;
        initialRotation = transform.rotation;

        Util.EditorAssert(track != null, "HeadCameraController.Awake(): no track set");
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();
        //Tobii camera control
        if (pose.IsValid)
        {
            //Saves current position and rotation
            Quaternion rotation = transform.rotation;
            Vector3 position = transform.position;

            //Reset position and rotation
            transform.position = playerRb.transform.position + initialTranslation;
            transform.rotation = initialRotation;

            //Apply transformation
            Quaternion trackRot = Quaternion.LookRotation( track.trackDirection );
            if (headRotation)
                transform.RotateAround(playerRb.transform.position, Vector3.up, trackRot.eulerAngles.y + headRotationMultiplier * pose.Rotation.eulerAngles.y);
            else
                transform.RotateAround(playerRb.transform.position, Vector3.up, trackRot.eulerAngles.y);


            //Lerp between current position/rotation and the wanted position/rotation
            transform.position = Vector3.Lerp(position, transform.position, lerpSpeedPosition);
            transform.rotation = Quaternion.Lerp(rotation, transform.rotation, lerpSpeedRotation);
        }
        //Auto control
        else
        {
            //Saves current position and rotation
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            //Reset position and rotation
            transform.position = playerRb.transform.position + initialTranslation;
            transform.rotation = initialRotation;

            //Apply transformation
            //Quaternion trackRot = Quaternion.LookRotation(track.trackDirection);
            Quaternion trackRot = Quaternion.LookRotation(playerRb.velocity);
            transform.RotateAround(playerRb.transform.position, Vector3.up, trackRot.eulerAngles.y);

            //Lerp between current position/rotation and the wanted position/rotation
            transform.position = Vector3.Lerp(position, transform.position, lerpSpeedPosition);
            transform.rotation = Quaternion.Lerp(rotation, transform.rotation, lerpSpeedRotation);
        }
    }


}
