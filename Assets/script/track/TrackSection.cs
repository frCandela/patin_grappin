using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof(Spline))]
public class TrackSection : MonoBehaviour
{
    public Vector3 trackDirection { get; private set; }
    public Vector3 trackPosition { get; private set; }

    //Components references
    [SerializeField]  private Spline spline;
    [SerializeField] bool invertDirection = false;

    private void Awake()
    {
        //Get components references
        //spline = GetComponent<Spline>();

        trackDirection = Vector3.zero;
        trackPosition = Vector3.zero;
    }

    //Updates the tangent on the track spline at the closest position from "target"
    public void UpdateTrack(Vector3 target)
    {
        float bestT = 0f;
        float bestDistance = float.MaxValue;


        float[] searchDeltas = new float[] { 5f, 1f, 0.1f};

        //First search on the whole spline
        for (float t = 0; t < spline.Length; t += searchDeltas[0])
        {
            float dist = Vector3.SqrMagnitude(target - spline.GetLocationAlongSplineAtDistance(t) - spline.transform.position);
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
                float dist = Vector3.SqrMagnitude(target - spline.GetLocationAlongSplineAtDistance(t) - spline.transform.position);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestT = t;
                }
            }
        }

        //Set direction
        if(invertDirection)
            trackDirection = - spline.GetTangentAlongSplineAtDistance(bestT).normalized;
        else
            trackDirection = spline.GetTangentAlongSplineAtDistance(bestT).normalized;

        //Set position
        trackPosition = spline.GetLocationAlongSplineAtDistance(bestT);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(trackPosition, 2f);

    }
}
