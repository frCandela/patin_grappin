using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float m_delayShowStats = 4f;

    [Header("References")]
    [SerializeField] private GameObject m_inGameInterface;
    [SerializeField] private GameObject m_finalInterface;

    [Header("Text references")]
    [SerializeField] private GameObject m_timeNumber;
    [SerializeField] private GameObject m_finalTimeNumber;
    [SerializeField] private GameObject m_finalAirTimeNumber;
    [SerializeField] private GameObject m_finalGroundTimeNumber;
    [SerializeField] private GameObject m_finalRagdollTimeNumber;
    [SerializeField] private GameObject m_finalBoostsTaken;
    [SerializeField] private GameObject m_finalFallFromTrack;

    private Text m_timeNumberText;

    private AnimationController m_animationController;
    private ModifiersController m_modifiersController;
    private RagdollController m_ragdollController;
    private Track m_track;

    private float m_time;
    private float m_airTime;
    private float m_groundTime;
    private float m_ragdollTime;
    private int m_boostTaken;
    private int m_nbTrackFall;

    private void Awake()
    {
        m_animationController = FindObjectOfType<AnimationController>();
        m_modifiersController = FindObjectOfType<ModifiersController>();
        m_ragdollController = FindObjectOfType<RagdollController>();
        m_track = FindObjectOfType<Track>();

        m_timeNumberText = m_timeNumber.GetComponent<Text>();

        m_modifiersController.onBoostStart.AddListener(AddBoost);
        m_track.onRespawn.AddListener(AddFall);

    }
    // Use this for initialization
    void Start ()
    {
        m_inGameInterface.SetActive(true);
        m_finalInterface.SetActive(false);

        m_time = 0f;
        m_airTime = 0f;
        m_groundTime = 0f;
        m_ragdollTime = 0f;
        m_boostTaken = 0;
        m_nbTrackFall = 0;
    }

    public void ShowStats()
    {
        UpdateFinalStats();
        StartCoroutine(ShowStatsCor());
    }

    IEnumerator ShowStatsCor()
    {
        yield return new WaitForSeconds(m_delayShowStats);
        m_finalInterface.SetActive(true);
        m_inGameInterface.SetActive(false);
        enabled = false;
    }

    void UpdateFinalStats()
    {
        m_finalTimeNumber.GetComponent<Text>().text = RoundFloat(m_time).ToString();
        m_finalAirTimeNumber.GetComponent<Text>().text = RoundFloat(m_airTime).ToString();
        m_finalGroundTimeNumber.GetComponent<Text>().text = RoundFloat(m_groundTime).ToString();
        m_finalRagdollTimeNumber.GetComponent<Text>().text = RoundFloat(m_ragdollTime).ToString();
        m_finalBoostsTaken.GetComponent<Text>().text = m_boostTaken.ToString();
        m_finalFallFromTrack.GetComponent<Text>().text = m_nbTrackFall.ToString();
    }

    void AddBoost()
    {
        ++m_boostTaken;
    }

    void  AddFall()
    {
        ++m_nbTrackFall;
    }
	
    float RoundFloat( float number)
    {
        return ((int)(number * 100)) / 100f;
    }


	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.A))
            ShowStats();

        m_time += Time.deltaTime;

        if (!m_animationController.grounded)
            m_airTime += Time.deltaTime;
        else
            m_groundTime += Time.deltaTime;

        if(m_ragdollController.ragdollActivated)
            m_ragdollTime += Time.deltaTime;

        m_timeNumberText.text = RoundFloat(m_time).ToString();
    }
}
