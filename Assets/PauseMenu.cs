using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private LevelManager m_levelManager;
    private bool m_active = false;

    private void Start()
    {
        m_levelManager = FindObjectOfType<LevelManager>();
        Hide();
    }

    private void Update()
    {
        //Pauses the level
        UserPresence userPresence = TobiiAPI.GetUserPresence();
        HeadPose headPose = TobiiAPI.GetHeadPose();
        if (!m_levelManager.paused )
        {
            if (Input.GetButtonDown("Pause") || (GazeManager.TobiiConnected && (userPresence != UserPresence.Present || !headPose.IsValid)))
            {
                m_levelManager.TooglePause();
                Show();
                m_active = true;
            }
                
        }
        //Resume
        else if (m_active && Input.GetButtonDown("Pause"))
        {
            m_levelManager.TooglePause();
            Hide();
            m_active = false;
        }
            
    }

    void Show()
    {   
        foreach( Transform child in transform)
            child.gameObject.SetActive(true);
    }

    void Hide()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        m_levelManager.QuitToMenu();
    }

    public void ResumeGame()
    {
        Hide();
        m_levelManager.TooglePause();
    }

}
