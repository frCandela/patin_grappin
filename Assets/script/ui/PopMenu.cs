using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopMenu : MonoBehaviour
{
    private bool m_active = false;

    private void Awake()
    {
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
