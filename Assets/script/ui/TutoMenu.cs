using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoMenu : MonoBehaviour
{
    private LevelManager m_levelManager;
    private bool m_active = false;

    private void Awake()
    {
        m_levelManager = FindObjectOfType<LevelManager>();
        HideTuto();
    }

    public void PopTuto( int id )
    {
        m_active = true;
        if( ! m_levelManager.paused)
            m_levelManager.TooglePause();
        transform.GetChild(id).gameObject.SetActive(true);
    }

    public void HideTuto()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        m_active = false;
    }

    private void Update()
    {
        if(m_active && Input.GetButtonDown("Grapple"))  
        {
            HideTuto();
            if (m_levelManager.paused)
                m_levelManager.TooglePause();
        }
    }

}
