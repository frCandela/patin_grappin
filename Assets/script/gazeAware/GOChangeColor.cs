﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Slowly changes the color of the gameobject when it is gazed at
[RequireComponent( typeof(MeshRenderer))]
public class GOChangeColor : GazeObject
{
    //Editor
    [Header("Materials:")]
    [SerializeField] private Color fadeColor = Color.green;
    [SerializeField] private Color finalColor = Color.yellow;
    [SerializeField] private float duration = 1f;

    //Public
    public UnityEvent onColorChanged;

    //Private
    private float m_fadeDelta = 0.01f;
    private float m_ratio = 0f;
    private bool m_fixed = false;
    private Color m_baseColor;
    private Material m_material;

    private void Awake()
    {
        //Get References
        m_material = GetComponent<MeshRenderer>().material;
        m_baseColor = m_material.color;

        //Set members
        onColorChanged = new UnityEvent();
    }

    public override void SetGazed()
    {
        if( !m_fixed )
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
    }

    public override void SetNotGazed()
    {
        if (!m_fixed)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
    }

    //Coroutine for fading thhe color in
    IEnumerator FadeIn()
    {
        float delta = m_fadeDelta * duration ;
        while(m_ratio < 1f)
        {
            m_ratio += m_fadeDelta;
            m_material.color = Color.Lerp(m_baseColor, fadeColor, m_ratio);
            yield return new WaitForSeconds(delta);
        }
        m_material.color = finalColor;
        m_fixed = true;
        onColorChanged.Invoke();
    }

    //Coroutine for fading thhe color out
    IEnumerator FadeOut()
    {
        float delta = m_fadeDelta * duration;
        while (m_ratio > 0f)
        {
            m_ratio -= m_fadeDelta;
            m_material.color = Color.Lerp(m_baseColor, fadeColor, m_ratio);
            yield return new WaitForSeconds(delta);
        }
    }
}
