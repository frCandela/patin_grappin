using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class MouseScreenPosToShader : MonoBehaviour {

	Vector2 m_UVMouseScreenPos, m_screenSize;

	void Update () {

		m_UVMouseScreenPos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
		m_screenSize = new Vector2(Screen.width, Screen.height);

		//Shader.SetGlobalVector("_MouseScreenPos", new Vector4(GazeManager.AverageGazePoint.x, GazeManager.AverageGazePoint.y, 0, 0));
		Shader.SetGlobalVector("_MouseScreenPos", m_UVMouseScreenPos);
		Shader.SetGlobalVector("_ScreenSize", m_screenSize);
	}
}
