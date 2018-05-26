using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class PostEffect : MonoBehaviour {

[SerializeField] private Material mat;
//[SerializeField] private float deformationValue = 0;
	
	void Update ()
	{
		//donne la valeur du boost qui est animé
		//Shader.SetGlobalFloat("_SpeedTexAnim", deformationValue);
	}
	void OnRenderImage (RenderTexture src, RenderTexture dest) {
		Graphics.Blit(src, dest, mat);
	}
}
