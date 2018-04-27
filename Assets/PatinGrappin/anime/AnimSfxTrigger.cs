using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSfxTrigger : MonoBehaviour {

	// Use this for initialization
	void OnTriggerSFX (AnimationEvent animEvent) {
		Debug.Log("Event");

		//float floatParam = animEvent.floatParameter;
		//int intParam = animEvent.intParameter;
		string stringParam = animEvent.stringParameter;

		if(stringParam == "softContact")
		{
			//do
		}

		if(stringParam == "hardContact")
		{
			//do
		}

		if(stringParam == "softPush")
		{
			AkSoundEngine.PostEvent("Play_Ice_skate_move", gameObject);
		}

		if(stringParam == "hardPush")
		{
			//do
		}


	}
}