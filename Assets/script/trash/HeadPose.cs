using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class HeadPose : MonoBehaviour
{

    [Header("RealSpeedPeerFrame = speedRatio * distance(eyePoint, cube)")]
    public float speedRatio = 0.1f;

    [Header("References")]
    public Material moving;
    public Material notMoving;
    public GameObject targetObject = null;

    Camera m_camera;

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
        TobiiAPI.SubscribeGazePointData();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        SetObjectMaterial(targetObject, moving);

        Tobii.Gaming.HeadPose pose =  TobiiAPI.GetHeadPose();
        targetObject.transform.rotation = pose.Rotation;
    }

    private void SetObjectMaterial(GameObject obj, Material material)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer)
            renderer.material = material;
    }
}
