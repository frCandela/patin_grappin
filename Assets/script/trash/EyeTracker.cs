using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class EyeTracker : MonoBehaviour
{

    RectTransform recTrans;
    //UnityEngine.UI.Image image;

    private GazePoint m_lastHandledPoint = GazePoint.Invalid;
    private Queue<GazePoint> m_samples;
    public int nbSamples = 100;

    private bool m_skipFrame = true;

    private void Awake()
    {
        //Set references
        recTrans = GetComponent<RectTransform>();
        //image = GetComponent<UnityEngine.UI.Image>();

        //Create objects
        m_samples = new Queue<GazePoint>();

    }


    private Vector2 GetGazeViewport()
    {
        Vector2 result = new Vector2(0, 0);
        foreach (GazePoint point in m_samples)
            result += point.Viewport;
        return result / m_samples.Count;
    }


	// Update is called once per frame
	void Update ()
    {
        IEnumerable<GazePoint> pointsSinceLastHandled = TobiiAPI.GetGazePointsSince(m_lastHandledPoint);
        foreach (GazePoint point in pointsSinceLastHandled)
        {
            m_lastHandledPoint = point;
            if ( point.IsValid )
            {
                m_samples.Enqueue(point);
                if( m_samples.Count > nbSamples)
                    m_samples.Dequeue();
            }
        }
        

        if (TobiiAPI.GetUserPresence() == UserPresence.Present)
        {
            if(m_skipFrame)
            {
                TobiiAPI.SubscribeGazePointData();
                m_skipFrame = false;
            }
            else 
            {
                //image.enabled = true;
                Vector2 pos = GetGazeViewport();
                if( ! float.IsNaN(pos.x)  )
                    recTrans.position = new Vector3(Screen.width * pos.x, Screen.height * pos.y, 0);
            }
        }
        else
        {
            m_skipFrame = true;
            //image.enabled = false;
        }

        

    }
}
