using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class GazeManager : MonoBehaviour
{
    [Header("Sampling:")]
    [SerializeField] private int nbSamples = 50;
    [SerializeField] private float gazeRemanance = 1f;
    private static float staticGazeRemanance = 1f;
    [SerializeField] private float gazeDistance = 650f;
    private static float staticGazeDistance = 1f;

    //Read only 
    public static Vector2 AverageGazePoint { get; private set; }
    public static GameObject GazedObject { get; private set; }

    //Private
    private static GazeManager m_instance = null;
    private static Queue<GazePoint> m_samples;
    private static GazePoint m_lastHandledPoint = GazePoint.Invalid;
    private static Camera m_camera = null;

    //Grap remanence
    private static GazeInfo m_lastGazeInfo;
    private static float lastGazeWorldPointTime = 0f;

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
            staticGazeRemanance = gazeRemanance;
            staticGazeDistance = gazeDistance;

            m_lastGazeInfo = new GazeInfo();
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
        Rect rect = m_camera.pixelRect;
        AverageGazePoint = new Vector2(rect.width * AverageGazePoint.x, rect.height * AverageGazePoint.y);

        //If out of screen use mouse position instead
        if (TobiiAPI.GetUserPresence() != UserPresence.Present)
            AverageGazePoint = Input.mousePosition;

        GameObject currendGazedObject = null;
        //Gets the gazed object
        if (AverageGazePoint.x >= 0 && AverageGazePoint.x <= Screen.width && AverageGazePoint.y >= 0 && AverageGazePoint.y <= Screen.height)
        {
            Ray ray = m_camera.ScreenPointToRay(AverageGazePoint);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, staticGazeDistance, LayerMask.GetMask("GazeObject")))
            {
                currendGazedObject = raycastHit.collider.gameObject;
            }
        }

        //Gaze at nothing
        if(!currendGazedObject)
        {
            //Reset previous gazed abject
            if (GazedObject)
            {
                GazeObject gaze = GazedObject.GetComponent<GazeObject>();
                gaze.SetNotGazed();
            }
            GazedObject = null;
        }
        //Gaze at something
        else
        {
            if (currendGazedObject != GazedObject)
            {
                //Reset previous gazed object
                if (GazedObject)
                {
                    GazedObject.GetComponent<GazeObject>().SetNotGazed();
                }

                //Set new gazed object
                GazedObject = currendGazedObject;
                GazedObject.GetComponent<GazeObject>().SetGazed();
            }
        }
    }

    public class GazeInfo
    {
        public GameObject gameobject = null;
        public Vector3 position = Vector3.zero;
    }

    public static GazeInfo GetGazeWorldPoint()
    {
        //Gets the gazed object
        if (AverageGazePoint.x >= 0 && AverageGazePoint.x <= Screen.width && AverageGazePoint.y >= 0 && AverageGazePoint.y <= Screen.height)
        {
            Ray ray = m_camera.ScreenPointToRay(AverageGazePoint);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, staticGazeDistance))
            {
                if (raycastHit.collider.gameObject.tag != "noGrab")
                {
                    m_lastGazeInfo.position = raycastHit.point;
                    m_lastGazeInfo.gameobject = raycastHit.collider.gameObject;
                    lastGazeWorldPointTime = Time.realtimeSinceStartup;
                    
                }                    
            }  
        }

        if (Time.realtimeSinceStartup - lastGazeWorldPointTime < staticGazeRemanance )
            return m_lastGazeInfo;
        else
            return null;
        
            
    }
}
