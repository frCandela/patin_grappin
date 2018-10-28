using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignVeocity : MonoBehaviour
{
    Rigidbody m_rb;

    public float offset;

	// Use this for initialization
	void Awake ()
    {
        m_rb = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(transform.position - m_rb.velocity);
	}
}
