using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandAim : MonoBehaviour {

    [SerializeField] private GameObject aimPrefab = null;
    private GameObject m_aim = null;

    // Use this for initialization
    void Awake ()
    {


        Util.EditorAssert(aimPrefab != null, "Grapple.Awake: ropePrefab not set");
        m_aim = Instantiate(aimPrefab);
        m_aim.GetComponent<MeshRenderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Quaternion rHandRot = InputTracking.GetLocalRotation(XRNode.RightHand);
        Vector3 rHandPos = transform.parent.position + InputTracking.GetLocalPosition(XRNode.RightHand);
        Vector3 rHandRotForward = rHandRot * Vector3.forward;

        float length = 0.1f;

        Ray ray = new Ray(rHandPos, rHandRotForward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            length = hit.distance;
        }   

        m_aim.transform.rotation = rHandRot;
        m_aim.transform.localScale = new Vector3(0.01f, 0.01f, length);
        m_aim.transform.position = rHandPos + 0.5f * length * rHandRotForward;

        if (Input.GetButtonDown("GrappleAim"))
            m_aim.GetComponent<MeshRenderer>().enabled = true;
        if (Input.GetButtonUp("GrappleAim"))
            m_aim.GetComponent<MeshRenderer>().enabled = false;
    }
}
