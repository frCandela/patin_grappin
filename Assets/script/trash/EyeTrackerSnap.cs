using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class EyeTrackerSnap : MonoBehaviour
{
    public Camera cameraRef;


    RectTransform recTrans;
    UnityEngine.UI.Image image;

    private void Awake()
    {
        //Set references
        recTrans = GetComponent<RectTransform>();
        image = GetComponent<UnityEngine.UI.Image>();

    }

	
	// Update is called once per frame
	void Update ()
    {
        if (TobiiAPI.GetUserPresence() == UserPresence.Present)
        {
            image.enabled = true;
            GameObject focusedObject = TobiiAPI.GetFocusedObject();
            if (focusedObject != null)
            {
                Vector3 screenPos = cameraRef.WorldToScreenPoint(focusedObject.transform.position);
                if (!float.IsNaN(screenPos.x))
                    recTrans.position = new Vector3(screenPos.x, screenPos.y, 0);
            }
            else
            {

                Vector2 pos = TobiiAPI.GetGazePoint().Screen;
                if (!float.IsNaN(pos.x))
                    recTrans.position = new Vector3(pos.x, pos.y, 0);
            }
        }
        else
        {
            image.enabled = false;
        }
    }
}
