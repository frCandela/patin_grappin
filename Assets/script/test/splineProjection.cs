using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splineProjection : MonoBehaviour
{

    public Spline spline;
    public GameObject target;

    private Vector3 projection;
    private float currentT = -1f;
    private float time = 0f;

    public List<float> searchDeltas = new List<float>();

    // Use this for initialization
    void Awake ()
    {
        if (searchDeltas.Count == 0)
            searchDeltas.Add(1f);
    }
	
	// Update is called once per frame
	void Update ()
    {



        //if( Input.GetKeyDown( KeyCode.Space))
        {
            float t1 = Time.realtimeSinceStartup;

            float bestT = 0f;
            float bestDistannce = float.MaxValue;

            //First search on the whole spline
            for (float t = 0; t < spline.Length; t += searchDeltas[0])
            {
                float dist = Vector3.SqrMagnitude(target.transform.position - spline.GetLocationAlongSplineAtDistance(t) - spline.transform.position);
                if (dist < bestDistannce)
                {
                    bestDistannce = dist;
                    bestT = t;
                }
            }

            //Second search around the last best distance
            for( int i = 1; i < searchDeltas.Count; ++i)
            {
                for (float t = Mathf.Clamp(bestT - searchDeltas[i-1], 0, spline.Length); t < Mathf.Clamp(bestT + searchDeltas[i-1], 0, spline.Length); t += searchDeltas[i])
                {
                    float dist = Vector3.SqrMagnitude(target.transform.position - spline.GetLocationAlongSplineAtDistance(t) - spline.transform.position);
                    if (dist < bestDistannce)
                    {
                        bestDistannce = dist;
                        bestT = t;
                    }
                }
            }

            currentT = bestT;

            projection = spline.transform.position + spline.GetLocationAlongSplineAtDistance(currentT);
            float t2 = Time.realtimeSinceStartup;
            time = t2-t1;
        }
    }



    private void OnDrawGizmos()
    {

        Gizmos.DrawSphere(projection, 1f);

    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;

        GUI.Label(new Rect(0, 0, 100, 10), "Time: " + (1000 * time) + " ms", style);
        GUI.Label(new Rect(0, 10, 100, 10), "Lenght: " + spline.Length, style);
    }
}
