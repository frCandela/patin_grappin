using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class MoveBlockFixedSpeed : MonoBehaviour
{
    [Header("Speed in blocks peer frame")]
    public float speed = 0.3f;

    [Header("References")]
    public Material moving;
    public Material notMoving;
    public GameObject targetObject = null;
    public GameObject test = null;

    Camera m_camera;

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
        TobiiAPI.SubscribeGazePointData();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        GameObject focusedObject = TobiiAPI.GetFocusedObject();
        if (focusedObject == targetObject)
            SetObjectMaterial(targetObject, notMoving);
        else
        {
            Vector2 lookAtScreen = TobiiAPI.GetGazePoint().Screen;
            if (lookAtScreen.x >= 0 && lookAtScreen.x <= Screen.width && lookAtScreen.y >= 0 && lookAtScreen.y <= Screen.height)
            {
                SetObjectMaterial(targetObject, moving);
                Ray ray = m_camera.ScreenPointToRay(lookAtScreen);
                Plane plane = new Plane(new Vector3(0, 0, -1), targetObject.transform.position);

                float enter = 0.0f;

                if (plane.Raycast(ray, out enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    test.transform.position = hitPoint;

                    Rigidbody rb = targetObject.GetComponent<Rigidbody>();

                    Vector3 dir = hitPoint - rb.transform.position;
                    dir.Normalize();
                    dir *= speed;

                    rb.transform.position = rb.transform.position + dir;
                }
            }
        }
    }

    private void SetObjectMaterial(GameObject obj, Material material)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer)
            renderer.material = material;
    }
}
