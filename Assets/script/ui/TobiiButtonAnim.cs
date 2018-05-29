using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TobiiButtonAnim : MonoBehaviour
{
    public UnityEvent onButtonPressed;

    public bool highlighted { get; private set;}

    private Collider2D m_collider;
    private Animator m_buttonAnim;


    // Use this for initialization
    void Awake ()
    {
        m_collider = GetComponent<Collider2D>();
        m_buttonAnim = GetComponent<Animator>();

        highlighted = false;
    }

    // Update is called once per frame
    void Update ()
    {
        //Pressed
		if( Input.GetButtonDown("Grapple") && highlighted)
        {
            AkSoundEngine.PostEvent("Play_UI_Select", gameObject); 
            onButtonPressed.Invoke();
            SetHidden();
        }

        //Higlighted
        if(m_collider.bounds.Contains(GazeManager.AverageGazePoint))
        {
            if(!highlighted)
                SetHighlighted();
        }
        else
        {
            if(highlighted)
                SetHidden();
        }

    }

    private void SetHighlighted()
    {
        AkSoundEngine.PostEvent("Play_UI_On", gameObject);
        
        m_buttonAnim.Play("ON_anim", -1, 0f);
        highlighted = true;
    }

    private void SetHidden()
    {
        AkSoundEngine.PostEvent("Play_UI_Off", gameObject);
        m_buttonAnim.Play("OFF_anim", -1, 0f);
        highlighted = false;
    }
}
