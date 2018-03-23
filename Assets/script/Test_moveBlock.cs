using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class Test_moveBlock : MonoBehaviour
{
    public Material seen;
    public Material notSeen;

    private GameObject lastFocused = null;


    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        GameObject focusedObject = TobiiAPI.GetFocusedObject();
        if (focusedObject != null)
        {
            if (focusedObject != lastFocused)
            {
                if(lastFocused != null)
                    SetObjectMaterial(lastFocused, notSeen);
                lastFocused = focusedObject;
                SetObjectMaterial(lastFocused, seen);
            }
        }
        else
        {
            if (lastFocused != null)
                SetObjectMaterial(lastFocused, notSeen);
            lastFocused = null;
        }


    }

    private void SetObjectMaterial(GameObject obj, Material material)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if( renderer )
        {
            renderer.material = material;
        }
    
    }
}
