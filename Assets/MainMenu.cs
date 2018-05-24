using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    private Transform m_play;
    private Transform m_options;
    private Transform m_credits;
    private Transform m_quit;

    private void Start()
    {
        m_play = transform.GetChild(0);
        m_options = transform.GetChild(1);
        m_credits = transform.GetChild(2);
        m_quit = transform.GetChild(3);
    }

    public void Play()
    {

    }

    public void Options()
    {

    }

    public void Credits()
    {

    }

    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #endif
    }



}
