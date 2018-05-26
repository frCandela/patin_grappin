using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[ExecuteInEditMode]
public class Track : MonoBehaviour
{
    //Editor parameters

    [Header("Track parameters: ")]
    [SerializeField] private float fallFromTrackHeight = 50;
    [SerializeField] private float respawnHeight = 100f;
    [SerializeField] private float updateDelta = 0.2f;
    [SerializeField] private bool alignVelocityOnRespawn = false;


    [Header("Track Sections Tree: ")]
    [SerializeField] private TrackSection currentSection = null;
    [SerializeField] private bool checkPreviousSections = true;
    [SerializeField] private bool checkParallelSections = true;

    //Private members
    private Rigidbody m_targetRb = null;

    //Debug
    float splineUpdateMs = 0f;

    public TrackSection trackSection
    {
        get { return currentSection; }
        private set { }
    }

    private void Start()
    {
        if (!currentSection)
        {
            currentSection = GetComponent<TrackSection>();
        }
        SetupTarget();

        if (currentSection)
            currentSection.UpdateTrack(m_targetRb.position);

        StartCoroutine(UpdateTrack());
    }

    IEnumerator UpdateTrack()
    {
        while( true )
        {
            //Use the closest track section
            float t1 = Time.realtimeSinceStartup;
            UpdateClosestSection();
            splineUpdateMs = 1000 * (Time.realtimeSinceStartup - t1);
            yield return new WaitForSeconds(updateDelta);
        }
        
    }

    private void Update()
    {
        DetectPlayerFall();
    }



    private void SetupTarget()
    {
        m_targetRb = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();
    }

    private void DetectPlayerFall()
    {
        if (currentSection != null)
        {
            //Detects the player fall
            Vector3 diff = currentSection.trackPosition - m_targetRb.transform.position;
            if (diff.y > fallFromTrackHeight)
            {
                TrackSection respawnTrack = currentSection.respawnTrackSection ? currentSection.respawnTrackSection : currentSection;

                //If respawn set, change the respawn point
                if (respawnTrack.respawnTransform)
                    diff = currentSection.respawnTrackSection.respawnTransform.position - m_targetRb.transform.position;

                //Translate the player
                m_targetRb.transform.parent.transform.position = m_targetRb.transform.parent.transform.position + diff + respawnHeight * Vector3.up;

                //Change track and update it
                currentSection = respawnTrack;
                respawnTrack.UpdateTrack(m_targetRb.transform.position);

                //Changes his velocity to match the direction of the track
                if (alignVelocityOnRespawn)
                {
                    float prevVelY = m_targetRb.velocity.y;
                    float prevVelXZ = new Vector2(m_targetRb.velocity.x, m_targetRb.velocity.z).magnitude;
                    m_targetRb.velocity = prevVelXZ * new Vector3(respawnTrack.trackDirection.x, 0, respawnTrack.trackDirection.z) + new Vector3(0, prevVelY, 0);
                }
            }

        }
        else
            Debug.LogError("no section set");
    }

    public void UpdateClosestSection()
    {
        if (currentSection != null)
        {
            //Variables and lambda used to find the best section
            TrackSection bestTrackSection = null;
            float bestDistance = float.MaxValue;
            Action<TrackSection> LookupSection = (section) =>
            {
                section.UpdateTrack(m_targetRb.position);
                float dist = Vector3.SqrMagnitude(m_targetRb.position - section.trackPosition);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestTrackSection = section;
                }
            };

            //LookUp parallel sections
            foreach (TrackSection prevSection in currentSection.prevSections)
            {
                if (checkPreviousSections)
                    LookupSection(prevSection);

                if (checkParallelSections)
                    foreach (TrackSection parallelSection in prevSection.nextSections)
                        LookupSection(parallelSection);
            }

            //LookUp next sections
            foreach (TrackSection nextSection in currentSection.nextSections)
                LookupSection(nextSection);

            //Update new section
            if (bestTrackSection)
                currentSection = bestTrackSection;
            else
                currentSection.UpdateTrack(m_targetRb.position);
        }
        else
            Debug.LogError("no section set");

    }

    private void OnDrawGizmos()
    {
        if(currentSection)
            Gizmos.DrawSphere(currentSection.trackPosition, 4f);
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;

        GUI.Label(new Rect(0, 0, 100, 10), "Spline update ms: " + splineUpdateMs, style);
    }
}
