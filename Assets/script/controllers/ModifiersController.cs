using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiersController : MonoBehaviour
{
    [Header("Boost:")]
    [SerializeField] private float boostMultiplier = 2;
    [SerializeField] private float boostDuration = 2f;

    private Rigidbody m_rb = null;
    private PlayerController m_playerController = null;

    private float m_boostTimeRemaining = 0f;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_playerController = GetComponent<PlayerController>();
    }

    // Use this for initialization
    void Start ()
    {
        var objects = FindObjectsOfType<GOChangeColor>();
        foreach (GOChangeColor changeColor in objects)
            changeColor.onColorChanged.AddListener(Boost);
    }

    private void Update()
    {
        if (m_boostTimeRemaining > 0f)
        {
            m_playerController.boostMultiplier = boostMultiplier;
            m_boostTimeRemaining -= Time.deltaTime;
        }
        else
            m_playerController.boostMultiplier = 1f;

    }

    public void Boost()
    {
        m_boostTimeRemaining = boostDuration;
    }
}
