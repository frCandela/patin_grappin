using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public GameObject follow;
    public float offset = 1;

    // Update is called once per frame
    void FixedUpdate ()
    {
        transform.position = follow.transform.position + offset * follow.transform.forward;

    }
}
