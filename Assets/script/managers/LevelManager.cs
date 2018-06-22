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
    [SerializeField] private bool m_isTuto = false;

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
        {
            if(m_isTuto)
                AkSoundEngine.PostEvent("Play_Tuto", gameObject);
            else
                //AkSoundEngine.PostEvent("Play_Music_Placeholder", gameObject);
                AkSoundEngine.PostEvent("Play_Interactive", gameObject);
        }            
    }

    public void LoadScene(string sceneString)
    {
        AkSoundEngine.PostEvent("Stop_Tuto", gameObject);
        // AkSoundEngine.PostEvent("Stop_Music_Placeholder", gameObject);
        AkSoundEngine.PostEvent("Stop_Interactive", gameObject);

        AkSoundEngine.SetRTPCValue("Speed_RTPC", 0f);
        AkSoundEngine.PostEvent("Stop_Speed_RTPC", gameObject);
        if (paused)
            TooglePause();
        SceneManager.LoadScene(sceneString, LoadSceneMode.Single);
    }

    public void QuitToMenu()
    {
        LoadScene(m_menuSceneString);
    }

    private void Update()
    {
        //Restarts the level
        if (GazeManager.DebugActive && Input.GetButtonDown("Restart"))
        {
            //AkSoundEngine.PostEvent("Stop_Music_Placeholder", gameObject);
            AkSoundEngine.PostEvent("Stop_Interactive", gameObject);
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
            AkSoundEngine.SetRTPCValue("Speed_RTPC", 0f);
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
