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
        if (Input.GetKeyDown(KeyCode.F2))
            PopTuto(0);
        if (Input.GetKeyDown(KeyCode.F3))
            PopTuto(1);
        if (Input.GetKeyDown(KeyCode.F4))
            PopTuto(2);
        if (Input.GetKeyDown(KeyCode.F5))
            PopTuto(3);

        if(m_active && Input.GetButtonDown("Grapple"))  
        {
            HideTuto();
            if (m_levelManager.paused)
                m_levelManager.TooglePause();
        }
    }

}
