using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof(Spline))]
public class TrackSection : MonoBehaviour
{


    [SerializeField] private List<float> searchDeltas = new List<float>();


    //Components references
    private Spline spline;

    private void Awake()
    {
        //Get components references
        spline = GetComponent<Spline>();

        //Set search deltas if needed
        if (searchDeltas.Count == 0)
            searchDeltas.Add(1f);
    }


    public void AlignWith( Track track )
    {

    }

    //Returns the tangent on the track spline at the closest position from "target"
    public Vector3 GetTrackTangent(Vector3 target)
    {
        float bestT = 0f;
        float bestDistannce = float.MaxValue;

        //First search on the whole spline
        for (float t = 0; t < spline.Length; t += searchDeltas[0])
        {
            float dist = Vector3.SqrMagnitude(target - spline.GetLocationAlongSplineAtDistance(t) - spline.transform.position);
            if (dist < bestDistannce)
            {
                bestDistannce = dist;
                bestT = t;
            }
        }

        //Second search around the last best distance
        for (int i = 1; i < searchDeltas.Count; ++i)
        {
            for (float t = Mathf.Clamp(bestT - searchDeltas[i - 1], 0, spline.Length); t < Mathf.Clamp(bestT + searchDeltas[i - 1], 0, spline.Length); t += searchDeltas[i])
            {
                float dist = Vector3.SqrMagnitude(target - spline.GetLocationAlongSplineAtDistance(t) - spline.transform.position);
                if (dist < bestDistannce)
                {
                    bestDistannce = dist;
                    bestT = t;
                }
            }
        }

        return spline.transform.position + spline.GetLocationAlongSplineAtDistance(bestT);
    }
}
