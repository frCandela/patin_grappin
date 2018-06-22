using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TobiiCheckBox : MonoBehaviour
{
    [SerializeField] private bool m_defaultState = true;

    //Events
    public UnityEvent onChecked;
    public UnityEvent onUnChecked;

    //State
    public bool isHighlighted { get; private set;}
    public bool isChecked { get; private set; }

    //Graphic components references
    private Transform m_checkedBox, m_unCheckedBox;

    //Private references
    private Collider2D m_collider;

    // Use this for initialization
    void Awake ()
    {
        /* 
        Ordre des childs
            - name sprite
            - checked box
            - unchecked box
        */

        //Get graphic components references
        m_checkedBox = transform.GetChild(1);
        m_unCheckedBox = transform.GetChild(2);

        //Set highlights
        isHighlighted = false;
        isChecked = m_defaultState;

        if (isChecked)
        {
            m_checkedBox.gameObject.SetActive(true);
            m_unCheckedBox.gameObject.SetActive(false);
        }
        else
        {
            m_checkedBox.gameObject.SetActive(false);
            m_unCheckedBox.gameObject.SetActive(true);
        }

        //Get references
        m_collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Grapple") && isHighlighted)
            Toogle();

        //Higlighted
        if (m_collider.bounds.Contains(GazeManager.AverageGazePoint))
        {
            if( ! isHighlighted)
                SetHighlighted();
        }
        else
        {
            if (isHighlighted)
                SetHidden();
        }

            
    }

    public void Toogle()
    {
        if(isChecked)
        {
            onUnChecked.Invoke();
            m_checkedBox.gameObject.SetActive(false);
            m_unCheckedBox.gameObject.SetActive(true);
        }
        else
        {
            onChecked.Invoke();
            m_checkedBox.gameObject.SetActive(true);
            m_unCheckedBox.gameObject.SetActive(false);
        }
        AkSoundEngine.PostEvent("Play_UI_Select", gameObject);
        isChecked = ! isChecked;
    }

    private void SetHighlighted()
    {
        AkSoundEngine.PostEvent("Play_UI_On", gameObject);
        isHighlighted = true;
    }

    private void SetHidden()
    {
        AkSoundEngine.PostEvent("Play_UI_Off", gameObject);
        isHighlighted = false;
    }
}
