using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public UnityEvent onGameResumed;
    public UnityEvent onGameQuit;

    private void Awake()
    {
       // gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        onGameQuit.Invoke();
    }

    public void ResumeGame()
    {
        onGameResumed.Invoke();
    }

}
