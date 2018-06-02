using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private bool m_activateMusic = true;
    [SerializeField] private string m_menuSceneString = "menu";

    public UnityEvent onLevelPaused;
    public UnityEvent onLevelResumed;

    //References
    private List<MonoBehaviour> m_pausedScripts;

    //Private members

    public bool paused { get; private set; }

    private void Awake()
    {
        paused = false;
          m_pausedScripts = new List<MonoBehaviour>(FindObjectsOfType<boostEyeFX>());
        m_pausedScripts.Add(FindObjectOfType<PlayerController>());
    }

    void Start ()
    {
        //Set music and sounds
        AkSoundEngine.PostEvent("Play_Speed_RTPC", gameObject);
        if (m_activateMusic)
            AkSoundEngine.PostEvent("Play_Music_Placeholder", gameObject);
    }

    public void QuitToMenu()
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
    }

    public void TooglePause()
    {
        paused = !paused;

        //Pause the game
        if(paused)
        {
            onLevelPaused.Invoke();
            AkSoundEngine.SetState("Music","off_game");
            AkSoundEngine.PostEvent("Stop_Speed_RTPC", gameObject);
            Time.timeScale = 0f;
            foreach(MonoBehaviour m in m_pausedScripts)
                m.enabled = false;
        }
        //Resumes the game
        else
        {
            onLevelResumed.Invoke();
            AkSoundEngine.SetState("Music", "in_game");
            AkSoundEngine.PostEvent("Play_Speed_RTPC", gameObject);
            Time.timeScale = 1f;
            foreach (MonoBehaviour m in m_pausedScripts)
                m.enabled = true;
        }
    }


}
