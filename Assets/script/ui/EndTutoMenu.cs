using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTutoMenu : MonoBehaviour
{
    private LevelManager m_levelManager;

    private void Awake()
    {
        m_levelManager = FindObjectOfType<LevelManager>();
    }

    private void Start()
    {
        Hide();
    }

    public void PopEndTuto()
    {
        if (!m_levelManager.paused)
            m_levelManager.TooglePause();
        Show();
    }

    public void QuitGame()
    {
        m_levelManager.QuitToMenu();
    }

    public void NextStep( string sceneName)
    {
        m_levelManager.LoadScene(sceneName);
    }

    void Show()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);
    }

    void Hide()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

}
