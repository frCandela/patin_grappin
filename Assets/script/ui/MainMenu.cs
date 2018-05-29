using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    [Header("Scenes names")]
    [SerializeField] private string m_trackSceneName;
    [SerializeField] private string m_arcadeSceneName;

    [Header("Menus references")]
    [SerializeField] private Transform m_mainMenu;
    [SerializeField] private Transform m_creditsMenu;
    [SerializeField] private Transform m_playMenu;
    [SerializeField] private Transform m_optionsMenu;

    private void Start()
    {
        ShowMainMenu(); 
        AkSoundEngine.PostEvent("Play_Music_Placeholder", gameObject);
    }
        
    public void MuteFx( bool state )
    {
        if(state)
            AkSoundEngine.PostEvent("Mute_FX_Mix", gameObject);
        else
        {
            AkSoundEngine.PostEvent("UnMute_FX_Mix", gameObject);
            print("UnMute_FX_Mix ??");
        }
            
    }

    public void MuteMusic(bool state)
    {
        if (state)
            AkSoundEngine.PostEvent("Mute_Music_Mix", gameObject);
        else
            AkSoundEngine.PostEvent("UnMute_Music_Mix", gameObject);
    }

    public void PlayTrackScene ()
    {
        AkSoundEngine.PostEvent("Stop_Music_Placeholder", gameObject);
        SceneManager.LoadScene(m_trackSceneName, LoadSceneMode.Single);
    }

    public void PlayArcadeScene()
    {
        AkSoundEngine.PostEvent("Stop_Music_Placeholder", gameObject);
        SceneManager.LoadScene(m_arcadeSceneName, LoadSceneMode.Single);
    }

    public void ShowPlayMenu()
    {
        m_mainMenu.gameObject.SetActive(false);
        m_creditsMenu.gameObject.SetActive(false);
        m_optionsMenu.gameObject.SetActive(false);

        m_playMenu.gameObject.SetActive(true);
    }

    public void ShowMainMenu()
    {
        m_playMenu.gameObject.SetActive(false);
        m_creditsMenu.gameObject.SetActive(false);
        m_optionsMenu.gameObject.SetActive(false);

        m_mainMenu.gameObject.SetActive(true);
    }

    public void ShowOptions()
    {
        m_mainMenu.gameObject.SetActive(false);
        m_playMenu.gameObject.SetActive(false);
        m_creditsMenu.gameObject.SetActive(false);

        m_optionsMenu.gameObject.SetActive(true);  
    }

    public void ShowCredits()
    {
        m_mainMenu.gameObject.SetActive(false);
        m_playMenu.gameObject.SetActive(false);
        m_optionsMenu.gameObject.SetActive(false);

        m_creditsMenu.gameObject.SetActive(true);
    }


    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #endif
    }



}
