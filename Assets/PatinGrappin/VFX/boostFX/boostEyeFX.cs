using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class boostEyeFX : MonoBehaviour {

private GameObject depopFX, chargeFX;
private ParticleSystem chargePS, depopPS;
private Animator animator;

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
    }

	void DisableGO ()
	{
		// DEPOP DU BOOST DESACTIVÉ ICI
	//	if(animator.GetBool("isLookedAt")) gameObject.SetActive(false);
		
	}

	void Update ()
	{
		if(!animator.GetBool("isLookedAt"))
		{
			chargePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
	}


}
