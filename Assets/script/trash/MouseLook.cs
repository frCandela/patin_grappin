using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;



public class MouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    float rotationY = 0F;

    public float speed = 0.1f;
    public float dashForce = 1f;


    private Camera m_camera;
    private Rigidbody m_rigidbody;

    void Update()
    {
        if (Input.GetKey( KeyCode.Z))
            transform.position = transform.position + speed * transform.forward;
        if (Input.GetKey(KeyCode.S))
            transform.position = transform.position - speed * transform.forward;
        if (Input.GetKey(KeyCode.D))
            transform.position = transform.position + speed * transform.right;
        if (Input.GetKey(KeyCode.Q))
            transform.position = transform.position - speed * transform.right;

        


        if ( Input.GetKey(KeyCode.Space) )
        {
            Vector2 lookAtScreen = TobiiAPI.GetGazePoint().Screen;
            if (lookAtScreen.x >= 0 && lookAtScreen.x <= Screen.width && lookAtScreen.y >= 0 && lookAtScreen.y <= Screen.height)
            {
                Ray ray = m_camera.ScreenPointToRay(lookAtScreen);
                Vector3 hitPoint = ray.GetPoint(1f) - ray.origin;
                m_rigidbody.AddForce(dashForce * hitPoint, ForceMode.VelocityChange);

            }
        }


        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }

    void Awake()
    {
        // Make the rigid body not change rotation
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
            rb.freezeRotation = true;

        m_camera = GetComponent<Camera>();
        m_rigidbody = GetComponent<Rigidbody>();
    }
}
