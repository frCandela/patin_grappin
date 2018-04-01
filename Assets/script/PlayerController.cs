using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Rigidbody))]

public class PlayerController : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private PlayerCameraController cameraController = null;

    [Header("Movement:")]
    [SerializeField] private float forwardAcceleration = 10f;
    [SerializeField] private float lateralAcceleration = 10f;
    [SerializeField] private float maximumVelocity = 30f;
    [SerializeField] private float boostSpeedFactor = 4;

    //private movements
    float boostSpeed = 1f;

    //Components references
    private Rigidbody m_rb;

	void Awake ()
    {
        //Get references
        m_rb = GetComponent<Rigidbody>();

        Util.EditorAssert(m_rb != null, "PlayerController.Awake(): No rigidbody set");
        Util.EditorAssert(cameraController != null, "PlayerController.Awake(): No cameraController set");
    }

    private void FixedUpdate()
    {
        if ( Input.GetButton("BoostSpeed"))
            boostSpeed = boostSpeedFactor;
        else
            boostSpeed= 1f;

            //Sets maximum velocity
            float velocity = m_rb.velocity.magnitude;
        if (velocity > maximumVelocity)
            m_rb.velocity = maximumVelocity * m_rb.velocity.normalized;

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
        Vector3 force = boostSpeed * (forwardAcceleration * vertical * transform.forward + lateralAcceleration * horizontal * transform.right);
        m_rb.AddForce(force, ForceMode.Acceleration);
        
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