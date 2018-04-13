using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BestGrapple : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Range(0f, float.MaxValue)] private float attractionForce;

    private Rigidbody m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start ()
    {
        Util.EditorAssert(target != null, "BestGrapple.Start: target not set");
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Vector3 direction = target.position - transform.position;
		//m_rigidbody.AddForce( )
	}
}
