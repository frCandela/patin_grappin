using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModifiersController : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private Material boostMaterial = null;
    [SerializeField] private Material patinMaterial = null;

    [Header("Prefabs:")]
    [SerializeField] private GameObject traceBase = null;
    [SerializeField] private GameObject traceBoost = null;

    [Header("Boost Parameters:")]
    [SerializeField] private float boostMultiplier = 2;
    [SerializeField] private float boostDuration = 2f;

    [Header("Speed postprocess:")]
    [SerializeField, Range(0, 200)] private float minSpeed = 55f;
    [SerializeField, Range(0, 200)] private float maxSpeed = 100f;
    [SerializeField, Range(0, 0.6f)] private float maxDeformation = 0.6f;

    [Header("Events:")]
    public UnityEvent onBoostStart;
    public UnityEvent onBoostStop;

    //References
    private AnimationController m_animationController = null;
    private PlayerController m_playerController = null;
    private RagdollController m_ragdollController = null;
    private Rigidbody m_rb = null;

    private ParticleSystem m_particleSystemPofPof = null;
    private ParticleSystem m_particleSystemBoost = null;

    private GameObject m_particleSystemTraceBaseL = null;
    private GameObject m_particleSystemTraceBaseR = null;
    private GameObject m_particleSystemTraceBoostL = null;
    private GameObject m_particleSystemTraceBoostR = null;

    private Transform m_leftFoot;
    private Transform m_rightFoot;

    private float m_boostTimeRemaining = 0f;

    private void Awake()
    {
        //Instanciate
        m_particleSystemTraceBaseL = Instantiate(traceBase);
        m_particleSystemTraceBaseR = Instantiate(traceBase);
        m_particleSystemTraceBoostL = Instantiate(traceBoost);
        m_particleSystemTraceBoostR = Instantiate(traceBoost);

        m_particleSystemTraceBaseL.GetComponent<ParticleSystem>().Play();
        m_particleSystemTraceBaseR.GetComponent<ParticleSystem>().Play();

        //Get references
        m_animationController = FindObjectOfType<AnimationController>();
        m_ragdollController = GetComponent<RagdollController>();
        m_particleSystemPofPof = GetComponentInChildren<ParticleSystem>();
        m_particleSystemBoost = Camera.main.GetComponentInChildren<ParticleSystem>();
        m_playerController = GetComponent<PlayerController>();
        m_rb = GetComponent<Rigidbody>();

        //Init
        m_particleSystemBoost.Stop();
    }

    // Use this for initialization
    void Start ()
    {
        //Events
        m_animationController.onLanding.AddListener(Land);

        //Boost eye fx
        boostEyeFX[] boosts = FindObjectsOfType<boostEyeFX>();
        foreach (boostEyeFX boost in boosts)
            boost.onBoost.AddListener(Boost);

        //Get feets
        m_leftFoot = m_animationController.animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        m_rightFoot = m_animationController.animator.GetBoneTransform(HumanBodyBones.RightFoot);
    }

    private void Update()
    {
        //Get velocity (ragdoll or not)
        Vector3 velocity;
        if (m_ragdollController.ragdollActivated)
            velocity = m_ragdollController.averageVelocity;
        else
            velocity = m_rb.velocity;
        Vector2 velXZ = new Vector2(m_rb.velocity.x, m_rb.velocity.z);

        //base boost foot speed effect
        m_particleSystemTraceBaseL.transform.position = m_leftFoot.transform.position;
        m_particleSystemTraceBaseR.transform.position = m_rightFoot.transform.position;
        m_particleSystemTraceBaseL.transform.LookAt(m_leftFoot.transform.position - velocity);
        m_particleSystemTraceBaseR.transform.LookAt(m_rightFoot.transform.position - velocity);

        //Boost
        if (m_boostTimeRemaining > 0f)
        {
            //Set feets boost FX position and direction
            m_particleSystemTraceBoostL.transform.position = m_leftFoot.transform.position;
            m_particleSystemTraceBoostR.transform.position = m_rightFoot.transform.position;
            m_particleSystemTraceBoostL.transform.LookAt(m_leftFoot.transform.position - velocity);
            m_particleSystemTraceBoostR.transform.LookAt(m_rightFoot.transform.position - velocity);

            //Boost timer
            m_playerController.boostMultiplier = boostMultiplier;
            m_boostTimeRemaining -= Time.deltaTime;
        }
        else if(m_boostTimeRemaining != -42f)
        {
            m_boostTimeRemaining = -42f;
            m_particleSystemBoost.Stop();
            m_playerController.boostMultiplier = 1f;
            boostMaterial.SetFloat("_TexFactor", 0f);
            patinMaterial.SetFloat("_Shiness", 0f);

            onBoostStop.Invoke();
        }

        //Speed postprocess
        boostMaterial.SetFloat("_Offset", Mathf.Lerp(0, maxDeformation, (velXZ.magnitude - minSpeed) / (maxSpeed - minSpeed)));

        //Wind fx
        float rtpc = Mathf.Clamp((velocity.magnitude) / maxSpeed, 0f, 1f);
        AkSoundEngine.SetRTPCValue("Speed_RTPC", rtpc);
    }

    public void Boost()
    {
        m_boostTimeRemaining = boostDuration;

        //Particle Systems
        m_particleSystemBoost.Play();
        m_particleSystemTraceBoostL.GetComponent<ParticleSystem>().Play();
        m_particleSystemTraceBoostR.GetComponent<ParticleSystem>().Play();

        //Effects
        patinMaterial.SetFloat("_Shiness", 1f);

        onBoostStart.Invoke();
    }

    public void Land()
    {
        if( ! m_ragdollController.ragdollActivated )
        {
            AkSoundEngine.PostEvent("Play_Ice_Skate_Reception", gameObject);
            m_particleSystemPofPof.Emit(6);
        }
    }
}
