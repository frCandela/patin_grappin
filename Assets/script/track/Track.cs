using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.Events;
using UnityEditor;

[ExecuteInEditMode]
public class Track : MonoBehaviour
{
    //Editor parameters
    [Header("Track parameters: ")]
    [SerializeField] private float fallFromTrackHeight = 50;
    [SerializeField, Range(0, 1000f)] public float respawnHeight = 100f;
    [SerializeField] private float updateDelta = 0.2f;
    [SerializeField] private bool alignVelocityOnRespawn = false;

    [Header("Boost: ")]
    [SerializeField] private float boostHeight = 100f;
    [SerializeField] private float boostDelta = 0.1f;

    [Header("Track Sections Tree: ")]
    [SerializeField] private TrackSection currentSection = null;
    [SerializeField] private bool checkPreviousSections = true;
    [SerializeField] private bool checkParallelSections = true;

    [Header("Events")]
    public UnityEvent onRespawn;

    //Private members
    private Rigidbody m_targetRb = null;
    private Grap m_grap = null;

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
            currentSection = GetComponent<TrackSection>();

        m_targetRb = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();
        m_grap = FindObjectOfType<Grap>();

        if (currentSection)
            currentSection.UpdateTrack(m_targetRb.position);

        StartCoroutine(UpdateTrack());
    }

    IEnumerator UpdateTrack()
    {
        while (true)
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
        float height =  m_targetRb.transform.position.y - currentSection.trackPosition.y;

        if (height > boostHeight)
            m_targetRb.transform.parent.position = m_targetRb.transform.parent.position - Time.deltaTime * boostDelta * Vector3.up;           

        if (Input.GetKeyDown(KeyCode.T) || height < fallFromTrackHeight)
            RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        //Translate the player
        onRespawn.Invoke();

        currentSection = currentSection.respawnTrackSection;
        updateDelta = 1f;
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
                float dist = Vector3.Magnitude(m_targetRb.position - section.trackPosition);
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
            
            LookupSection(currentSection);

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
        if( GazeManager.DebugActive)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;

            GUI.Label(new Rect(0, 0, 100, 10), "Spline update ms: " + splineUpdateMs, style);

            float height = m_targetRb.transform.position.y - currentSection.trackPosition.y;
            GUI.Label(new Rect(0, 40, 100, 10), "Player height: " + height, style);
        }
    }
}
