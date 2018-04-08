using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class GazeTrace : MonoBehaviour
{
    private RectTransform m_recTrans;

    private void Awake()
    {
        m_recTrans = GetComponent<RectTransform>();
        Util.EditorAssert(m_recTrans != null, "GazeTrace.Awake(): RectTransform not set");
    }

    // Update is called once per frame
    void Update ()
    {
        Vector2 pos = GazeManager.AverageGazePoint;
        if (!float.IsNaN(pos.x))
            m_recTrans.position = new Vector3(Screen.width * pos.x, Screen.height * pos.y, 0);
    }
}
