using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public UnityEvent onGameResumed;
    public UnityEvent onGameQuit;
    public UnityEvent onUseKeyboard;
    public UnityEvent onUnUseKeyboard;

    private void Awake()
    {
       // gameObject.SetActive(false);
    }

    public void UseKeyboard( bool state)
    {
        if (state)
            onUseKeyboard.Invoke();
        else
            onUnUseKeyboard.Invoke();
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
