using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadPlayerController : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private HeadCameraController cameraController = null;

    [SerializeField] private float velocity = 2f;
    [SerializeField] private float boostForce = 15f;
    [SerializeField] private float turningSpeed = 30f;

    //Components references
    private Rigidbody m_rb;

    private float m_boostMultiplier = 1f;


    // Use this for initialization
    void Awake ()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        object[] objArray = GameObject.FindObjectsOfType(typeof(GOChangeColor));
        foreach (object obj in objArray)
        {
            GOChangeColor boost = (GOChangeColor)obj;
            boost.onColorChanged.AddListener(StartBoost);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void StartBoost()
    {
        m_rb.AddForce(boostForce * m_rb.velocity.normalized, ForceMode.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        Tobii.Gaming.HeadPose pose = TobiiAPI.GetHeadPose();
        if (pose.IsValid)
        {
            m_rb.velocity = new Vector3(turningSpeed * (pose.Rotation.y), m_rb.velocity.y, m_rb.velocity.z);
        }

        print(m_boostMultiplier);
        m_rb.AddForce(m_boostMultiplier * velocity * Vector3.forward, ForceMode.Acceleration);

        //Sets rotation to match the curve of the terrain
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, Vector3.down, out raycastHit, 100f, LayerMask.GetMask("Terrain")))
        {
            Vector3 direction = Vector3.Cross(cameraController.transform.right, raycastHit.normal);
            Quaternion rotation = Quaternion.LookRotation(direction, raycastHit.normal);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1f);
        }

        //rotates the model depending on his speed
        transform.rotation = Quaternion.LookRotation(m_rb.velocity);
    }
}
