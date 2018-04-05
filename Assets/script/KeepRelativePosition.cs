using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Keep an object at the same position relative to it's parent
/// </summary>
public class KeepRelativePosition : MonoBehaviour
{
    private Vector3 translation;
    public bool showInEditor = true;

    private void OnDrawGizmos()
    {
        if(showInEditor)
        {
            Gizmos.DrawSphere(transform.position, 0.05f);
        }
    }

    // Use this for initialization
    void Start ()
    {
        translation = transform.position - transform.parent.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = transform.parent.position + translation;
    }
}
