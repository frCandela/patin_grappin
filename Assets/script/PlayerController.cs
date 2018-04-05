using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent( typeof( Rigidbody))]

public class PlayerController : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private PlayerCameraController cameraController = null;

    [Header("Movement:")]
    [SerializeField] private float forwardAcceleration = 10f;
    [SerializeField] private float lateralAcceleration = 10f;
    [SerializeField] private float maximumVelocityNormal = 20f;
    [SerializeField] private float maximumVelocityBoosting = 30f;
    [SerializeField] private float boostSpeedFactor = 4;

    [Header("Special:")]
    [SerializeField] private float boostDuration = 2f;

    //private movements
    private float m_speedFactor = 1f;
    private bool m_boosting = false;
    private float m_maximumVelocity;

    //Components references
    private Rigidbody m_rb;

	void Awake ()
    {
        //Get references
        m_rb = GetComponent<Rigidbody>();

        Util.EditorAssert(m_rb != null, "PlayerController.Awake(): No rigidbody set");
        Util.EditorAssert(cameraController != null, "PlayerController.Awake(): No cameraController set");

        Physics.gravity = new Vector3( 0, -20f, 0);
    }

    private void Start()
    {
        object[] objArray = GameObject.FindObjectsOfType(typeof(GOChangeColor));
        foreach(object obj in objArray)
        {
            GOChangeColor boost = (GOChangeColor)obj;
            boost.onColorChanged.AddListener(StartBoost);
        }
    }

    private void Update()
    {
        if( Input.GetKeyDown( KeyCode.R) )
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void FixedUpdate()
    {
        if ( Input.GetButton("BoostSpeed") || m_boosting)
        {
            m_speedFactor = boostSpeedFactor;
            m_maximumVelocity = maximumVelocityBoosting;
        }
        else
        {
            m_speedFactor = 1f;
            m_maximumVelocity = maximumVelocityNormal;
        }
            

        //Sets maximum velocity
        float velocity = m_rb.velocity.magnitude;
        if (velocity > m_maximumVelocity)
            m_rb.velocity = m_maximumVelocity * m_rb.velocity.normalized;

        //Sets rotation to match the curve of the terrain
        RaycastHit raycastHit;
        if(  Physics.Raycast(transform.position, Vector3.down, out raycastHit, 100f, LayerMask.GetMask("Terrain")) )
        {
            float lerpSpeed;
            if (raycastHit.distance > 5f)
                lerpSpeed = 0.02f;
            else
                lerpSpeed = 0.1f;

            Vector3 direction = Vector3.Cross(cameraController.transform.right, raycastHit.normal);
            Quaternion rotation = Quaternion.LookRotation(direction, raycastHit.normal);
            transform.rotation = Quaternion.Lerp(transform.rotation,  rotation, lerpSpeed);
        }
        else
        {
            Quaternion rotation = Quaternion.LookRotation(cameraController.transform.forward, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.02f);
        }

        //Moves the player in the requested direction
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");



        //Calculates directions
        Vector3 forwardDir = cameraController.transform.forward;
        forwardDir.y = 0;
        forwardDir.Normalize();
        
        //Moving force
        Vector3 force = m_speedFactor * (forwardAcceleration * vertical * transform.forward + lateralAcceleration * horizontal * transform.right);
        m_rb.AddForce(force, ForceMode.Acceleration);
    }

    private void StartBoost()
    {
        StopAllCoroutines();
        StartCoroutine(Boost());
    }

    IEnumerator Boost()
    {
        m_boosting = true;
        yield return new WaitForSeconds(boostDuration);
        m_boosting = false;
    }


    private void OnGUI()
    {
        //Style
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;

        //Draw velocity
        GUI.Label(new Rect(0, 0, 100, 10), "Player speed: " + m_rb.velocity.magnitude.ToString(), style);
    }
}