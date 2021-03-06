﻿using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MeshCleaner : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
#if UNITY_EDITOR
[CustomEditor(typeof(MeshCleaner))]
public class MeshCleanerEditor : Editor
{
    private MeshCleaner m_meshCleaner;

    private void OnEnable()
    {
        m_meshCleaner = (MeshCleaner)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (GUILayout.Button("Reprocess"))
        {
            MeshFilter mesh = m_meshCleaner.GetComponent<MeshFilter>();
            mesh.sharedMesh.RecalculateNormals();
            mesh.sharedMesh.RecalculateTangents();
        }
    }
}
#endif