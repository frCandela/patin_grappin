using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToShaderData : MonoBehaviour {
	
	// Update is called once per frame
	void Update ()
	{

		Shader.SetGlobalVector("_PlayerPosition", this.transform.position);
		
	}
}
