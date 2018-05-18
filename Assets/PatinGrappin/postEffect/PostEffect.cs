using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class PostEffect : MonoBehaviour {

[SerializeField] private Material mat;
	
	// Update is called once per frame
	void OnRenderImage (RenderTexture src, RenderTexture dest) {
		Graphics.Blit(src, dest, mat);
	}
}
