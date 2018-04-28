using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class sideCurveToBool : MonoBehaviour {

	private Animator animator;
	private float sideCurve;

	void Start () {
		animator = GetComponent<Animator>();
	}
	
	void OnAnimatorIK () {

		sideCurve = animator.GetFloat("sideCurve");

		// sur le pied droit
		if(sideCurve >= 0.5f) animator.SetBool("side", true);

		// sur le pied gauche
		else if(sideCurve < 0.5f) animator.SetBool("side", false);
	}
}
