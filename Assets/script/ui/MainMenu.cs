﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    [Header("Scenes names")]
    [SerializeField] private string m_tuto1SceneName;
    [SerializeField] private string m_tuto2SceneName;
    [SerializeField] private string m_tuto3SceneName;
    [SerializeField] private string m_arcadeSceneName;

    [Header("Menus references")]
    [SerializeField] private Transform m_mainMenu;
    [SerializeField] private Transform m_playMenu;
    [SerializeField] private Transform m_optionsMenu;
    [SerializeField] private Transform m_tutoMenu;

    [Header("Dirty references")]
    [SerializeField] private TobiiCheckBox m_MusicCheckBox;
    [SerializeField] private TobiiCheckBox m_FXCheckBox;


    private void Start()
    {
        ShowMainMenu(); 
        AkSoundEngine.PostEvent("Play_Menu", gameObject);

        MuteMusic(DataManager.gameData.music_muted);
        MuteFx(DataManager.gameData.fx_muted);
    }
        
    public void MuteFx( bool state )
    {
        
        if (state)
            AkSoundEngine.PostEvent("Mute_FX_Mix", gameObject);
        else
            AkSoundEngine.PostEvent("UnMute_FX_Mix", gameObject);

        DataManager.gameData.fx_muted = state;

    }

    public void MuteMusic(bool state)
    {
        if (state)
            AkSoundEngine.PostEvent("Mute_Music_Mix", gameObject);
        else         
            AkSoundEngine.PostEvent("UnMute_Music_Mix", gameObject);

        DataManager.gameData.music_muted = state;
    }

    public void ApplySettings()
    {
        DataManager.SaveGameData();
    }

    public void PlayTuto1 ()
    {
        AkSoundEngine.PostEvent("Stop_Menu", gameObject);
        SceneManager.LoadScene(m_tuto1SceneName, LoadSceneMode.Single);
    }
    public void PlayTuto2()
    {
        AkSoundEngine.PostEvent("Stop_Menu", gameObject);
        SceneManager.LoadScene(m_tuto2SceneName, LoadSceneMode.Single);
    }
    public void PlayTuto3()
    {
        AkSoundEngine.PostEvent("Stop_Menu", gameObject);
        SceneManager.LoadScene(m_tuto3SceneName, LoadSceneMode.Single);
    }

    public void PlayArcadeScene()
    {
        AkSoundEngine.PostEvent("Stop_Menu", gameObject);
        SceneManager.LoadScene(m_arcadeSceneName, LoadSceneMode.Single);
    }

    public void ShowTutosMenu()
    {
        m_mainMenu.gameObject.SetActive(false);
        m_optionsMenu.gameObject.SetActive(false); 
        m_playMenu.gameObject.SetActive(false);

        m_tutoMenu.gameObject.SetActive(true);
    }

    public void ShowPlayMenu()
    {
        m_mainMenu.gameObject.SetActive(false);
        m_optionsMenu.gameObject.SetActive(false);
        m_tutoMenu.gameObject.SetActive(false);

        m_playMenu.gameObject.SetActive(true);
    }

    public void ShowMainMenu()
    {
        m_playMenu.gameObject.SetActive(false);
        m_optionsMenu.gameObject.SetActive(false);
        m_tutoMenu.gameObject.SetActive(false);

        m_mainMenu.gameObject.SetActive(true);
    }

    public void ShowOptions()
    {
        m_mainMenu.gameObject.SetActive(false);
        m_playMenu.gameObject.SetActive(false);
        m_tutoMenu.gameObject.SetActive(false);

        m_optionsMenu.gameObject.SetActive(true);

        if (m_MusicCheckBox.isChecked != !DataManager.gameData.music_muted)
            m_MusicCheckBox.Toogle();
        if (m_FXCheckBox.isChecked != !DataManager.gameData.fx_muted)
            m_FXCheckBox.Toogle();
    }

    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #endif
    }
}
