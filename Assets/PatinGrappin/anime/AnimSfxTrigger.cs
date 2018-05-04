using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSfxTrigger : MonoBehaviour {

	Animator animator;
	private float isLeftGrounded, isRightGrounded;

	[SerializeField, Range(0f,1f)] private float footGroundedThreshold = 0.7f;
	

	void Start ()
	{
		animator = GetComponent<Animator>();
	}

	void Update ()
	{
		isLeftGrounded = animator.GetFloat("leftIK");
		isRightGrounded = animator.GetFloat("rightIK");
		
		if (isLeftGrounded >= footGroundedThreshold || isRightGrounded >= footGroundedThreshold)
		{
			// play le son de patin
			// /!\ SAUF SI IL SE JOUE DEJA /!\
			// mais ya une fonction Wwise pour ça lol
			//AkSoundEngine.PostEvent("Play_Granular_Ice", gameObject);
		}
		else if (isLeftGrounded < footGroundedThreshold && isRightGrounded < footGroundedThreshold)
		{
			// arreter le son de patin
		}
	}

	// Function called at every animation event
	void OnTriggerSFX (AnimationEvent animEvent)
	{
		string stringParam = animEvent.stringParameter;

		if(stringParam == "softContact")
		{
			//do
		}

		else if(stringParam == "hardContact")
		{
			//do
		}

		else if(stringParam == "softPush")
		{
			AkSoundEngine.PostEvent("Play_Ice_skate_move", gameObject);
		}

		else if(stringParam == "hardPush")
		{
			//do
		}
	}
}