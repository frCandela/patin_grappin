using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private bool m_activateMusic = true;

    //References
    List<MonoBehaviour> m_pausedScripts;

    //Private members
    private bool m_paused = false;

    private void Awake()
    {
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

    private void Update()
    {
        //Restarts the level
        if (Input.GetKeyDown(KeyCode.R))
        {
            AkSoundEngine.PostEvent("Stop_Music_Placeholder", gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //Pauses the level
        if (Input.GetKeyDown(KeyCode.P))
            TooglePause();
    }

    private void TooglePause()
    {
        m_paused = !m_paused;

        if( m_paused)
        {
            Time.timeScale = 0f;

            foreach(MonoBehaviour m in m_pausedScripts)
                m.enabled = false;
        }
        else
        {
            Time.timeScale = 1f;
            foreach (MonoBehaviour m in m_pausedScripts)
                m.enabled = true;
        }
    }


}
