using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class GazeManager : MonoBehaviour
{
    [Header("Sampling:")]
    [SerializeField] public int nbSamples = 50;

    //Read only 
    public static Vector2 AverageGazePoint { get; private set; }
    public static GameObject GazedObject { get; private set; }

    //Private
    private static GazeManager m_instance = null;
    private static Queue<GazePoint> m_samples;
    private GazePoint m_lastHandledPoint = GazePoint.Invalid;
    private Camera m_camera = null;

    // Use this for initialization
    private void Awake()
    {
        //Singleton design pattern
        if ( ! m_instance)
        {
            m_instance = this;
            m_samples = new Queue<GazePoint>();
            m_camera = GetComponent<Camera>();
            Util.EditorAssert(m_camera != null, "GazeManager.Awake(): No camera component ");
            GazedObject = null;
        }
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update ()
    {
        //Makes an average of the payer's gaze points
        IEnumerable<GazePoint> pointsSinceLastHandled = TobiiAPI.GetGazePointsSince(m_lastHandledPoint);
        foreach (GazePoint point in pointsSinceLastHandled)
        {
            m_lastHandledPoint = point;
            if (point.IsValid)
            {
                m_samples.Enqueue(point);
                if (m_samples.Count > nbSamples)
                    m_samples.Dequeue();
            }
        }

        //Calculates the average Gaze point
        AverageGazePoint = new Vector2(0, 0);
        foreach (GazePoint point in m_samples)
            AverageGazePoint += point.Viewport;
        AverageGazePoint /= m_samples.Count;

        GameObject currendGazedObject = null;
        //Gets the gazed object
        if (AverageGazePoint.x >= 0 && AverageGazePoint.x <= 1 && AverageGazePoint.y >= 0 && AverageGazePoint.y <= 1)
        {
            Vector2 screenPoint = new Vector2(Screen.width * AverageGazePoint.x, Screen.height * AverageGazePoint.y);
            Ray ray = m_camera.ScreenPointToRay(screenPoint);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 100f, LayerMask.GetMask("GazeObject")))
                currendGazedObject = raycastHit.collider.gameObject;
        }

        //Gaze at nothing
        if(!currendGazedObject)
        {
            if (GazedObject)
            {
                GazedObject.GetComponent<GazeObject>().SetNotGazed();
            }
            GazedObject = null;
        }
        //Gaze at something
        else
        {
            if (currendGazedObject != GazedObject)
            {
                if (GazedObject)
                {
                    GazedObject.GetComponent<GazeObject>().SetNotGazed();
                }
                GazedObject = currendGazedObject;
                GazedObject.GetComponent<GazeObject>().SetGazed();
            }
        }

    }
}
