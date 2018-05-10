using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSection : MonoBehaviour
{
    public Vector3 trackDirection { get; private set; }
    public Vector3 trackPosition { get; private set; }
    public bool endTrackReached { get; private set; }


    [Header("Track tree")]
    public List<TrackSection> prevSections = new List<TrackSection>();
    public List<TrackSection> nextSections = new List<TrackSection>();
    

    //Components references
    [Header("Section parameters")]
    [SerializeField]  private Spline spline;
    [SerializeField] bool invertDirection = false;

    private void Awake()
    {
        //Set Component spline if none is set
        if (!spline)
            spline = GetComponent<Spline>();

        trackDirection = Vector3.zero;
        trackPosition = Vector3.zero;
    }

    private void OnValidate()
    {
        //Set Component spline if none is set
        if (!spline)
            spline = GetComponent<Spline>();
    }

    //Updates the tangent on the track spline at the closest position from "target"
    public void UpdateTrack(Vector3 target)
    {
        //Error if no spline
        if( !spline )
        {
            trackPosition = Vector3.zero;
            trackDirection = Vector3.zero;
            Debug.LogError(gameObject.name + " : no spline set !");
            return;
        }

        float bestT = 0f;
        float bestDistance = float.MaxValue;

        float[] searchDeltas = new float[] { 5f, 1f, 0.1f};

        //First search on the whole spline
        for (float t = 0; t < spline.Length; t += searchDeltas[0])
        {
            float dist = Vector3.SqrMagnitude(target - (spline.GetLocationAlongSplineAtDistance(t) + transform.position));
            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestT = t;
            }
        }

        //Second search around the last best distance
        for (int i = 1; i < searchDeltas.Length; ++i)
        {
            for (float t = Mathf.Clamp(bestT - searchDeltas[i - 1], 0, spline.Length); t < Mathf.Clamp(bestT + searchDeltas[i - 1], 0, spline.Length); t += searchDeltas[i])
            {
                float dist = Vector3.SqrMagnitude(target - (spline.GetLocationAlongSplineAtDistance(t) + transform.position));
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestT = t;
                }
            }
        }

        //End of the track reached
        if(bestT + 0.1f > spline.Length)
            endTrackReached = true;
        else
            endTrackReached = false;

        //Set direction
        if (invertDirection)
            trackDirection = - spline.GetTangentAlongSplineAtDistance(bestT).normalized;
        else
            trackDirection = spline.GetTangentAlongSplineAtDistance(bestT).normalized;

        //Set position
        trackPosition = transform.position + spline.GetLocationAlongSplineAtDistance(bestT);
    }


}
