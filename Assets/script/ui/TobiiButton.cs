using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TobiiButton : MonoBehaviour
{
    public UnityEvent onButtonPressed;

    public bool highlighted { get; private set;}
    private Transform m_highlight;
    private Transform m_noHighlight;

    private Collider2D m_collider;


    // Use this for initialization
    void Awake ()
    {
        m_highlight = transform.GetChild(0);
        m_noHighlight = transform.GetChild(1);

        m_highlight.gameObject.SetActive(true);
        m_noHighlight.gameObject.SetActive(false);
        highlighted = false;

        m_collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update ()
    {
        //Pressed
		if( Input.GetButtonDown("Grapple") && highlighted)
        {
            onButtonPressed.Invoke();
            SetHidden();
        }

        //Higlighted
        if (m_collider.bounds.Contains(GazeManager.AverageGazePoint))
            SetHighlighted();
        else
            SetHidden();

    }

    private void SetHighlighted()
    {
        m_highlight.gameObject.SetActive(false);
        m_noHighlight.gameObject.SetActive(true);
        highlighted = true;
    }

    private void SetHidden()
    {
        m_highlight.gameObject.SetActive(true);
        m_noHighlight.gameObject.SetActive(false);
        highlighted = false;
    }
}
