using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopMenu : MonoBehaviour
{
    private LevelManager m_levelManager;
    private bool m_active = false;

    private void Awake()
    {
        m_levelManager = FindObjectOfType<LevelManager>();
        Hide();
    }

    public void Pop(int id)
    {
        transform.GetChild(id).gameObject.SetActive(true);
    }

    public void Hide()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }
}
