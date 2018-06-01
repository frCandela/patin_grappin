using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    [SerializeField, Range(0,1000f)] private float respawnTrackDistance = 300;
    [SerializeField, Range(0, 10000)] private float respawnSpeedDelta = 30;
    [SerializeField] private float downOffset = 10000;
    [SerializeField] private float lerpInValue = 0.1f;
    [SerializeField] private float lerpOutValue = 0.1f;

    private Rigidbody m_targetRb = null;

    private Track m_mainTrack;
    private GameObject m_rescueTrack;

    private float respawnTime = 0f;

    public State currentState = State.Following;

    public enum State {Following, LerpingOut, LerpingIn};

    // Use this for initialization
    void Awake ()
    {
        m_targetRb = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();
        m_mainTrack = FindObjectOfType<Track>();
        m_mainTrack.onRespawn.AddListener(Respawn);
    }

    private void Start()
    {
        m_rescueTrack = Instantiate(m_mainTrack.gameObject);
        Destroy(m_rescueTrack.GetComponent<Track>());

        TagHierarchy(m_rescueTrack.transform);
    }

    void TagHierarchy(Transform transform)
    {
        foreach (Transform trans in transform)
        {
            trans.gameObject.tag = "noGrab";
            TagHierarchy(trans);
        }
            
    }

    void Respawn()
    {
        if (currentState == State.Following)
        {
            currentState = State.LerpingOut;

            Vector3 diff = m_mainTrack.transform.position - m_targetRb.transform.parent.position;
            m_targetRb.transform.parent.position = m_targetRb.transform.parent.position - (m_rescueTrack.transform.position - m_mainTrack.transform.position);

            m_rescueTrack.transform.position = m_targetRb.transform.parent.position + diff;

            respawnTime = Time.time;
        }
            

       
    }

    private Vector3 GetRespawnTrackPosition()
    {
        Vector3 rescueTrackPos = m_targetRb.transform.position - m_mainTrack.trackSection.respawnTrackSection.respawnTransform.position;
        rescueTrackPos.y = m_targetRb.transform.position.y - respawnTrackDistance - m_mainTrack.trackSection.respawnTrackSection.respawnTransform.position.y;
        return rescueTrackPos;
    }


    private void FixedUpdate()   
    {
        switch (currentState)
        {
            case State.Following:
                m_rescueTrack.transform.position = GetRespawnTrackPosition();
                break;
            case State.LerpingOut:
                if (Time.time - respawnTime < 2)
                    m_rescueTrack.transform.position = m_rescueTrack.transform.position + respawnSpeedDelta * Vector3.up;
                else
                {

                    m_rescueTrack.transform.position = GetRespawnTrackPosition() - 10000 * Vector3.up;
                    currentState = State.LerpingIn;
                }
                break;

            case State.LerpingIn:
                if (Time.time - respawnTime < 4)
                    m_rescueTrack.transform.position = Vector3.Lerp(m_rescueTrack.transform.position, GetRespawnTrackPosition(), lerpInValue);                
                else
                {
                    currentState = State.Following;
                }
                break;


        }
    }
}
