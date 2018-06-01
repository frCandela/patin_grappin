using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private bool m_activateMusic = true;
    [SerializeField] private string m_menuSceneString = "menu";

    //References
    private PauseMenu m_pauseMenu;
    private List<MonoBehaviour> m_pausedScripts;

    //Private members
    private bool m_paused = false;

    private void Awake()
    {
        m_pauseMenu = FindObjectOfType<PauseMenu>();
        m_pauseMenu.gameObject.SetActive(false);

        m_pausedScripts = new List<MonoBehaviour>(FindObjectsOfType<boostEyeFX>());
        m_pausedScripts.Add(FindObjectOfType<PlayerController>());
    }

    void Start ()
    {
        //Link events
        m_pauseMenu.onGameQuit.AddListener(QuitToMenu);
        m_pauseMenu.onGameResumed.AddListener(TooglePause);

        //Set music and sounds
        AkSoundEngine.PostEvent("Play_Speed_RTPC", gameObject);
        if (m_activateMusic)
            AkSoundEngine.PostEvent("Play_Music_Placeholder", gameObject);
    }

    private void QuitToMenu()
    {
        AkSoundEngine.PostEvent("Stop_Music_Placeholder", gameObject);
        TooglePause();
        SceneManager.LoadScene(m_menuSceneString, LoadSceneMode.Single);
    }

    private void Update()
    {
        //Restarts the level
        if (GazeManager.DebugActive && Input.GetButtonDown("Restart"))
        {
            AkSoundEngine.PostEvent("Stop_Music_Placeholder", gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //Pauses the level
        UserPresence userPresence = TobiiAPI.GetUserPresence();
        HeadPose headPose = TobiiAPI.GetHeadPose();
        if ( ! m_paused )
        {
            if (Input.GetButtonDown("Pause") || (GazeManager.TobiiConnected && (userPresence != UserPresence.Present || !headPose.IsValid)))
                TooglePause();
        }
        else if (Input.GetButtonDown("Pause"))
                TooglePause();
    }

    private void TooglePause()
    {
        m_paused = !m_paused;

        //Pause the game
        if( m_paused)
        {
            AkSoundEngine.SetState("Music","off_game");
            m_pauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0f;
            foreach(MonoBehaviour m in m_pausedScripts)
                m.enabled = false;
        }
        //Resumes the game
        else
        {
            AkSoundEngine.SetState("Music", "in_game");
            m_pauseMenu.gameObject.SetActive(false);
            Time.timeScale = 1f;
            foreach (MonoBehaviour m in m_pausedScripts)
                m.enabled = true;
        }
    }


}
