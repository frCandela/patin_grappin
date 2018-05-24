using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class boostEyeFX : GazeObject
{ 
    private GameObject depopFX, chargeFX;
    private ParticleSystem chargePS, depopPS;
    private Animator animator;

    public UnityEvent onBoost;

    void Start()
	{
		animator = GetComponent<Animator>();

		depopFX = transform.parent.GetChild(1).gameObject;
		chargeFX =  transform.parent.GetChild(0).gameObject;

		if(chargeFX) chargePS = chargeFX.GetComponent<ParticleSystem>();
		else Debug.LogError("No GameObject for Particle System CHARGE");
		if(depopFX) depopPS = depopFX.GetComponent<ParticleSystem>();
		else Debug.LogError("No GameObject for Particle System DEPOP");

		chargePS.Stop();
	}

	void StartCharge ()
    {
		chargePS.Play();
    }

	void StartDepop ()
    {
        if(animator.GetBool("isLookedAt")) depopPS.Emit(1);
		chargePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        onBoost.Invoke();
    }

	void DisableGO ()
	{
		// DEPOP DU BOOST DESACTIVÉ ICI
		if(animator.GetBool("isLookedAt")) gameObject.SetActive(false);
		
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
        animator.SetBool("isLookedAt", true);
    }

    public override void SetNotGazed()
    {
        animator.SetBool("isLookedAt", false);        
    }
}
