using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class boostEyeFX : GazeObject
{
    [SerializeField] private bool m_repop = false;
    [SerializeField] private float m_respawnDelay = 4f;

    private GameObject depopFX, chargeFX;
    private ParticleSystem chargePS, depopPS;
    private Animator animator;

    public UnityEvent onBoost;

    void Start()
	{

        animator = GetComponent<Animator>();

        chargeFX = transform.parent.GetChild(0).gameObject;
        depopFX = transform.parent.GetChild(1).gameObject;
		
		if(chargeFX)
            chargePS = chargeFX.GetComponent<ParticleSystem>();
		else
            Debug.LogError("No GameObject for Particle System CHARGE");

		if(depopFX)
            depopPS = depopFX.GetComponent<ParticleSystem>();
		else
            Debug.LogError("No GameObject for Particle System DEPOP");

		chargePS.Stop();
	}

	void StartCharge ()
    {
		chargePS.Play();
    }

	void StartDepop ()
    {
        chargePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

	void DisableGO ()
	{
		// DEPOP DU BOOST DESACTIVÉ ICI
		if(animator.GetBool("isLookedAt"))
        {
            onBoost.Invoke();



            AkSoundEngine.PostEvent("Play_Boost_Go", gameObject);
            depopPS.Emit(1);

            if (m_repop)
                StartCoroutine(Respawn());

            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
	}

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(m_respawnDelay);
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }

	void Update ()
	{
		if(!animator.GetBool("isLookedAt"))
		{
			chargePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
	}

    public override void SetGazed()
    {
        if( enabled)
        {
            animator.SetBool("isLookedAt", true);
            AkSoundEngine.PostEvent("Play_Boost_Load", gameObject);
        }

    }

    public override void SetNotGazed()
    {
        if (gameObject.activeSelf)
        {
            animator.SetBool("isLookedAt", false);
            AkSoundEngine.PostEvent("Stop_Boost_Load", gameObject);
        }
    }
}
