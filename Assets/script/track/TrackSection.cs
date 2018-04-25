using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent( typeof(Spline))]
public class TrackSection : MonoBehaviour
{
    public GameObject target;


    //Components
    [SerializeField] private Spline spline = null;

    public static Vector3 trackDir =  Vector3.forward;

    //private members
    private float t = 0f;

    private void Awake()
    {
        Util.EditorAssert(spline != null, "TrackSection.Awake: spline not set ");
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Looks for the closest node in the spline
        int index = -1;
        float distance = float.MaxValue;
        for (int i = 0; i < spline.nodes.Count; ++i)
        {
            SplineNode node = spline.nodes[i];
            float d = Vector3.SqrMagnitude(target.transform.position - node.position);
            if (d < distance)
            {
                index = i;
                distance = d;
            }
        }



        SplineNode node1 = spline.nodes[index - 1];
        SplineNode node2 = spline.nodes[index];
        SplineNode node3 = spline.nodes[index + 1];


        ///////////////////////////////////////////////////////////////////////////

        //Get the approximate tangent on the bezierCurve at the target position
        /*float distanceProj = Vector3.Dot(target.transform.position - node1.position, (node2.position - node1.position).normalized);
        Vector3 newTargetPos = node1.position + (node2.position - node1.position).normalized * distanceProj;

        float distanceProj2 = Vector3.Dot(target.transform.position - node2.position, (node3.position - node2.position).normalized);
        Vector3 newTargetPos2 = node2.position + (node3.position - node2.position).normalized * distanceProj2;

        

        if (distanceProj2 > 0)
            distanceProj += 2 * distanceProj2;

        if (distanceProj > Vector3.Distance(node2.position, node1.position))
        {
            node1 = spline.nodes[index ];
            node2 = spline.nodes[index + 1];
            node3 = spline.nodes[index + 2];

             distanceProj = Vector3.Dot(target.transform.position - node1.position, (node2.position - node1.position).normalized);
             newTargetPos = node1.position + (node2.position - node1.position).normalized * distanceProj;

             distanceProj2 = Vector3.Dot(target.transform.position - node2.position, (node3.position - node2.position).normalized);
             newTargetPos2 = node2.position + (node3.position - node2.position).normalized * distanceProj2;
        }

            //Vector3 proj1 = node1.position + Vector3.Project(target.transform.position - node1.position, node2.position - node1.position);
            //Vector3 proj2 = node2.position + Vector3.Project(target.transform.position - node2.position, node3.position - node2.position);



            //Gizmos.DrawSphere(proj1, 2f);
            Gizmos.DrawSphere(newTargetPos, 2f);
        Gizmos.DrawSphere(newTargetPos2, 2f);

        Gizmos.DrawSphere(node1.position, 3f);
        Gizmos.DrawSphere(node2.position, 3f);
        Gizmos.DrawSphere(node3.position, 3f);*/

        ///////////////////////////////////////////////////////////////////////////


        //if the node is not behind, get the previous node
        Vector3 dir1 = spline.nodes[index].direction;
        Vector3 dir2 = target.transform.position - spline.nodes[index].position;
        float length = Vector3.Dot(dir1, dir2.normalized);
        if (length < 0)
        {
            if (index > 0)
                --index;
        }

        //Get the corresponding bezier curve
        CubicBezierCurve bezierCurve = null;
        foreach (CubicBezierCurve curve in spline.curves)
        {
            if (curve.n1 == spline.nodes[index])
            {
                bezierCurve = curve;
                break;
            }
        }

        //Get the approximate tangent on the bezierCurve at the target position
        Vector3 pos1 = spline.nodes[index].position;
        Vector3 pos2 = spline.nodes[index + 1].position;
        float distanceProj = Vector3.Dot(target.transform.position - pos1, (pos2 - pos1).normalized);
        Vector3 newTargetPos = pos1 + (pos2 - pos1).normalized * distanceProj;
        float time = distanceProj / (pos2 - pos1).magnitude;

        time = Mathf.Clamp(time, 0f, 1f);

        Vector3 finalPos = bezierCurve.GetLocation(time);


        trackDir = Vector3.Lerp(trackDir, bezierCurve.GetTangent(time), 0.1f);
    }

    private void OnDrawGizmos()
    {
        //Vector3 position = spline.GetLocationAlongSplineAtDistance(t * spline.Length);
        /* Vector3 position = GetClosestPointAlongSpline(target.transform.position);
         Gizmos.DrawSphere(position, 3f);*/



        Gizmos.DrawLine(target.transform.position, target.transform.position + 5 * trackDir);
    }

    

}
