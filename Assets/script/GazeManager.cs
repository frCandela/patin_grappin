using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class GazeManager : MonoBehaviour
{
    [Header("Sampling:")]
    [SerializeField] public int nbSamples = 50;

    public static Vector2 AverageGazePoint { get; private set; }

    private static GazeManager m_instance = null;
    private GazePoint m_lastHandledPoint = GazePoint.Invalid;
    private static Queue<GazePoint> m_samples;

    // Use this for initialization
    private void Awake()
    {
        //Singleton design pattern
        if ( ! m_instance)
        {
            m_instance = this;
            m_samples = new Queue<GazePoint>();
            DontDestroyOnLoad(gameObject);
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
        
    }
}
