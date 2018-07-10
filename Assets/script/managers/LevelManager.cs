using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Music experiments")]
    public bool verticalMusicEnabled = false;
    public static bool s_verticalMusicEnabled;

    public bool horizontalMusicEnabled = false;
    public static bool s_horizontalMusicEnabled;
    public int nbGrapChangeLayer = 3;
    public static int s_nbGrapChangeLayer;


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
        s_verticalMusicEnabled = verticalMusicEnabled;
        s_horizontalMusicEnabled = horizontalMusicEnabled;
        s_nbGrapChangeLayer = nbGrapChangeLayer;

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
            {
                if(s_verticalMusicEnabled)
                {
                    print("s_verticalMusicEnabled");
                    AkSoundEngine.PostEvent("Play_Interactive", gameObject);
                }
                else if (s_horizontalMusicEnabled)
                {
                    print("s_horizontalMusicEnabled");
                    AkSoundEngine.PostEvent("Play_Interactive", gameObject);
                    AkSoundEngine.SetState("Interactive", "Level0");
                }                    
                else
                {
                    print("Play_Music_Placeholder");
                    AkSoundEngine.PostEvent("Play_Music_Placeholder", gameObject);
                }
                    
            }
        }            
    }

    public void LoadScene(string sceneString)
    {
        AkSoundEngine.PostEvent("Stop_Tuto", gameObject);

        if (s_verticalMusicEnabled || s_horizontalMusicEnabled)
            AkSoundEngine.PostEvent("Stop_Interactive", gameObject);
        else
            AkSoundEngine.PostEvent("Stop_Music_Placeholder", gameObject);

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
