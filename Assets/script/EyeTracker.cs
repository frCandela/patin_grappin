using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class EyeTracker : MonoBehaviour
{

    RectTransform recTrans;
    UnityEngine.UI.Image image;

    private void Awake()
    {
        recTrans = GetComponent<RectTransform>();
        image = GetComponent<UnityEngine.UI.Image>();

        TobiiAPI.SubscribeGazePointData();

    }

    // Use this for initialization
    void Start ()
    {
		
	}

	// Update is called once per frame
	void Update ()
    {
        if (TobiiAPI.GetUserPresence() == UserPresence.Present)
        {
            image.enabled = true;
            Vector2 pos = TobiiAPI.GetGazePoint().Viewport;
            recTrans.position = new Vector3( Screen.width * pos.x, Screen.height * pos.y, 0);

        }
        else
        {
            image.enabled = false;
        }

        

    }
}
