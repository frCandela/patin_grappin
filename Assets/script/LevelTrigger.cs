using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelTrigger : MonoBehaviour
{
    [SerializeField] private float m_delay = 4f;

    public UnityEvent onPlayerEnter;

    bool m_disabled = false;

    void Start()
    {
        FindObjectOfType<LevelManager>().onLevelResumed.AddListener(Reactivate);
    }

    IEnumerator ReActivateCor()
    {
        yield return new WaitForSeconds(m_delay);
        m_disabled = false;
    }

    void Reactivate()
    {
        StartCoroutine(ReActivateCor());
    }

    private void OnTriggerEnter(Collider other)
    {
        if( !m_disabled)
        {
            onPlayerEnter.Invoke();
            m_disabled = true;
        }
    }
}
