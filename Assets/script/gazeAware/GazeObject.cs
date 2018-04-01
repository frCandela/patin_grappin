using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base Class for objects the player can gaze at
public abstract class GazeObject : MonoBehaviour
{
    public void Start () 
    {
        gameObject.layer = LayerMask.NameToLayer("GazeObject");
    }

    public abstract void SetGazed();
    public abstract void SetNotGazed();
}
