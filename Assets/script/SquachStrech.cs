using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class SquachStrech : MonoBehaviour
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

        Vector2 lookAtScreen = TobiiAPI.GetGazePoint().Screen;
        if (lookAtScreen.x >= 0 && lookAtScreen.x <= Screen.width && lookAtScreen.y >= 0 && lookAtScreen.y <= Screen.height)
        {
            Vector3 scale = targetObject.transform.localScale;
            float ratioX = lookAtScreen.x / Screen.width;
            float ratioY = lookAtScreen.y / Screen.height;
            targetObject.transform.localScale = new Vector3(ratioX, ratioY, scale.z);
        }

    }

    private void SetObjectMaterial(GameObject obj, Material material)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer)
            renderer.material = material;
    }
}
