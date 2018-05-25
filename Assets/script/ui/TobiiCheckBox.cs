using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TobiiCheckBox : MonoBehaviour
{
    //Events
    public UnityEvent onChecked;
    public UnityEvent onUnChecked;

    //State
    public bool isHighlighted { get; private set;}
    public bool isChecked { get; private set; }

    //Graphic components references
    private Transform m_highlight;
    private Transform m_noHighlight;
    private Transform m_cross;

    // Use this for initialization
    void Awake ()
    {
        //Get graphic components references
        m_highlight = transform.GetChild(0);
        m_noHighlight = transform.GetChild(1);
        m_cross = transform.GetChild(2);

        //Set graphic components
        m_highlight.gameObject.SetActive(true);
        m_noHighlight.gameObject.SetActive(false);
        m_cross.gameObject.SetActive(true);

        //Set highlights
        isHighlighted = false;
        isChecked = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Grapple") && isHighlighted)
            Toogle();
    }

    private void Toogle()
    {
        if(isChecked)
        {
            onUnChecked.Invoke();
            m_cross.gameObject.SetActive(false);
        }
        else
        {
            onChecked.Invoke();
            m_cross.gameObject.SetActive(true);
        }

        isChecked = ! isChecked;
    }

    private void SetHighlighted()
    {
        m_highlight.gameObject.SetActive(false);
        m_noHighlight.gameObject.SetActive(true);
        isHighlighted = true;
    }

    private void SetHidden()
    {
        m_highlight.gameObject.SetActive(true);
        m_noHighlight.gameObject.SetActive(false);
        isHighlighted = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetHighlighted();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SetHidden();
    }
}
