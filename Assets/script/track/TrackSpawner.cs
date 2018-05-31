using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{

    private Track track;
    private Rigidbody m_targetRb = null;


    private GameObject rescueTrack;

    // Use this for initialization
    void Awake ()
    {
        m_targetRb = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();
        track = FindObjectOfType<Track>();
    }

    private void Start()
    {
        rescueTrack = Instantiate(track.gameObject);
        rescueTrack.transform.position = rescueTrack.transform.position - 100 * Vector3.up;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
