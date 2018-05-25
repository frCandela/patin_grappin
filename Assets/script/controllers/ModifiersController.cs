using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiersController : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private Material boostMaterial = null;

    [Header("Boost Parameters:")]
    [SerializeField] private float boostMultiplier = 2;
    [SerializeField] private float boostDuration = 2f;

    [Header("Speed postprocess:")]
    [SerializeField, Range(0, 200)] private float minSpeed = 55f;
    [SerializeField, Range(0, 200)] private float maxSpeed = 100f;
    [SerializeField, Range(0, 0.6f)] private float maxDeformation = 0.6f;
    [SerializeField, Range(0, 1f)] private float maxSpeedTex = 0.5f;


    private PlayerController m_playerController = null;
    private Rigidbody m_rb = null;

    private float m_boostTimeRemaining = 0f;

    private void Awake()
    {
        m_playerController = GetComponent<PlayerController>();
        m_rb = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start ()
    {
        boostEyeFX[] boosts = FindObjectsOfType<boostEyeFX>();
        foreach (boostEyeFX boost in boosts)
            boost.onBoost.AddListener(Boost);
    }

    private void Update()
    {
        Vector2 velXZ = new Vector2(m_rb.velocity.x, m_rb.velocity.z);

        //Boost
        if (m_boostTimeRemaining > 0f)
        {
            m_playerController.boostMultiplier = boostMultiplier;
            m_boostTimeRemaining -= Time.deltaTime;

            boostMaterial.SetFloat("_TexFactor", Mathf.Lerp(0, maxSpeedTex, (velXZ.magnitude - minSpeed) / (maxSpeed - minSpeed)));//Speed postprocess
        }
        else
        {
            m_playerController.boostMultiplier = 1f;
            boostMaterial.SetFloat("_TexFactor", 0f);
        }

        //Speed postprocess
        boostMaterial.SetFloat("_Offset", Mathf.Lerp(0, maxDeformation, (velXZ.magnitude - minSpeed) / (maxSpeed - minSpeed)));

        //Wind fx
        float rtpc =  100f * Mathf.Clamp( (m_rb.velocity.magnitude)/ maxSpeed, 0f,1f);
        AkSoundEngine.SetRTPCValue("Speed_RTPC", rtpc);
    }

    public void Boost()
    {
        m_boostTimeRemaining = boostDuration;
    }
}
