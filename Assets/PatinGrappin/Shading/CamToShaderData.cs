using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToShaderData : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {

		Shader.SetGlobalVector("_camWorldDir", transform.forward);
		
	}
}
