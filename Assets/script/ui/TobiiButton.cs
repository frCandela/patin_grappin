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

    // Use this for initialization
    void Awake ()
    {
        m_highlight = transform.GetChild(0);
        m_noHighlight = transform.GetChild(1);

        m_highlight.gameObject.SetActive(true);
        m_noHighlight.gameObject.SetActive(false);
        highlighted = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if( Input.GetButtonDown("Grapple") && highlighted)
        {
            onButtonPressed.Invoke();
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_highlight.gameObject.SetActive(false);
        m_noHighlight.gameObject.SetActive(true);
        highlighted = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        m_highlight.gameObject.SetActive(true);
        m_noHighlight.gameObject.SetActive(false);
        highlighted = false;
    }
}
