using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[ExecuteInEditMode]
public class Track : MonoBehaviour
{
    //Editor parameters

    [Header("Track parameters: ")]
    [SerializeField, Tooltip("If false use camera position instead")] private bool usePlayerPosition = true;
    [SerializeField] private float fallFromTrackHeight = 10f;

    [Header("Track Sections Tree: ")]
    [SerializeField] private TrackSection currentSection = null;
    [SerializeField] private bool checkPreviousSections = true;
    [SerializeField] private bool checkParallelSections = true;

    //Private members
    Transform targetTransform = null;

    //Debug
    float splineUpdateMs = 0f;

    public TrackSection trackSection
    {
        get { return currentSection; }
        private set { }
    }

    private void Awake()
    {
    }

    private void Start()
    {
        if (currentSection)
            currentSection.UpdateTrack(Camera.main.transform.position);    

        SetupTarget();
    }

    private void OnValidate()
    {
        SetupTarget();
    }

    private void Update()
    {
        //Use the closest track section
        float t1 = Time.realtimeSinceStartup;
        UpdateClosestSection();
        splineUpdateMs = 1000 * (Time.realtimeSinceStartup - t1);

        DetectPlayerFall();
    }

    private void SetupTarget()
    {
        if (usePlayerPosition)
            targetTransform = FindObjectOfType<PlayerController>().transform;
        else
            targetTransform = Camera.main.transform;
    }

    private void DetectPlayerFall()
    {
        Vector3 diff = currentSection.trackPosition - targetTransform.transform.position;
        if (diff.y > fallFromTrackHeight)
            targetTransform.parent.transform.position = targetTransform.parent.transform.position + diff + fallFromTrackHeight * Vector3.up;   
    }


    public void UpdateClosestSection()
    {
        //Variables and lambda used to find the best section
        TrackSection bestTrackSection = null;
        float bestDistance = float.MaxValue;
        Action<TrackSection> LookupSection = (section) =>
        {
            section.UpdateTrack(targetTransform.position);
            float dist = Vector3.SqrMagnitude(targetTransform.position - section.trackPosition);
            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestTrackSection = section;
            }
        };

        //LookUp parallel sections
        foreach (TrackSection prevSection in currentSection.prevSections)
        {
            if( checkPreviousSections)
                LookupSection(prevSection);

            if(checkParallelSections)
                foreach (TrackSection parallelSection in prevSection.nextSections)
                    LookupSection(parallelSection);
        }

        //LookUp next sections
        foreach (TrackSection nextSection in currentSection.nextSections)
                LookupSection(nextSection);

        currentSection = bestTrackSection;

    }

    private void OnDrawGizmos()
    {
        if(currentSection)
            Gizmos.DrawSphere(currentSection.trackPosition, 4f);
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;

        GUI.Label(new Rect(0, 0, 100, 10), "Spline update ms: " + splineUpdateMs, style);
    }
}
