using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FollowXRNode : MonoBehaviour
{
    public XRNode node = XRNode.LeftHand;
    public Vector3 offsetRot;

    // Update is called once per frame
    void Update ()
    {
        transform.position = transform.parent.position + InputTracking.GetLocalPosition(node);
        transform.rotation = InputTracking.GetLocalRotation(node)*Quaternion.Euler(offsetRot);
    }
}
