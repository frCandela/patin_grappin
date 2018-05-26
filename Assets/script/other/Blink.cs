using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Blink : MonoBehaviour
{
    [SerializeField] private float deltaBlink = 0.1f;


    private MeshRenderer m_meshRenderer;



    private void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Start()
    {
        StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine()
    {
        while ( true)
        {
            m_meshRenderer.enabled = true;
            yield return new WaitForSeconds(deltaBlink);
            m_meshRenderer.enabled = false;
            yield return new WaitForSeconds(deltaBlink);
        }
    }
}

