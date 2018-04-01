using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Rigidbody))]

public class PlayerController : MonoBehaviour
{

    public float forwardSpeed = 10f;


    //Components references
    private Rigidbody m_rb;


	void Awake ()
    {
        m_rb = GetComponent<Rigidbody>();
        Util.EditorAssert(m_rb != null, "PlayerController.Awake(): No rigidbody set");
    }
	

	void Update ()
    {
		
	}

    private void FixedUpdate()
    {
        
        float vertical = Input.GetAxisRaw("Vertical");
        
        if (vertical != 0f)
        {
            m_rb.AddForce(forwardSpeed * vertical * transform.forward, ForceMode.Acceleration);
        }
    }
}